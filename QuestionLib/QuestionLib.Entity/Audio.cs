using System;

namespace QuestionLib.Entity;

[Serializable]
public class Audio
{
	private int _auID;

	private int _chID;

	private string _audioFile;

	private int _audioSize;

	private byte[] _audioData;

	private int _audioLength;

	private byte _repeatTime;

	private int _paddingTime;

	private byte _playOrder;

	public int AuID
	{
		get
		{
			return _auID;
		}
		set
		{
			_auID = value;
		}
	}

	public int ChID
	{
		get
		{
			return _chID;
		}
		set
		{
			_chID = value;
		}
	}

	public string AudioFile
	{
		get
		{
			return _audioFile;
		}
		set
		{
			_audioFile = value;
		}
	}

	public int AudioSize
	{
		get
		{
			return _audioSize;
		}
		set
		{
			_audioSize = value;
		}
	}

	public byte[] AudioData
	{
		get
		{
			return _audioData;
		}
		set
		{
			_audioData = value;
		}
	}

	public int AudioLength
	{
		get
		{
			return _audioLength;
		}
		set
		{
			_audioLength = value;
		}
	}

	public byte RepeatTime
	{
		get
		{
			return _repeatTime;
		}
		set
		{
			_repeatTime = value;
		}
	}

	public int PaddingTime
	{
		get
		{
			return _paddingTime;
		}
		set
		{
			_paddingTime = value;
		}
	}

	public byte PlayOrder
	{
		get
		{
			return _playOrder;
		}
		set
		{
			_playOrder = value;
		}
	}

	public Audio()
	{
		_audioData = null;
	}
}
