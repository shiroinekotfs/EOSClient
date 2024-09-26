using System.IO;
using System.Text;

namespace NAudio.Midi;

public class SequencerSpecificEvent : MetaEvent
{
	private byte[] data;

	public byte[] Data
	{
		get
		{
			return data;
		}
		set
		{
			data = value;
			metaDataLength = data.Length;
		}
	}

	public SequencerSpecificEvent(BinaryReader br, int length)
	{
		data = br.ReadBytes(length);
	}

	public SequencerSpecificEvent(byte[] data, long absoluteTime)
		: base(MetaEventType.SequencerSpecific, data.Length, absoluteTime)
	{
		this.data = data;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(base.ToString());
		stringBuilder.Append(" ");
		byte[] array = data;
		foreach (byte b in array)
		{
			stringBuilder.AppendFormat("{0:X2} ", b);
		}
		stringBuilder.Length--;
		return stringBuilder.ToString();
	}

	public override void Export(ref long absoluteTime, BinaryWriter writer)
	{
		base.Export(ref absoluteTime, writer);
		writer.Write(data);
	}
}
