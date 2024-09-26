using System;
using NAudio.Wave.Compression;

namespace NAudio.Wave;

public class WaveFormatConversionStream : WaveStream
{
	private AcmStream conversionStream;

	private WaveStream sourceStream;

	private WaveFormat targetFormat;

	private long length;

	private long position;

	private int preferredSourceReadSize;

	private int leftoverDestBytes;

	private int leftoverDestOffset;

	private int leftoverSourceBytes;

	public override long Length => length;

	public override long Position
	{
		get
		{
			return position;
		}
		set
		{
			value -= value % BlockAlign;
			long num = EstimateDestToSource(value);
			sourceStream.Position = num;
			position = EstimateSourceToDest(sourceStream.Position);
			leftoverDestBytes = 0;
			leftoverDestOffset = 0;
			conversionStream.Reposition();
		}
	}

	public override WaveFormat WaveFormat => targetFormat;

	public static WaveStream CreatePcmStream(WaveStream sourceStream)
	{
		if (sourceStream.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
		{
			return sourceStream;
		}
		WaveFormat waveFormat = AcmStream.SuggestPcmFormat(sourceStream.WaveFormat);
		if (waveFormat.SampleRate < 8000)
		{
			if (sourceStream.WaveFormat.Encoding != WaveFormatEncoding.G723)
			{
				throw new InvalidOperationException("Invalid suggested output format, please explicitly provide a target format");
			}
			waveFormat = new WaveFormat(8000, 16, 1);
		}
		return new WaveFormatConversionStream(waveFormat, sourceStream);
	}

	public WaveFormatConversionStream(WaveFormat targetFormat, WaveStream sourceStream)
	{
		this.sourceStream = sourceStream;
		this.targetFormat = targetFormat;
		conversionStream = new AcmStream(sourceStream.WaveFormat, targetFormat);
		length = EstimateSourceToDest((int)sourceStream.Length);
		position = 0L;
		preferredSourceReadSize = Math.Min(sourceStream.WaveFormat.AverageBytesPerSecond, conversionStream.SourceBuffer.Length);
		preferredSourceReadSize -= preferredSourceReadSize % sourceStream.WaveFormat.BlockAlign;
	}

	[Obsolete("can be unreliable, use of this method not encouraged")]
	public int SourceToDest(int source)
	{
		return (int)EstimateSourceToDest(source);
	}

	private long EstimateSourceToDest(long source)
	{
		long num = source * targetFormat.AverageBytesPerSecond / sourceStream.WaveFormat.AverageBytesPerSecond;
		return num - num % targetFormat.BlockAlign;
	}

	private long EstimateDestToSource(long dest)
	{
		long num = dest * sourceStream.WaveFormat.AverageBytesPerSecond / targetFormat.AverageBytesPerSecond;
		num -= num % sourceStream.WaveFormat.BlockAlign;
		return (int)num;
	}

	[Obsolete("can be unreliable, use of this method not encouraged")]
	public int DestToSource(int dest)
	{
		return (int)EstimateDestToSource(dest);
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		int i = 0;
		if (count % BlockAlign != 0)
		{
			count -= count % BlockAlign;
		}
		int num4;
		for (; i < count; i += num4)
		{
			int num = Math.Min(count - i, leftoverDestBytes);
			if (num > 0)
			{
				Array.Copy(conversionStream.DestBuffer, leftoverDestOffset, buffer, offset + i, num);
				leftoverDestOffset += num;
				leftoverDestBytes -= num;
				i += num;
			}
			if (i >= count)
			{
				break;
			}
			_ = leftoverSourceBytes;
			_ = 0;
			int num2 = sourceStream.Read(conversionStream.SourceBuffer, 0, preferredSourceReadSize);
			if (num2 == 0)
			{
				break;
			}
			int sourceBytesConverted;
			int num3 = conversionStream.Convert(num2, out sourceBytesConverted);
			if (sourceBytesConverted == 0)
			{
				break;
			}
			if (sourceBytesConverted < num2)
			{
				sourceStream.Position -= num2 - sourceBytesConverted;
			}
			if (num3 <= 0)
			{
				break;
			}
			int val = count - i;
			num4 = Math.Min(num3, val);
			if (num4 < num3)
			{
				leftoverDestBytes = num3 - num4;
				leftoverDestOffset = num4;
			}
			Array.Copy(conversionStream.DestBuffer, 0, buffer, i + offset, num4);
		}
		position += i;
		return i;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (conversionStream != null)
			{
				conversionStream.Dispose();
				conversionStream = null;
			}
			if (sourceStream != null)
			{
				sourceStream.Dispose();
				sourceStream = null;
			}
		}
		base.Dispose(disposing);
	}
}
