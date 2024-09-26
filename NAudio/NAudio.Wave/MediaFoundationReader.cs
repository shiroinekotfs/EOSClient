using System;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.MediaFoundation;

namespace NAudio.Wave;

public class MediaFoundationReader : WaveStream
{
	public class MediaFoundationReaderSettings
	{
		public bool RequestFloatOutput { get; set; }

		public bool SingleReaderObject { get; set; }

		public bool RepositionInRead { get; set; }

		public MediaFoundationReaderSettings()
		{
			RepositionInRead = true;
		}
	}

	private WaveFormat waveFormat;

	private readonly long length;

	private readonly MediaFoundationReaderSettings settings;

	private readonly string file;

	private IMFSourceReader pReader;

	private long position;

	private byte[] decoderOutputBuffer;

	private int decoderOutputOffset;

	private int decoderOutputCount;

	private long repositionTo = -1L;

	public override WaveFormat WaveFormat => waveFormat;

	public override long Length => length;

	public override long Position
	{
		get
		{
			return position;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("value", "Position cannot be less than 0");
			}
			if (settings.RepositionInRead)
			{
				repositionTo = value;
				position = value;
			}
			else
			{
				Reposition(value);
			}
		}
	}

	public event EventHandler WaveFormatChanged;

	public MediaFoundationReader(string file)
		: this(file, new MediaFoundationReaderSettings())
	{
	}

	public MediaFoundationReader(string file, MediaFoundationReaderSettings settings)
	{
		MediaFoundationApi.Startup();
		this.settings = settings;
		this.file = file;
		IMFSourceReader iMFSourceReader = CreateReader(settings);
		waveFormat = GetCurrentWaveFormat(iMFSourceReader);
		iMFSourceReader.SetStreamSelection(-3, pSelected: true);
		length = GetLength(iMFSourceReader);
		if (settings.SingleReaderObject)
		{
			pReader = iMFSourceReader;
		}
	}

	private WaveFormat GetCurrentWaveFormat(IMFSourceReader reader)
	{
		reader.GetCurrentMediaType(-3, out var ppMediaType);
		MediaType mediaType = new MediaType(ppMediaType);
		_ = mediaType.MajorType;
		Guid subType = mediaType.SubType;
		int channelCount = mediaType.ChannelCount;
		int bitsPerSample = mediaType.BitsPerSample;
		int sampleRate = mediaType.SampleRate;
		if (!(subType == AudioSubtypes.MFAudioFormat_PCM))
		{
			return WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount);
		}
		return new WaveFormat(sampleRate, bitsPerSample, channelCount);
	}

	protected virtual IMFSourceReader CreateReader(MediaFoundationReaderSettings settings)
	{
		MediaFoundationInterop.MFCreateSourceReaderFromURL(file, null, out var ppSourceReader);
		ppSourceReader.SetStreamSelection(-2, pSelected: false);
		ppSourceReader.SetStreamSelection(-3, pSelected: true);
		MediaType mediaType = new MediaType();
		mediaType.MajorType = MediaTypes.MFMediaType_Audio;
		mediaType.SubType = (settings.RequestFloatOutput ? AudioSubtypes.MFAudioFormat_Float : AudioSubtypes.MFAudioFormat_PCM);
		ppSourceReader.SetCurrentMediaType(-3, IntPtr.Zero, mediaType.MediaFoundationObject);
		return ppSourceReader;
	}

	private long GetLength(IMFSourceReader reader)
	{
		PropVariant pvarAttribute;
		int presentationAttribute = reader.GetPresentationAttribute(-1, MediaFoundationAttributes.MF_PD_DURATION, out pvarAttribute);
		switch (presentationAttribute)
		{
		case -1072875802:
			return 0L;
		default:
			Marshal.ThrowExceptionForHR(presentationAttribute);
			break;
		case 0:
			break;
		}
		long result = (long)pvarAttribute.Value * waveFormat.AverageBytesPerSecond / 10000000;
		pvarAttribute.Clear();
		return result;
	}

	private void EnsureBuffer(int bytesRequired)
	{
		if (decoderOutputBuffer == null || decoderOutputBuffer.Length < bytesRequired)
		{
			decoderOutputBuffer = new byte[bytesRequired];
		}
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		if (pReader == null)
		{
			pReader = CreateReader(settings);
		}
		if (repositionTo != -1)
		{
			Reposition(repositionTo);
		}
		int num = 0;
		if (decoderOutputCount > 0)
		{
			num += ReadFromDecoderBuffer(buffer, offset, count - num);
		}
		while (num < count)
		{
			pReader.ReadSample(-3, 0, out var _, out var pdwStreamFlags, out var _, out var ppSample);
			if ((pdwStreamFlags & MF_SOURCE_READER_FLAG.MF_SOURCE_READERF_ENDOFSTREAM) != 0)
			{
				break;
			}
			if ((pdwStreamFlags & MF_SOURCE_READER_FLAG.MF_SOURCE_READERF_CURRENTMEDIATYPECHANGED) != 0)
			{
				waveFormat = GetCurrentWaveFormat(pReader);
				OnWaveFormatChanged();
			}
			else if (pdwStreamFlags != 0)
			{
				throw new InvalidOperationException($"MediaFoundationReadError {pdwStreamFlags}");
			}
			ppSample.ConvertToContiguousBuffer(out var ppBuffer);
			ppBuffer.Lock(out var ppbBuffer, out var _, out var pcbCurrentLength);
			EnsureBuffer(pcbCurrentLength);
			Marshal.Copy(ppbBuffer, decoderOutputBuffer, 0, pcbCurrentLength);
			decoderOutputOffset = 0;
			decoderOutputCount = pcbCurrentLength;
			num += ReadFromDecoderBuffer(buffer, offset + num, count - num);
			ppBuffer.Unlock();
			Marshal.ReleaseComObject(ppBuffer);
			Marshal.ReleaseComObject(ppSample);
		}
		position += num;
		return num;
	}

	private int ReadFromDecoderBuffer(byte[] buffer, int offset, int needed)
	{
		int num = Math.Min(needed, decoderOutputCount);
		Array.Copy(decoderOutputBuffer, decoderOutputOffset, buffer, offset, num);
		decoderOutputOffset += num;
		decoderOutputCount -= num;
		if (decoderOutputCount == 0)
		{
			decoderOutputOffset = 0;
		}
		return num;
	}

	private void Reposition(long desiredPosition)
	{
		long value = 10000000 * repositionTo / waveFormat.AverageBytesPerSecond;
		PropVariant varPosition = PropVariant.FromLong(value);
		pReader.SetCurrentPosition(Guid.Empty, ref varPosition);
		decoderOutputCount = 0;
		decoderOutputOffset = 0;
		position = desiredPosition;
		repositionTo = -1L;
	}

	protected override void Dispose(bool disposing)
	{
		if (pReader != null)
		{
			Marshal.ReleaseComObject(pReader);
			pReader = null;
		}
		base.Dispose(disposing);
	}

	private void OnWaveFormatChanged()
	{
		this.WaveFormatChanged?.Invoke(this, EventArgs.Empty);
	}
}
