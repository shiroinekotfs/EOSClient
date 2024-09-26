using System;
using NAudio.CoreAudioApi;

namespace NAudio.Wave;

public class WasapiLoopbackCapture : WasapiCapture
{
	public override WaveFormat WaveFormat
	{
		get
		{
			return base.WaveFormat;
		}
		set
		{
			throw new InvalidOperationException("WaveFormat cannot be set for WASAPI Loopback Capture");
		}
	}

	public WasapiLoopbackCapture()
		: this(GetDefaultLoopbackCaptureDevice())
	{
	}

	public WasapiLoopbackCapture(MMDevice captureDevice)
		: base(captureDevice)
	{
	}

	public static MMDevice GetDefaultLoopbackCaptureDevice()
	{
		MMDeviceEnumerator mMDeviceEnumerator = new MMDeviceEnumerator();
		return mMDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
	}

	protected override AudioClientStreamFlags GetAudioClientStreamFlags()
	{
		return AudioClientStreamFlags.Loopback;
	}
}
