using System;
using System.IO;

namespace NAudio.Midi;

internal class SmpteOffsetEvent : MetaEvent
{
	private byte hours;

	private byte minutes;

	private byte seconds;

	private byte frames;

	private byte subFrames;

	public int Hours => hours;

	public int Minutes => minutes;

	public int Seconds => seconds;

	public int Frames => frames;

	public int SubFrames => subFrames;

	public SmpteOffsetEvent(BinaryReader br, int length)
	{
		if (length != 5)
		{
			throw new FormatException($"Invalid SMPTE Offset length: Got {length}, expected 5");
		}
		hours = br.ReadByte();
		minutes = br.ReadByte();
		seconds = br.ReadByte();
		frames = br.ReadByte();
		subFrames = br.ReadByte();
	}

	public override string ToString()
	{
		return $"{base.ToString()} {hours}:{minutes}:{seconds}:{frames}:{subFrames}";
	}

	public override void Export(ref long absoluteTime, BinaryWriter writer)
	{
		base.Export(ref absoluteTime, writer);
		writer.Write(hours);
		writer.Write(minutes);
		writer.Write(seconds);
		writer.Write(frames);
		writer.Write(subFrames);
	}
}
