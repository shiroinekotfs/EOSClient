using System;
using System.Collections;
using NHibernate;
using QuestionLib.Entity;

namespace QuestionLib.Business;

public class BOTest : BOBase
{
	public BOTest(ISessionFactory sessionFactory)
		: base(sessionFactory)
	{
	}

	public IList LoadTest(string courseId)
	{
		session = sessionFactory.OpenSession();
		IList result;
		try
		{
			string text = null;
			text = "from Test t Where CourseId=:courseId";
			IQuery val = session.CreateQuery(text);
			val.SetParameter("courseId", (object)courseId);
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

	public Test LoadTestByTestId(string testId)
	{
		session = sessionFactory.OpenSession();
		IList list;
		try
		{
			string text = null;
			text = "from Test t Where TestId=:testId";
			IQuery val = session.CreateQuery(text);
			val.SetParameter("testId", (object)testId);
			list = val.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		if (list.Count > 0)
		{
			return (Test)list[0];
		}
		return null;
	}

	public IList LoadTestByCourse(string courseId)
	{
		session = sessionFactory.OpenSession();
		IList result;
		try
		{
			string text = null;
			text = "from Test t Where CourseId=:courseId";
			IQuery val = session.CreateQuery(text);
			val.SetParameter("courseId", (object)courseId);
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

	public bool IsTestExists(string testId)
	{
		session = sessionFactory.OpenSession();
		IList list;
		try
		{
			string text = null;
			text = "from Test t Where TestId=:testId";
			IQuery val = session.CreateQuery(text);
			val.SetParameter("testId", (object)testId);
			list = val.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		if (list.Count == 0)
		{
			return false;
		}
		return true;
	}
}
