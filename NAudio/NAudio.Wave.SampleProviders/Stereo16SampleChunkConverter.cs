using NAudio.Utils;

namespace NAudio.Wave.SampleProviders;

internal class Stereo16SampleChunkConverter : ISampleChunkConverter
{
	private int sourceSample;

	private byte[] sourceBuffer;

	private WaveBuffer sourceWaveBuffer;

	private int sourceSamples;

	public bool Supports(WaveFormat waveFormat)
	{
		if (waveFormat.Encoding == WaveFormatEncoding.Pcm && waveFormat.BitsPerSample == 16)
		{
			return waveFormat.Channels == 2;
		}
		return false;
	}

	public void LoadNextChunk(IWaveProvider source, int samplePairsRequired)
	{
		int num = samplePairsRequired * 4;
		sourceBuffer = BufferHelpers.Ensure(sourceBuffer, num);
		sourceWaveBuffer = new WaveBuffer(sourceBuffer);
		sourceSamples = source.Read(sourceBuffer, 0, num) / 2;
		sourceSample = 0;
	}

	public bool GetNextSample(out float sampleLeft, out float sampleRight)
	{
		if (sourceSample < sourceSamples)
		{
			sampleLeft = (float)sourceWaveBuffer.ShortBuffer[sourceSample++] / 32768f;
			sampleRight = (float)sourceWaveBuffer.ShortBuffer[sourceSample++] / 32768f;
			return true;
		}
		sampleLeft = 0f;
		sampleRight = 0f;
		return false;
	}
}
