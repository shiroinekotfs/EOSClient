using System;
using NAudio.Utils;

namespace NAudio.Wave;

public class BufferedWaveProvider : IWaveProvider
{
	private CircularBuffer circularBuffer;

	private readonly WaveFormat waveFormat;

	public int BufferLength { get; set; }

	public TimeSpan BufferDuration
	{
		get
		{
			return TimeSpan.FromSeconds((double)BufferLength / (double)WaveFormat.AverageBytesPerSecond);
		}
		set
		{
			BufferLength = (int)(value.TotalSeconds * (double)WaveFormat.AverageBytesPerSecond);
		}
	}

	public bool DiscardOnBufferOverflow { get; set; }

	public int BufferedBytes
	{
		get
		{
			if (circularBuffer != null)
			{
				return circularBuffer.Count;
			}
			return 0;
		}
	}

	public TimeSpan BufferedDuration => TimeSpan.FromSeconds((double)BufferedBytes / (double)WaveFormat.AverageBytesPerSecond);

	public WaveFormat WaveFormat => waveFormat;

	public BufferedWaveProvider(WaveFormat waveFormat)
	{
		this.waveFormat = waveFormat;
		BufferLength = waveFormat.AverageBytesPerSecond * 5;
	}

	public void AddSamples(byte[] buffer, int offset, int count)
	{
		if (circularBuffer == null)
		{
			circularBuffer = new CircularBuffer(BufferLength);
		}
		int num = circularBuffer.Write(buffer, offset, count);
		if (num < count && !DiscardOnBufferOverflow)
		{
			throw new InvalidOperationException("Buffer full");
		}
	}

	public int Read(byte[] buffer, int offset, int count)
	{
		int num = 0;
		if (circularBuffer != null)
		{
			num = circularBuffer.Read(buffer, offset, count);
		}
		if (num < count)
		{
			Array.Clear(buffer, offset + num, count - num);
		}
		return count;
	}

	public void ClearBuffer()
	{
		if (circularBuffer != null)
		{
			circularBuffer.Reset();
		}
	}
}
