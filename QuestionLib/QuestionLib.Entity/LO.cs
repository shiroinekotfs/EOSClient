namespace QuestionLib.Entity;

public class LO
{
	private int _LOID;

	private string _CID;

	private string _LO_Name;

	private string _LO_Desc;

	private string _Dec_No;

	public int LOID
	{
		get
		{
			return _LOID;
		}
		set
		{
			_LOID = value;
		}
	}

	public string CID
	{
		get
		{
			return _CID;
		}
		set
		{
			_CID = value;
		}
	}

	public string LO_Name
	{
		get
		{
			return _LO_Name;
		}
		set
		{
			_LO_Name = value;
		}
	}

	public string LO_Desc
	{
		get
		{
			return _LO_Desc;
		}
		set
		{
			_LO_Desc = value;
		}
	}

	public string Dec_No
	{
		get
		{
			return _Dec_No;
		}
		set
		{
			_Dec_No = value;
		}
	}

	public override string ToString()
	{
		return LO_Name + " - " + LO_Desc;
	}
}
