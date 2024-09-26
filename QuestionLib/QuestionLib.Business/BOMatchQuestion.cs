using System;
using System.Collections;
using System.Data.SqlClient;
using NHibernate;
using QuestionLib.Entity;

namespace QuestionLib.Business;

public class BOMatchQuestion : BOBase
{
	public BOMatchQuestion(ISessionFactory sessionFactory)
		: base(sessionFactory)
	{
	}

	public MatchQuestion LoadMatch(int mid)
	{
		session = sessionFactory.OpenSession();
		IList list;
		try
		{
			string text = null;
			text = "from MatchQuestion mq Where  mq.MID=:mid";
			IQuery val = session.CreateQuery(text);
			val.SetParameter("mid", (object)mid);
			list = val.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		return (list.Count > 0) ? ((MatchQuestion)list[0]) : null;
	}

	public IList LoadMatchOfCourse(string courseId)
	{
		session = sessionFactory.OpenSession();
		IList result;
		try
		{
			string text = null;
			text = "from MatchQuestion mq Where  mq.CourseId=:courseId";
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

	public bool SaveList(IList list)
	{
		ISession val = sessionFactory.OpenSession();
		ITransaction val2 = val.BeginTransaction();
		try
		{
			foreach (MatchQuestion item in list)
			{
				val.Save((object)item);
				item.QuestionLOs = BOLO.RemoveDupLO(item.QuestionLOs);
				foreach (QuestionLO questionLO in item.QuestionLOs)
				{
					questionLO.QID = item.MID;
					questionLO.QType = QuestionType.MATCH;
					val.Save((object)questionLO);
				}
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

	public bool Delete(int chapterID, string conStr)
	{
		int num = 3;
		SqlConnection sqlConnection = new SqlConnection(conStr);
		sqlConnection.Open();
		SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
		string cmdText = "DELETE FROM QuestionLO WHERE QType = " + num + " AND qid IN (SELECT mid FROM MatchQuestion WHERE chapterID=" + chapterID + ")";
		string cmdText2 = "DELETE FROM MatchQuestion WHERE chapterID=" + chapterID;
		SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
		sqlCommand.Transaction = sqlTransaction;
		SqlCommand sqlCommand2 = new SqlCommand(cmdText2, sqlConnection);
		sqlCommand2.Transaction = sqlTransaction;
		try
		{
			sqlCommand.ExecuteNonQuery();
			sqlCommand2.ExecuteNonQuery();
			sqlTransaction.Commit();
			sqlConnection.Close();
			return true;
		}
		catch (Exception ex)
		{
			sqlTransaction.Rollback();
			sqlConnection.Close();
			throw ex;
		}
	}

	public void Delete(MatchQuestion m)
	{
		int mID = m.MID;
		session = sessionFactory.OpenSession();
		ITransaction val = session.BeginTransaction();
		try
		{
			string text = "from MatchQuestion mq Where mq.MID=" + mID;
			session.Delete(text);
			int num = 3;
			text = "from QuestionLO qlo Where qlo.QType=" + num + " and qlo.QID=" + mID;
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
