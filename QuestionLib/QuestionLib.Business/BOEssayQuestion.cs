using System;
using System.Collections;
using NHibernate;
using QuestionLib.Entity;

namespace QuestionLib.Business;

public class BOEssayQuestion : BOBase
{
	public BOEssayQuestion(ISessionFactory sessionFactory)
		: base(sessionFactory)
	{
	}

	public EssayQuestion Load(int eqid)
	{
		IList list = null;
		session = sessionFactory.OpenSession();
		try
		{
			string text = null;
			text = "from EssayQuestion q Where q.EQID=:eqid";
			IQuery val = session.CreateQuery(text);
			val.SetParameter("eqid", (object)eqid);
			list = val.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		return (list.Count > 0) ? ((EssayQuestion)list[0]) : null;
	}

	public IList LoadByCourse(string courseId)
	{
		IList list = null;
		session = sessionFactory.OpenSession();
		try
		{
			string text = null;
			text = "from EssayQuestion q Where CourseId=:courseId";
			IQuery val = session.CreateQuery(text);
			val.SetParameter("courseId", (object)courseId);
			list = val.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		return list;
	}

	public IList LoadByChapter(int chapterId)
	{
		IList list = null;
		session = sessionFactory.OpenSession();
		try
		{
			string text = null;
			text = "from EssayQuestion q Where ChapterId=:chapterId";
			IQuery val = session.CreateQuery(text);
			val.SetParameter("chapterId", (object)chapterId);
			list = val.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		return list;
	}

	public void Delete(int eqid)
	{
		session = sessionFactory.OpenSession();
		ITransaction val = session.BeginTransaction();
		try
		{
			string text = "from EssayQuestion q Where q.EQID=" + eqid;
			session.Delete(text);
			val.Commit();
			session.Close();
		}
		catch (Exception ex)
		{
			val.Rollback();
			session.Close();
			throw ex;
		}
	}

	public bool SaveList(IList list)
	{
		ISession val = sessionFactory.OpenSession();
		ITransaction val2 = val.BeginTransaction();
		try
		{
			foreach (EssayQuestion item in list)
			{
				val.Save((object)item);
			}
			val2.Commit();
			return true;
		}
		catch
		{
			val2.Rollback();
			return false;
		}
	}

	public void DeleteQuestionInChapter(int chapterId)
	{
		session = sessionFactory.OpenSession();
		ITransaction val = session.BeginTransaction();
		try
		{
			string text = "from EssayQuestion q Where q.ChapterId=" + chapterId;
			session.Delete(text);
			val.Commit();
			session.Close();
		}
		catch (Exception ex)
		{
			val.Rollback();
			session.Close();
			throw ex;
		}
	}
}
