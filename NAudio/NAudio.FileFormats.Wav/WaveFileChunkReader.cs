using System;
using System.Collections.Generic;
using System.IO;
using NAudio.Utils;
using NAudio.Wave;

namespace NAudio.FileFormats.Wav;

internal class WaveFileChunkReader
{
	private WaveFormat waveFormat;

	private long dataChunkPosition;

	private long dataChunkLength;

	private List<RiffChunk> riffChunks;

	private readonly bool strictMode;

	private bool isRf64;

	private readonly bool storeAllChunks;

	private long riffSize;

	public WaveFormat WaveFormat => waveFormat;

	public long DataChunkPosition => dataChunkPosition;

	public long DataChunkLength => dataChunkLength;

	public List<RiffChunk> RiffChunks => riffChunks;

	public WaveFileChunkReader()
	{
		storeAllChunks = true;
		strictMode = false;
	}

	public void ReadWaveHeader(Stream stream)
	{
		dataChunkPosition = -1L;
		waveFormat = null;
		riffChunks = new List<RiffChunk>();
		dataChunkLength = 0L;
		BinaryReader binaryReader = new BinaryReader(stream);
		ReadRiffHeader(binaryReader);
		riffSize = binaryReader.ReadUInt32();
		if (binaryReader.ReadInt32() != ChunkIdentifier.ChunkIdentifierToInt32("WAVE"))
		{
			throw new FormatException("Not a WAVE file - no WAVE header");
		}
		if (isRf64)
		{
			ReadDs64Chunk(binaryReader);
		}
		int num = ChunkIdentifier.ChunkIdentifierToInt32("data");
		int num2 = ChunkIdentifier.ChunkIdentifierToInt32("fmt ");
		long num3 = Math.Min(riffSize + 8, stream.Length);
		while (stream.Position <= num3 - 8)
		{
			int num4 = binaryReader.ReadInt32();
			uint num5 = binaryReader.ReadUInt32();
			if (num4 == num)
			{
				dataChunkPosition = stream.Position;
				if (!isRf64)
				{
					dataChunkLength = num5;
				}
				stream.Position += num5;
				continue;
			}
			if (num4 == num2)
			{
				if (num5 > int.MaxValue)
				{
					throw new InvalidDataException($"Format chunk length must be between 0 and {int.MaxValue}.");
				}
				waveFormat = WaveFormat.FromFormatChunk(binaryReader, (int)num5);
				continue;
			}
			if (num5 > stream.Length - stream.Position)
			{
				if (!strictMode)
				{
				}
				break;
			}
			if (storeAllChunks)
			{
				if (num5 > int.MaxValue)
				{
					throw new InvalidDataException($"RiffChunk chunk length must be between 0 and {int.MaxValue}.");
				}
				riffChunks.Add(GetRiffChunk(stream, num4, (int)num5));
			}
			stream.Position += num5;
		}
		if (waveFormat == null)
		{
			throw new FormatException("Invalid WAV file - No fmt chunk found");
		}
		if (dataChunkPosition == -1)
		{
			throw new FormatException("Invalid WAV file - No data chunk found");
		}
	}

	private void ReadDs64Chunk(BinaryReader reader)
	{
		int num = ChunkIdentifier.ChunkIdentifierToInt32("ds64");
		int num2 = reader.ReadInt32();
		if (num2 != num)
		{
			throw new FormatException("Invalid RF64 WAV file - No ds64 chunk found");
		}
		int num3 = reader.ReadInt32();
		riffSize = reader.ReadInt64();
		dataChunkLength = reader.ReadInt64();
		reader.ReadInt64();
		reader.ReadBytes(num3 - 24);
	}

	private static RiffChunk GetRiffChunk(Stream stream, int chunkIdentifier, int chunkLength)
	{
		return new RiffChunk(chunkIdentifier, chunkLength, stream.Position);
	}

	private void ReadRiffHeader(BinaryReader br)
	{
		int num = br.ReadInt32();
		if (num == ChunkIdentifier.ChunkIdentifierToInt32("RF64"))
		{
			isRf64 = true;
		}
		else if (num != ChunkIdentifier.ChunkIdentifierToInt32("RIFF"))
		{
			throw new FormatException("Not a WAVE file - no RIFF header");
		}
	}
}
