using System;
using NAudio.Utils;

namespace NAudio.Wave;

public class WaveFloatTo16Provider : IWaveProvider
{
	private readonly IWaveProvider sourceProvider;

	private readonly WaveFormat waveFormat;

	private volatile float volume;

	private byte[] sourceBuffer;

	public WaveFormat WaveFormat => waveFormat;

	public float Volume
	{
		get
		{
			return volume;
		}
		set
		{
			volume = value;
		}
	}

	public WaveFloatTo16Provider(IWaveProvider sourceProvider)
	{
		if (sourceProvider.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
		{
			throw new ArgumentException("Input wave provider must be IEEE float", "sourceProvider");
		}
		if (sourceProvider.WaveFormat.BitsPerSample != 32)
		{
			throw new ArgumentException("Input wave provider must be 32 bit", "sourceProvider");
		}
		waveFormat = new WaveFormat(sourceProvider.WaveFormat.SampleRate, 16, sourceProvider.WaveFormat.Channels);
		this.sourceProvider = sourceProvider;
		volume = 1f;
	}

	public int Read(byte[] destBuffer, int offset, int numBytes)
	{
		int num = numBytes * 2;
		sourceBuffer = BufferHelpers.Ensure(sourceBuffer, num);
		int num2 = sourceProvider.Read(sourceBuffer, 0, num);
		WaveBuffer waveBuffer = new WaveBuffer(sourceBuffer);
		WaveBuffer waveBuffer2 = new WaveBuffer(destBuffer);
		int num3 = num2 / 4;
		int num4 = offset / 2;
		for (int i = 0; i < num3; i++)
		{
			float num5 = waveBuffer.FloatBuffer[i] * volume;
			if (num5 > 1f)
			{
				num5 = 1f;
			}
			if (num5 < -1f)
			{
				num5 = -1f;
			}
			waveBuffer2.ShortBuffer[num4++] = (short)(num5 * 32767f);
		}
		return num3 * 2;
	}
}
