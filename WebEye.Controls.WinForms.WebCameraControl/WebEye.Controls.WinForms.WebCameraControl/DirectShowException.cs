using System;
using System.Runtime.InteropServices;

namespace WebEye.Controls.WinForms.WebCameraControl;

public sealed class DirectShowException : Exception
{
	internal DirectShowException(string message, int hresult)
		: base(message, Marshal.GetExceptionForHR(hresult))
	{
	}
}
