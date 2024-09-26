using System;
using System.IO;

namespace NAudio.Midi;

public class KeySignatureEvent : MetaEvent
{
	private byte sharpsFlats;

	private byte majorMinor;

	public int SharpsFlats => sharpsFlats;

	public int MajorMinor => majorMinor;

	public KeySignatureEvent(BinaryReader br, int length)
	{
		if (length != 2)
		{
			throw new FormatException("Invalid key signature length");
		}
		sharpsFlats = br.ReadByte();
		majorMinor = br.ReadByte();
	}

	public KeySignatureEvent(int sharpsFlats, int majorMinor, long absoluteTime)
		: base(MetaEventType.KeySignature, 2, absoluteTime)
	{
		this.sharpsFlats = (byte)sharpsFlats;
		this.majorMinor = (byte)majorMinor;
	}

	public override string ToString()
	{
		return $"{base.ToString()} {sharpsFlats} {majorMinor}";
	}

	public override void Export(ref long absoluteTime, BinaryWriter writer)
	{
		base.Export(ref absoluteTime, writer);
		writer.Write(sharpsFlats);
		writer.Write(majorMinor);
	}
}
