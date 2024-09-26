using System;

namespace NAudio.Wave.SampleProviders;

internal static class SampleProviderConverters
{
	public static ISampleProvider ConvertWaveProviderIntoSampleProvider(IWaveProvider waveProvider)
	{
		if (waveProvider.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
		{
			if (waveProvider.WaveFormat.BitsPerSample == 8)
			{
				return new Pcm8BitToSampleProvider(waveProvider);
			}
			if (waveProvider.WaveFormat.BitsPerSample == 16)
			{
				return new Pcm16BitToSampleProvider(waveProvider);
			}
			if (waveProvider.WaveFormat.BitsPerSample == 24)
			{
				return new Pcm24BitToSampleProvider(waveProvider);
			}
			if (waveProvider.WaveFormat.BitsPerSample == 32)
			{
				return new Pcm32BitToSampleProvider(waveProvider);
			}
			throw new InvalidOperationException("Unsupported bit depth");
		}
		if (waveProvider.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
		{
			if (waveProvider.WaveFormat.BitsPerSample == 64)
			{
				return new WaveToSampleProvider64(waveProvider);
			}
			return new WaveToSampleProvider(waveProvider);
		}
		throw new ArgumentException("Unsupported source encoding");
	}
}
