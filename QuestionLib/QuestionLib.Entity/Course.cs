namespace QuestionLib.Entity;

public class Course
{
	private string _cid;

	private string _name;

	public string CID
	{
		get
		{
			return _cid;
		}
		set
		{
			_cid = value;
		}
	}

	public string Name
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
		}
	}

	public Course()
	{
	}

	public Course(string _cid, string _name)
	{
		this._cid = _cid;
		this._name = _name;
	}
}
