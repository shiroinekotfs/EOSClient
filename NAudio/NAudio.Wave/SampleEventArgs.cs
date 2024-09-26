using System;

namespace NAudio.Wave;

public class SampleEventArgs : EventArgs
{
	public float Left { get; set; }

	public float Right { get; set; }

	public SampleEventArgs(float left, float right)
	{
		Left = left;
		Right = right;
	}
}
