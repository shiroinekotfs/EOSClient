using System;
using System.IO;
using NAudio.Utils;

namespace NAudio.SoundFont;

internal class RiffChunk
{
	private string chunkID;

	private uint chunkSize;

	private long dataOffset;

	private BinaryReader riffFile;

	public string ChunkID
	{
		get
		{
			return chunkID;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("ChunkID may not be null");
			}
			if (value.Length != 4)
			{
				throw new ArgumentException("ChunkID must be four characters");
			}
			chunkID = value;
		}
	}

	public uint ChunkSize => chunkSize;

	public long DataOffset => dataOffset;

	public static RiffChunk GetTopLevelChunk(BinaryReader file)
	{
		RiffChunk riffChunk = new RiffChunk(file);
		riffChunk.ReadChunk();
		return riffChunk;
	}

	private RiffChunk(BinaryReader file)
	{
		riffFile = file;
		chunkID = "????";
		chunkSize = 0u;
		dataOffset = 0L;
	}

	public string ReadChunkID()
	{
		byte[] array = riffFile.ReadBytes(4);
		if (array.Length != 4)
		{
			throw new InvalidDataException("Couldn't read Chunk ID");
		}
		return ByteEncoding.Instance.GetString(array, 0, array.Length);
	}

	private void ReadChunk()
	{
		chunkID = ReadChunkID();
		chunkSize = riffFile.ReadUInt32();
		dataOffset = riffFile.BaseStream.Position;
	}

	public RiffChunk GetNextSubChunk()
	{
		if (riffFile.BaseStream.Position + 8 < dataOffset + chunkSize)
		{
			RiffChunk riffChunk = new RiffChunk(riffFile);
			riffChunk.ReadChunk();
			return riffChunk;
		}
		return null;
	}

	public byte[] GetData()
	{
		riffFile.BaseStream.Position = dataOffset;
		byte[] array = riffFile.ReadBytes((int)chunkSize);
		if (array.Length != chunkSize)
		{
			throw new InvalidDataException($"Couldn't read chunk's data Chunk: {this}, read {array.Length} bytes");
		}
		return array;
	}

	public string GetDataAsString()
	{
		byte[] data = GetData();
		if (data == null)
		{
			return null;
		}
		return ByteEncoding.Instance.GetString(data, 0, data.Length);
	}

	public T GetDataAsStructure<T>(StructureBuilder<T> s)
	{
		riffFile.BaseStream.Position = dataOffset;
		if (s.Length != chunkSize)
		{
			throw new InvalidDataException($"Chunk size is: {chunkSize} so can't read structure of: {s.Length}");
		}
		return s.Read(riffFile);
	}

	public T[] GetDataAsStructureArray<T>(StructureBuilder<T> s)
	{
		riffFile.BaseStream.Position = dataOffset;
		if (chunkSize % s.Length != 0)
		{
			throw new InvalidDataException($"Chunk size is: {chunkSize} not a multiple of structure size: {s.Length}");
		}
		int num = (int)(chunkSize / s.Length);
		T[] array = new T[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = s.Read(riffFile);
		}
		return array;
	}

	public override string ToString()
	{
		return $"RiffChunk ID: {ChunkID} Size: {ChunkSize} Data Offset: {DataOffset}";
	}
}
