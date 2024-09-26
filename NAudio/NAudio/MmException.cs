using System;

namespace NAudio;

public class MmException : Exception
{
	private MmResult result;

	private string function;

	public MmResult Result => result;

	public MmException(MmResult result, string function)
		: base(ErrorMessage(result, function))
	{
		this.result = result;
		this.function = function;
	}

	private static string ErrorMessage(MmResult result, string function)
	{
		return $"{result} calling {function}";
	}

	public static void Try(MmResult result, string function)
	{
		if (result != 0)
		{
			throw new MmException(result, function);
		}
	}
}
