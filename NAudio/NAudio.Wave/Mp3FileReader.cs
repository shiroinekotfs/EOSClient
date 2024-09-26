using System;
using System.Collections.Generic;
using System.IO;

namespace NAudio.Wave;

public class Mp3FileReader : WaveStream
{
	public delegate IMp3FrameDecompressor FrameDecompressorBuilder(WaveFormat mp3Format);

	private readonly WaveFormat waveFormat;

	private Stream mp3Stream;

	private readonly long mp3DataLength;

	private readonly long dataStartPosition;

	private readonly Id3v2Tag id3v2Tag;

	private readonly XingHeader xingHeader;

	private readonly byte[] id3v1Tag;

	private readonly bool ownInputStream;

	private List<Mp3Index> tableOfContents;

	private int tocIndex;

	private long totalSamples;

	private readonly int bytesPerSample;

	private IMp3FrameDecompressor decompressor;

	private readonly byte[] decompressBuffer;

	private int decompressBufferOffset;

	private int decompressLeftovers;

	private bool repositionedFlag;

	private readonly object repositionLock = new object();

	public Mp3WaveFormat Mp3WaveFormat { get; private set; }

	public Id3v2Tag Id3v2Tag => id3v2Tag;

	public byte[] Id3v1Tag => id3v1Tag;

	public override long Length => totalSamples * bytesPerSample;

	public override WaveFormat WaveFormat => waveFormat;

	public override long Position
	{
		get
		{
			if (tocIndex >= tableOfContents.Count)
			{
				return Length;
			}
			return tableOfContents[tocIndex].SamplePosition * bytesPerSample + decompressBufferOffset;
		}
		set
		{
			lock (repositionLock)
			{
				value = Math.Max(Math.Min(value, Length), 0L);
				long num = value / bytesPerSample;
				Mp3Index mp3Index = null;
				for (int i = 0; i < tableOfContents.Count; i++)
				{
					if (tableOfContents[i].SamplePosition >= num)
					{
						mp3Index = tableOfContents[i];
						tocIndex = i;
						break;
					}
				}
				if (mp3Index != null)
				{
					mp3Stream.Position = mp3Index.FilePosition;
				}
				else
				{
					mp3Stream.Position = mp3DataLength + dataStartPosition;
				}
				decompressBufferOffset = 0;
				decompressLeftovers = 0;
				repositionedFlag = true;
			}
		}
	}

	public XingHeader XingHeader => xingHeader;

	public Mp3FileReader(string mp3FileName)
		: this(File.OpenRead(mp3FileName))
	{
		ownInputStream = true;
	}

	public Mp3FileReader(string mp3FileName, FrameDecompressorBuilder frameDecompressorBuilder)
		: this(File.OpenRead(mp3FileName), frameDecompressorBuilder)
	{
		ownInputStream = true;
	}

	public Mp3FileReader(Stream inputStream)
		: this(inputStream, CreateAcmFrameDecompressor)
	{
	}

	public Mp3FileReader(Stream inputStream, FrameDecompressorBuilder frameDecompressorBuilder)
	{
		if (inputStream == null)
		{
			throw new ArgumentNullException("inputStream");
		}
		try
		{
			mp3Stream = inputStream;
			id3v2Tag = Id3v2Tag.ReadTag(mp3Stream);
			dataStartPosition = mp3Stream.Position;
			Mp3Frame mp3Frame = Mp3Frame.LoadFromStream(mp3Stream);
			if (mp3Frame == null)
			{
				throw new InvalidDataException("Invalid MP3 file - no MP3 Frames Detected");
			}
			double num = mp3Frame.BitRate;
			xingHeader = XingHeader.LoadXingHeader(mp3Frame);
			if (xingHeader != null)
			{
				dataStartPosition = mp3Stream.Position;
			}
			Mp3Frame mp3Frame2 = Mp3Frame.LoadFromStream(mp3Stream);
			if (mp3Frame2 != null && (mp3Frame2.SampleRate != mp3Frame.SampleRate || mp3Frame2.ChannelMode != mp3Frame.ChannelMode))
			{
				dataStartPosition = mp3Frame2.FileOffset;
				mp3Frame = mp3Frame2;
			}
			mp3DataLength = mp3Stream.Length - dataStartPosition;
			mp3Stream.Position = mp3Stream.Length - 128;
			byte[] array = new byte[128];
			mp3Stream.Read(array, 0, 128);
			if (array[0] == 84 && array[1] == 65 && array[2] == 71)
			{
				id3v1Tag = array;
				mp3DataLength -= 128L;
			}
			mp3Stream.Position = dataStartPosition;
			Mp3WaveFormat = new Mp3WaveFormat(mp3Frame.SampleRate, (mp3Frame.ChannelMode == ChannelMode.Mono) ? 1 : 2, mp3Frame.FrameLength, (int)num);
			CreateTableOfContents();
			tocIndex = 0;
			num = (double)mp3DataLength * 8.0 / TotalSeconds();
			mp3Stream.Position = dataStartPosition;
			Mp3WaveFormat = new Mp3WaveFormat(mp3Frame.SampleRate, (mp3Frame.ChannelMode == ChannelMode.Mono) ? 1 : 2, mp3Frame.FrameLength, (int)num);
			decompressor = frameDecompressorBuilder(Mp3WaveFormat);
			waveFormat = decompressor.OutputFormat;
			bytesPerSample = decompressor.OutputFormat.BitsPerSample / 8 * decompressor.OutputFormat.Channels;
			decompressBuffer = new byte[1152 * bytesPerSample * 2];
		}
		catch (Exception)
		{
			if (ownInputStream)
			{
				inputStream.Dispose();
			}
			throw;
		}
	}

	public static IMp3FrameDecompressor CreateAcmFrameDecompressor(WaveFormat mp3Format)
	{
		return new AcmMp3FrameDecompressor(mp3Format);
	}

	private void CreateTableOfContents()
	{
		try
		{
			tableOfContents = new List<Mp3Index>((int)(mp3DataLength / 400));
			Mp3Frame mp3Frame = null;
			do
			{
				Mp3Index mp3Index = new Mp3Index();
				mp3Index.FilePosition = mp3Stream.Position;
				mp3Index.SamplePosition = totalSamples;
				mp3Frame = ReadNextFrame(readData: false);
				if (mp3Frame != null)
				{
					ValidateFrameFormat(mp3Frame);
					totalSamples += mp3Frame.SampleCount;
					mp3Index.SampleCount = mp3Frame.SampleCount;
					mp3Index.ByteCount = (int)(mp3Stream.Position - mp3Index.FilePosition);
					tableOfContents.Add(mp3Index);
				}
			}
			while (mp3Frame != null);
		}
		catch (EndOfStreamException)
		{
		}
	}

	private void ValidateFrameFormat(Mp3Frame frame)
	{
		if (frame.SampleRate != Mp3WaveFormat.SampleRate)
		{
			string message = $"Got a frame at sample rate {frame.SampleRate}, in an MP3 with sample rate {Mp3WaveFormat.SampleRate}. Mp3FileReader does not support sample rate changes.";
			throw new InvalidOperationException(message);
		}
		int num = ((frame.ChannelMode == ChannelMode.Mono) ? 1 : 2);
		if (num != Mp3WaveFormat.Channels)
		{
			string message2 = $"Got a frame with channel mode {frame.ChannelMode}, in an MP3 with {Mp3WaveFormat.Channels} channels. Mp3FileReader does not support changes to channel count.";
			throw new InvalidOperationException(message2);
		}
	}

	private double TotalSeconds()
	{
		return (double)totalSamples / (double)Mp3WaveFormat.SampleRate;
	}

	public Mp3Frame ReadNextFrame()
	{
		return ReadNextFrame(readData: true);
	}

	private Mp3Frame ReadNextFrame(bool readData)
	{
		Mp3Frame mp3Frame = null;
		try
		{
			mp3Frame = Mp3Frame.LoadFromStream(mp3Stream, readData);
			if (mp3Frame != null)
			{
				tocIndex++;
			}
		}
		catch (EndOfStreamException)
		{
		}
		return mp3Frame;
	}

	public override int Read(byte[] sampleBuffer, int offset, int numBytes)
	{
		int i = 0;
		lock (repositionLock)
		{
			if (decompressLeftovers != 0)
			{
				int num = Math.Min(decompressLeftovers, numBytes);
				Array.Copy(decompressBuffer, decompressBufferOffset, sampleBuffer, offset, num);
				decompressLeftovers -= num;
				if (decompressLeftovers == 0)
				{
					decompressBufferOffset = 0;
				}
				else
				{
					decompressBufferOffset += num;
				}
				i += num;
				offset += num;
			}
			int num3;
			for (; i < numBytes; i += num3)
			{
				Mp3Frame mp3Frame = ReadNextFrame();
				if (mp3Frame != null)
				{
					if (repositionedFlag)
					{
						decompressor.Reset();
						repositionedFlag = false;
					}
					int num2 = decompressor.DecompressFrame(mp3Frame, decompressBuffer, 0);
					num3 = Math.Min(num2, numBytes - i);
					Array.Copy(decompressBuffer, 0, sampleBuffer, offset, num3);
					if (num3 < num2)
					{
						decompressBufferOffset = num3;
						decompressLeftovers = num2 - num3;
					}
					else
					{
						decompressBufferOffset = 0;
						decompressLeftovers = 0;
					}
					offset += num3;
					continue;
				}
				break;
			}
		}
		return i;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (mp3Stream != null)
			{
				if (ownInputStream)
				{
					mp3Stream.Dispose();
				}
				mp3Stream = null;
			}
			if (decompressor != null)
			{
				decompressor.Dispose();
				decompressor = null;
			}
		}
		base.Dispose(disposing);
	}
}
