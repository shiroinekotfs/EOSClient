using System;

namespace NAudio.Wave.SampleProviders;

public class SampleChannel : ISampleProvider
{
	private readonly VolumeSampleProvider volumeProvider;

	private readonly MeteringSampleProvider preVolumeMeter;

	private readonly WaveFormat waveFormat;

	public WaveFormat WaveFormat => waveFormat;

	public float Volume
	{
		get
		{
			return volumeProvider.Volume;
		}
		set
		{
			volumeProvider.Volume = value;
		}
	}

	public event EventHandler<StreamVolumeEventArgs> PreVolumeMeter
	{
		add
		{
			preVolumeMeter.StreamVolume += value;
		}
		remove
		{
			preVolumeMeter.StreamVolume -= value;
		}
	}

	public SampleChannel(IWaveProvider waveProvider)
		: this(waveProvider, forceStereo: false)
	{
	}

	public SampleChannel(IWaveProvider waveProvider, bool forceStereo)
	{
		ISampleProvider sampleProvider = SampleProviderConverters.ConvertWaveProviderIntoSampleProvider(waveProvider);
		if (sampleProvider.WaveFormat.Channels == 1 && forceStereo)
		{
			sampleProvider = new MonoToStereoSampleProvider(sampleProvider);
		}
		waveFormat = sampleProvider.WaveFormat;
		preVolumeMeter = new MeteringSampleProvider(sampleProvider);
		volumeProvider = new VolumeSampleProvider(preVolumeMeter);
	}

	public int Read(float[] buffer, int offset, int sampleCount)
	{
		return volumeProvider.Read(buffer, offset, sampleCount);
	}
}
