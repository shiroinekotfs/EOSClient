using System;
using NAudio.Utils;

namespace NAudio.Wave;

public class StereoToMonoProvider16 : IWaveProvider
{
	private IWaveProvider sourceProvider;

	private WaveFormat outputFormat;

	private byte[] sourceBuffer;

	public float LeftVolume { get; set; }

	public float RightVolume { get; set; }

	public WaveFormat WaveFormat => outputFormat;

	public StereoToMonoProvider16(IWaveProvider sourceProvider)
	{
		if (sourceProvider.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
		{
			throw new ArgumentException("Source must be PCM");
		}
		if (sourceProvider.WaveFormat.Channels != 2)
		{
			throw new ArgumentException("Source must be stereo");
		}
		if (sourceProvider.WaveFormat.BitsPerSample != 16)
		{
			throw new ArgumentException("Source must be 16 bit");
		}
		this.sourceProvider = sourceProvider;
		outputFormat = new WaveFormat(sourceProvider.WaveFormat.SampleRate, 1);
	}

	public int Read(byte[] buffer, int offset, int count)
	{
		int num = count * 2;
		sourceBuffer = BufferHelpers.Ensure(sourceBuffer, num);
		WaveBuffer waveBuffer = new WaveBuffer(sourceBuffer);
		WaveBuffer waveBuffer2 = new WaveBuffer(buffer);
		int num2 = sourceProvider.Read(sourceBuffer, 0, num);
		int num3 = num2 / 2;
		int num4 = offset / 2;
		for (int i = 0; i < num3; i += 2)
		{
			short num5 = waveBuffer.ShortBuffer[i];
			short num6 = waveBuffer.ShortBuffer[i + 1];
			float num7 = (float)num5 * LeftVolume + (float)num6 * RightVolume;
			if (num7 > 32767f)
			{
				num7 = 32767f;
			}
			if (num7 < -32768f)
			{
				num7 = -32768f;
			}
			waveBuffer2.ShortBuffer[num4++] = (short)num7;
		}
		return num2 / 2;
	}
}
