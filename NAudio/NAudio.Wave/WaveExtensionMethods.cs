using NAudio.Wave.SampleProviders;

namespace NAudio.Wave;

public static class WaveExtensionMethods
{
	public static ISampleProvider ToSampleProvider(this IWaveProvider waveProvider)
	{
		return SampleProviderConverters.ConvertWaveProviderIntoSampleProvider(waveProvider);
	}

	public static void Init(this IWavePlayer wavePlayer, ISampleProvider sampleProvider, bool convertTo16Bit = false)
	{
		object obj;
		if (!convertTo16Bit)
		{
			IWaveProvider waveProvider = new SampleToWaveProvider(sampleProvider);
			obj = waveProvider;
		}
		else
		{
			obj = new SampleToWaveProvider16(sampleProvider);
		}
		IWaveProvider waveProvider2 = (IWaveProvider)obj;
		wavePlayer.Init(waveProvider2);
	}
}
