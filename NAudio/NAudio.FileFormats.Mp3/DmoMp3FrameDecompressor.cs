using System;
using NAudio.Dmo;
using NAudio.Wave;

namespace NAudio.FileFormats.Mp3;

public class DmoMp3FrameDecompressor : IMp3FrameDecompressor, IDisposable
{
	private WindowsMediaMp3Decoder mp3Decoder;

	private WaveFormat pcmFormat;

	private MediaBuffer inputMediaBuffer;

	private DmoOutputDataBuffer outputBuffer;

	private bool reposition;

	public WaveFormat OutputFormat => pcmFormat;

	public DmoMp3FrameDecompressor(WaveFormat sourceFormat)
	{
		mp3Decoder = new WindowsMediaMp3Decoder();
		if (!mp3Decoder.MediaObject.SupportsInputWaveFormat(0, sourceFormat))
		{
			throw new ArgumentException("Unsupported input format");
		}
		mp3Decoder.MediaObject.SetInputWaveFormat(0, sourceFormat);
		pcmFormat = new WaveFormat(sourceFormat.SampleRate, sourceFormat.Channels);
		if (!mp3Decoder.MediaObject.SupportsOutputWaveFormat(0, pcmFormat))
		{
			throw new ArgumentException($"Unsupported output format {pcmFormat}");
		}
		mp3Decoder.MediaObject.SetOutputWaveFormat(0, pcmFormat);
		inputMediaBuffer = new MediaBuffer(sourceFormat.AverageBytesPerSecond);
		outputBuffer = new DmoOutputDataBuffer(pcmFormat.AverageBytesPerSecond);
	}

	public int DecompressFrame(Mp3Frame frame, byte[] dest, int destOffset)
	{
		inputMediaBuffer.LoadData(frame.RawData, frame.FrameLength);
		if (reposition)
		{
			mp3Decoder.MediaObject.Flush();
			reposition = false;
		}
		mp3Decoder.MediaObject.ProcessInput(0, inputMediaBuffer, DmoInputDataBufferFlags.None, 0L, 0L);
		outputBuffer.MediaBuffer.SetLength(0);
		outputBuffer.StatusFlags = DmoOutputDataBufferFlags.None;
		mp3Decoder.MediaObject.ProcessOutput(DmoProcessOutputFlags.None, 1, new DmoOutputDataBuffer[1] { outputBuffer });
		if (outputBuffer.Length == 0)
		{
			return 0;
		}
		outputBuffer.RetrieveData(dest, destOffset);
		return outputBuffer.Length;
	}

	public void Reset()
	{
		reposition = true;
	}

	public void Dispose()
	{
		if (inputMediaBuffer != null)
		{
			inputMediaBuffer.Dispose();
			inputMediaBuffer = null;
		}
		outputBuffer.Dispose();
		if (mp3Decoder != null)
		{
			mp3Decoder.Dispose();
			mp3Decoder = null;
		}
	}
}
