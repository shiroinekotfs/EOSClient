using System;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi;

public class AudioEndpointVolumeChannel
{
	private readonly uint channel;

	private readonly IAudioEndpointVolume audioEndpointVolume;

	public float VolumeLevel
	{
		get
		{
			Marshal.ThrowExceptionForHR(audioEndpointVolume.GetChannelVolumeLevel(channel, out var pfLevelDB));
			return pfLevelDB;
		}
		set
		{
			Marshal.ThrowExceptionForHR(audioEndpointVolume.SetChannelVolumeLevel(channel, value, Guid.Empty));
		}
	}

	public float VolumeLevelScalar
	{
		get
		{
			Marshal.ThrowExceptionForHR(audioEndpointVolume.GetChannelVolumeLevelScalar(channel, out var pfLevel));
			return pfLevel;
		}
		set
		{
			Marshal.ThrowExceptionForHR(audioEndpointVolume.SetChannelVolumeLevelScalar(channel, value, Guid.Empty));
		}
	}

	internal AudioEndpointVolumeChannel(IAudioEndpointVolume parent, int channel)
	{
		this.channel = (uint)channel;
		audioEndpointVolume = parent;
	}
}
