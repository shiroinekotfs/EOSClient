using System;
using System.IO;

namespace NAudio.Midi;

public class TempoEvent : MetaEvent
{
	private int microsecondsPerQuarterNote;

	public int MicrosecondsPerQuarterNote
	{
		get
		{
			return microsecondsPerQuarterNote;
		}
		set
		{
			microsecondsPerQuarterNote = value;
		}
	}

	public double Tempo
	{
		get
		{
			return 60000000.0 / (double)microsecondsPerQuarterNote;
		}
		set
		{
			microsecondsPerQuarterNote = (int)(60000000.0 / value);
		}
	}

	public TempoEvent(BinaryReader br, int length)
	{
		if (length != 3)
		{
			throw new FormatException("Invalid tempo length");
		}
		microsecondsPerQuarterNote = (br.ReadByte() << 16) + (br.ReadByte() << 8) + br.ReadByte();
	}

	public TempoEvent(int microsecondsPerQuarterNote, long absoluteTime)
		: base(MetaEventType.SetTempo, 3, absoluteTime)
	{
		this.microsecondsPerQuarterNote = microsecondsPerQuarterNote;
	}

	public override string ToString()
	{
		return string.Format("{0} {2}bpm ({1})", base.ToString(), microsecondsPerQuarterNote, 60000000 / microsecondsPerQuarterNote);
	}

	public override void Export(ref long absoluteTime, BinaryWriter writer)
	{
		base.Export(ref absoluteTime, writer);
		writer.Write((byte)((uint)(microsecondsPerQuarterNote >> 16) & 0xFFu));
		writer.Write((byte)((uint)(microsecondsPerQuarterNote >> 8) & 0xFFu));
		writer.Write((byte)((uint)microsecondsPerQuarterNote & 0xFFu));
	}
}
