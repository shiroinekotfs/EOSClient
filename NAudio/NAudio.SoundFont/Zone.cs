namespace NAudio.SoundFont;

public class Zone
{
	internal ushort generatorIndex;

	internal ushort modulatorIndex;

	internal ushort generatorCount;

	internal ushort modulatorCount;

	private Modulator[] modulators;

	private Generator[] generators;

	public Modulator[] Modulators
	{
		get
		{
			return modulators;
		}
		set
		{
			modulators = value;
		}
	}

	public Generator[] Generators
	{
		get
		{
			return generators;
		}
		set
		{
			generators = value;
		}
	}

	public override string ToString()
	{
		return $"Zone {generatorCount} Gens:{generatorIndex} {modulatorCount} Mods:{modulatorIndex}";
	}
}
