using System;
using System.Runtime.InteropServices;

namespace NAudio.Wave;

internal class WaveInBuffer : IDisposable
{
	private readonly WaveHeader header;

	private readonly int bufferSize;

	private readonly byte[] buffer;

	private GCHandle hBuffer;

	private IntPtr waveInHandle;

	private GCHandle hHeader;

	private GCHandle hThis;

	public byte[] Data => buffer;

	public bool Done => (header.flags & WaveHeaderFlags.Done) == WaveHeaderFlags.Done;

	public bool InQueue => (header.flags & WaveHeaderFlags.InQueue) == WaveHeaderFlags.InQueue;

	public int BytesRecorded => header.bytesRecorded;

	public int BufferSize => bufferSize;

	public WaveInBuffer(IntPtr waveInHandle, int bufferSize)
	{
		this.bufferSize = bufferSize;
		buffer = new byte[bufferSize];
		hBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
		this.waveInHandle = waveInHandle;
		header = new WaveHeader();
		hHeader = GCHandle.Alloc(header, GCHandleType.Pinned);
		header.dataBuffer = hBuffer.AddrOfPinnedObject();
		header.bufferLength = bufferSize;
		header.loops = 1;
		hThis = GCHandle.Alloc(this);
		header.userData = (IntPtr)hThis;
		MmException.Try(WaveInterop.waveInPrepareHeader(waveInHandle, header, Marshal.SizeOf(header)), "waveInPrepareHeader");
	}

	public void Reuse()
	{
		MmException.Try(WaveInterop.waveInUnprepareHeader(waveInHandle, header, Marshal.SizeOf(header)), "waveUnprepareHeader");
		MmException.Try(WaveInterop.waveInPrepareHeader(waveInHandle, header, Marshal.SizeOf(header)), "waveInPrepareHeader");
		MmException.Try(WaveInterop.waveInAddBuffer(waveInHandle, header, Marshal.SizeOf(header)), "waveInAddBuffer");
	}

	~WaveInBuffer()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
		Dispose(disposing: true);
	}

	protected void Dispose(bool disposing)
	{
		if (waveInHandle != IntPtr.Zero)
		{
			WaveInterop.waveInUnprepareHeader(waveInHandle, header, Marshal.SizeOf(header));
			waveInHandle = IntPtr.Zero;
		}
		if (hHeader.IsAllocated)
		{
			hHeader.Free();
		}
		if (hBuffer.IsAllocated)
		{
			hBuffer.Free();
		}
		if (hThis.IsAllocated)
		{
			hThis.Free();
		}
	}
}
