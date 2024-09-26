namespace NAudio.SoundFont;

public class Preset
{
	private string name;

	private ushort patchNumber;

	private ushort bank;

	internal ushort startPresetZoneIndex;

	internal ushort endPresetZoneIndex;

	internal uint library;

	internal uint genre;

	internal uint morphology;

	private Zone[] zones;

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
		}
	}

	public ushort PatchNumber
	{
		get
		{
			return patchNumber;
		}
		set
		{
			patchNumber = value;
		}
	}

	public ushort Bank
	{
		get
		{
			return bank;
		}
		set
		{
			bank = value;
		}
	}

	public Zone[] Zones
	{
		get
		{
			return zones;
		}
		set
		{
			zones = value;
		}
	}

	public override string ToString()
	{
		return $"{bank}-{patchNumber} {name}";
	}
}
