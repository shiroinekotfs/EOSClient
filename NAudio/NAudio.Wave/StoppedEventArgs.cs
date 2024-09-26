using System;

namespace NAudio.Wave;

public class StoppedEventArgs : EventArgs
{
	private readonly Exception exception;

	public Exception Exception => exception;

	public StoppedEventArgs(Exception exception = null)
	{
		this.exception = exception;
	}
}
