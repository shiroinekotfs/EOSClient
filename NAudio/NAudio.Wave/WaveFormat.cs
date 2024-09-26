using System;
using System.IO;
using System.Runtime.InteropServices;

namespace NAudio.Wave;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public class WaveFormat
{
	protected WaveFormatEncoding waveFormatTag;

	protected short channels;

	protected int sampleRate;

	protected int averageBytesPerSecond;

	protected short blockAlign;

	protected short bitsPerSample;

	protected short extraSize;

	public WaveFormatEncoding Encoding => waveFormatTag;

	public int Channels => channels;

	public int SampleRate => sampleRate;

	public int AverageBytesPerSecond => averageBytesPerSecond;

	public virtual int BlockAlign => blockAlign;

	public int BitsPerSample => bitsPerSample;

	public int ExtraSize => extraSize;

	public WaveFormat()
		: this(44100, 16, 2)
	{
	}

	public WaveFormat(int sampleRate, int channels)
		: this(sampleRate, 16, channels)
	{
	}

	public int ConvertLatencyToByteSize(int milliseconds)
	{
		int num = (int)((double)AverageBytesPerSecond / 1000.0 * (double)milliseconds);
		if (num % BlockAlign != 0)
		{
			num = num + BlockAlign - num % BlockAlign;
		}
		return num;
	}

	public static WaveFormat CreateCustomFormat(WaveFormatEncoding tag, int sampleRate, int channels, int averageBytesPerSecond, int blockAlign, int bitsPerSample)
	{
		WaveFormat waveFormat = new WaveFormat();
		waveFormat.waveFormatTag = tag;
		waveFormat.channels = (short)channels;
		waveFormat.sampleRate = sampleRate;
		waveFormat.averageBytesPerSecond = averageBytesPerSecond;
		waveFormat.blockAlign = (short)blockAlign;
		waveFormat.bitsPerSample = (short)bitsPerSample;
		waveFormat.extraSize = 0;
		return waveFormat;
	}

	public static WaveFormat CreateALawFormat(int sampleRate, int channels)
	{
		return CreateCustomFormat(WaveFormatEncoding.ALaw, sampleRate, channels, sampleRate * channels, channels, 8);
	}

	public static WaveFormat CreateMuLawFormat(int sampleRate, int channels)
	{
		return CreateCustomFormat(WaveFormatEncoding.MuLaw, sampleRate, channels, sampleRate * channels, channels, 8);
	}

	public WaveFormat(int rate, int bits, int channels)
	{
		if (channels < 1)
		{
			throw new ArgumentOutOfRangeException("channels", "Channels must be 1 or greater");
		}
		waveFormatTag = WaveFormatEncoding.Pcm;
		this.channels = (short)channels;
		sampleRate = rate;
		bitsPerSample = (short)bits;
		extraSize = 0;
		blockAlign = (short)(channels * (bits / 8));
		averageBytesPerSecond = sampleRate * blockAlign;
	}

	public static WaveFormat CreateIeeeFloatWaveFormat(int sampleRate, int channels)
	{
		WaveFormat waveFormat = new WaveFormat();
		waveFormat.waveFormatTag = WaveFormatEncoding.IeeeFloat;
		waveFormat.channels = (short)channels;
		waveFormat.bitsPerSample = 32;
		waveFormat.sampleRate = sampleRate;
		waveFormat.blockAlign = (short)(4 * channels);
		waveFormat.averageBytesPerSecond = sampleRate * waveFormat.blockAlign;
		waveFormat.extraSize = 0;
		return waveFormat;
	}

	public static WaveFormat MarshalFromPtr(IntPtr pointer)
	{
		WaveFormat waveFormat = (WaveFormat)Marshal.PtrToStructure(pointer, typeof(WaveFormat));
		switch (waveFormat.Encoding)
		{
		case WaveFormatEncoding.Pcm:
			waveFormat.extraSize = 0;
			break;
		case WaveFormatEncoding.Extensible:
			waveFormat = (WaveFormatExtensible)Marshal.PtrToStructure(pointer, typeof(WaveFormatExtensible));
			break;
		case WaveFormatEncoding.Adpcm:
			waveFormat = (AdpcmWaveFormat)Marshal.PtrToStructure(pointer, typeof(AdpcmWaveFormat));
			break;
		case WaveFormatEncoding.Gsm610:
			waveFormat = (Gsm610WaveFormat)Marshal.PtrToStructure(pointer, typeof(Gsm610WaveFormat));
			break;
		default:
			if (waveFormat.ExtraSize > 0)
			{
				waveFormat = (WaveFormatExtraData)Marshal.PtrToStructure(pointer, typeof(WaveFormatExtraData));
			}
			break;
		}
		return waveFormat;
	}

	public static IntPtr MarshalToPtr(WaveFormat format)
	{
		int cb = Marshal.SizeOf(format);
		IntPtr intPtr = Marshal.AllocHGlobal(cb);
		Marshal.StructureToPtr(format, intPtr, fDeleteOld: false);
		return intPtr;
	}

	public static WaveFormat FromFormatChunk(BinaryReader br, int formatChunkLength)
	{
		WaveFormatExtraData waveFormatExtraData = new WaveFormatExtraData();
		waveFormatExtraData.ReadWaveFormat(br, formatChunkLength);
		waveFormatExtraData.ReadExtraData(br);
		return waveFormatExtraData;
	}

	private void ReadWaveFormat(BinaryReader br, int formatChunkLength)
	{
		if (formatChunkLength < 16)
		{
			throw new InvalidDataException("Invalid WaveFormat Structure");
		}
		waveFormatTag = (WaveFormatEncoding)br.ReadUInt16();
		channels = br.ReadInt16();
		sampleRate = br.ReadInt32();
		averageBytesPerSecond = br.ReadInt32();
		blockAlign = br.ReadInt16();
		bitsPerSample = br.ReadInt16();
		if (formatChunkLength > 16)
		{
			extraSize = br.ReadInt16();
			if (extraSize != formatChunkLength - 18)
			{
				extraSize = (short)(formatChunkLength - 18);
			}
		}
	}

	public WaveFormat(BinaryReader br)
	{
		int formatChunkLength = br.ReadInt32();
		ReadWaveFormat(br, formatChunkLength);
	}

	public override string ToString()
	{
		WaveFormatEncoding waveFormatEncoding = waveFormatTag;
		if (waveFormatEncoding == WaveFormatEncoding.Pcm || waveFormatEncoding == WaveFormatEncoding.Extensible)
		{
			return $"{bitsPerSample} bit PCM: {sampleRate / 1000}kHz {channels} channels";
		}
		return waveFormatTag.ToString();
	}

	public override bool Equals(object obj)
	{
		if (obj is WaveFormat waveFormat)
		{
			if (waveFormatTag == waveFormat.waveFormatTag && channels == waveFormat.channels && sampleRate == waveFormat.sampleRate && averageBytesPerSecond == waveFormat.averageBytesPerSecond && blockAlign == waveFormat.blockAlign)
			{
				return bitsPerSample == waveFormat.bitsPerSample;
			}
			return false;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (int)waveFormatTag ^ (int)channels ^ sampleRate ^ averageBytesPerSecond ^ blockAlign ^ bitsPerSample;
	}

	public virtual void Serialize(BinaryWriter writer)
	{
		writer.Write(18 + extraSize);
		writer.Write((short)Encoding);
		writer.Write((short)Channels);
		writer.Write(SampleRate);
		writer.Write(AverageBytesPerSecond);
		writer.Write((short)BlockAlign);
		writer.Write((short)BitsPerSample);
		writer.Write(extraSize);
	}
}
