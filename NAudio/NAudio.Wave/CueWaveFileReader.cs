namespace NAudio.Wave;

public class CueWaveFileReader : WaveFileReader
{
	private CueList cues;

	public CueList Cues
	{
		get
		{
			if (cues == null)
			{
				cues = CueList.FromChunks(this);
			}
			return cues;
		}
	}

	public CueWaveFileReader(string fileName)
		: base(fileName)
	{
	}
}
