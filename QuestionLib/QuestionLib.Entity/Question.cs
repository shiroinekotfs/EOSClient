using System;
using System.Collections;
using NHibernate;
using QuestionLib.Business;

namespace QuestionLib.Entity;

[Serializable]
public class Question
{
	private int _qid;

	private string _courseId;

	private int _chapterId;

	private int _pid;

	private string _text;

	private float _mark;

	private ArrayList _questionAnswers;

	private QuestionType _qType;

	private bool _lock;

	private byte[] _imageData;

	private int _imageSize;

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

	public int QID
	{
		get
		{
			return _qid;
		}
		set
		{
			_qid = value;
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

	public int PID
	{
		get
		{
			return _pid;
		}
		set
		{
			_pid = value;
		}
	}

	public string Text
	{
		get
		{
			return _text;
		}
		set
		{
			_text = value;
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

	public ArrayList QuestionAnswers
	{
		get
		{
			return _questionAnswers;
		}
		set
		{
			_questionAnswers = value;
		}
	}

	public QuestionType QType
	{
		get
		{
			return _qType;
		}
		set
		{
			_qType = value;
		}
	}

	public bool Lock
	{
		get
		{
			return _lock;
		}
		set
		{
			_lock = value;
		}
	}

	public byte[] ImageData
	{
		get
		{
			return _imageData;
		}
		set
		{
			_imageData = value;
		}
	}

	public int ImageSize
	{
		get
		{
			return _imageSize;
		}
		set
		{
			_imageSize = value;
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

	public Question()
	{
		_questionAnswers = new ArrayList();
		_questionLOs = new ArrayList();
	}

	public override string ToString()
	{
		return _text;
	}

	public void LoadAnswers(ISessionFactory sessionFactory)
	{
		BOQuestionAnswer bOQuestionAnswer = new BOQuestionAnswer(sessionFactory);
		_questionAnswers = (ArrayList)bOQuestionAnswer.LoadAnswer(_qid);
	}

	public void Preapare2Submit()
	{
		Text = null;
		CourseId = null;
		ImageData = null;
		ImageSize = 0;
		if (QType == QuestionType.FILL_BLANK_ALL || QType == QuestionType.FILL_BLANK_GROUP || QType == QuestionType.FILL_BLANK_EMPTY)
		{
			return;
		}
		foreach (QuestionAnswer questionAnswer in QuestionAnswers)
		{
			questionAnswer.Text = null;
		}
	}
}
