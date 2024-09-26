using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NAudio.Midi;

public class SysexEvent : MidiEvent
{
	private byte[] data;

	public static SysexEvent ReadSysexEvent(BinaryReader br)
	{
		SysexEvent sysexEvent = new SysexEvent();
		List<byte> list = new List<byte>();
		bool flag = true;
		while (flag)
		{
			byte b = br.ReadByte();
			if (b == 247)
			{
				flag = false;
			}
			else
			{
				list.Add(b);
			}
		}
		sysexEvent.data = list.ToArray();
		return sysexEvent;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		byte[] array = data;
		foreach (byte b in array)
		{
			stringBuilder.AppendFormat("{0:X2} ", b);
		}
		return $"{base.AbsoluteTime} Sysex: {data.Length} bytes\r\n{stringBuilder.ToString()}";
	}

	public override void Export(ref long absoluteTime, BinaryWriter writer)
	{
		base.Export(ref absoluteTime, writer);
		writer.Write(data, 0, data.Length);
		writer.Write((byte)247);
	}
}
