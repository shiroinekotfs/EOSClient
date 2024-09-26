using NAudio.Utils;

namespace NAudio.Wave.SampleProviders;

internal class Stereo8SampleChunkConverter : ISampleChunkConverter
{
	private int offset;

	private byte[] sourceBuffer;

	private int sourceBytes;

	public bool Supports(WaveFormat waveFormat)
	{
		if (waveFormat.Encoding == WaveFormatEncoding.Pcm && waveFormat.BitsPerSample == 8)
		{
			return waveFormat.Channels == 2;
		}
		return false;
	}

	public void LoadNextChunk(IWaveProvider source, int samplePairsRequired)
	{
		int num = samplePairsRequired * 2;
		sourceBuffer = BufferHelpers.Ensure(sourceBuffer, num);
		sourceBytes = source.Read(sourceBuffer, 0, num);
		offset = 0;
	}

	public bool GetNextSample(out float sampleLeft, out float sampleRight)
	{
		if (offset < sourceBytes)
		{
			sampleLeft = (float)(int)sourceBuffer[offset++] / 256f;
			sampleRight = (float)(int)sourceBuffer[offset++] / 256f;
			return true;
		}
		sampleLeft = 0f;
		sampleRight = 0f;
		return false;
	}
}
