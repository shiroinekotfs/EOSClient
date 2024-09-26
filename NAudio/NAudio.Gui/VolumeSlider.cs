using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NAudio.Gui;

public class VolumeSlider : UserControl
{
	private Container components;

	private float volume = 1f;

	private float MinDb = -48f;

	[DefaultValue(1f)]
	public float Volume
	{
		get
		{
			return volume;
		}
		set
		{
			if (value < 0f)
			{
				value = 0f;
			}
			if (value > 1f)
			{
				value = 1f;
			}
			if (volume != value)
			{
				volume = value;
				if (this.VolumeChanged != null)
				{
					this.VolumeChanged(this, EventArgs.Empty);
				}
				Invalidate();
			}
		}
	}

	public event EventHandler VolumeChanged;

	public VolumeSlider()
	{
		InitializeComponent();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		base.Name = "VolumeSlider";
		base.Size = new System.Drawing.Size(96, 16);
	}

	protected override void OnPaint(PaintEventArgs pe)
	{
		StringFormat stringFormat = new StringFormat();
		stringFormat.LineAlignment = StringAlignment.Center;
		stringFormat.Alignment = StringAlignment.Center;
		pe.Graphics.DrawRectangle(Pens.Black, 0, 0, base.Width - 1, base.Height - 1);
		float num = 20f * (float)Math.Log10(Volume);
		float num2 = 1f - num / MinDb;
		pe.Graphics.FillRectangle(Brushes.LightGreen, 1, 1, (int)((float)(base.Width - 2) * num2), base.Height - 2);
		string s = $"{num:F2} dB";
		pe.Graphics.DrawString(s, Font, Brushes.Black, base.ClientRectangle, stringFormat);
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
		{
			SetVolumeFromMouse(e.X);
		}
		base.OnMouseMove(e);
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		SetVolumeFromMouse(e.X);
		base.OnMouseDown(e);
	}

	private void SetVolumeFromMouse(int x)
	{
		float num = (1f - (float)x / (float)base.Width) * MinDb;
		if (x <= 0)
		{
			Volume = 0f;
		}
		else
		{
			Volume = (float)Math.Pow(10.0, num / 20f);
		}
	}
}
