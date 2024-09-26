using System;

namespace IRemote;

[Serializable]
public class RegisterData
{
	private string _login;

	private string _password;

	private DateTime _startTime;

	private string _machine;

	private string _examCode;

	public string Login
	{
		get
		{
			return _login;
		}
		set
		{
			_login = value;
		}
	}

	public string Password
	{
		get
		{
			return _password;
		}
		set
		{
			_password = value;
		}
	}

	public DateTime StartDate
	{
		get
		{
			return _startTime;
		}
		set
		{
			_startTime = value;
		}
	}

	public string Machine
	{
		get
		{
			return _machine;
		}
		set
		{
			_machine = value;
		}
	}

	public string ExamCode
	{
		get
		{
			return _examCode;
		}
		set
		{
			_examCode = value;
		}
	}
}
