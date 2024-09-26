namespace NAudio.SoundFont;

public class Generator
{
	private GeneratorEnum generatorType;

	private ushort rawAmount;

	private Instrument instrument;

	private SampleHeader sampleHeader;

	public GeneratorEnum GeneratorType
	{
		get
		{
			return generatorType;
		}
		set
		{
			generatorType = value;
		}
	}

	public ushort UInt16Amount
	{
		get
		{
			return rawAmount;
		}
		set
		{
			rawAmount = value;
		}
	}

	public short Int16Amount
	{
		get
		{
			return (short)rawAmount;
		}
		set
		{
			rawAmount = (ushort)value;
		}
	}

	public byte LowByteAmount
	{
		get
		{
			return (byte)(rawAmount & 0xFFu);
		}
		set
		{
			rawAmount &= 65280;
			rawAmount += value;
		}
	}

	public byte HighByteAmount
	{
		get
		{
			return (byte)((rawAmount & 0xFF00) >> 8);
		}
		set
		{
			rawAmount &= 255;
			rawAmount += (ushort)(value << 8);
		}
	}

	public Instrument Instrument
	{
		get
		{
			return instrument;
		}
		set
		{
			instrument = value;
		}
	}

	public SampleHeader SampleHeader
	{
		get
		{
			return sampleHeader;
		}
		set
		{
			sampleHeader = value;
		}
	}

	public override string ToString()
	{
		if (generatorType == GeneratorEnum.Instrument)
		{
			return $"Generator Instrument {instrument.Name}";
		}
		if (generatorType == GeneratorEnum.SampleID)
		{
			return $"Generator SampleID {sampleHeader}";
		}
		return $"Generator {generatorType} {rawAmount}";
	}
}
