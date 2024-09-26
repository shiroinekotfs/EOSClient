using System;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi;

public class SimpleAudioVolume : IDisposable
{
	private readonly ISimpleAudioVolume simpleAudioVolume;

	public float Volume
	{
		get
		{
			Marshal.ThrowExceptionForHR(simpleAudioVolume.GetMasterVolume(out var levelNorm));
			return levelNorm;
		}
		set
		{
			if ((double)value >= 0.0 && (double)value <= 1.0)
			{
				Marshal.ThrowExceptionForHR(simpleAudioVolume.SetMasterVolume(value, Guid.Empty));
			}
		}
	}

	public bool Mute
	{
		get
		{
			Marshal.ThrowExceptionForHR(simpleAudioVolume.GetMute(out var isMuted));
			return isMuted;
		}
		set
		{
			Marshal.ThrowExceptionForHR(simpleAudioVolume.SetMute(value, Guid.Empty));
		}
	}

	internal SimpleAudioVolume(ISimpleAudioVolume realSimpleVolume)
	{
		simpleAudioVolume = realSimpleVolume;
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
	}

	~SimpleAudioVolume()
	{
		Dispose();
	}
}
