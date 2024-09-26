using System;
using NAudio.Wave.SampleProviders;

namespace NAudio.Wave;

public class WaveChannel32 : WaveStream, ISampleNotifier
{
	private WaveStream sourceStream;

	private readonly WaveFormat waveFormat;

	private readonly long length;

	private readonly int destBytesPerSample;

	private readonly int sourceBytesPerSample;

	private volatile float volume;

	private volatile float pan;

	private long position;

	private readonly ISampleChunkConverter sampleProvider;

	private readonly object lockObject = new object();

	private SampleEventArgs sampleEventArgs = new SampleEventArgs(0f, 0f);

	public override int BlockAlign => (int)SourceToDest(sourceStream.BlockAlign);

	public override long Length => length;

	public override long Position
	{
		get
		{
			return position;
		}
		set
		{
			lock (lockObject)
			{
				value -= value % BlockAlign;
				if (value < 0)
				{
					sourceStream.Position = 0L;
				}
				else
				{
					sourceStream.Position = DestToSource(value);
				}
				position = SourceToDest(sourceStream.Position);
			}
		}
	}

	public bool PadWithZeroes { get; set; }

	public override WaveFormat WaveFormat => waveFormat;

	public float Volume
	{
		get
		{
			return volume;
		}
		set
		{
			volume = value;
		}
	}

	public float Pan
	{
		get
		{
			return pan;
		}
		set
		{
			pan = value;
		}
	}

	public event EventHandler<SampleEventArgs> Sample;

	public WaveChannel32(WaveStream sourceStream, float volume, float pan)
	{
		PadWithZeroes = true;
		ISampleChunkConverter[] array = new ISampleChunkConverter[8]
		{
			new Mono8SampleChunkConverter(),
			new Stereo8SampleChunkConverter(),
			new Mono16SampleChunkConverter(),
			new Stereo16SampleChunkConverter(),
			new Mono24SampleChunkConverter(),
			new Stereo24SampleChunkConverter(),
			new MonoFloatSampleChunkConverter(),
			new StereoFloatSampleChunkConverter()
		};
		ISampleChunkConverter[] array2 = array;
		foreach (ISampleChunkConverter sampleChunkConverter in array2)
		{
			if (sampleChunkConverter.Supports(sourceStream.WaveFormat))
			{
				sampleProvider = sampleChunkConverter;
				break;
			}
		}
		if (sampleProvider == null)
		{
			throw new ArgumentException("Unsupported sourceStream format");
		}
		waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sourceStream.WaveFormat.SampleRate, 2);
		destBytesPerSample = 8;
		this.sourceStream = sourceStream;
		this.volume = volume;
		this.pan = pan;
		sourceBytesPerSample = sourceStream.WaveFormat.Channels * sourceStream.WaveFormat.BitsPerSample / 8;
		length = SourceToDest(sourceStream.Length);
		position = 0L;
	}

	private long SourceToDest(long sourceBytes)
	{
		return sourceBytes / sourceBytesPerSample * destBytesPerSample;
	}

	private long DestToSource(long destBytes)
	{
		return destBytes / destBytesPerSample * sourceBytesPerSample;
	}

	public WaveChannel32(WaveStream sourceStream)
		: this(sourceStream, 1f, 0f)
	{
	}

	public override int Read(byte[] destBuffer, int offset, int numBytes)
	{
		lock (lockObject)
		{
			int num = 0;
			WaveBuffer waveBuffer = new WaveBuffer(destBuffer);
			if (position < 0)
			{
				num = (int)Math.Min(numBytes, -position);
				for (int i = 0; i < num; i++)
				{
					destBuffer[i + offset] = 0;
				}
			}
			if (num < numBytes)
			{
				sampleProvider.LoadNextChunk(sourceStream, (numBytes - num) / 8);
				int num2 = offset / 4 + num / 4;
				float sampleLeft;
				float sampleRight;
				while (sampleProvider.GetNextSample(out sampleLeft, out sampleRight) && num < numBytes)
				{
					sampleLeft = ((pan <= 0f) ? sampleLeft : (sampleLeft * (1f - pan) / 2f));
					sampleRight = ((pan >= 0f) ? sampleRight : (sampleRight * (pan + 1f) / 2f));
					sampleLeft *= volume;
					sampleRight *= volume;
					waveBuffer.FloatBuffer[num2++] = sampleLeft;
					waveBuffer.FloatBuffer[num2++] = sampleRight;
					num += 8;
					if (this.Sample != null)
					{
						RaiseSample(sampleLeft, sampleRight);
					}
				}
			}
			if (PadWithZeroes && num < numBytes)
			{
				Array.Clear(destBuffer, offset + num, numBytes - num);
				num = numBytes;
			}
			position += num;
			return num;
		}
	}

	public override bool HasData(int count)
	{
		if (sourceStream.HasData(count))
		{
			if (position + count < 0)
			{
				return false;
			}
			if (position < length)
			{
				return volume != 0f;
			}
			return false;
		}
		return false;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && sourceStream != null)
		{
			sourceStream.Dispose();
			sourceStream = null;
		}
		base.Dispose(disposing);
	}

	private void RaiseSample(float left, float right)
	{
		sampleEventArgs.Left = left;
		sampleEventArgs.Right = right;
		this.Sample(this, sampleEventArgs);
	}
}
