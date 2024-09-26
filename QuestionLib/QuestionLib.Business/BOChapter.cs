using System;
using System.Collections;
using NHibernate;
using QuestionLib.Entity;

namespace QuestionLib.Business;

public class BOChapter : BOBase
{
	public BOChapter(ISessionFactory sessionFactory)
		: base(sessionFactory)
	{
	}

	public IList LoadFillBlankQuestionByChapter(int chapterId)
	{
		QuestionType questionType = QuestionType.FILL_BLANK_ALL;
		QuestionType questionType2 = QuestionType.FILL_BLANK_GROUP;
		QuestionType questionType3 = QuestionType.FILL_BLANK_EMPTY;
		session = sessionFactory.OpenSession();
		IList result;
		try
		{
			string text = null;
			text = "from Question q Where (q.QType=:type1 OR q.QType=:type2 OR q.QType=:type3)  AND ChapterId=:chapterId";
			IQuery val = session.CreateQuery(text);
			val.SetParameter("type1", (object)questionType);
			val.SetParameter("type2", (object)questionType2);
			val.SetParameter("type3", (object)questionType3);
			val.SetParameter("chapterId", (object)chapterId.ToString());
			result = val.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		return result;
	}

	public IList LoadQuestionByChapter(QuestionType qt, int chapterId)
	{
		session = sessionFactory.OpenSession();
		IList result;
		try
		{
			string text = null;
			text = "from Question q Where q.QType=:type and ChapterId=:chapterId";
			IQuery val = session.CreateQuery(text);
			val.SetParameter("type", (object)qt);
			val.SetParameter("chapterId", (object)chapterId.ToString());
			result = val.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		return result;
	}

	public IList LoadPassageByChapter(int chapterId)
	{
		session = sessionFactory.OpenSession();
		IList result;
		try
		{
			string text = null;
			text = "from Passage p Where ChapterId=:chapterId";
			IQuery val = session.CreateQuery(text);
			val.SetParameter("chapterId", (object)chapterId);
			result = val.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		return result;
	}

	public IList LoadMatchQuestionByChapter(int chapterId)
	{
		session = sessionFactory.OpenSession();
		IList result;
		try
		{
			string text = null;
			text = "from MatchQuestion m Where ChapterId=:chapterId";
			IQuery val = session.CreateQuery(text);
			val.SetParameter("chapterId", (object)chapterId);
			result = val.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		return result;
	}
}
