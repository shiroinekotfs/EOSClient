using System;
using System.Collections;
using NHibernate;
using QuestionLib.Entity;

namespace QuestionLib.Business;

public class BOQuestionLO : BOBase
{
	public BOQuestionLO(ISessionFactory sessionFactory)
		: base(sessionFactory)
	{
	}

	public IList LoadLO(QuestionType qType, int qid)
	{
		IList list = null;
		session = sessionFactory.OpenSession();
		try
		{
			string text = null;
			text = "from QuestionLO qlo Where qlo.QType=:qType and qlo.QID=:qid";
			IQuery val = session.CreateQuery(text);
			val.SetParameter("qType", (object)qType);
			val.SetParameter("qid", (object)qid);
			list = val.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		ArrayList arrayList = new ArrayList();
		BOLO bOLO = new BOLO(NHHelper.GetSessionFactory());
		foreach (QuestionLO item in list)
		{
			LO value = bOLO.Load(item.LOID);
			arrayList.Add(value);
		}
		return arrayList;
	}

	public void DeleteQuestionLO(int qid, QuestionType qType)
	{
		session = sessionFactory.OpenSession();
		ITransaction val = session.BeginTransaction();
		try
		{
			string text = "from QuestionLO qlo Where qlo.QType=" + (int)qType + " and qlo.QID=" + qid;
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
