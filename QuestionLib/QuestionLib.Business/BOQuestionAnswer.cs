using System;
using System.Collections;
using NHibernate;

namespace QuestionLib.Business;

public class BOQuestionAnswer : BOBase
{
	public BOQuestionAnswer(ISessionFactory sessionFactory)
		: base(sessionFactory)
	{
	}

	public IList LoadAnswer(int qid)
	{
		session = sessionFactory.OpenSession();
		IList result;
		try
		{
			string text = null;
			text = "from QuestionAnswer qa Where qa.QID=:qid";
			IQuery val = session.CreateQuery(text);
			val.SetParameter("qid", (object)qid);
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
