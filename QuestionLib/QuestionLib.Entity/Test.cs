namespace QuestionLib.Entity;

public class Test
{
	private string _testId;

	private string _courseId;

	private string _questions;

	private int _numOfQuestion;

	private float _mark;

	private string _studentGuide;

	public string TestId
	{
		get
		{
			return _testId;
		}
		set
		{
			_testId = value;
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

	public string Questions
	{
		get
		{
			return _questions;
		}
		set
		{
			_questions = value;
		}
	}

	public int NumOfQuestion
	{
		get
		{
			return _numOfQuestion;
		}
		set
		{
			_numOfQuestion = value;
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

	public string StudentGuide
	{
		get
		{
			return _studentGuide;
		}
		set
		{
			_studentGuide = value;
		}
	}
}
