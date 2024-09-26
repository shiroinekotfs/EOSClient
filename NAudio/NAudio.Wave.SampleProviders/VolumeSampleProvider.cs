namespace NAudio.Wave.SampleProviders;

public class VolumeSampleProvider : ISampleProvider
{
	private readonly ISampleProvider source;

	private float volume;

	public WaveFormat WaveFormat => source.WaveFormat;

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

	public VolumeSampleProvider(ISampleProvider source)
	{
		this.source = source;
		volume = 1f;
	}

	public int Read(float[] buffer, int offset, int sampleCount)
	{
		int result = source.Read(buffer, offset, sampleCount);
		if (volume != 1f)
		{
			for (int i = 0; i < sampleCount; i++)
			{
				buffer[offset + i] *= volume;
			}
		}
		return result;
	}
}
