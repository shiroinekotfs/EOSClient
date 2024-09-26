using System;
using System.Runtime.InteropServices;

namespace NAudio.Midi;

public class MidiIn : IDisposable
{
	private IntPtr hMidiIn = IntPtr.Zero;

	private bool disposed;

	private MidiInterop.MidiInCallback callback;

	public static int NumberOfDevices => MidiInterop.midiInGetNumDevs();

	public event EventHandler<MidiInMessageEventArgs> MessageReceived;

	public event EventHandler<MidiInMessageEventArgs> ErrorReceived;

	public MidiIn(int deviceNo)
	{
		callback = Callback;
		MmException.Try(MidiInterop.midiInOpen(out hMidiIn, (IntPtr)deviceNo, callback, IntPtr.Zero, 196608), "midiInOpen");
	}

	public void Close()
	{
		Dispose();
	}

	public void Dispose()
	{
		GC.KeepAlive(callback);
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public void Start()
	{
		MmException.Try(MidiInterop.midiInStart(hMidiIn), "midiInStart");
	}

	public void Stop()
	{
		MmException.Try(MidiInterop.midiInStop(hMidiIn), "midiInStop");
	}

	public void Reset()
	{
		MmException.Try(MidiInterop.midiInReset(hMidiIn), "midiInReset");
	}

	private void Callback(IntPtr midiInHandle, MidiInterop.MidiInMessage message, IntPtr userData, IntPtr messageParameter1, IntPtr messageParameter2)
	{
		switch (message)
		{
		case MidiInterop.MidiInMessage.Data:
			if (this.MessageReceived != null)
			{
				this.MessageReceived(this, new MidiInMessageEventArgs(messageParameter1.ToInt32(), messageParameter2.ToInt32()));
			}
			break;
		case MidiInterop.MidiInMessage.Error:
			if (this.ErrorReceived != null)
			{
				this.ErrorReceived(this, new MidiInMessageEventArgs(messageParameter1.ToInt32(), messageParameter2.ToInt32()));
			}
			break;
		case MidiInterop.MidiInMessage.Open:
		case MidiInterop.MidiInMessage.Close:
		case MidiInterop.MidiInMessage.LongData:
		case MidiInterop.MidiInMessage.LongError:
		case (MidiInterop.MidiInMessage)967:
		case (MidiInterop.MidiInMessage)968:
		case (MidiInterop.MidiInMessage)969:
		case (MidiInterop.MidiInMessage)970:
		case (MidiInterop.MidiInMessage)971:
		case MidiInterop.MidiInMessage.MoreData:
			break;
		}
	}

	public static MidiInCapabilities DeviceInfo(int midiInDeviceNumber)
	{
		MidiInCapabilities capabilities = default(MidiInCapabilities);
		int size = Marshal.SizeOf(capabilities);
		MmException.Try(MidiInterop.midiInGetDevCaps((IntPtr)midiInDeviceNumber, out capabilities, size), "midiInGetDevCaps");
		return capabilities;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposed)
		{
			MidiInterop.midiInClose(hMidiIn);
		}
		disposed = true;
	}

	~MidiIn()
	{
		Dispose(disposing: false);
	}
}
