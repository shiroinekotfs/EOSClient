using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NAudio.Gui;

public class WaveformPainter : Control
{
	private Pen foregroundPen;

	private List<float> samples = new List<float>(1000);

	private int maxSamples;

	private int insertPos;

	private IContainer components;

	public WaveformPainter()
	{
		SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
		InitializeComponent();
		OnForeColorChanged(EventArgs.Empty);
		OnResize(EventArgs.Empty);
	}

	protected override void OnResize(EventArgs e)
	{
		maxSamples = base.Width;
		base.OnResize(e);
	}

	protected override void OnForeColorChanged(EventArgs e)
	{
		foregroundPen = new Pen(ForeColor);
		base.OnForeColorChanged(e);
	}

	public void AddMax(float maxSample)
	{
		if (maxSamples != 0)
		{
			if (samples.Count <= maxSamples)
			{
				samples.Add(maxSample);
			}
			else if (insertPos < maxSamples)
			{
				samples[insertPos] = maxSample;
			}
			insertPos++;
			insertPos %= maxSamples;
			Invalidate();
		}
	}

	protected override void OnPaint(PaintEventArgs pe)
	{
		base.OnPaint(pe);
		for (int i = 0; i < base.Width; i++)
		{
			float num = (float)base.Height * GetSample(i - base.Width + insertPos);
			float num2 = ((float)base.Height - num) / 2f;
			pe.Graphics.DrawLine(foregroundPen, i, num2, i, num2 + num);
		}
	}

	private float GetSample(int index)
	{
		if (index < 0)
		{
			index += maxSamples;
		}
		if ((index >= 0) & (index < samples.Count))
		{
			return samples[index];
		}
		return 0f;
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
