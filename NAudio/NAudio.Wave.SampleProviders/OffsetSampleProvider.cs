using System;

namespace NAudio.Wave.SampleProviders;

public class OffsetSampleProvider : ISampleProvider
{
	private readonly ISampleProvider sourceProvider;

	private int phase;

	private int phasePos;

	private int delayBySamples;

	private int skipOverSamples;

	private int takeSamples;

	private int leadOutSamples;

	public int DelayBySamples
	{
		get
		{
			return delayBySamples;
		}
		set
		{
			if (phase != 0)
			{
				throw new InvalidOperationException("Can't set DelayBySamples after calling Read");
			}
			if (value % WaveFormat.Channels != 0)
			{
				throw new ArgumentException("DelayBySamples must be a multiple of WaveFormat.Channels");
			}
			delayBySamples = value;
		}
	}

	public TimeSpan DelayBy
	{
		get
		{
			return SamplesToTimeSpan(delayBySamples);
		}
		set
		{
			delayBySamples = TimeSpanToSamples(value);
		}
	}

	public int SkipOverSamples
	{
		get
		{
			return skipOverSamples;
		}
		set
		{
			if (phase != 0)
			{
				throw new InvalidOperationException("Can't set SkipOverSamples after calling Read");
			}
			if (value % WaveFormat.Channels != 0)
			{
				throw new ArgumentException("SkipOverSamples must be a multiple of WaveFormat.Channels");
			}
			skipOverSamples = value;
		}
	}

	public TimeSpan SkipOver
	{
		get
		{
			return SamplesToTimeSpan(skipOverSamples);
		}
		set
		{
			skipOverSamples = TimeSpanToSamples(value);
		}
	}

	public int TakeSamples
	{
		get
		{
			return takeSamples;
		}
		set
		{
			if (phase != 0)
			{
				throw new InvalidOperationException("Can't set TakeSamples after calling Read");
			}
			if (value % WaveFormat.Channels != 0)
			{
				throw new ArgumentException("TakeSamples must be a multiple of WaveFormat.Channels");
			}
			takeSamples = value;
		}
	}

	public TimeSpan Take
	{
		get
		{
			return SamplesToTimeSpan(takeSamples);
		}
		set
		{
			takeSamples = TimeSpanToSamples(value);
		}
	}

	public int LeadOutSamples
	{
		get
		{
			return leadOutSamples;
		}
		set
		{
			if (phase != 0)
			{
				throw new InvalidOperationException("Can't set LeadOutSamples after calling Read");
			}
			if (value % WaveFormat.Channels != 0)
			{
				throw new ArgumentException("LeadOutSamples must be a multiple of WaveFormat.Channels");
			}
			leadOutSamples = value;
		}
	}

	public TimeSpan LeadOut
	{
		get
		{
			return SamplesToTimeSpan(leadOutSamples);
		}
		set
		{
			leadOutSamples = TimeSpanToSamples(value);
		}
	}

	public WaveFormat WaveFormat => sourceProvider.WaveFormat;

	private int TimeSpanToSamples(TimeSpan time)
	{
		return (int)(time.TotalSeconds * (double)WaveFormat.SampleRate) * WaveFormat.Channels;
	}

	private TimeSpan SamplesToTimeSpan(int samples)
	{
		return TimeSpan.FromSeconds((double)(samples / WaveFormat.Channels) / (double)WaveFormat.SampleRate);
	}

	public OffsetSampleProvider(ISampleProvider sourceProvider)
	{
		this.sourceProvider = sourceProvider;
	}

	public int Read(float[] buffer, int offset, int count)
	{
		int num = 0;
		if (phase == 0)
		{
			phase++;
		}
		if (phase == 1)
		{
			int num2 = Math.Min(count, DelayBySamples - phasePos);
			for (int i = 0; i < num2; i++)
			{
				buffer[offset + i] = 0f;
			}
			phasePos += num2;
			num += num2;
			if (phasePos >= DelayBySamples)
			{
				phase++;
				phasePos = 0;
			}
		}
		if (phase == 2)
		{
			if (SkipOverSamples > 0)
			{
				float[] array = new float[WaveFormat.SampleRate * WaveFormat.Channels];
				int num3;
				for (int j = 0; j < SkipOverSamples; j += num3)
				{
					int count2 = Math.Min(SkipOverSamples - j, array.Length);
					num3 = sourceProvider.Read(array, 0, count2);
					if (num3 == 0)
					{
						break;
					}
				}
			}
			phase++;
			phasePos = 0;
		}
		if (phase == 3)
		{
			int num4 = count - num;
			if (TakeSamples != 0)
			{
				num4 = Math.Min(num4, TakeSamples - phasePos);
			}
			int num5 = sourceProvider.Read(buffer, offset + num, num4);
			phasePos += num5;
			num += num5;
			if (num5 < num4)
			{
				phase++;
				phasePos = 0;
			}
		}
		if (phase == 4)
		{
			int num6 = Math.Min(count - num, LeadOutSamples - phasePos);
			for (int k = 0; k < num6; k++)
			{
				buffer[offset + num + k] = 0f;
			}
			phasePos += num6;
			num += num6;
			if (phasePos >= LeadOutSamples)
			{
				phase++;
				phasePos = 0;
			}
		}
		return num;
	}
}
