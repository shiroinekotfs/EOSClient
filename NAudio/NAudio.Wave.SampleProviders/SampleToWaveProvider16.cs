using System;
using NAudio.Utils;

namespace NAudio.Wave.SampleProviders;

public class SampleToWaveProvider16 : IWaveProvider
{
	private readonly ISampleProvider sourceProvider;

	private readonly WaveFormat waveFormat;

	private volatile float volume;

	private float[] sourceBuffer;

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

	public SampleToWaveProvider16(ISampleProvider sourceProvider)
	{
		if (sourceProvider.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
		{
			throw new ArgumentException("Input source provider must be IEEE float", "sourceProvider");
		}
		if (sourceProvider.WaveFormat.BitsPerSample != 32)
		{
			throw new ArgumentException("Input source provider must be 32 bit", "sourceProvider");
		}
		waveFormat = new WaveFormat(sourceProvider.WaveFormat.SampleRate, 16, sourceProvider.WaveFormat.Channels);
		this.sourceProvider = sourceProvider;
		volume = 1f;
	}

	public int Read(byte[] destBuffer, int offset, int numBytes)
	{
		int num = numBytes / 2;
		sourceBuffer = BufferHelpers.Ensure(sourceBuffer, num);
		int num2 = sourceProvider.Read(sourceBuffer, 0, num);
		WaveBuffer waveBuffer = new WaveBuffer(destBuffer);
		int num3 = offset / 2;
		for (int i = 0; i < num2; i++)
		{
			float num4 = sourceBuffer[i] * volume;
			if (num4 > 1f)
			{
				num4 = 1f;
			}
			if (num4 < -1f)
			{
				num4 = -1f;
			}
			waveBuffer.ShortBuffer[num3++] = (short)(num4 * 32767f);
		}
		return num2 * 2;
	}
}
