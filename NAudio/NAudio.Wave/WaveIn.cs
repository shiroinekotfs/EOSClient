using System;
using System.Runtime.InteropServices;
using System.Threading;
using NAudio.Mixer;

namespace NAudio.Wave;

public class WaveIn : IWaveIn, IDisposable
{
	private IntPtr waveInHandle;

	private volatile bool recording;

	private WaveInBuffer[] buffers;

	private readonly WaveInterop.WaveCallback callback;

	private WaveCallbackInfo callbackInfo;

	private readonly SynchronizationContext syncContext;

	private int lastReturnedBufferIndex;

	public static int DeviceCount => WaveInterop.waveInGetNumDevs();

	public int BufferMilliseconds { get; set; }

	public int NumberOfBuffers { get; set; }

	public int DeviceNumber { get; set; }

	public WaveFormat WaveFormat { get; set; }

	public event EventHandler<WaveInEventArgs> DataAvailable;

	public event EventHandler<StoppedEventArgs> RecordingStopped;

	public WaveIn()
		: this(WaveCallbackInfo.NewWindow())
	{
	}

	public WaveIn(IntPtr windowHandle)
		: this(WaveCallbackInfo.ExistingWindow(windowHandle))
	{
	}

	public WaveIn(WaveCallbackInfo callbackInfo)
	{
		syncContext = SynchronizationContext.Current;
		if ((callbackInfo.Strategy == WaveCallbackStrategy.NewWindow || callbackInfo.Strategy == WaveCallbackStrategy.ExistingWindow) && syncContext == null)
		{
			throw new InvalidOperationException("Use WaveInEvent to record on a background thread");
		}
		DeviceNumber = 0;
		WaveFormat = new WaveFormat(8000, 16, 1);
		BufferMilliseconds = 100;
		NumberOfBuffers = 3;
		callback = Callback;
		this.callbackInfo = callbackInfo;
		callbackInfo.Connect(callback);
	}

	public static WaveInCapabilities GetCapabilities(int devNumber)
	{
		WaveInCapabilities waveInCaps = default(WaveInCapabilities);
		int waveInCapsSize = Marshal.SizeOf(waveInCaps);
		MmException.Try(WaveInterop.waveInGetDevCaps((IntPtr)devNumber, out waveInCaps, waveInCapsSize), "waveInGetDevCaps");
		return waveInCaps;
	}

	private void CreateBuffers()
	{
		int num = BufferMilliseconds * WaveFormat.AverageBytesPerSecond / 1000;
		if (num % WaveFormat.BlockAlign != 0)
		{
			num -= num % WaveFormat.BlockAlign;
		}
		buffers = new WaveInBuffer[NumberOfBuffers];
		for (int i = 0; i < buffers.Length; i++)
		{
			buffers[i] = new WaveInBuffer(waveInHandle, num);
		}
	}

	private void Callback(IntPtr waveInHandle, WaveInterop.WaveMessage message, IntPtr userData, WaveHeader waveHeader, IntPtr reserved)
	{
		if (message != WaveInterop.WaveMessage.WaveInData || !recording)
		{
			return;
		}
		WaveInBuffer waveInBuffer = (WaveInBuffer)((GCHandle)waveHeader.userData).Target;
		if (waveInBuffer != null)
		{
			lastReturnedBufferIndex = Array.IndexOf(buffers, waveInBuffer);
			RaiseDataAvailable(waveInBuffer);
			try
			{
				waveInBuffer.Reuse();
			}
			catch (Exception e)
			{
				recording = false;
				RaiseRecordingStopped(e);
			}
		}
	}

	private void RaiseDataAvailable(WaveInBuffer buffer)
	{
		this.DataAvailable?.Invoke(this, new WaveInEventArgs(buffer.Data, buffer.BytesRecorded));
	}

	private void RaiseRecordingStopped(Exception e)
	{
		EventHandler<StoppedEventArgs> handler = this.RecordingStopped;
		if (handler == null)
		{
			return;
		}
		if (syncContext == null)
		{
			handler(this, new StoppedEventArgs(e));
			return;
		}
		syncContext.Post(delegate
		{
			handler(this, new StoppedEventArgs(e));
		}, null);
	}

	private void OpenWaveInDevice()
	{
		CloseWaveInDevice();
		MmResult result = callbackInfo.WaveInOpen(out waveInHandle, DeviceNumber, WaveFormat, callback);
		MmException.Try(result, "waveInOpen");
		CreateBuffers();
	}

	public void StartRecording()
	{
		if (recording)
		{
			throw new InvalidOperationException("Already recording");
		}
		OpenWaveInDevice();
		EnqueueBuffers();
		MmException.Try(WaveInterop.waveInStart(waveInHandle), "waveInStart");
		recording = true;
	}

	private void EnqueueBuffers()
	{
		WaveInBuffer[] array = buffers;
		foreach (WaveInBuffer waveInBuffer in array)
		{
			if (!waveInBuffer.InQueue)
			{
				waveInBuffer.Reuse();
			}
		}
	}

	public void StopRecording()
	{
		if (!recording)
		{
			return;
		}
		recording = false;
		MmException.Try(WaveInterop.waveInStop(waveInHandle), "waveInStop");
		for (int i = 0; i < buffers.Length; i++)
		{
			int num = (i + lastReturnedBufferIndex + 1) % buffers.Length;
			WaveInBuffer waveInBuffer = buffers[num];
			if (waveInBuffer.Done)
			{
				RaiseDataAvailable(waveInBuffer);
			}
		}
		RaiseRecordingStopped(null);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (recording)
			{
				StopRecording();
			}
			CloseWaveInDevice();
			if (callbackInfo != null)
			{
				callbackInfo.Disconnect();
				callbackInfo = null;
			}
		}
	}

	private void CloseWaveInDevice()
	{
		if (waveInHandle == IntPtr.Zero)
		{
			return;
		}
		WaveInterop.waveInReset(waveInHandle);
		if (buffers != null)
		{
			for (int i = 0; i < buffers.Length; i++)
			{
				buffers[i].Dispose();
			}
			buffers = null;
		}
		WaveInterop.waveInClose(waveInHandle);
		waveInHandle = IntPtr.Zero;
	}

	public MixerLine GetMixerLine()
	{
		if (waveInHandle != IntPtr.Zero)
		{
			return new MixerLine(waveInHandle, 0, MixerFlags.WaveInHandle);
		}
		return new MixerLine((IntPtr)DeviceNumber, 0, MixerFlags.WaveIn);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
