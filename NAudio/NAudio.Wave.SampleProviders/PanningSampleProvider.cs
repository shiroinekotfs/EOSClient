using System;
using NAudio.Utils;

namespace NAudio.Wave.SampleProviders;

public class PanningSampleProvider : ISampleProvider
{
	private readonly ISampleProvider source;

	private float pan;

	private float leftMultiplier;

	private float rightMultiplier;

	private readonly WaveFormat waveFormat;

	private float[] sourceBuffer;

	private IPanStrategy panStrategy;

	public float Pan
	{
		get
		{
			return pan;
		}
		set
		{
			if (value < -1f || value > 1f)
			{
				throw new ArgumentOutOfRangeException("value", "Pan must be in the range -1 to 1");
			}
			pan = value;
			UpdateMultipliers();
		}
	}

	public IPanStrategy PanStrategy
	{
		get
		{
			return panStrategy;
		}
		set
		{
			panStrategy = value;
			UpdateMultipliers();
		}
	}

	public WaveFormat WaveFormat => waveFormat;

	public PanningSampleProvider(ISampleProvider source)
	{
		if (source.WaveFormat.Channels != 1)
		{
			throw new ArgumentException("Source sample provider must be mono");
		}
		this.source = source;
		waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(source.WaveFormat.SampleRate, 2);
		panStrategy = new SinPanStrategy();
	}

	private void UpdateMultipliers()
	{
		StereoSamplePair multipliers = panStrategy.GetMultipliers(Pan);
		leftMultiplier = multipliers.Left;
		rightMultiplier = multipliers.Right;
	}

	public int Read(float[] buffer, int offset, int count)
	{
		int num = count / 2;
		sourceBuffer = BufferHelpers.Ensure(sourceBuffer, num);
		int num2 = source.Read(sourceBuffer, 0, num);
		int num3 = offset;
		for (int i = 0; i < num2; i++)
		{
			buffer[num3++] = leftMultiplier * sourceBuffer[i];
			buffer[num3++] = rightMultiplier * sourceBuffer[i];
		}
		return num2 * 2;
	}
}
