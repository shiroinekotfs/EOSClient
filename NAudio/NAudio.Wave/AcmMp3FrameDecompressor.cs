using System;
using NAudio.Wave.Compression;

namespace NAudio.Wave;

public class AcmMp3FrameDecompressor : IMp3FrameDecompressor, IDisposable
{
	private readonly AcmStream conversionStream;

	private readonly WaveFormat pcmFormat;

	private bool disposed;

	public WaveFormat OutputFormat => pcmFormat;

	public AcmMp3FrameDecompressor(WaveFormat sourceFormat)
	{
		pcmFormat = AcmStream.SuggestPcmFormat(sourceFormat);
		try
		{
			conversionStream = new AcmStream(sourceFormat, pcmFormat);
		}
		catch (Exception)
		{
			disposed = true;
			GC.SuppressFinalize(this);
			throw;
		}
	}

	public int DecompressFrame(Mp3Frame frame, byte[] dest, int destOffset)
	{
		if (frame == null)
		{
			throw new ArgumentNullException("frame", "You must provide a non-null Mp3Frame to decompress");
		}
		Array.Copy(frame.RawData, conversionStream.SourceBuffer, frame.FrameLength);
		int sourceBytesConverted = 0;
		int num = conversionStream.Convert(frame.FrameLength, out sourceBytesConverted);
		if (sourceBytesConverted != frame.FrameLength)
		{
			throw new InvalidOperationException($"Couldn't convert the whole MP3 frame (converted {sourceBytesConverted}/{frame.FrameLength})");
		}
		Array.Copy(conversionStream.DestBuffer, 0, dest, destOffset, num);
		return num;
	}

	public void Reset()
	{
		conversionStream.Reposition();
	}

	public void Dispose()
	{
		if (!disposed)
		{
			disposed = true;
			if (conversionStream != null)
			{
				conversionStream.Dispose();
			}
			GC.SuppressFinalize(this);
		}
	}

	~AcmMp3FrameDecompressor()
	{
		Dispose();
	}
}
