namespace QuestionLib.Entity;

public class QuestionLO
{
	private int _QuestionLOID;

	private QuestionType _QType;

	private int _QID;

	private int _LOID;

	public int QuestionLOID
	{
		get
		{
			return _QuestionLOID;
		}
		set
		{
			_QuestionLOID = value;
		}
	}

	public QuestionType QType
	{
		get
		{
			return _QType;
		}
		set
		{
			_QType = value;
		}
	}

	public int QID
	{
		get
		{
			return _QID;
		}
		set
		{
			_QID = value;
		}
	}

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
}
