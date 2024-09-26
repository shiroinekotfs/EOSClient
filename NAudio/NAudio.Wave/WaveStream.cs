using System;
using System.IO;

namespace NAudio.Wave;

public abstract class WaveStream : Stream, IWaveProvider
{
	public abstract WaveFormat WaveFormat { get; }

	public override bool CanRead => true;

	public override bool CanSeek => true;

	public override bool CanWrite => false;

	public virtual int BlockAlign => WaveFormat.BlockAlign;

	public virtual TimeSpan CurrentTime
	{
		get
		{
			return TimeSpan.FromSeconds((double)Position / (double)WaveFormat.AverageBytesPerSecond);
		}
		set
		{
			Position = (long)(value.TotalSeconds * (double)WaveFormat.AverageBytesPerSecond);
		}
	}

	public virtual TimeSpan TotalTime => TimeSpan.FromSeconds((double)Length / (double)WaveFormat.AverageBytesPerSecond);

	public override void Flush()
	{
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		switch (origin)
		{
		case SeekOrigin.Begin:
			Position = offset;
			break;
		case SeekOrigin.Current:
			Position += offset;
			break;
		default:
			Position = Length + offset;
			break;
		}
		return Position;
	}

	public override void SetLength(long length)
	{
		throw new NotSupportedException("Can't set length of a WaveFormatString");
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		throw new NotSupportedException("Can't write to a WaveFormatString");
	}

	public void Skip(int seconds)
	{
		long num = Position + WaveFormat.AverageBytesPerSecond * seconds;
		if (num > Length)
		{
			Position = Length;
		}
		else if (num < 0)
		{
			Position = 0L;
		}
		else
		{
			Position = num;
		}
	}

	public virtual bool HasData(int count)
	{
		return Position < Length;
	}
}
