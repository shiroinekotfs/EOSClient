using System;
using System.Collections.Generic;
using System.IO;
using NAudio.FileFormats.Wav;

namespace NAudio.Wave;

public class WaveFileReader : WaveStream
{
	private readonly WaveFormat waveFormat;

	private readonly bool ownInput;

	private readonly long dataPosition;

	private readonly long dataChunkLength;

	private readonly List<RiffChunk> chunks = new List<RiffChunk>();

	private readonly object lockObject = new object();

	private Stream waveStream;

	public List<RiffChunk> ExtraChunks => chunks;

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
			throw new InvalidOperationException("Sample count is calculated only for the standard encodings");
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

	public WaveFileReader(string waveFile)
		: this(File.OpenRead(waveFile))
	{
		ownInput = true;
	}

	public WaveFileReader(Stream inputStream)
	{
		waveStream = inputStream;
		WaveFileChunkReader waveFileChunkReader = new WaveFileChunkReader();
		waveFileChunkReader.ReadWaveHeader(inputStream);
		waveFormat = waveFileChunkReader.WaveFormat;
		dataPosition = waveFileChunkReader.DataChunkPosition;
		dataChunkLength = waveFileChunkReader.DataChunkLength;
		chunks = waveFileChunkReader.RiffChunks;
		Position = 0L;
	}

	public byte[] GetChunkData(RiffChunk chunk)
	{
		long position = waveStream.Position;
		waveStream.Position = chunk.StreamPosition;
		byte[] array = new byte[chunk.Length];
		waveStream.Read(array, 0, array.Length);
		waveStream.Position = position;
		return array;
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
				count = (int)(dataChunkLength - Position);
			}
			return waveStream.Read(array, offset, count);
		}
	}

	public float[] ReadNextSampleFrame()
	{
		switch (waveFormat.Encoding)
		{
		default:
			throw new InvalidOperationException("Only 16, 24 or 32 bit PCM or IEEE float audio data supported");
		case WaveFormatEncoding.Pcm:
		case WaveFormatEncoding.IeeeFloat:
		case WaveFormatEncoding.Extensible:
		{
			float[] array = new float[waveFormat.Channels];
			int num = waveFormat.Channels * (waveFormat.BitsPerSample / 8);
			byte[] array2 = new byte[num];
			int num2 = Read(array2, 0, num);
			if (num2 == 0)
			{
				return null;
			}
			if (num2 < num)
			{
				throw new InvalidDataException("Unexpected end of file");
			}
			int num3 = 0;
			for (int i = 0; i < waveFormat.Channels; i++)
			{
				if (waveFormat.BitsPerSample == 16)
				{
					array[i] = (float)BitConverter.ToInt16(array2, num3) / 32768f;
					num3 += 2;
					continue;
				}
				if (waveFormat.BitsPerSample == 24)
				{
					array[i] = (float)(((sbyte)array2[num3 + 2] << 16) | (array2[num3 + 1] << 8) | array2[num3]) / 8388608f;
					num3 += 3;
					continue;
				}
				if (waveFormat.BitsPerSample == 32 && waveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
				{
					array[i] = BitConverter.ToSingle(array2, num3);
					num3 += 4;
					continue;
				}
				if (waveFormat.BitsPerSample == 32)
				{
					array[i] = (float)BitConverter.ToInt32(array2, num3) / 2.1474836E+09f;
					num3 += 4;
					continue;
				}
				throw new InvalidOperationException("Unsupported bit depth");
			}
			return array;
		}
		}
	}

	[Obsolete("Use ReadNextSampleFrame instead (this version does not support stereo properly)")]
	public bool TryReadFloat(out float sampleValue)
	{
		float[] array = ReadNextSampleFrame();
		sampleValue = ((array != null) ? array[0] : 0f);
		return array != null;
	}
}
