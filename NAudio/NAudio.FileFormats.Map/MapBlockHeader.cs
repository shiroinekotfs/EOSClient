using System.IO;

namespace NAudio.FileFormats.Map;

internal class MapBlockHeader
{
	private int length;

	private int value2;

	private short value3;

	private short value4;

	public int Length => length;

	public static MapBlockHeader Read(BinaryReader reader)
	{
		MapBlockHeader mapBlockHeader = new MapBlockHeader();
		mapBlockHeader.length = reader.ReadInt32();
		mapBlockHeader.value2 = reader.ReadInt32();
		mapBlockHeader.value3 = reader.ReadInt16();
		mapBlockHeader.value4 = reader.ReadInt16();
		return mapBlockHeader;
	}

	public override string ToString()
	{
		return $"{length} {value2:X8} {value3:X4} {value4:X4}";
	}
}
