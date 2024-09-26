using System;
using System.Collections;

namespace QuestionLib.Entity;

[Serializable]
public class MatchQuestion
{
	private int _mid;

	private string _courseId;

	private int _chapterId;

	private string _columnA;

	private string _columnB;

	private string _solution;

	private float _mark;

	private string _studentAnswer;

	private ArrayList _questionLOs;

	private int _QBID;

	public int QBID
	{
		get
		{
			return _QBID;
		}
		set
		{
			_QBID = value;
		}
	}

	public int MID
	{
		get
		{
			return _mid;
		}
		set
		{
			_mid = value;
		}
	}

	public string CourseId
	{
		get
		{
			return _courseId;
		}
		set
		{
			_courseId = value;
		}
	}

	public int ChapterId
	{
		get
		{
			return _chapterId;
		}
		set
		{
			_chapterId = value;
		}
	}

	public string ColumnA
	{
		get
		{
			return _columnA;
		}
		set
		{
			_columnA = value;
		}
	}

	public string ColumnB
	{
		get
		{
			return _columnB;
		}
		set
		{
			_columnB = value;
		}
	}

	public string Solution
	{
		get
		{
			return _solution;
		}
		set
		{
			_solution = value;
		}
	}

	public float Mark
	{
		get
		{
			return _mark;
		}
		set
		{
			_mark = value;
		}
	}

	public string SudentAnswer
	{
		get
		{
			return _studentAnswer;
		}
		set
		{
			_studentAnswer = value;
		}
	}

	public ArrayList QuestionLOs
	{
		get
		{
			return _questionLOs;
		}
		set
		{
			_questionLOs = value;
		}
	}

	public MatchQuestion()
	{
		_questionLOs = new ArrayList();
	}

	public override string ToString()
	{
		return _mid.ToString();
	}

	public void Preapare2Submit()
	{
		Solution = null;
		ColumnA = null;
		ColumnB = null;
		CourseId = null;
	}
}
