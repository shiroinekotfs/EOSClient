using System.IO;
using System.Runtime.InteropServices;

namespace NAudio.Wave;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public class WaveFormatExtraData : WaveFormat
{
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
	private byte[] extraData = new byte[100];

	public byte[] ExtraData => extraData;

	internal WaveFormatExtraData()
	{
	}

	public WaveFormatExtraData(BinaryReader reader)
		: base(reader)
	{
		ReadExtraData(reader);
	}

	internal void ReadExtraData(BinaryReader reader)
	{
		if (extraSize > 0)
		{
			reader.Read(extraData, 0, extraSize);
		}
	}

	public override void Serialize(BinaryWriter writer)
	{
		base.Serialize(writer);
		if (extraSize > 0)
		{
			writer.Write(extraData, 0, extraSize);
		}
	}
}
