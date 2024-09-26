using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using WebEye.Controls.WinForms.WebCameraControl.Properties;

namespace WebEye.Controls.WinForms.WebCameraControl;

internal sealed class DirectShowProxy : IDisposable
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct VideoInputDeviceInfo
	{
		[MarshalAs(UnmanagedType.BStr)]
		internal string FriendlyName;

		[MarshalAs(UnmanagedType.BStr)]
		internal string DevicePath;
	}

	internal delegate void EnumVideoInputDevicesCallback(ref VideoInputDeviceInfo info);

	private delegate void EnumVideoInputDevicesDelegate(EnumVideoInputDevicesCallback callback);

	private delegate int BuildCaptureGraphDelegate();

	private delegate int AddRenderFilterDelegate(IntPtr hWnd);

	private delegate int AddCaptureFilterDelegate([MarshalAs(UnmanagedType.BStr)] string devicePath);

	private delegate int ResetCaptureGraphDelegate();

	private delegate int StartDelegate();

	private delegate int GetCurrentImageDelegate(out IntPtr dibPtr);

	private delegate int GetVideoSizeDelegate(out int width, out int height);

	private delegate int StopDelegate();

	private delegate void DestroyCaptureGraphDelegate();

	private struct BITMAPINFOHEADER
	{
		public uint biSize;

		public int biWidth;

		public int biHeight;

		public ushort biPlanes;

		public ushort biBitCount;

		public uint biCompression;

		public uint biSizeImage;

		public int biXPelsPerMeter;

		public int biYPelsPerMeter;

		public uint biClrUsed;

		public uint biClrImportant;
	}

	private EnumVideoInputDevicesDelegate _enumVideoInputDevices;

	private BuildCaptureGraphDelegate _buildCaptureGraph;

	private AddRenderFilterDelegate _addRenderFilter;

	private AddCaptureFilterDelegate _addCaptureFilter;

	private ResetCaptureGraphDelegate _resetCaptureGraph;

	private StartDelegate _start;

	private GetCurrentImageDelegate _getCurrentImage;

	private GetVideoSizeDelegate _getVideoSize;

	private StopDelegate _stop;

	private DestroyCaptureGraphDelegate _destroyCaptureGraph;

	private string _dllFile = string.Empty;

	private IntPtr _hDll = IntPtr.Zero;

	private bool IsX86Platform => IntPtr.Size == 4;

	[DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern IntPtr LoadLibrary(string lpFileName);

	private void LoadDll()
	{
		_dllFile = Path.GetTempFileName();
		using (FileStream output = new FileStream(_dllFile, FileMode.Create, FileAccess.Write))
		{
			using BinaryWriter binaryWriter = new BinaryWriter(output);
			binaryWriter.Write(IsX86Platform ? Resources.DirectShowFacade : Resources.DirectShowFacade64);
		}
		_hDll = LoadLibrary(_dllFile);
		if (_hDll == IntPtr.Zero)
		{
			throw new Win32Exception(Marshal.GetLastWin32Error());
		}
	}

	[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
	private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

	private void BindToDll(IntPtr hDll)
	{
		IntPtr procAddress = GetProcAddress(hDll, "EnumVideoInputDevices");
		_enumVideoInputDevices = (EnumVideoInputDevicesDelegate)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(EnumVideoInputDevicesDelegate));
		procAddress = GetProcAddress(hDll, "BuildCaptureGraph");
		_buildCaptureGraph = (BuildCaptureGraphDelegate)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(BuildCaptureGraphDelegate));
		procAddress = GetProcAddress(hDll, "AddRenderFilter");
		_addRenderFilter = (AddRenderFilterDelegate)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(AddRenderFilterDelegate));
		procAddress = GetProcAddress(hDll, "AddCaptureFilter");
		_addCaptureFilter = (AddCaptureFilterDelegate)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(AddCaptureFilterDelegate));
		procAddress = GetProcAddress(hDll, "ResetCaptureGraph");
		_resetCaptureGraph = (ResetCaptureGraphDelegate)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(ResetCaptureGraphDelegate));
		procAddress = GetProcAddress(hDll, "Start");
		_start = (StartDelegate)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(StartDelegate));
		procAddress = GetProcAddress(hDll, "GetCurrentImage");
		_getCurrentImage = (GetCurrentImageDelegate)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(GetCurrentImageDelegate));
		procAddress = GetProcAddress(hDll, "GetVideoSize");
		_getVideoSize = (GetVideoSizeDelegate)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(GetVideoSizeDelegate));
		procAddress = GetProcAddress(hDll, "Stop");
		_stop = (StopDelegate)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(StopDelegate));
		procAddress = GetProcAddress(hDll, "DestroyCaptureGraph");
		_destroyCaptureGraph = (DestroyCaptureGraphDelegate)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(DestroyCaptureGraphDelegate));
	}

	internal DirectShowProxy()
	{
		LoadDll();
		BindToDll(_hDll);
	}

	internal void EnumVideoInputDevices(EnumVideoInputDevicesCallback callback)
	{
		_enumVideoInputDevices(callback);
	}

	private static void ThrowExceptionForResult(int hresult, string message)
	{
		if (hresult < 0)
		{
			throw new DirectShowException(message, hresult);
		}
	}

	internal void BuildCaptureGraph()
	{
		ThrowExceptionForResult(_buildCaptureGraph(), "Failed to build a video capture graph.");
	}

	internal void AddRenderFilter(IntPtr hWnd)
	{
		ThrowExceptionForResult(_addRenderFilter(hWnd), "Failed to setup a render filter.");
	}

	internal void AddCaptureFilter(string devicePath)
	{
		ThrowExceptionForResult(_addCaptureFilter(devicePath), "Failed to add a video capture filter.");
	}

	internal void ResetCaptureGraph()
	{
		ThrowExceptionForResult(_resetCaptureGraph(), "Failed to reset a video capture graph.");
	}

	internal void Start()
	{
		ThrowExceptionForResult(_start(), "Failed to run a capture graph.");
	}

	internal Bitmap GetCurrentImage()
	{
		ThrowExceptionForResult(_getCurrentImage(out var dibPtr), "Failed to get the current image.");
		try
		{
			BITMAPINFOHEADER bITMAPINFOHEADER = (BITMAPINFOHEADER)Marshal.PtrToStructure(dibPtr, typeof(BITMAPINFOHEADER));
			int num = bITMAPINFOHEADER.biWidth * (bITMAPINFOHEADER.biBitCount / 8);
			int num2 = ((num % 4 > 0) ? (4 - num % 4) : 0);
			num += num2;
			PixelFormat format = PixelFormat.Undefined;
			switch (bITMAPINFOHEADER.biBitCount)
			{
			case 1:
				format = PixelFormat.Format1bppIndexed;
				break;
			case 4:
				format = PixelFormat.Format4bppIndexed;
				break;
			case 8:
				format = PixelFormat.Format8bppIndexed;
				break;
			case 16:
				format = PixelFormat.Format16bppRgb555;
				break;
			case 24:
				format = PixelFormat.Format24bppRgb;
				break;
			case 32:
				format = PixelFormat.Format32bppRgb;
				break;
			}
			Bitmap bitmap = new Bitmap(bITMAPINFOHEADER.biWidth, bITMAPINFOHEADER.biHeight, num, format, (IntPtr)(dibPtr.ToInt64() + Marshal.SizeOf(bITMAPINFOHEADER)));
			bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
			return bitmap;
		}
		finally
		{
			if (dibPtr != IntPtr.Zero)
			{
				Marshal.FreeCoTaskMem(dibPtr);
			}
		}
	}

	internal Size GetVideoSize()
	{
		ThrowExceptionForResult(_getVideoSize(out var width, out var height), "Failed to get the video size.");
		return new Size(width, height);
	}

	internal void Stop()
	{
		ThrowExceptionForResult(_stop(), "Failed to stop a video capture graph.");
	}

	internal void DestroyCaptureGraph()
	{
		_destroyCaptureGraph();
	}

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool FreeLibrary(IntPtr hModule);

	public void Dispose()
	{
		if (_hDll != IntPtr.Zero)
		{
			FreeLibrary(_hDll);
			_hDll = IntPtr.Zero;
		}
		if (File.Exists(_dllFile))
		{
			File.Delete(_dllFile);
		}
	}
}
