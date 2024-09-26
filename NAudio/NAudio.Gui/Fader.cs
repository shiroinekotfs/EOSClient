using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NAudio.Gui;

public class Fader : Control
{
	private int minimum;

	private int maximum;

	private float percent;

	private Orientation orientation;

	private Container components;

	private readonly int SliderHeight = 30;

	private readonly int SliderWidth = 15;

	private Rectangle sliderRectangle = default(Rectangle);

	private bool dragging;

	private int dragY;

	public int Minimum
	{
		get
		{
			return minimum;
		}
		set
		{
			minimum = value;
		}
	}

	public int Maximum
	{
		get
		{
			return maximum;
		}
		set
		{
			maximum = value;
		}
	}

	public int Value
	{
		get
		{
			return (int)(percent * (float)(maximum - minimum)) + minimum;
		}
		set
		{
			percent = (float)(value - minimum) / (float)(maximum - minimum);
		}
	}

	public Orientation Orientation
	{
		get
		{
			return orientation;
		}
		set
		{
			orientation = value;
		}
	}

	public Fader()
	{
		InitializeComponent();
		SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, value: true);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void DrawSlider(Graphics g)
	{
		Brush brush = new SolidBrush(Color.White);
		Pen pen = new Pen(Color.Black);
		sliderRectangle.X = (base.Width - SliderWidth) / 2;
		sliderRectangle.Width = SliderWidth;
		sliderRectangle.Y = (int)((float)(base.Height - SliderHeight) * percent);
		sliderRectangle.Height = SliderHeight;
		g.FillRectangle(brush, sliderRectangle);
		g.DrawLine(pen, sliderRectangle.Left, sliderRectangle.Top + sliderRectangle.Height / 2, sliderRectangle.Right, sliderRectangle.Top + sliderRectangle.Height / 2);
		brush.Dispose();
		pen.Dispose();
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		Graphics graphics = e.Graphics;
		if (Orientation == Orientation.Vertical)
		{
			Brush brush = new SolidBrush(Color.Black);
			graphics.FillRectangle(brush, base.Width / 2, SliderHeight / 2, 2, base.Height - SliderHeight);
			brush.Dispose();
			DrawSlider(graphics);
		}
		base.OnPaint(e);
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		if (sliderRectangle.Contains(e.X, e.Y))
		{
			dragging = true;
			dragY = e.Y - sliderRectangle.Y;
		}
		base.OnMouseDown(e);
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		if (dragging)
		{
			int num = e.Y - dragY;
			if (num < 0)
			{
				percent = 0f;
			}
			else if (num > base.Height - SliderHeight)
			{
				percent = 1f;
			}
			else
			{
				percent = (float)num / (float)(base.Height - SliderHeight);
			}
			Invalidate();
		}
		base.OnMouseMove(e);
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		dragging = false;
		base.OnMouseUp(e);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
	}
}
