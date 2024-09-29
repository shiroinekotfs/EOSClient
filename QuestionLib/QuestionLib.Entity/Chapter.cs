namespace QuestionLib.Entity;

public class Chapter
{
	private int _chid;

	private string _cid;

	private string _name;

	public int ChID
	{
		get
		{
			return _chid;
		}
		set
		{
			_chid = value;
		}
	}

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

	public Chapter()
	{
	}

	public Chapter(int _chid, string _cid, string _name)
	{
		this._chid = _chid;
		this._cid = _cid;
		this._name = _name;
	}

	public override string ToString()
	{
		return _name;
	}
}
