using System.IO;

namespace NAudio.Wave;

public class CueWaveFileWriter : WaveFileWriter
{
	private CueList cues;

	public CueWaveFileWriter(string fileName, WaveFormat waveFormat)
		: base(fileName, waveFormat)
	{
	}

	public void AddCue(int position, string label)
	{
		if (cues == null)
		{
			cues = new CueList();
		}
		cues.Add(new Cue(position, label));
	}

	private void WriteCues(BinaryWriter w)
	{
		if (cues != null)
		{
			byte[] rIFFChunks = cues.GetRIFFChunks();
			int count = rIFFChunks.Length;
			w.Seek(0, SeekOrigin.End);
			if (w.BaseStream.Length % 2 == 1)
			{
				w.Write((byte)0);
			}
			w.Write(cues.GetRIFFChunks(), 0, count);
			w.Seek(4, SeekOrigin.Begin);
			w.Write((int)(w.BaseStream.Length - 8));
		}
	}

	protected override void UpdateHeader(BinaryWriter writer)
	{
		base.UpdateHeader(writer);
		WriteCues(writer);
	}
}
