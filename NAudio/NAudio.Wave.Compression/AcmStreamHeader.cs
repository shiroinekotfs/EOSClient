using System;
using System.Runtime.InteropServices;

namespace NAudio.Wave.Compression;

internal class AcmStreamHeader : IDisposable
{
	private AcmStreamHeaderStruct streamHeader;

	private byte[] sourceBuffer;

	private GCHandle hSourceBuffer;

	private byte[] destBuffer;

	private GCHandle hDestBuffer;

	private IntPtr streamHandle;

	private bool firstTime;

	private bool disposed;

	public byte[] SourceBuffer => sourceBuffer;

	public byte[] DestBuffer => destBuffer;

	public AcmStreamHeader(IntPtr streamHandle, int sourceBufferLength, int destBufferLength)
	{
		streamHeader = new AcmStreamHeaderStruct();
		sourceBuffer = new byte[sourceBufferLength];
		hSourceBuffer = GCHandle.Alloc(sourceBuffer, GCHandleType.Pinned);
		destBuffer = new byte[destBufferLength];
		hDestBuffer = GCHandle.Alloc(destBuffer, GCHandleType.Pinned);
		this.streamHandle = streamHandle;
		firstTime = true;
	}

	private void Prepare()
	{
		streamHeader.cbStruct = Marshal.SizeOf(streamHeader);
		streamHeader.sourceBufferLength = sourceBuffer.Length;
		streamHeader.sourceBufferPointer = hSourceBuffer.AddrOfPinnedObject();
		streamHeader.destBufferLength = destBuffer.Length;
		streamHeader.destBufferPointer = hDestBuffer.AddrOfPinnedObject();
		MmException.Try(AcmInterop.acmStreamPrepareHeader(streamHandle, streamHeader, 0), "acmStreamPrepareHeader");
	}

	private void Unprepare()
	{
		streamHeader.sourceBufferLength = sourceBuffer.Length;
		streamHeader.sourceBufferPointer = hSourceBuffer.AddrOfPinnedObject();
		streamHeader.destBufferLength = destBuffer.Length;
		streamHeader.destBufferPointer = hDestBuffer.AddrOfPinnedObject();
		MmResult mmResult = AcmInterop.acmStreamUnprepareHeader(streamHandle, streamHeader, 0);
		if (mmResult != 0)
		{
			throw new MmException(mmResult, "acmStreamUnprepareHeader");
		}
	}

	public void Reposition()
	{
		firstTime = true;
	}

	public int Convert(int bytesToConvert, out int sourceBytesConverted)
	{
		Prepare();
		try
		{
			streamHeader.sourceBufferLength = bytesToConvert;
			streamHeader.sourceBufferLengthUsed = bytesToConvert;
			AcmStreamConvertFlags streamConvertFlags = (firstTime ? (AcmStreamConvertFlags.BlockAlign | AcmStreamConvertFlags.Start) : AcmStreamConvertFlags.BlockAlign);
			MmException.Try(AcmInterop.acmStreamConvert(streamHandle, streamHeader, streamConvertFlags), "acmStreamConvert");
			firstTime = false;
			sourceBytesConverted = streamHeader.sourceBufferLengthUsed;
		}
		finally
		{
			Unprepare();
		}
		return streamHeader.destBufferLengthUsed;
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
		Dispose(disposing: true);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposed)
		{
			sourceBuffer = null;
			destBuffer = null;
			hSourceBuffer.Free();
			hDestBuffer.Free();
		}
		disposed = true;
	}

	~AcmStreamHeader()
	{
		Dispose(disposing: false);
	}
}
