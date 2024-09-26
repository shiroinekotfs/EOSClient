using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace NAudio.Gui;

public class Pot : UserControl
{
	private double minimum;

	private double maximum = 1.0;

	private double value = 0.5;

	private int beginDragY;

	private double beginDragValue;

	private bool dragging;

	private IContainer components;

	public double Minimum
	{
		get
		{
			return minimum;
		}
		set
		{
			if (value >= maximum)
			{
				throw new ArgumentOutOfRangeException("Minimum must be less than maximum");
			}
			minimum = value;
			if (Value < minimum)
			{
				Value = minimum;
			}
		}
	}

	public double Maximum
	{
		get
		{
			return maximum;
		}
		set
		{
			if (value <= minimum)
			{
				throw new ArgumentOutOfRangeException("Maximum must be greater than minimum");
			}
			maximum = value;
			if (Value > maximum)
			{
				Value = maximum;
			}
		}
	}

	public double Value
	{
		get
		{
			return value;
		}
		set
		{
			SetValue(value, raiseEvents: false);
		}
	}

	public event EventHandler ValueChanged;

	public Pot()
	{
		SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, value: true);
		InitializeComponent();
	}

	private void SetValue(double newValue, bool raiseEvents)
	{
		if (value != newValue)
		{
			value = newValue;
			if (raiseEvents && this.ValueChanged != null)
			{
				this.ValueChanged(this, EventArgs.Empty);
			}
			Invalidate();
		}
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		int num = Math.Min(base.Width - 4, base.Height - 4);
		Pen pen = new Pen(ForeColor, 3f);
		pen.LineJoin = LineJoin.Round;
		GraphicsState gstate = e.Graphics.Save();
		e.Graphics.TranslateTransform(base.Width / 2, base.Height / 2);
		e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
		e.Graphics.DrawArc(pen, new Rectangle(num / -2, num / -2, num, num), 135f, 270f);
		double num2 = (value - minimum) / (maximum - minimum);
		double num3 = 135.0 + num2 * 270.0;
		double num4 = (double)num / 2.0 * Math.Cos(Math.PI * num3 / 180.0);
		double num5 = (double)num / 2.0 * Math.Sin(Math.PI * num3 / 180.0);
		e.Graphics.DrawLine(pen, 0f, 0f, (float)num4, (float)num5);
		e.Graphics.Restore(gstate);
		base.OnPaint(e);
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		dragging = true;
		beginDragY = e.Y;
		beginDragValue = value;
		base.OnMouseDown(e);
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		dragging = false;
		base.OnMouseUp(e);
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		if (dragging)
		{
			int num = beginDragY - e.Y;
			double num2 = (maximum - minimum) * ((double)num / 150.0);
			double num3 = beginDragValue + num2;
			if (num3 < minimum)
			{
				num3 = minimum;
			}
			if (num3 > maximum)
			{
				num3 = maximum;
			}
			SetValue(num3, raiseEvents: true);
		}
		base.OnMouseMove(e);
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
		base.SuspendLayout();
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Name = "Pot";
		base.Size = new System.Drawing.Size(32, 32);
		base.ResumeLayout(false);
	}
}
