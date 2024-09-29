using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using QuestionLib.Entity;

namespace QuestionLib;

public class QuestionHelper
{
	public static char[] lo_deli = new char[1] { ';' };

	public static string[] MultipleChoiceQuestionType = new string[2] { "Grammar", "Indicate Mistake" };

	public static string fillBlank_Pattern = "\\([0-9a-zA-Z;:=?<>/`,'’ .+_~!@#$%^&*\\r\\n-]+\\)";

	private static char[] splitChars = new char[3] { ' ', '-', '\t' };

	public static int LineWidth = 100;

	public static void SaveSubmitPaper(string folder, SubmitPaper submitPaper)
	{
		submitPaper.SubmitTime = DateTime.Now;
		string path = folder + submitPaper.LoginId + ".dat";
		FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		binaryFormatter.Serialize(fileStream, submitPaper);
		fileStream.Flush();
		fileStream.Close();
	}

	public static SubmitPaper LoadSubmitPaper(string savedFile)
	{
		try
		{
			FileStream fileStream = new FileStream(savedFile, FileMode.Open, FileAccess.Read);
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			SubmitPaper result = (SubmitPaper)binaryFormatter.Deserialize(fileStream);
			fileStream.Close();
			return result;
		}
		catch (Exception)
		{
			return null;
		}
	}

	private static Passage GetPassage(Paper oPaper, int pid)
	{
		foreach (Passage readingQuestion in oPaper.ReadingQuestions)
		{
			if (readingQuestion.PID == pid)
			{
				return readingQuestion;
			}
		}
		return null;
	}

	private static bool ReConstructQuestion(Question sq, Question oq)
	{
		if (sq.QID == oq.QID)
		{
			sq.QBID = oq.QBID;
			sq.CourseId = oq.CourseId;
			sq.Text = oq.Text;
			sq.Mark = oq.Mark;
			sq.ImageData = oq.ImageData;
			sq.ImageSize = oq.ImageSize;
			bool flag = false;
			if (sq.QType == QuestionType.FILL_BLANK_ALL)
			{
				flag = true;
			}
			if (sq.QType == QuestionType.FILL_BLANK_GROUP)
			{
				flag = true;
			}
			if (sq.QType == QuestionType.FILL_BLANK_EMPTY)
			{
				flag = true;
			}
			foreach (QuestionAnswer questionAnswer3 in sq.QuestionAnswers)
			{
				foreach (QuestionAnswer questionAnswer4 in oq.QuestionAnswers)
				{
					if (questionAnswer3.QAID != questionAnswer4.QAID)
					{
						continue;
					}
					if (flag)
					{
						string text = RemoveSpaces(questionAnswer3.Text).Trim().ToLower();
						string value = RemoveSpaces(questionAnswer4.Text).Trim().ToLower();
						if (text.Equals(value))
						{
							questionAnswer3.Chosen = true;
							questionAnswer3.Selected = true;
						}
					}
					else
					{
						questionAnswer3.Text = questionAnswer4.Text;
						questionAnswer3.Chosen = questionAnswer4.Chosen;
					}
					break;
				}
			}
			return true;
		}
		return false;
	}

	private static void ReConstructEssay(EssayQuestion sEssay, EssayQuestion oEssay)
	{
		if (sEssay.EQID == oEssay.EQID)
		{
			sEssay.QBID = oEssay.QBID;
			sEssay.CourseId = oEssay.CourseId;
			sEssay.Question = oEssay.Question;
		}
	}

	private static void ReConstructImagePaper(ImagePaper sIP, ImagePaper oIP)
	{
		sIP.Image = oIP.Image;
	}

	public static Paper Re_ConstructPaper(Paper oPaper, SubmitPaper submitPaper)
	{
		Paper sPaper = submitPaper.SPaper;
		foreach (Passage readingQuestion in sPaper.ReadingQuestions)
		{
			Passage passage2 = GetPassage(oPaper, readingQuestion.PID);
			readingQuestion.QBID = passage2.QBID;
			readingQuestion.Text = passage2.Text;
			readingQuestion.CourseId = passage2.CourseId;
			foreach (Question passageQuestion in readingQuestion.PassageQuestions)
			{
				foreach (Question passageQuestion2 in passage2.PassageQuestions)
				{
					if (ReConstructQuestion(passageQuestion, passageQuestion2))
					{
						break;
					}
				}
			}
		}
		foreach (MatchQuestion matchQuestion3 in sPaper.MatchQuestions)
		{
			foreach (MatchQuestion matchQuestion4 in oPaper.MatchQuestions)
			{
				if (matchQuestion3.MID == matchQuestion4.MID)
				{
					matchQuestion3.QBID = matchQuestion4.QBID;
					matchQuestion3.CourseId = matchQuestion4.CourseId;
					matchQuestion3.ColumnA = matchQuestion4.ColumnA;
					matchQuestion3.ColumnB = matchQuestion4.ColumnB;
					matchQuestion3.Solution = matchQuestion4.Solution;
					matchQuestion3.Mark = matchQuestion4.Mark;
					break;
				}
			}
		}
		foreach (Question grammarQuestion in sPaper.GrammarQuestions)
		{
			foreach (Question grammarQuestion2 in oPaper.GrammarQuestions)
			{
				if (ReConstructQuestion(grammarQuestion, grammarQuestion2))
				{
					break;
				}
			}
		}
		foreach (Question indicateMQuestion in sPaper.IndicateMQuestions)
		{
			foreach (Question indicateMQuestion2 in oPaper.IndicateMQuestions)
			{
				if (ReConstructQuestion(indicateMQuestion, indicateMQuestion2))
				{
					break;
				}
			}
		}
		foreach (Question fillBlankQuestion in sPaper.FillBlankQuestions)
		{
			foreach (Question fillBlankQuestion2 in oPaper.FillBlankQuestions)
			{
				if (ReConstructQuestion(fillBlankQuestion, fillBlankQuestion2))
				{
					break;
				}
			}
		}
		if (oPaper.EssayQuestion != null)
		{
			ReConstructEssay(sPaper.EssayQuestion, oPaper.EssayQuestion);
		}
		if (oPaper.ImgPaper != null)
		{
			ReConstructImagePaper(sPaper.ImgPaper, oPaper.ImgPaper);
		}
		sPaper.OneSecSilence = oPaper.OneSecSilence;
		sPaper.ListAudio = oPaper.ListAudio;
		return sPaper;
	}

	public static string RemoveSpaces(string s)
	{
		s = s.Trim();
		string text;
		do
		{
			text = s;
			s = s.Replace("  ", " ");
		}
		while (s.Length != text.Length);
		return s;
	}

	public static string RemoveAllSpaces(string s)
	{
		s = s.Trim();
		string text;
		do
		{
			text = s;
			s = s.Replace(" ", "");
		}
		while (s.Length != text.Length);
		return s;
	}

	public static bool IsFillBlank(QuestionType qt)
	{
		return qt switch
		{
			QuestionType.FILL_BLANK_ALL => true, 
			QuestionType.FILL_BLANK_GROUP => true, 
			QuestionType.FILL_BLANK_EMPTY => true, 
			_ => false, 
		};
	}

	private static string RemoveNewLine(string s)
	{
		s = s.Replace(Environment.NewLine, "");
		s = RemoveSpaces(s);
		return s;
	}

	public static string WordWrap(string str, int width)
	{
		string pattern = fillBlank_Pattern;
		Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
		MatchCollection matchCollection = regex.Matches(str);
		str = regex.Replace(str, "(###)");
		string[] array = SplitLines(str);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			string str2 = array[i];
			if (i < array.Length - 1)
			{
				str2 = array[i] + Environment.NewLine;
			}
			ArrayList arrayList = Explode(str2, splitChars);
			int num = 0;
			for (int j = 0; j < arrayList.Count; j++)
			{
				string text = (string)arrayList[j];
				if (num + text.Length > width)
				{
					if (num > 0)
					{
						if (!stringBuilder.ToString().EndsWith(Environment.NewLine))
						{
							stringBuilder.Append(Environment.NewLine);
						}
						num = 0;
					}
					while (text.Length > width)
					{
						stringBuilder.Append(text.Substring(0, width - 1) + "-");
						text = text.Substring(width - 1);
						if (!stringBuilder.ToString().EndsWith(Environment.NewLine))
						{
							stringBuilder.Append(Environment.NewLine);
						}
						stringBuilder.Append(Environment.NewLine);
					}
					text = text.TrimStart();
				}
				stringBuilder.Append(text);
				num += text.Length;
			}
		}
		str = stringBuilder.ToString();
		pattern = "\\(###\\)";
		regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
		for (int k = 0; k < matchCollection.Count; k++)
		{
			string replacement = RemoveNewLine(matchCollection[k].Value);
			str = regex.Replace(str, replacement, 1);
		}
		return str;
	}

	private static ArrayList Explode(string str, char[] splitChars)
	{
		ArrayList arrayList = new ArrayList();
		int num = 0;
		while (true)
		{
			int num2 = str.IndexOfAny(splitChars, num);
			if (num2 == -1)
			{
				break;
			}
			string text = str.Substring(num, num2 - num);
			char c = str.Substring(num2, 1)[0];
			if (char.IsWhiteSpace(c))
			{
				arrayList.Add(text);
				arrayList.Add(c.ToString());
			}
			else
			{
				arrayList.Add(text + c);
			}
			num = num2 + 1;
		}
		arrayList.Add(str.Substring(num));
		return arrayList;
	}

	private static string[] SplitLines(string str)
	{
		string newLine = Environment.NewLine;
		Regex regex = new Regex(newLine, RegexOptions.IgnoreCase | RegexOptions.Compiled);
		return regex.Split(str);
	}

	public static string[] GetFillBlankWord(string text)
	{
		string pattern = fillBlank_Pattern;
		Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
		MatchCollection matchCollection = regex.Matches(text);
		string[] array = new string[matchCollection.Count];
		for (int i = 0; i < matchCollection.Count; i++)
		{
			string text2 = matchCollection[i].Value.Remove(0, 1);
			text2 = text2.Remove(text2.Length - 1, 1);
			array[i] = text2;
		}
		return array;
	}

	public static string Sec2TimeString(int sec)
	{
		int num = sec / 3600;
		int num2 = sec % 3600 / 60;
		int num3 = sec % 60;
		string text = "0" + num;
		text = text.Substring(text.Length - 2, 2);
		string text2 = "0" + num2;
		text2 = text2.Substring(text2.Length - 2, 2);
		string text3 = "0" + num3;
		text3 = text3.Substring(text3.Length - 2, 2);
		return text + ":" + text2 + ":" + text3;
	}
}
