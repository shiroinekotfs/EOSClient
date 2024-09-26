using System;
using System.IO;

namespace NAudio.Midi;

public class PitchWheelChangeEvent : MidiEvent
{
	private int pitch;

	public int Pitch
	{
		get
		{
			return pitch;
		}
		set
		{
			if (value < 0 || value > 16384)
			{
				throw new ArgumentOutOfRangeException("value", "Pitch value must be in the range 0 - 0x4000");
			}
			pitch = value;
		}
	}

	public PitchWheelChangeEvent(BinaryReader br)
	{
		byte b = br.ReadByte();
		byte b2 = br.ReadByte();
		if ((b & 0x80u) != 0)
		{
			throw new FormatException("Invalid pitchwheelchange byte 1");
		}
		if ((b2 & 0x80u) != 0)
		{
			throw new FormatException("Invalid pitchwheelchange byte 2");
		}
		pitch = b + (b2 << 7);
	}

	public PitchWheelChangeEvent(long absoluteTime, int channel, int pitchWheel)
		: base(absoluteTime, channel, MidiCommandCode.PitchWheelChange)
	{
		Pitch = pitchWheel;
	}

	public override string ToString()
	{
		return $"{base.ToString()} Pitch {pitch} ({pitch - 8192})";
	}

	public override int GetAsShortMessage()
	{
		return base.GetAsShortMessage() + ((pitch & 0x7F) << 8) + (((pitch >> 7) & 0x7F) << 16);
	}

	public override void Export(ref long absoluteTime, BinaryWriter writer)
	{
		base.Export(ref absoluteTime, writer);
		writer.Write((byte)((uint)pitch & 0x7Fu));
		writer.Write((byte)((uint)(pitch >> 7) & 0x7Fu));
	}
}
