using NAudio.Utils;

namespace NAudio.Wave.SampleProviders;

internal class Mono8SampleChunkConverter : ISampleChunkConverter
{
	private int offset;

	private byte[] sourceBuffer;

	private int sourceBytes;

	public bool Supports(WaveFormat waveFormat)
	{
		if (waveFormat.Encoding == WaveFormatEncoding.Pcm && waveFormat.BitsPerSample == 8)
		{
			return waveFormat.Channels == 1;
		}
		return false;
	}

	public void LoadNextChunk(IWaveProvider source, int samplePairsRequired)
	{
		sourceBuffer = BufferHelpers.Ensure(sourceBuffer, samplePairsRequired);
		sourceBytes = source.Read(sourceBuffer, 0, samplePairsRequired);
		offset = 0;
	}

	public bool GetNextSample(out float sampleLeft, out float sampleRight)
	{
		if (offset < sourceBytes)
		{
			sampleLeft = (float)(int)sourceBuffer[offset] / 256f;
			offset++;
			sampleRight = sampleLeft;
			return true;
		}
		sampleLeft = 0f;
		sampleRight = 0f;
		return false;
	}
}
