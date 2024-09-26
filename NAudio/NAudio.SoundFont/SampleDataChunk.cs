using System.IO;

namespace NAudio.SoundFont;

internal class SampleDataChunk
{
	private byte[] sampleData;

	public byte[] SampleData => sampleData;

	public SampleDataChunk(RiffChunk chunk)
	{
		string text = chunk.ReadChunkID();
		if (text != "sdta")
		{
			throw new InvalidDataException($"Not a sample data chunk ({text})");
		}
		sampleData = chunk.GetData();
	}
}
