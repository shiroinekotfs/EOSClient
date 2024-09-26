using System;
using System.IO;

namespace NAudio.Midi;

public class TimeSignatureEvent : MetaEvent
{
	private byte numerator;

	private byte denominator;

	private byte ticksInMetronomeClick;

	private byte no32ndNotesInQuarterNote;

	public int Numerator => numerator;

	public int Denominator => denominator;

	public int TicksInMetronomeClick => ticksInMetronomeClick;

	public int No32ndNotesInQuarterNote => no32ndNotesInQuarterNote;

	public string TimeSignature
	{
		get
		{
			string arg = $"Unknown ({denominator})";
			switch (denominator)
			{
			case 1:
				arg = "2";
				break;
			case 2:
				arg = "4";
				break;
			case 3:
				arg = "8";
				break;
			case 4:
				arg = "16";
				break;
			case 5:
				arg = "32";
				break;
			}
			return $"{numerator}/{arg}";
		}
	}

	public TimeSignatureEvent(BinaryReader br, int length)
	{
		if (length != 4)
		{
			throw new FormatException($"Invalid time signature length: Got {length}, expected 4");
		}
		numerator = br.ReadByte();
		denominator = br.ReadByte();
		ticksInMetronomeClick = br.ReadByte();
		no32ndNotesInQuarterNote = br.ReadByte();
	}

	public TimeSignatureEvent(long absoluteTime, int numerator, int denominator, int ticksInMetronomeClick, int no32ndNotesInQuarterNote)
		: base(MetaEventType.TimeSignature, 4, absoluteTime)
	{
		this.numerator = (byte)numerator;
		this.denominator = (byte)denominator;
		this.ticksInMetronomeClick = (byte)ticksInMetronomeClick;
		this.no32ndNotesInQuarterNote = (byte)no32ndNotesInQuarterNote;
	}

	[Obsolete("Use the constructor that has absolute time first")]
	public TimeSignatureEvent(int numerator, int denominator, int ticksInMetronomeClick, int no32ndNotesInQuarterNote, long absoluteTime)
		: base(MetaEventType.TimeSignature, 4, absoluteTime)
	{
		this.numerator = (byte)numerator;
		this.denominator = (byte)denominator;
		this.ticksInMetronomeClick = (byte)ticksInMetronomeClick;
		this.no32ndNotesInQuarterNote = (byte)no32ndNotesInQuarterNote;
	}

	public override string ToString()
	{
		return $"{base.ToString()} {TimeSignature} TicksInClick:{ticksInMetronomeClick} 32ndsInQuarterNote:{no32ndNotesInQuarterNote}";
	}

	public override void Export(ref long absoluteTime, BinaryWriter writer)
	{
		base.Export(ref absoluteTime, writer);
		writer.Write(numerator);
		writer.Write(denominator);
		writer.Write(ticksInMetronomeClick);
		writer.Write(no32ndNotesInQuarterNote);
	}
}
