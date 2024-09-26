namespace NAudio.Dsp;

internal class AttRelEnvelope
{
	protected const double DC_OFFSET = 1E-25;

	private readonly EnvelopeDetector attack;

	private readonly EnvelopeDetector release;

	public double Attack
	{
		get
		{
			return attack.TimeConstant;
		}
		set
		{
			attack.TimeConstant = value;
		}
	}

	public double Release
	{
		get
		{
			return release.TimeConstant;
		}
		set
		{
			release.TimeConstant = value;
		}
	}

	public double SampleRate
	{
		get
		{
			return attack.SampleRate;
		}
		set
		{
			EnvelopeDetector envelopeDetector = attack;
			double sampleRate = (release.SampleRate = value);
			envelopeDetector.SampleRate = sampleRate;
		}
	}

	public AttRelEnvelope(double attackMilliseconds, double releaseMilliseconds, double sampleRate)
	{
		attack = new EnvelopeDetector(attackMilliseconds, sampleRate);
		release = new EnvelopeDetector(releaseMilliseconds, sampleRate);
	}

	public void Run(double inValue, ref double state)
	{
		if (inValue > state)
		{
			attack.Run(inValue, ref state);
		}
		else
		{
			release.Run(inValue, ref state);
		}
	}
}
