using System;

namespace NAudio.Wave;

public class WaveRecorder : IWaveProvider, IDisposable
{
	private WaveFileWriter writer;

	private IWaveProvider source;

	public WaveFormat WaveFormat => source.WaveFormat;

	public WaveRecorder(IWaveProvider source, string destination)
	{
		this.source = source;
		writer = new WaveFileWriter(destination, source.WaveFormat);
	}

	public int Read(byte[] buffer, int offset, int count)
	{
		int num = source.Read(buffer, offset, count);
		writer.Write(buffer, offset, num);
		return num;
	}

	public void Dispose()
	{
		if (writer != null)
		{
			writer.Dispose();
			writer = null;
		}
	}
}
