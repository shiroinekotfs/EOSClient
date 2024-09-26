using System;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi;

public class MMDevice
{
	private readonly IMMDevice deviceInterface;

	private PropertyStore propertyStore;

	private AudioMeterInformation audioMeterInformation;

	private AudioEndpointVolume audioEndpointVolume;

	private AudioSessionManager audioSessionManager;

	private static Guid IID_IAudioMeterInformation = new Guid("C02216F6-8C67-4B5B-9D00-D008E73E0064");

	private static Guid IID_IAudioEndpointVolume = new Guid("5CDF2C82-841E-4546-9722-0CF74078229A");

	private static Guid IID_IAudioClient = new Guid("1CB9AD4C-DBFA-4c32-B178-C2F568A703B2");

	private static Guid IDD_IAudioSessionManager = new Guid("BFA971F1-4D5E-40BB-935E-967039BFBEE4");

	public AudioClient AudioClient => GetAudioClient();

	public AudioMeterInformation AudioMeterInformation
	{
		get
		{
			if (audioMeterInformation == null)
			{
				GetAudioMeterInformation();
			}
			return audioMeterInformation;
		}
	}

	public AudioEndpointVolume AudioEndpointVolume
	{
		get
		{
			if (audioEndpointVolume == null)
			{
				GetAudioEndpointVolume();
			}
			return audioEndpointVolume;
		}
	}

	public AudioSessionManager AudioSessionManager
	{
		get
		{
			if (audioSessionManager == null)
			{
				GetAudioSessionManager();
			}
			return audioSessionManager;
		}
	}

	public PropertyStore Properties
	{
		get
		{
			if (propertyStore == null)
			{
				GetPropertyInformation();
			}
			return propertyStore;
		}
	}

	public string FriendlyName
	{
		get
		{
			if (propertyStore == null)
			{
				GetPropertyInformation();
			}
			if (propertyStore.Contains(PropertyKeys.PKEY_Device_FriendlyName))
			{
				return (string)propertyStore[PropertyKeys.PKEY_Device_FriendlyName].Value;
			}
			return "Unknown";
		}
	}

	public string DeviceFriendlyName
	{
		get
		{
			if (propertyStore == null)
			{
				GetPropertyInformation();
			}
			if (propertyStore.Contains(PropertyKeys.PKEY_DeviceInterface_FriendlyName))
			{
				return (string)propertyStore[PropertyKeys.PKEY_DeviceInterface_FriendlyName].Value;
			}
			return "Unknown";
		}
	}

	public string IconPath
	{
		get
		{
			if (propertyStore == null)
			{
				GetPropertyInformation();
			}
			if (propertyStore.Contains(PropertyKeys.PKEY_Device_IconPath))
			{
				return (string)propertyStore[PropertyKeys.PKEY_Device_IconPath].Value;
			}
			return "Unknown";
		}
	}

	public string ID
	{
		get
		{
			Marshal.ThrowExceptionForHR(deviceInterface.GetId(out var id));
			return id;
		}
	}

	public DataFlow DataFlow
	{
		get
		{
			IMMEndpoint iMMEndpoint = deviceInterface as IMMEndpoint;
			iMMEndpoint.GetDataFlow(out var dataFlow);
			return dataFlow;
		}
	}

	public DeviceState State
	{
		get
		{
			Marshal.ThrowExceptionForHR(deviceInterface.GetState(out var state));
			return state;
		}
	}

	private void GetPropertyInformation()
	{
		Marshal.ThrowExceptionForHR(deviceInterface.OpenPropertyStore(StorageAccessMode.Read, out var properties));
		propertyStore = new PropertyStore(properties);
	}

	private AudioClient GetAudioClient()
	{
		Marshal.ThrowExceptionForHR(deviceInterface.Activate(ref IID_IAudioClient, ClsCtx.ALL, IntPtr.Zero, out var interfacePointer));
		return new AudioClient(interfacePointer as IAudioClient);
	}

	private void GetAudioMeterInformation()
	{
		Marshal.ThrowExceptionForHR(deviceInterface.Activate(ref IID_IAudioMeterInformation, ClsCtx.ALL, IntPtr.Zero, out var interfacePointer));
		audioMeterInformation = new AudioMeterInformation(interfacePointer as IAudioMeterInformation);
	}

	private void GetAudioEndpointVolume()
	{
		Marshal.ThrowExceptionForHR(deviceInterface.Activate(ref IID_IAudioEndpointVolume, ClsCtx.ALL, IntPtr.Zero, out var interfacePointer));
		audioEndpointVolume = new AudioEndpointVolume(interfacePointer as IAudioEndpointVolume);
	}

	private void GetAudioSessionManager()
	{
		Marshal.ThrowExceptionForHR(deviceInterface.Activate(ref IDD_IAudioSessionManager, ClsCtx.ALL, IntPtr.Zero, out var interfacePointer));
		audioSessionManager = new AudioSessionManager(interfacePointer as IAudioSessionManager);
	}

	internal MMDevice(IMMDevice realDevice)
	{
		deviceInterface = realDevice;
	}

	public override string ToString()
	{
		return FriendlyName;
	}
}
