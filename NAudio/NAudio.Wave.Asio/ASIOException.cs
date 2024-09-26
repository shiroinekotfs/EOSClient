using System;

namespace NAudio.Wave.Asio;

internal class ASIOException : Exception
{
	private ASIOError error;

	public ASIOError Error
	{
		get
		{
			return error;
		}
		set
		{
			error = value;
			Data["ASIOError"] = error;
		}
	}

	public ASIOException()
	{
	}

	public ASIOException(string message)
		: base(message)
	{
	}

	public ASIOException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public static string getErrorName(ASIOError error)
	{
		return Enum.GetName(typeof(ASIOError), error);
	}
}
