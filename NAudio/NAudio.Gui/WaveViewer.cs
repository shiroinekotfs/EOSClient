using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using NAudio.Wave;

namespace NAudio.Gui;

public class WaveViewer : UserControl
{
	private Container components;

	private WaveStream waveStream;

	private int samplesPerPixel = 128;

	private long startPosition;

	private int bytesPerSample;

	public WaveStream WaveStream
	{
		get
		{
			return waveStream;
		}
		set
		{
			waveStream = value;
			if (waveStream != null)
			{
				bytesPerSample = waveStream.WaveFormat.BitsPerSample / 8 * waveStream.WaveFormat.Channels;
			}
			Invalidate();
		}
	}

	public int SamplesPerPixel
	{
		get
		{
			return samplesPerPixel;
		}
		set
		{
			samplesPerPixel = value;
			Invalidate();
		}
	}

	public long StartPosition
	{
		get
		{
			return startPosition;
		}
		set
		{
			startPosition = value;
		}
	}

	public WaveViewer()
	{
		InitializeComponent();
		DoubleBuffered = true;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		if (waveStream != null)
		{
			waveStream.Position = 0L;
			byte[] array = new byte[samplesPerPixel * bytesPerSample];
			waveStream.Position = startPosition + e.ClipRectangle.Left * bytesPerSample * samplesPerPixel;
			for (float num = e.ClipRectangle.X; num < (float)e.ClipRectangle.Right; num += 1f)
			{
				short num2 = 0;
				short num3 = 0;
				int num4 = waveStream.Read(array, 0, samplesPerPixel * bytesPerSample);
				if (num4 == 0)
				{
					break;
				}
				for (int i = 0; i < num4; i += 2)
				{
					short num5 = BitConverter.ToInt16(array, i);
					if (num5 < num2)
					{
						num2 = num5;
					}
					if (num5 > num3)
					{
						num3 = num5;
					}
				}
				float num6 = ((float)num2 - -32768f) / 65535f;
				float num7 = ((float)num3 - -32768f) / 65535f;
				e.Graphics.DrawLine(Pens.Black, num, (float)base.Height * num6, num, (float)base.Height * num7);
			}
		}
		base.OnPaint(e);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
	}
}
