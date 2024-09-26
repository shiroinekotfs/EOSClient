using System;
using NAudio.Dsp;

namespace NAudio.Wave.SampleProviders;

public class AdsrSampleProvider : ISampleProvider
{
	private readonly ISampleProvider source;

	private readonly EnvelopeGenerator adsr;

	private float attackSeconds;

	private float releaseSeconds;

	public float AttackSeconds
	{
		get
		{
			return attackSeconds;
		}
		set
		{
			attackSeconds = value;
			adsr.AttackRate = attackSeconds * (float)WaveFormat.SampleRate;
		}
	}

	public float ReleaseSeconds
	{
		get
		{
			return releaseSeconds;
		}
		set
		{
			releaseSeconds = value;
			adsr.ReleaseRate = releaseSeconds * (float)WaveFormat.SampleRate;
		}
	}

	public WaveFormat WaveFormat => source.WaveFormat;

	public AdsrSampleProvider(ISampleProvider source)
	{
		if (source.WaveFormat.Channels > 1)
		{
			throw new ArgumentException("Currently only supports mono inputs");
		}
		this.source = source;
		adsr = new EnvelopeGenerator();
		AttackSeconds = 0.01f;
		adsr.SustainLevel = 1f;
		adsr.DecayRate = 0f * (float)WaveFormat.SampleRate;
		ReleaseSeconds = 0.3f;
		adsr.Gate(gate: true);
	}

	public int Read(float[] buffer, int offset, int count)
	{
		if (adsr.State == EnvelopeGenerator.EnvelopeState.Idle)
		{
			return 0;
		}
		int num = source.Read(buffer, offset, count);
		for (int i = 0; i < num; i++)
		{
			buffer[offset++] *= adsr.Process();
		}
		return num;
	}

	public void Stop()
	{
		adsr.Gate(gate: false);
	}
}
