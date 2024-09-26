using System;

namespace NAudio.Wave.SampleProviders;

public class SampleToWaveProvider : IWaveProvider
{
	private ISampleProvider source;

	public WaveFormat WaveFormat => source.WaveFormat;

	public SampleToWaveProvider(ISampleProvider source)
	{
		if (source.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
		{
			throw new ArgumentException("Must be already floating point");
		}
		this.source = source;
	}

	public int Read(byte[] buffer, int offset, int count)
	{
		int count2 = count / 4;
		WaveBuffer waveBuffer = new WaveBuffer(buffer);
		int num = source.Read(waveBuffer.FloatBuffer, offset / 4, count2);
		return num * 4;
	}
}
