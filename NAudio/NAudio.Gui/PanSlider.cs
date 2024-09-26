using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NAudio.Gui;

public class PanSlider : UserControl
{
	private Container components;

	private float pan;

	public float Pan
	{
		get
		{
			return pan;
		}
		set
		{
			if (value < -1f)
			{
				value = -1f;
			}
			if (value > 1f)
			{
				value = 1f;
			}
			if (value != pan)
			{
				pan = value;
				if (this.PanChanged != null)
				{
					this.PanChanged(this, EventArgs.Empty);
				}
				Invalidate();
			}
		}
	}

	public event EventHandler PanChanged;

	public PanSlider()
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
		base.Name = "PanSlider";
		base.Size = new System.Drawing.Size(104, 16);
	}

	protected override void OnPaint(PaintEventArgs pe)
	{
		StringFormat stringFormat = new StringFormat();
		stringFormat.LineAlignment = StringAlignment.Center;
		stringFormat.Alignment = StringAlignment.Center;
		string s;
		if ((double)pan == 0.0)
		{
			pe.Graphics.FillRectangle(Brushes.Orange, base.Width / 2 - 1, 1, 3, base.Height - 2);
			s = "C";
		}
		else if (pan > 0f)
		{
			pe.Graphics.FillRectangle(Brushes.Orange, base.Width / 2, 1, (int)((float)(base.Width / 2) * pan), base.Height - 2);
			s = $"{pan * 100f:F0}%R";
		}
		else
		{
			pe.Graphics.FillRectangle(Brushes.Orange, (int)((float)(base.Width / 2) * (pan + 1f)), 1, (int)((float)(base.Width / 2) * (0f - pan)), base.Height - 2);
			s = $"{pan * -100f:F0}%L";
		}
		pe.Graphics.DrawRectangle(Pens.Black, 0, 0, base.Width - 1, base.Height - 1);
		pe.Graphics.DrawString(s, Font, Brushes.Black, base.ClientRectangle, stringFormat);
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
		{
			SetPanFromMouse(e.X);
		}
		base.OnMouseMove(e);
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		SetPanFromMouse(e.X);
		base.OnMouseDown(e);
	}

	private void SetPanFromMouse(int x)
	{
		Pan = (float)x / (float)base.Width * 2f - 1f;
	}
}
