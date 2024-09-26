using System;
using NAudio.Wave.SampleProviders;

namespace NAudio.Wave;

public class AudioFileReader : WaveStream, ISampleProvider
{
	private string fileName;

	private WaveStream readerStream;

	private readonly SampleChannel sampleChannel;

	private readonly int destBytesPerSample;

	private readonly int sourceBytesPerSample;

	private readonly long length;

	private readonly object lockObject;

	public override WaveFormat WaveFormat => sampleChannel.WaveFormat;

	public override long Length => length;

	public override long Position
	{
		get
		{
			return SourceToDest(readerStream.Position);
		}
		set
		{
			lock (lockObject)
			{
				readerStream.Position = DestToSource(value);
			}
		}
	}

	public float Volume
	{
		get
		{
			return sampleChannel.Volume;
		}
		set
		{
			sampleChannel.Volume = value;
		}
	}

	public AudioFileReader(string fileName)
	{
		lockObject = new object();
		this.fileName = fileName;
		CreateReaderStream(fileName);
		sourceBytesPerSample = readerStream.WaveFormat.BitsPerSample / 8 * readerStream.WaveFormat.Channels;
		sampleChannel = new SampleChannel(readerStream, forceStereo: false);
		destBytesPerSample = 4 * sampleChannel.WaveFormat.Channels;
		length = SourceToDest(readerStream.Length);
	}

	private void CreateReaderStream(string fileName)
	{
		if (fileName.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
		{
			readerStream = new WaveFileReader(fileName);
			if (readerStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm && readerStream.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
			{
				readerStream = WaveFormatConversionStream.CreatePcmStream(readerStream);
				readerStream = new BlockAlignReductionStream(readerStream);
			}
		}
		else if (fileName.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
		{
			readerStream = new Mp3FileReader(fileName);
		}
		else if (fileName.EndsWith(".aiff"))
		{
			readerStream = new AiffFileReader(fileName);
		}
		else
		{
			readerStream = new MediaFoundationReader(fileName);
		}
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		WaveBuffer waveBuffer = new WaveBuffer(buffer);
		int count2 = count / 4;
		int num = Read(waveBuffer.FloatBuffer, offset / 4, count2);
		return num * 4;
	}

	public int Read(float[] buffer, int offset, int count)
	{
		lock (lockObject)
		{
			return sampleChannel.Read(buffer, offset, count);
		}
	}

	private long SourceToDest(long sourceBytes)
	{
		return destBytesPerSample * (sourceBytes / sourceBytesPerSample);
	}

	private long DestToSource(long destBytes)
	{
		return sourceBytesPerSample * (destBytes / destBytesPerSample);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			readerStream.Dispose();
			readerStream = null;
		}
		base.Dispose(disposing);
	}
}
