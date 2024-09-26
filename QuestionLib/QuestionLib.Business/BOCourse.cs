using System;
using System.Collections;
using NHibernate;

namespace QuestionLib.Business;

public class BOCourse : BOBase
{
	public BOCourse(ISessionFactory sessionFactory)
		: base(sessionFactory)
	{
	}

	public IList LoadChapterByCourse(string courseId)
	{
		session = sessionFactory.OpenSession();
		IList result;
		try
		{
			string text = null;
			text = "from Chapter ch Where CID=:courseId";
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

	public bool IsCourseExists(string courseId)
	{
		bool result = false;
		session = sessionFactory.OpenSession();
		try
		{
			string text = null;
			text = "from Course c Where CID=:courseId";
			IQuery val = session.CreateQuery(text);
			val.SetParameter("courseId", (object)courseId);
			IList list = val.List();
			session.Close();
			if (list.Count > 0)
			{
				result = true;
			}
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		return result;
	}
}
