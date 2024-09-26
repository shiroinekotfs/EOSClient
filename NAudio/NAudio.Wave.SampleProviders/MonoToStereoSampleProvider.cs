using System;

namespace NAudio.Wave.SampleProviders;

public class MonoToStereoSampleProvider : ISampleProvider
{
	private readonly ISampleProvider source;

	private readonly WaveFormat waveFormat;

	private float[] sourceBuffer;

	public WaveFormat WaveFormat => waveFormat;

	public MonoToStereoSampleProvider(ISampleProvider source)
	{
		if (source.WaveFormat.Channels != 1)
		{
			throw new ArgumentException("Source must be mono");
		}
		this.source = source;
		waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(source.WaveFormat.SampleRate, 2);
	}

	public int Read(float[] buffer, int offset, int count)
	{
		int count2 = count / 2;
		int num = offset;
		EnsureSourceBuffer(count2);
		int num2 = source.Read(sourceBuffer, 0, count2);
		for (int i = 0; i < num2; i++)
		{
			buffer[num++] = sourceBuffer[i];
			buffer[num++] = sourceBuffer[i];
		}
		return num2 * 2;
	}

	private void EnsureSourceBuffer(int count)
	{
		if (sourceBuffer == null || sourceBuffer.Length < count)
		{
			sourceBuffer = new float[count];
		}
	}
}
