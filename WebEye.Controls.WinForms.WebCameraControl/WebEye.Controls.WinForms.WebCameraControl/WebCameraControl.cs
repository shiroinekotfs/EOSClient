using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WebEye.Controls.WinForms.WebCameraControl;

public sealed class WebCameraControl : UserControl
{
	private DirectShowProxy _proxy;

	private readonly List<WebCameraId> _captureDevices = new List<WebCameraId>();

	private bool _isCapturing;

	private bool _captureGraphInitialized;

	private WebCameraId _currentCamera;

	private IContainer components;

	private DirectShowProxy Proxy => _proxy ?? (_proxy = new DirectShowProxy());

	[Browsable(false)]
	public bool IsCapturing => _isCapturing;

	[Browsable(false)]
	public Size VideoSize
	{
		get
		{
			if (!_isCapturing)
			{
				return new Size(0, 0);
			}
			return Proxy.GetVideoSize();
		}
	}

	public WebCameraControl()
	{
		InitializeComponent();
	}

	private void SaveVideoDevice(ref DirectShowProxy.VideoInputDeviceInfo info)
	{
		if (!string.IsNullOrEmpty(info.DevicePath))
		{
			_captureDevices.Add(new WebCameraId(info));
		}
	}

	public IEnumerable<WebCameraId> GetVideoCaptureDevices()
	{
		_captureDevices.Clear();
		Proxy.EnumVideoInputDevices(SaveVideoDevice);
		return new List<WebCameraId>(_captureDevices);
	}

	private void InitializeCaptureGraph()
	{
		Proxy.BuildCaptureGraph();
		Proxy.AddRenderFilter(base.Handle);
	}

	public void StartCapture(WebCameraId camera)
	{
		if (camera == null)
		{
			throw new ArgumentNullException();
		}
		if (!_captureGraphInitialized)
		{
			InitializeCaptureGraph();
			_captureGraphInitialized = true;
		}
		if (_isCapturing)
		{
			if (_currentCamera == camera)
			{
				return;
			}
			StopCapture();
		}
		if (_currentCamera != null)
		{
			Proxy.ResetCaptureGraph();
			_currentCamera = null;
		}
		Proxy.AddCaptureFilter(camera.DevicePath);
		_currentCamera = camera;
		try
		{
			Proxy.Start();
			_isCapturing = true;
		}
		catch (DirectShowException)
		{
			Proxy.ResetCaptureGraph();
			_currentCamera = null;
			throw;
		}
	}

	public Bitmap GetCurrentImage()
	{
		if (!_isCapturing)
		{
			throw new InvalidOperationException();
		}
		return Proxy.GetCurrentImage();
	}

	public void StopCapture()
	{
		if (!_isCapturing)
		{
			throw new InvalidOperationException();
		}
		Proxy.Stop();
		_isCapturing = false;
		Proxy.ResetCaptureGraph();
		_currentCamera = null;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && _proxy != null)
		{
			if (_isCapturing)
			{
				StopCapture();
			}
			Proxy.DestroyCaptureGraph();
			Proxy.Dispose();
		}
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
	}
}
