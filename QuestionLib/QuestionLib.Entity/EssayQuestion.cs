using System;

namespace QuestionLib.Entity;

[Serializable]
public class EssayQuestion
{
	private int _QBID;

	public int EQID { get; set; }

	public string CourseId { get; set; }

	public int ChapterId { get; set; }

	public byte[] Question { get; set; }

	public int QuestionFileSize { get; set; }

	public string Name { get; set; }

	public byte[] Guide2Mark { get; set; }

	public int Guide2MarkFileSize { get; set; }

	public string Development { get; set; }

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

	public void Preapare2Submit()
	{
		CourseId = null;
		Question = null;
		Name = null;
		Guide2Mark = null;
	}
}
