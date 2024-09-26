using System;
using NAudio.Utils;

namespace NAudio.Wave;

public class MonoToStereoProvider16 : IWaveProvider
{
	private IWaveProvider sourceProvider;

	private WaveFormat outputFormat;

	private byte[] sourceBuffer;

	public float LeftVolume { get; set; }

	public float RightVolume { get; set; }

	public WaveFormat WaveFormat => outputFormat;

	public MonoToStereoProvider16(IWaveProvider sourceProvider)
	{
		if (sourceProvider.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
		{
			throw new ArgumentException("Source must be PCM");
		}
		if (sourceProvider.WaveFormat.Channels != 1)
		{
			throw new ArgumentException("Source must be Mono");
		}
		if (sourceProvider.WaveFormat.BitsPerSample != 16)
		{
			throw new ArgumentException("Source must be 16 bit");
		}
		this.sourceProvider = sourceProvider;
		outputFormat = new WaveFormat(sourceProvider.WaveFormat.SampleRate, 2);
		RightVolume = 1f;
		LeftVolume = 1f;
	}

	public int Read(byte[] buffer, int offset, int count)
	{
		int num = count / 2;
		sourceBuffer = BufferHelpers.Ensure(sourceBuffer, num);
		WaveBuffer waveBuffer = new WaveBuffer(sourceBuffer);
		WaveBuffer waveBuffer2 = new WaveBuffer(buffer);
		int num2 = sourceProvider.Read(sourceBuffer, 0, num);
		int num3 = num2 / 2;
		int num4 = offset / 2;
		for (int i = 0; i < num3; i++)
		{
			short num5 = waveBuffer.ShortBuffer[i];
			waveBuffer2.ShortBuffer[num4++] = (short)(LeftVolume * (float)num5);
			waveBuffer2.ShortBuffer[num4++] = (short)(RightVolume * (float)num5);
		}
		return num3 * 4;
	}
}
