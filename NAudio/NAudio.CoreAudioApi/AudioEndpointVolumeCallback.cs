using System;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi;

internal class AudioEndpointVolumeCallback : IAudioEndpointVolumeCallback
{
	private readonly AudioEndpointVolume parent;

	internal AudioEndpointVolumeCallback(AudioEndpointVolume parent)
	{
		this.parent = parent;
	}

	public void OnNotify(IntPtr notifyData)
	{
		AudioVolumeNotificationDataStruct audioVolumeNotificationDataStruct = (AudioVolumeNotificationDataStruct)Marshal.PtrToStructure(notifyData, typeof(AudioVolumeNotificationDataStruct));
		IntPtr intPtr = Marshal.OffsetOf(typeof(AudioVolumeNotificationDataStruct), "ChannelVolume");
		IntPtr ptr = (IntPtr)((long)notifyData + (long)intPtr);
		float[] array = new float[audioVolumeNotificationDataStruct.nChannels];
		for (int i = 0; i < audioVolumeNotificationDataStruct.nChannels; i++)
		{
			array[i] = (float)Marshal.PtrToStructure(ptr, typeof(float));
		}
		AudioVolumeNotificationData notificationData = new AudioVolumeNotificationData(audioVolumeNotificationDataStruct.guidEventContext, audioVolumeNotificationDataStruct.bMuted, audioVolumeNotificationDataStruct.fMasterVolume, array);
		parent.FireNotification(notificationData);
	}
}
