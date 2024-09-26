using System;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.Wave;

namespace NAudio.CoreAudioApi;

public class AudioClient : IDisposable
{
	private IAudioClient audioClientInterface;

	private WaveFormat mixFormat;

	private AudioRenderClient audioRenderClient;

	private AudioCaptureClient audioCaptureClient;

	private AudioClockClient audioClockClient;

	private AudioStreamVolume audioStreamVolume;

	private AudioClientShareMode shareMode;

	public WaveFormat MixFormat
	{
		get
		{
			if (mixFormat == null)
			{
				Marshal.ThrowExceptionForHR(audioClientInterface.GetMixFormat(out var deviceFormatPointer));
				WaveFormat waveFormat = WaveFormat.MarshalFromPtr(deviceFormatPointer);
				Marshal.FreeCoTaskMem(deviceFormatPointer);
				mixFormat = waveFormat;
			}
			return mixFormat;
		}
	}

	public int BufferSize
	{
		get
		{
			Marshal.ThrowExceptionForHR(audioClientInterface.GetBufferSize(out var bufferSize));
			return (int)bufferSize;
		}
	}

	public long StreamLatency => audioClientInterface.GetStreamLatency();

	public int CurrentPadding
	{
		get
		{
			Marshal.ThrowExceptionForHR(audioClientInterface.GetCurrentPadding(out var currentPadding));
			return currentPadding;
		}
	}

	public long DefaultDevicePeriod
	{
		get
		{
			Marshal.ThrowExceptionForHR(audioClientInterface.GetDevicePeriod(out var defaultDevicePeriod, out var _));
			return defaultDevicePeriod;
		}
	}

	public long MinimumDevicePeriod
	{
		get
		{
			Marshal.ThrowExceptionForHR(audioClientInterface.GetDevicePeriod(out var _, out var minimumDevicePeriod));
			return minimumDevicePeriod;
		}
	}

	public AudioStreamVolume AudioStreamVolume
	{
		get
		{
			if (shareMode == AudioClientShareMode.Exclusive)
			{
				throw new InvalidOperationException("AudioStreamVolume is ONLY supported for shared audio streams.");
			}
			if (audioStreamVolume == null)
			{
				Guid interfaceId = new Guid("93014887-242D-4068-8A15-CF5E93B90FE3");
				Marshal.ThrowExceptionForHR(audioClientInterface.GetService(interfaceId, out var interfacePointer));
				audioStreamVolume = new AudioStreamVolume((IAudioStreamVolume)interfacePointer);
			}
			return audioStreamVolume;
		}
	}

	public AudioClockClient AudioClockClient
	{
		get
		{
			if (audioClockClient == null)
			{
				Guid interfaceId = new Guid("CD63314F-3FBA-4a1b-812C-EF96358728E7");
				Marshal.ThrowExceptionForHR(audioClientInterface.GetService(interfaceId, out var interfacePointer));
				audioClockClient = new AudioClockClient((IAudioClock)interfacePointer);
			}
			return audioClockClient;
		}
	}

	public AudioRenderClient AudioRenderClient
	{
		get
		{
			if (audioRenderClient == null)
			{
				Guid interfaceId = new Guid("F294ACFC-3146-4483-A7BF-ADDCA7C260E2");
				Marshal.ThrowExceptionForHR(audioClientInterface.GetService(interfaceId, out var interfacePointer));
				audioRenderClient = new AudioRenderClient((IAudioRenderClient)interfacePointer);
			}
			return audioRenderClient;
		}
	}

	public AudioCaptureClient AudioCaptureClient
	{
		get
		{
			if (audioCaptureClient == null)
			{
				Guid interfaceId = new Guid("c8adbd64-e71e-48a0-a4de-185c395cd317");
				Marshal.ThrowExceptionForHR(audioClientInterface.GetService(interfaceId, out var interfacePointer));
				audioCaptureClient = new AudioCaptureClient((IAudioCaptureClient)interfacePointer);
			}
			return audioCaptureClient;
		}
	}

	internal AudioClient(IAudioClient audioClientInterface)
	{
		this.audioClientInterface = audioClientInterface;
	}

	public void Initialize(AudioClientShareMode shareMode, AudioClientStreamFlags streamFlags, long bufferDuration, long periodicity, WaveFormat waveFormat, Guid audioSessionGuid)
	{
		this.shareMode = shareMode;
		int errorCode = audioClientInterface.Initialize(shareMode, streamFlags, bufferDuration, periodicity, waveFormat, ref audioSessionGuid);
		Marshal.ThrowExceptionForHR(errorCode);
		mixFormat = null;
	}

	public bool IsFormatSupported(AudioClientShareMode shareMode, WaveFormat desiredFormat)
	{
		WaveFormatExtensible closestMatchFormat;
		return IsFormatSupported(shareMode, desiredFormat, out closestMatchFormat);
	}

	public bool IsFormatSupported(AudioClientShareMode shareMode, WaveFormat desiredFormat, out WaveFormatExtensible closestMatchFormat)
	{
		int num = audioClientInterface.IsFormatSupported(shareMode, desiredFormat, out closestMatchFormat);
		switch (num)
		{
		case 0:
			return true;
		case 1:
			return false;
		case -2004287480:
			return false;
		default:
			Marshal.ThrowExceptionForHR(num);
			throw new NotSupportedException("Unknown hresult " + num);
		}
	}

	public void Start()
	{
		audioClientInterface.Start();
	}

	public void Stop()
	{
		audioClientInterface.Stop();
	}

	public void SetEventHandle(IntPtr eventWaitHandle)
	{
		audioClientInterface.SetEventHandle(eventWaitHandle);
	}

	public void Reset()
	{
		audioClientInterface.Reset();
	}

	public void Dispose()
	{
		if (audioClientInterface != null)
		{
			if (audioClockClient != null)
			{
				audioClockClient.Dispose();
				audioClockClient = null;
			}
			if (audioRenderClient != null)
			{
				audioRenderClient.Dispose();
				audioRenderClient = null;
			}
			if (audioCaptureClient != null)
			{
				audioCaptureClient.Dispose();
				audioCaptureClient = null;
			}
			if (audioStreamVolume != null)
			{
				audioStreamVolume.Dispose();
				audioStreamVolume = null;
			}
			Marshal.ReleaseComObject(audioClientInterface);
			audioClientInterface = null;
			GC.SuppressFinalize(this);
		}
	}
}
