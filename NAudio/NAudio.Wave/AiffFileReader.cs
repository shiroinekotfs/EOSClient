using System;
using System.Collections.Generic;
using System.IO;
using NAudio.Utils;

namespace NAudio.Wave;

public class AiffFileReader : WaveStream
{
	public struct AiffChunk
	{
		public string ChunkName;

		public uint ChunkLength;

		public uint ChunkStart;

		public AiffChunk(uint start, string name, uint length)
		{
			ChunkStart = start;
			ChunkName = name;
			ChunkLength = length + ((length % 2 == 1) ? 1u : 0u);
		}
	}

	private readonly WaveFormat waveFormat;

	private readonly bool ownInput;

	private readonly long dataPosition;

	private readonly int dataChunkLength;

	private readonly List<AiffChunk> chunks = new List<AiffChunk>();

	private Stream waveStream;

	private readonly object lockObject = new object();

	public override WaveFormat WaveFormat => waveFormat;

	public override long Length => dataChunkLength;

	public long SampleCount
	{
		get
		{
			if (waveFormat.Encoding == WaveFormatEncoding.Pcm || waveFormat.Encoding == WaveFormatEncoding.Extensible || waveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
			{
				return dataChunkLength / BlockAlign;
			}
			throw new FormatException("Sample count is calculated only for the standard encodings");
		}
	}

	public override long Position
	{
		get
		{
			return waveStream.Position - dataPosition;
		}
		set
		{
			lock (lockObject)
			{
				value = Math.Min(value, Length);
				value -= value % waveFormat.BlockAlign;
				waveStream.Position = value + dataPosition;
			}
		}
	}

	public AiffFileReader(string aiffFile)
		: this(File.OpenRead(aiffFile))
	{
		ownInput = true;
	}

	public AiffFileReader(Stream inputStream)
	{
		waveStream = inputStream;
		ReadAiffHeader(waveStream, out waveFormat, out dataPosition, out dataChunkLength, chunks);
		Position = 0L;
	}

	public static void ReadAiffHeader(Stream stream, out WaveFormat format, out long dataChunkPosition, out int dataChunkLength, List<AiffChunk> chunks)
	{
		dataChunkPosition = -1L;
		format = null;
		BinaryReader binaryReader = new BinaryReader(stream);
		if (ReadChunkName(binaryReader) != "FORM")
		{
			throw new FormatException("Not an AIFF file - no FORM header.");
		}
		ConvertInt(binaryReader.ReadBytes(4));
		string text = ReadChunkName(binaryReader);
		if (text != "AIFC" && text != "AIFF")
		{
			throw new FormatException("Not an AIFF file - no AIFF/AIFC header.");
		}
		dataChunkLength = 0;
		while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
		{
			AiffChunk item = ReadChunkHeader(binaryReader);
			if (item.ChunkName == "COMM")
			{
				short channels = ConvertShort(binaryReader.ReadBytes(2));
				ConvertInt(binaryReader.ReadBytes(4));
				short bits = ConvertShort(binaryReader.ReadBytes(2));
				double num = IEEE.ConvertFromIeeeExtended(binaryReader.ReadBytes(10));
				format = new WaveFormat((int)num, bits, channels);
				if (item.ChunkLength > 18 && text == "AIFC")
				{
					string text2 = new string(binaryReader.ReadChars(4)).ToLower();
					if (text2 != "none")
					{
						throw new FormatException("Compressed AIFC is not supported.");
					}
					binaryReader.ReadBytes((int)(item.ChunkLength - 22));
				}
				else
				{
					binaryReader.ReadBytes((int)(item.ChunkLength - 18));
				}
			}
			else if (item.ChunkName == "SSND")
			{
				uint num2 = ConvertInt(binaryReader.ReadBytes(4));
				ConvertInt(binaryReader.ReadBytes(4));
				dataChunkPosition = item.ChunkStart + 16 + num2;
				dataChunkLength = (int)(item.ChunkLength - 8);
				binaryReader.ReadBytes((int)(item.ChunkLength - 8));
			}
			else
			{
				chunks?.Add(item);
				binaryReader.ReadBytes((int)item.ChunkLength);
			}
			if (item.ChunkName == "\0\0\0\0")
			{
				break;
			}
		}
		if (format == null)
		{
			throw new FormatException("Invalid AIFF file - No COMM chunk found.");
		}
		if (dataChunkPosition == -1)
		{
			throw new FormatException("Invalid AIFF file - No SSND chunk found.");
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && waveStream != null)
		{
			if (ownInput)
			{
				waveStream.Close();
			}
			waveStream = null;
		}
		base.Dispose(disposing);
	}

	public override int Read(byte[] array, int offset, int count)
	{
		if (count % waveFormat.BlockAlign != 0)
		{
			throw new ArgumentException($"Must read complete blocks: requested {count}, block align is {WaveFormat.BlockAlign}");
		}
		lock (lockObject)
		{
			if (Position + count > dataChunkLength)
			{
				count = dataChunkLength - (int)Position;
			}
			byte[] array2 = new byte[count];
			int num = waveStream.Read(array2, offset, count);
			int num2 = WaveFormat.BitsPerSample / 8;
			for (int i = 0; i < num; i += num2)
			{
				if (WaveFormat.BitsPerSample == 8)
				{
					array[i] = array2[i];
					continue;
				}
				if (WaveFormat.BitsPerSample == 16)
				{
					array[i] = array2[i + 1];
					array[i + 1] = array2[i];
					continue;
				}
				if (WaveFormat.BitsPerSample == 24)
				{
					array[i] = array2[i + 2];
					array[i + 1] = array2[i + 1];
					array[i + 2] = array2[i];
					continue;
				}
				if (WaveFormat.BitsPerSample == 32)
				{
					array[i] = array2[i + 3];
					array[i + 1] = array2[i + 2];
					array[i + 2] = array2[i + 1];
					array[i + 3] = array2[i];
					continue;
				}
				throw new FormatException("Unsupported PCM format.");
			}
			return num;
		}
	}

	private static uint ConvertInt(byte[] buffer)
	{
		if (buffer.Length != 4)
		{
			throw new Exception("Incorrect length for long.");
		}
		return (uint)((buffer[0] << 24) | (buffer[1] << 16) | (buffer[2] << 8) | buffer[3]);
	}

	private static short ConvertShort(byte[] buffer)
	{
		if (buffer.Length != 2)
		{
			throw new Exception("Incorrect length for int.");
		}
		return (short)((buffer[0] << 8) | buffer[1]);
	}

	private static AiffChunk ReadChunkHeader(BinaryReader br)
	{
		return new AiffChunk((uint)br.BaseStream.Position, ReadChunkName(br), ConvertInt(br.ReadBytes(4)));
	}

	private static string ReadChunkName(BinaryReader br)
	{
		return new string(br.ReadChars(4));
	}
}
