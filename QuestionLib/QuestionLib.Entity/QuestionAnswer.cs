using System;

namespace QuestionLib.Entity;

[Serializable]
public class QuestionAnswer
{
	private int _qaid;

	private int _qid;

	private string _text;

	private bool _chosen;

	private bool _selected;

	private bool _done;

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

	public int QAID
	{
		get
		{
			return _qaid;
		}
		set
		{
			_qaid = value;
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

	public bool Chosen
	{
		get
		{
			return _chosen;
		}
		set
		{
			_chosen = value;
		}
	}

	public bool Selected
	{
		get
		{
			return _selected;
		}
		set
		{
			_selected = value;
		}
	}

	public bool Done
	{
		get
		{
			return _done;
		}
		set
		{
			_done = value;
		}
	}

	public QuestionAnswer()
	{
	}

	public QuestionAnswer(string text, bool chosen)
	{
		_text = text;
		_chosen = chosen;
	}
}
