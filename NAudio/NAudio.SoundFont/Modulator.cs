namespace NAudio.SoundFont;

public class Modulator
{
	private ModulatorType sourceModulationData;

	private GeneratorEnum destinationGenerator;

	private short amount;

	private ModulatorType sourceModulationAmount;

	private TransformEnum sourceTransform;

	public ModulatorType SourceModulationData
	{
		get
		{
			return sourceModulationData;
		}
		set
		{
			sourceModulationData = value;
		}
	}

	public GeneratorEnum DestinationGenerator
	{
		get
		{
			return destinationGenerator;
		}
		set
		{
			destinationGenerator = value;
		}
	}

	public short Amount
	{
		get
		{
			return amount;
		}
		set
		{
			amount = value;
		}
	}

	public ModulatorType SourceModulationAmount
	{
		get
		{
			return sourceModulationAmount;
		}
		set
		{
			sourceModulationAmount = value;
		}
	}

	public TransformEnum SourceTransform
	{
		get
		{
			return sourceTransform;
		}
		set
		{
			sourceTransform = value;
		}
	}

	public override string ToString()
	{
		return $"Modulator {sourceModulationData} {destinationGenerator} {amount} {sourceModulationAmount} {sourceTransform}";
	}
}
