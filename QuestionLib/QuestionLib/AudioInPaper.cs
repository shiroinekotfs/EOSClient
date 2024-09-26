using System;

namespace QuestionLib;

[Serializable]
public class AudioInPaper : IComparable<AudioInPaper>
{
	private int _audioSize;

	private byte[] _audioData;

	private int _audioLength;

	private byte _repeatTime;

	private int _paddingTime;

	private byte _playOrder;

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

	public int CompareTo(AudioInPaper aip)
	{
		return PlayOrder.CompareTo(aip.PlayOrder);
	}
}
