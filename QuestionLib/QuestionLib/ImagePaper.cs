using System;
using System.Collections.Generic;

namespace QuestionLib;

[Serializable]
public class ImagePaper
{
	public List<PaperSection> Sections { get; set; }

	public byte[] Image { get; set; }

	public int NumberOfPage { get; set; }

	public string LongAnswerGuide { get; set; }

	public void Preapare2Submit()
	{
		Image = null;
	}
}
