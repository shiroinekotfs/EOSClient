namespace NAudio.Wave.SampleProviders;

public class Pcm24BitToSampleProvider : SampleProviderConverterBase
{
	public Pcm24BitToSampleProvider(IWaveProvider source)
		: base(source)
	{
	}

	public override int Read(float[] buffer, int offset, int count)
	{
		int num = count * 3;
		EnsureSourceBuffer(num);
		int num2 = source.Read(sourceBuffer, 0, num);
		int num3 = offset;
		for (int i = 0; i < num2; i += 3)
		{
			buffer[num3++] = (float)(((sbyte)sourceBuffer[i + 2] << 16) | (sourceBuffer[i + 1] << 8) | sourceBuffer[i]) / 8388608f;
		}
		return num2 / 3;
	}
}
