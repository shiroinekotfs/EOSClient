using System;
using NAudio.Utils;

namespace NAudio.Wave;

public class Wave16ToFloatProvider : IWaveProvider
{
	private IWaveProvider sourceProvider;

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

	public Wave16ToFloatProvider(IWaveProvider sourceProvider)
	{
		if (sourceProvider.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
		{
			throw new ArgumentException("Only PCM supported");
		}
		if (sourceProvider.WaveFormat.BitsPerSample != 16)
		{
			throw new ArgumentException("Only 16 bit audio supported");
		}
		waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sourceProvider.WaveFormat.SampleRate, sourceProvider.WaveFormat.Channels);
		this.sourceProvider = sourceProvider;
		volume = 1f;
	}

	public int Read(byte[] destBuffer, int offset, int numBytes)
	{
		int num = numBytes / 2;
		sourceBuffer = BufferHelpers.Ensure(sourceBuffer, num);
		int num2 = sourceProvider.Read(sourceBuffer, offset, num);
		WaveBuffer waveBuffer = new WaveBuffer(sourceBuffer);
		WaveBuffer waveBuffer2 = new WaveBuffer(destBuffer);
		int num3 = num2 / 2;
		int num4 = offset / 4;
		for (int i = 0; i < num3; i++)
		{
			waveBuffer2.FloatBuffer[num4++] = (float)waveBuffer.ShortBuffer[i] / 32768f * volume;
		}
		return num3 * 4;
	}
}
