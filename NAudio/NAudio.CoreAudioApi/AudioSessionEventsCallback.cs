using System;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi;

public class AudioSessionEventsCallback : IAudioSessionEvents
{
	private readonly IAudioSessionEventsHandler audioSessionEventsHandler;

	public AudioSessionEventsCallback(IAudioSessionEventsHandler handler)
	{
		audioSessionEventsHandler = handler;
	}

	public int OnDisplayNameChanged([In][MarshalAs(UnmanagedType.LPWStr)] string displayName, [In] ref Guid eventContext)
	{
		audioSessionEventsHandler.OnDisplayNameChanged(displayName);
		return 0;
	}

	public int OnIconPathChanged([In][MarshalAs(UnmanagedType.LPWStr)] string iconPath, [In] ref Guid eventContext)
	{
		audioSessionEventsHandler.OnIconPathChanged(iconPath);
		return 0;
	}

	public int OnSimpleVolumeChanged([In][MarshalAs(UnmanagedType.R4)] float volume, [In][MarshalAs(UnmanagedType.Bool)] bool isMuted, [In] ref Guid eventContext)
	{
		audioSessionEventsHandler.OnVolumeChanged(volume, isMuted);
		return 0;
	}

	public int OnChannelVolumeChanged([In][MarshalAs(UnmanagedType.U4)] uint channelCount, [In][MarshalAs(UnmanagedType.SysInt)] IntPtr newVolumes, [In][MarshalAs(UnmanagedType.U4)] uint channelIndex, [In] ref Guid eventContext)
	{
		audioSessionEventsHandler.OnChannelVolumeChanged(channelCount, newVolumes, channelIndex);
		return 0;
	}

	public int OnGroupingParamChanged([In] ref Guid groupingId, [In] ref Guid eventContext)
	{
		audioSessionEventsHandler.OnGroupingParamChanged(ref groupingId);
		return 0;
	}

	public int OnStateChanged([In] AudioSessionState state)
	{
		audioSessionEventsHandler.OnStateChanged(state);
		return 0;
	}

	public int OnSessionDisconnected([In] AudioSessionDisconnectReason disconnectReason)
	{
		audioSessionEventsHandler.OnSessionDisconnected(disconnectReason);
		return 0;
	}
}
