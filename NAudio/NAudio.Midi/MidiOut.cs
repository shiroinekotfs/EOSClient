using System;
using System.Runtime.InteropServices;

namespace NAudio.Midi;

public class MidiOut : IDisposable
{
	private IntPtr hMidiOut = IntPtr.Zero;

	private bool disposed;

	private MidiInterop.MidiOutCallback callback;

	public static int NumberOfDevices => MidiInterop.midiOutGetNumDevs();

	public int Volume
	{
		get
		{
			int lpdwVolume = 0;
			MmException.Try(MidiInterop.midiOutGetVolume(hMidiOut, ref lpdwVolume), "midiOutGetVolume");
			return lpdwVolume;
		}
		set
		{
			MmException.Try(MidiInterop.midiOutSetVolume(hMidiOut, value), "midiOutSetVolume");
		}
	}

	public static MidiOutCapabilities DeviceInfo(int midiOutDeviceNumber)
	{
		MidiOutCapabilities caps = default(MidiOutCapabilities);
		int uSize = Marshal.SizeOf(caps);
		MmException.Try(MidiInterop.midiOutGetDevCaps((IntPtr)midiOutDeviceNumber, out caps, uSize), "midiOutGetDevCaps");
		return caps;
	}

	public MidiOut(int deviceNo)
	{
		callback = Callback;
		MmException.Try(MidiInterop.midiOutOpen(out hMidiOut, (IntPtr)deviceNo, callback, IntPtr.Zero, 196608), "midiOutOpen");
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

	public void Reset()
	{
		MmException.Try(MidiInterop.midiOutReset(hMidiOut), "midiOutReset");
	}

	public void SendDriverMessage(int message, int param1, int param2)
	{
		MmException.Try(MidiInterop.midiOutMessage(hMidiOut, message, (IntPtr)param1, (IntPtr)param2), "midiOutMessage");
	}

	public void Send(int message)
	{
		MmException.Try(MidiInterop.midiOutShortMsg(hMidiOut, message), "midiOutShortMsg");
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposed)
		{
			MidiInterop.midiOutClose(hMidiOut);
		}
		disposed = true;
	}

	private void Callback(IntPtr midiInHandle, MidiInterop.MidiOutMessage message, IntPtr userData, IntPtr messageParameter1, IntPtr messageParameter2)
	{
	}

	public void SendBuffer(byte[] byteBuffer)
	{
		MidiInterop.MIDIHDR lpMidiOutHdr = default(MidiInterop.MIDIHDR);
		lpMidiOutHdr.lpData = Marshal.AllocHGlobal(byteBuffer.Length);
		Marshal.Copy(byteBuffer, 0, lpMidiOutHdr.lpData, byteBuffer.Length);
		lpMidiOutHdr.dwBufferLength = byteBuffer.Length;
		lpMidiOutHdr.dwBytesRecorded = byteBuffer.Length;
		int uSize = Marshal.SizeOf(lpMidiOutHdr);
		MidiInterop.midiOutPrepareHeader(hMidiOut, ref lpMidiOutHdr, uSize);
		if (MidiInterop.midiOutLongMsg(hMidiOut, ref lpMidiOutHdr, uSize) != 0)
		{
			MidiInterop.midiOutUnprepareHeader(hMidiOut, ref lpMidiOutHdr, uSize);
		}
		Marshal.FreeHGlobal(lpMidiOutHdr.lpData);
	}

	~MidiOut()
	{
		Dispose(disposing: false);
	}
}
