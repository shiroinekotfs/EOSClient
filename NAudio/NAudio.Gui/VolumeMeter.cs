using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NAudio.Gui;

public class VolumeMeter : Control
{
	private Brush foregroundBrush;

	private float amplitude;

	private IContainer components;

	[DefaultValue(-3.0)]
	public float Amplitude
	{
		get
		{
			return amplitude;
		}
		set
		{
			amplitude = value;
			Invalidate();
		}
	}

	[DefaultValue(-60.0)]
	public float MinDb { get; set; }

	[DefaultValue(18.0)]
	public float MaxDb { get; set; }

	[DefaultValue(Orientation.Vertical)]
	public Orientation Orientation { get; set; }

	public VolumeMeter()
	{
		SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
		MinDb = -60f;
		MaxDb = 18f;
		Amplitude = 0f;
		Orientation = Orientation.Vertical;
		InitializeComponent();
		OnForeColorChanged(EventArgs.Empty);
	}

	protected override void OnForeColorChanged(EventArgs e)
	{
		foregroundBrush = new SolidBrush(ForeColor);
		base.OnForeColorChanged(e);
	}

	protected override void OnPaint(PaintEventArgs pe)
	{
		pe.Graphics.DrawRectangle(Pens.Black, 0, 0, base.Width - 1, base.Height - 1);
		double num = 20.0 * Math.Log10(Amplitude);
		if (num < (double)MinDb)
		{
			num = MinDb;
		}
		if (num > (double)MaxDb)
		{
			num = MaxDb;
		}
		double num2 = (num - (double)MinDb) / (double)(MaxDb - MinDb);
		int num3 = base.Width - 2;
		int num4 = base.Height - 2;
		if (Orientation == Orientation.Horizontal)
		{
			num3 = (int)((double)num3 * num2);
			pe.Graphics.FillRectangle(foregroundBrush, 1, 1, num3, num4);
		}
		else
		{
			num4 = (int)((double)num4 * num2);
			pe.Graphics.FillRectangle(foregroundBrush, 1, base.Height - 1 - num4, num3, num4);
		}
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
		this.components = new System.ComponentModel.Container();
	}
}
