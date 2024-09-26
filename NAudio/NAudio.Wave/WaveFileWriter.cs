using System;
using System.IO;
using System.Text;
using NAudio.Wave.SampleProviders;

namespace NAudio.Wave;

public class WaveFileWriter : Stream
{
	private Stream outStream;

	private readonly BinaryWriter writer;

	private long dataSizePos;

	private long factSampleCountPos;

	private long dataChunkSize;

	private readonly WaveFormat format;

	private readonly string filename;

	private readonly byte[] value24 = new byte[3];

	public string Filename => filename;

	public override long Length => dataChunkSize;

	public WaveFormat WaveFormat => format;

	public override bool CanRead => false;

	public override bool CanWrite => true;

	public override bool CanSeek => false;

	public override long Position
	{
		get
		{
			return dataChunkSize;
		}
		set
		{
			throw new InvalidOperationException("Repositioning a WaveFileWriter is not supported");
		}
	}

	public static void CreateWaveFile16(string filename, ISampleProvider sourceProvider)
	{
		CreateWaveFile(filename, new SampleToWaveProvider16(sourceProvider));
	}

	public static void CreateWaveFile(string filename, IWaveProvider sourceProvider)
	{
		using WaveFileWriter waveFileWriter = new WaveFileWriter(filename, sourceProvider.WaveFormat);
		long num = 0L;
		byte[] array = new byte[sourceProvider.WaveFormat.AverageBytesPerSecond * 4];
		while (true)
		{
			int num2 = sourceProvider.Read(array, 0, array.Length);
			if (num2 != 0)
			{
				num += num2;
				waveFileWriter.Write(array, 0, num2);
				continue;
			}
			break;
		}
	}

	public WaveFileWriter(Stream outStream, WaveFormat format)
	{
		this.outStream = outStream;
		this.format = format;
		writer = new BinaryWriter(outStream, Encoding.UTF8);
		writer.Write(Encoding.UTF8.GetBytes("RIFF"));
		writer.Write(0);
		writer.Write(Encoding.UTF8.GetBytes("WAVE"));
		writer.Write(Encoding.UTF8.GetBytes("fmt "));
		format.Serialize(writer);
		CreateFactChunk();
		WriteDataChunkHeader();
	}

	public WaveFileWriter(string filename, WaveFormat format)
		: this(new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read), format)
	{
		this.filename = filename;
	}

	private void WriteDataChunkHeader()
	{
		writer.Write(Encoding.UTF8.GetBytes("data"));
		dataSizePos = outStream.Position;
		writer.Write(0);
	}

	private void CreateFactChunk()
	{
		if (HasFactChunk())
		{
			writer.Write(Encoding.UTF8.GetBytes("fact"));
			writer.Write(4);
			factSampleCountPos = outStream.Position;
			writer.Write(0);
		}
	}

	private bool HasFactChunk()
	{
		if (format.Encoding != WaveFormatEncoding.Pcm)
		{
			return format.BitsPerSample != 0;
		}
		return false;
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		throw new InvalidOperationException("Cannot read from a WaveFileWriter");
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new InvalidOperationException("Cannot seek within a WaveFileWriter");
	}

	public override void SetLength(long value)
	{
		throw new InvalidOperationException("Cannot set length of a WaveFileWriter");
	}

	[Obsolete("Use Write instead")]
	public void WriteData(byte[] data, int offset, int count)
	{
		Write(data, offset, count);
	}

	public override void Write(byte[] data, int offset, int count)
	{
		if (outStream.Length + count > uint.MaxValue)
		{
			throw new ArgumentException("WAV file too large", "count");
		}
		outStream.Write(data, offset, count);
		dataChunkSize += count;
	}

	public void WriteSample(float sample)
	{
		if (WaveFormat.BitsPerSample == 16)
		{
			writer.Write((short)(32767f * sample));
			dataChunkSize += 2L;
		}
		else if (WaveFormat.BitsPerSample == 24)
		{
			byte[] bytes = BitConverter.GetBytes((int)(2.1474836E+09f * sample));
			value24[0] = bytes[1];
			value24[1] = bytes[2];
			value24[2] = bytes[3];
			writer.Write(value24);
			dataChunkSize += 3L;
		}
		else if (WaveFormat.BitsPerSample == 32 && WaveFormat.Encoding == WaveFormatEncoding.Extensible)
		{
			writer.Write(65535 * (int)sample);
			dataChunkSize += 4L;
		}
		else
		{
			if (WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
			{
				throw new InvalidOperationException("Only 16, 24 or 32 bit PCM or IEEE float audio data supported");
			}
			writer.Write(sample);
			dataChunkSize += 4L;
		}
	}

	public void WriteSamples(float[] samples, int offset, int count)
	{
		for (int i = 0; i < count; i++)
		{
			WriteSample(samples[offset + i]);
		}
	}

	[Obsolete("Use WriteSamples instead")]
	public void WriteData(short[] samples, int offset, int count)
	{
		WriteSamples(samples, offset, count);
	}

	public void WriteSamples(short[] samples, int offset, int count)
	{
		if (WaveFormat.BitsPerSample == 16)
		{
			for (int i = 0; i < count; i++)
			{
				writer.Write(samples[i + offset]);
			}
			dataChunkSize += count * 2;
		}
		else if (WaveFormat.BitsPerSample == 24)
		{
			for (int j = 0; j < count; j++)
			{
				byte[] bytes = BitConverter.GetBytes(65535 * samples[j + offset]);
				value24[0] = bytes[1];
				value24[1] = bytes[2];
				value24[2] = bytes[3];
				writer.Write(value24);
			}
			dataChunkSize += count * 3;
		}
		else if (WaveFormat.BitsPerSample == 32 && WaveFormat.Encoding == WaveFormatEncoding.Extensible)
		{
			for (int k = 0; k < count; k++)
			{
				writer.Write(65535 * samples[k + offset]);
			}
			dataChunkSize += count * 4;
		}
		else
		{
			if (WaveFormat.BitsPerSample != 32 || WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
			{
				throw new InvalidOperationException("Only 16, 24 or 32 bit PCM or IEEE float audio data supported");
			}
			for (int l = 0; l < count; l++)
			{
				writer.Write((float)samples[l + offset] / 32768f);
			}
			dataChunkSize += count * 4;
		}
	}

	public override void Flush()
	{
		long position = writer.BaseStream.Position;
		UpdateHeader(writer);
		writer.BaseStream.Position = position;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && outStream != null)
		{
			try
			{
				UpdateHeader(writer);
			}
			finally
			{
				outStream.Close();
				outStream = null;
			}
		}
	}

	protected virtual void UpdateHeader(BinaryWriter writer)
	{
		writer.Flush();
		UpdateRiffChunk(writer);
		UpdateFactChunk(writer);
		UpdateDataChunk(writer);
	}

	private void UpdateDataChunk(BinaryWriter writer)
	{
		writer.Seek((int)dataSizePos, SeekOrigin.Begin);
		writer.Write((uint)dataChunkSize);
	}

	private void UpdateRiffChunk(BinaryWriter writer)
	{
		writer.Seek(4, SeekOrigin.Begin);
		writer.Write((uint)(outStream.Length - 8));
	}

	private void UpdateFactChunk(BinaryWriter writer)
	{
		if (HasFactChunk())
		{
			int num = format.BitsPerSample * format.Channels;
			if (num != 0)
			{
				writer.Seek((int)factSampleCountPos, SeekOrigin.Begin);
				writer.Write((int)(dataChunkSize * 8 / num));
			}
		}
	}

	~WaveFileWriter()
	{
		Dispose(disposing: false);
	}
}
