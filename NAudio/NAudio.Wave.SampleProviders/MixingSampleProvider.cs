using System;
using System.Collections.Generic;
using NAudio.Utils;

namespace NAudio.Wave.SampleProviders;

public class MixingSampleProvider : ISampleProvider
{
	private const int maxInputs = 1024;

	private List<ISampleProvider> sources;

	private WaveFormat waveFormat;

	private float[] sourceBuffer;

	public bool ReadFully { get; set; }

	public WaveFormat WaveFormat => waveFormat;

	public MixingSampleProvider(WaveFormat waveFormat)
	{
		if (waveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
		{
			throw new ArgumentException("Mixer wave format must be IEEE float");
		}
		sources = new List<ISampleProvider>();
		this.waveFormat = waveFormat;
	}

	public MixingSampleProvider(IEnumerable<ISampleProvider> sources)
	{
		this.sources = new List<ISampleProvider>();
		foreach (ISampleProvider source in sources)
		{
			AddMixerInput(source);
		}
		if (this.sources.Count == 0)
		{
			throw new ArgumentException("Must provide at least one input in this constructor");
		}
	}

	public void AddMixerInput(IWaveProvider mixerInput)
	{
		AddMixerInput(SampleProviderConverters.ConvertWaveProviderIntoSampleProvider(mixerInput));
	}

	public void AddMixerInput(ISampleProvider mixerInput)
	{
		lock (sources)
		{
			if (sources.Count >= 1024)
			{
				throw new InvalidOperationException("Too many mixer inputs");
			}
			sources.Add(mixerInput);
		}
		if (waveFormat == null)
		{
			waveFormat = mixerInput.WaveFormat;
		}
		else if (WaveFormat.SampleRate != mixerInput.WaveFormat.SampleRate || WaveFormat.Channels != mixerInput.WaveFormat.Channels)
		{
			throw new ArgumentException("All mixer inputs must have the same WaveFormat");
		}
	}

	public void RemoveMixerInput(ISampleProvider mixerInput)
	{
		lock (sources)
		{
			sources.Remove(mixerInput);
		}
	}

	public void RemoveAllMixerInputs()
	{
		lock (sources)
		{
			sources.Clear();
		}
	}

	public int Read(float[] buffer, int offset, int count)
	{
		int num = 0;
		sourceBuffer = BufferHelpers.Ensure(sourceBuffer, count);
		lock (sources)
		{
			for (int num2 = sources.Count - 1; num2 >= 0; num2--)
			{
				ISampleProvider sampleProvider = sources[num2];
				int num3 = sampleProvider.Read(sourceBuffer, 0, count);
				int num4 = offset;
				for (int i = 0; i < num3; i++)
				{
					if (i >= num)
					{
						buffer[num4++] = sourceBuffer[i];
					}
					else
					{
						buffer[num4++] += sourceBuffer[i];
					}
				}
				num = Math.Max(num3, num);
				if (num3 == 0)
				{
					sources.RemoveAt(num2);
				}
			}
		}
		if (ReadFully && num < count)
		{
			int num5 = offset + num;
			while (num5 < offset + count)
			{
				buffer[num5++] = 0f;
			}
			num = count;
		}
		return num;
	}
}
