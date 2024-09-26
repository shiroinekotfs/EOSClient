using System;

namespace NAudio.CoreAudioApi;

public class AudioVolumeNotificationData
{
	private readonly Guid eventContext;

	private readonly bool muted;

	private readonly float masterVolume;

	private readonly int channels;

	private readonly float[] channelVolume;

	public Guid EventContext => eventContext;

	public bool Muted => muted;

	public float MasterVolume => masterVolume;

	public int Channels => channels;

	public float[] ChannelVolume => channelVolume;

	public AudioVolumeNotificationData(Guid eventContext, bool muted, float masterVolume, float[] channelVolume)
	{
		this.eventContext = eventContext;
		this.muted = muted;
		this.masterVolume = masterVolume;
		channels = channelVolume.Length;
		this.channelVolume = channelVolume;
	}
}
