using System;

namespace NAudio.Wave;

public class VolumeWaveProvider16 : IWaveProvider
{
	private readonly IWaveProvider sourceProvider;

	private float volume;

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

	public WaveFormat WaveFormat => sourceProvider.WaveFormat;

	public VolumeWaveProvider16(IWaveProvider sourceProvider)
	{
		Volume = 1f;
		this.sourceProvider = sourceProvider;
		if (sourceProvider.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
		{
			throw new ArgumentException("Expecting PCM input");
		}
		if (sourceProvider.WaveFormat.BitsPerSample != 16)
		{
			throw new ArgumentException("Expecting 16 bit");
		}
	}

	public int Read(byte[] buffer, int offset, int count)
	{
		int num = sourceProvider.Read(buffer, offset, count);
		if (volume == 0f)
		{
			for (int i = 0; i < num; i++)
			{
				buffer[offset++] = 0;
			}
		}
		else if (volume != 1f)
		{
			for (int j = 0; j < num; j += 2)
			{
				short num2 = (short)((buffer[offset + 1] << 8) | buffer[offset]);
				float num3 = (float)num2 * volume;
				num2 = (short)num3;
				if (Volume > 1f)
				{
					if (num3 > 32767f)
					{
						num2 = short.MaxValue;
					}
					else if (num3 < -32768f)
					{
						num2 = short.MinValue;
					}
				}
				buffer[offset++] = (byte)((uint)num2 & 0xFFu);
				buffer[offset++] = (byte)(num2 >> 8);
			}
		}
		return num;
	}
}
