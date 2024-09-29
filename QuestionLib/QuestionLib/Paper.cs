using System;
using System.Collections;
using System.Collections.Generic;
using QuestionLib.Entity;

namespace QuestionLib;

[Serializable]
public class Paper
{
	private TestTypeEnum _testType;

	private string _examCode;

	private int _duration;

	private float _mark;

	private int _noOfQuestion;

	private ArrayList _reading;

	private ArrayList _grammar;

	private ArrayList _match;

	private ArrayList _indicateMistake;

	private ArrayList _fillBlank;

	private EssayQuestion _essay;

	private bool _isShuffleReading;

	private bool _isShuffleGrammer;

	private bool _isShuffleMatch;

	private bool _isShuffleIndicateMistake;

	private bool _isShuffleFillBlank;

	private string _studentGuide;

	private string _listenCode;

	private string _pwd;

	private List<AudioInPaper> _listAudio;

	private byte[] _oneSecSilence;

	private int _audioHeadPadding;

	private ImagePaper _imagePaper;

	public bool IsShuffleReading
	{
		get
		{
			return _isShuffleReading;
		}
		set
		{
			_isShuffleReading = value;
		}
	}

	public bool IsShuffleGrammer
	{
		get
		{
			return _isShuffleGrammer;
		}
		set
		{
			_isShuffleGrammer = value;
		}
	}

	public bool IsShuffleMatch
	{
		get
		{
			return _isShuffleMatch;
		}
		set
		{
			_isShuffleMatch = value;
		}
	}

	public QuestionDistribution QD { get; set; }

	public bool IsShuffleIndicateMistake
	{
		get
		{
			return _isShuffleIndicateMistake;
		}
		set
		{
			_isShuffleIndicateMistake = value;
		}
	}

	public bool IsShuffleFillBlank
	{
		get
		{
			return _isShuffleFillBlank;
		}
		set
		{
			_isShuffleFillBlank = value;
		}
	}

	public int Duration
	{
		get
		{
			return _duration;
		}
		set
		{
			_duration = value;
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

	public int NoOfQuestion
	{
		get
		{
			return _noOfQuestion;
		}
		set
		{
			_noOfQuestion = value;
		}
	}

	public ArrayList ReadingQuestions
	{
		get
		{
			return _reading;
		}
		set
		{
			_reading = value;
		}
	}

	public ArrayList GrammarQuestions
	{
		get
		{
			return _grammar;
		}
		set
		{
			_grammar = value;
		}
	}

	public ArrayList MatchQuestions
	{
		get
		{
			return _match;
		}
		set
		{
			_match = value;
		}
	}

	public ArrayList IndicateMQuestions
	{
		get
		{
			return _indicateMistake;
		}
		set
		{
			_indicateMistake = value;
		}
	}

	public ArrayList FillBlankQuestions
	{
		get
		{
			return _fillBlank;
		}
		set
		{
			_fillBlank = value;
		}
	}

	public EssayQuestion EssayQuestion
	{
		get
		{
			return _essay;
		}
		set
		{
			_essay = value;
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

	public string ListenCode
	{
		get
		{
			return _listenCode;
		}
		set
		{
			_listenCode = value;
		}
	}

	public string Password
	{
		get
		{
			return _pwd;
		}
		set
		{
			_pwd = value;
		}
	}

	public TestTypeEnum TestType
	{
		get
		{
			return _testType;
		}
		set
		{
			_testType = value;
		}
	}

	public ImagePaper ImgPaper
	{
		get
		{
			return _imagePaper;
		}
		set
		{
			_imagePaper = value;
		}
	}

	public List<AudioInPaper> ListAudio
	{
		get
		{
			return _listAudio;
		}
		set
		{
			_listAudio = value;
		}
	}

	public byte[] OneSecSilence
	{
		get
		{
			return _oneSecSilence;
		}
		set
		{
			_oneSecSilence = value;
		}
	}

	public int AudioHeadPadding
	{
		get
		{
			return _audioHeadPadding;
		}
		set
		{
			_audioHeadPadding = value;
		}
	}

	public Paper()
	{
		_reading = new ArrayList();
		_grammar = new ArrayList();
		_match = new ArrayList();
		_indicateMistake = new ArrayList();
		_fillBlank = new ArrayList();
		TestType = TestTypeEnum.NOT_WRITING;
	}
}
