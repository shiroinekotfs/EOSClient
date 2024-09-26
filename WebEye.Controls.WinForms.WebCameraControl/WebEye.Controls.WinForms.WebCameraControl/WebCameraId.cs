using System;

namespace WebEye.Controls.WinForms.WebCameraControl;

public sealed class WebCameraId : IEquatable<WebCameraId>
{
	private readonly string _name;

	private readonly string _devicePath;

	public string Name => _name;

	internal string DevicePath => _devicePath;

	internal WebCameraId(DirectShowProxy.VideoInputDeviceInfo info)
	{
		_name = info.FriendlyName;
		_devicePath = info.DevicePath;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj.GetType() != typeof(WebCameraId))
		{
			return false;
		}
		return Equals((WebCameraId)obj);
	}

	public bool Equals(WebCameraId other)
	{
		if ((object)other == null)
		{
			return false;
		}
		if ((object)this == other)
		{
			return true;
		}
		if (object.Equals(other._name, _name))
		{
			return object.Equals(other._devicePath, _devicePath);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (_name.GetHashCode() * 397) ^ _devicePath.GetHashCode();
	}

	public static bool operator ==(WebCameraId left, WebCameraId right)
	{
		return object.Equals(left, right);
	}

	public static bool operator !=(WebCameraId left, WebCameraId right)
	{
		return !object.Equals(left, right);
	}
}
