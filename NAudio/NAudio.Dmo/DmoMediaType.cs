using System;
using System.Runtime.InteropServices;
using NAudio.Wave;

namespace NAudio.Dmo;

public struct DmoMediaType
{
	private Guid majortype;

	private Guid subtype;

	private bool bFixedSizeSamples;

	private bool bTemporalCompression;

	private int lSampleSize;

	private Guid formattype;

	private IntPtr pUnk;

	private int cbFormat;

	private IntPtr pbFormat;

	public Guid MajorType => majortype;

	public string MajorTypeName => MediaTypes.GetMediaTypeName(majortype);

	public Guid SubType => subtype;

	public string SubTypeName
	{
		get
		{
			if (majortype == MediaTypes.MEDIATYPE_Audio)
			{
				return AudioMediaSubtypes.GetAudioSubtypeName(subtype);
			}
			return subtype.ToString();
		}
	}

	public bool FixedSizeSamples => bFixedSizeSamples;

	public int SampleSize => lSampleSize;

	public Guid FormatType => formattype;

	public string FormatTypeName
	{
		get
		{
			if (formattype == DmoMediaTypeGuids.FORMAT_None)
			{
				return "None";
			}
			if (formattype == Guid.Empty)
			{
				return "Null";
			}
			if (formattype == DmoMediaTypeGuids.FORMAT_WaveFormatEx)
			{
				return "WaveFormatEx";
			}
			return FormatType.ToString();
		}
	}

	public WaveFormat GetWaveFormat()
	{
		if (formattype == DmoMediaTypeGuids.FORMAT_WaveFormatEx)
		{
			return WaveFormat.MarshalFromPtr(pbFormat);
		}
		throw new InvalidOperationException("Not a WaveFormat type");
	}

	public void SetWaveFormat(WaveFormat waveFormat)
	{
		majortype = MediaTypes.MEDIATYPE_Audio;
		if (waveFormat is WaveFormatExtensible waveFormatExtensible)
		{
			subtype = waveFormatExtensible.SubFormat;
		}
		else
		{
			switch (waveFormat.Encoding)
			{
			case WaveFormatEncoding.Pcm:
				subtype = AudioMediaSubtypes.MEDIASUBTYPE_PCM;
				break;
			case WaveFormatEncoding.IeeeFloat:
				subtype = AudioMediaSubtypes.MEDIASUBTYPE_IEEE_FLOAT;
				break;
			case WaveFormatEncoding.MpegLayer3:
				subtype = AudioMediaSubtypes.WMMEDIASUBTYPE_MP3;
				break;
			default:
				throw new ArgumentException($"Not a supported encoding {waveFormat.Encoding}");
			}
		}
		bFixedSizeSamples = SubType == AudioMediaSubtypes.MEDIASUBTYPE_PCM || SubType == AudioMediaSubtypes.MEDIASUBTYPE_IEEE_FLOAT;
		formattype = DmoMediaTypeGuids.FORMAT_WaveFormatEx;
		if (cbFormat < Marshal.SizeOf(waveFormat))
		{
			throw new InvalidOperationException("Not enough memory assigned for a WaveFormat structure");
		}
		Marshal.StructureToPtr(waveFormat, pbFormat, fDeleteOld: false);
	}
}
