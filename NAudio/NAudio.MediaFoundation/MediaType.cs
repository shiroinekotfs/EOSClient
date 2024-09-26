using System;
using System.Runtime.InteropServices;
using NAudio.Utils;
using NAudio.Wave;

namespace NAudio.MediaFoundation;

public class MediaType
{
	private readonly IMFMediaType mediaType;

	public int SampleRate
	{
		get
		{
			return GetUInt32(MediaFoundationAttributes.MF_MT_AUDIO_SAMPLES_PER_SECOND);
		}
		set
		{
			mediaType.SetUINT32(MediaFoundationAttributes.MF_MT_AUDIO_SAMPLES_PER_SECOND, value);
		}
	}

	public int ChannelCount
	{
		get
		{
			return GetUInt32(MediaFoundationAttributes.MF_MT_AUDIO_NUM_CHANNELS);
		}
		set
		{
			mediaType.SetUINT32(MediaFoundationAttributes.MF_MT_AUDIO_NUM_CHANNELS, value);
		}
	}

	public int BitsPerSample
	{
		get
		{
			return GetUInt32(MediaFoundationAttributes.MF_MT_AUDIO_BITS_PER_SAMPLE);
		}
		set
		{
			mediaType.SetUINT32(MediaFoundationAttributes.MF_MT_AUDIO_BITS_PER_SAMPLE, value);
		}
	}

	public int AverageBytesPerSecond => GetUInt32(MediaFoundationAttributes.MF_MT_AUDIO_AVG_BYTES_PER_SECOND);

	public Guid SubType
	{
		get
		{
			return GetGuid(MediaFoundationAttributes.MF_MT_SUBTYPE);
		}
		set
		{
			mediaType.SetGUID(MediaFoundationAttributes.MF_MT_SUBTYPE, value);
		}
	}

	public Guid MajorType
	{
		get
		{
			return GetGuid(MediaFoundationAttributes.MF_MT_MAJOR_TYPE);
		}
		set
		{
			mediaType.SetGUID(MediaFoundationAttributes.MF_MT_MAJOR_TYPE, value);
		}
	}

	public IMFMediaType MediaFoundationObject => mediaType;

	public MediaType(IMFMediaType mediaType)
	{
		this.mediaType = mediaType;
	}

	public MediaType()
	{
		mediaType = MediaFoundationApi.CreateMediaType();
	}

	public MediaType(WaveFormat waveFormat)
	{
		mediaType = MediaFoundationApi.CreateMediaTypeFromWaveFormat(waveFormat);
	}

	private int GetUInt32(Guid key)
	{
		mediaType.GetUINT32(key, out var punValue);
		return punValue;
	}

	private Guid GetGuid(Guid key)
	{
		mediaType.GetGUID(key, out var pguidValue);
		return pguidValue;
	}

	public int TryGetUInt32(Guid key, int defaultValue = -1)
	{
		int punValue = defaultValue;
		try
		{
			mediaType.GetUINT32(key, out punValue);
		}
		catch (COMException exception)
		{
			if (exception.GetHResult() != -1072875802)
			{
				if (exception.GetHResult() == -1072875843)
				{
					throw new ArgumentException("Not a UINT32 parameter");
				}
				throw;
			}
		}
		return punValue;
	}
}
