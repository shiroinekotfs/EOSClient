using System;
using System.Collections;
using System.Data.SqlClient;
using NHibernate;
using QuestionLib.Entity;

namespace QuestionLib.Business;

public class BOPassage : BOBase
{
	public BOPassage(ISessionFactory sessionFactory)
		: base(sessionFactory)
	{
	}

	public Passage LoadPassage(int pid)
	{
		session = sessionFactory.OpenSession();
		IList list;
		try
		{
			string text = null;
			text = "from Passage p Where p.PID=:pid";
			IQuery val = session.CreateQuery(text);
			val.SetParameter("pid", (object)pid);
			list = val.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		return (list.Count > 0) ? ((Passage)list[0]) : null;
	}

	public IList LoadPassageByCourse(string courseId)
	{
		session = sessionFactory.OpenSession();
		IList result;
		try
		{
			string text = null;
			text = "from Passage p Where CourseId=:courseId";
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

	public void Delete(int pid, int[] qid_list)
	{
		session = sessionFactory.OpenSession();
		ITransaction val = session.BeginTransaction();
		try
		{
			string text = "from Passage p Where p.PID=" + pid;
			session.Delete(text);
			text = "from Question q Where q.PID=" + pid;
			session.Delete(text);
			for (int i = 0; i < qid_list.Length; i++)
			{
				int num = qid_list[i];
				text = "from QuestionAnswer qa Where qa.QID=" + num;
				session.Delete(text);
				int num2 = 0;
				text = "from QuestionLO qLO Where Qtype=" + num2 + " and qLO.QID=" + num.ToString();
				session.Delete(text);
			}
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

	public bool Delete(int chapterID, string conStr)
	{
		int num = 0;
		SqlConnection sqlConnection = new SqlConnection(conStr);
		sqlConnection.Open();
		SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
		string cmdText = "DELETE FROM QuestionLO WHERE QType = " + num + " AND qid IN (SELECT qid FROM Question WHERE pid IN (SELECT pid FROM Passage WHERE chapterID=" + chapterID + "))";
		string cmdText2 = "DELETE FROM QuestionAnswer WHERE qid IN (SELECT qid FROM Question WHERE pid IN (SELECT pid FROM Passage WHERE chapterID=" + chapterID + "))";
		string cmdText3 = "DELETE FROM Question WHERE pid IN (SELECT pid FROM Passage WHERE chapterID=" + chapterID + ")";
		string cmdText4 = "DELETE FROM Passage WHERE chapterID=" + chapterID;
		SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
		sqlCommand.Transaction = sqlTransaction;
		SqlCommand sqlCommand2 = new SqlCommand(cmdText2, sqlConnection);
		sqlCommand2.Transaction = sqlTransaction;
		SqlCommand sqlCommand3 = new SqlCommand(cmdText3, sqlConnection);
		sqlCommand3.Transaction = sqlTransaction;
		SqlCommand sqlCommand4 = new SqlCommand(cmdText4, sqlConnection);
		sqlCommand4.Transaction = sqlTransaction;
		try
		{
			sqlCommand.ExecuteNonQuery();
			sqlCommand2.ExecuteNonQuery();
			sqlCommand3.ExecuteNonQuery();
			sqlCommand4.ExecuteNonQuery();
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

	public bool SaveList(IList list)
	{
		ISession val = sessionFactory.OpenSession();
		ITransaction val2 = val.BeginTransaction();
		try
		{
			foreach (Passage item in list)
			{
				val.Save((object)item);
				foreach (Question passageQuestion in item.PassageQuestions)
				{
					passageQuestion.PID = item.PID;
					val.Save((object)passageQuestion);
					foreach (QuestionAnswer questionAnswer in passageQuestion.QuestionAnswers)
					{
						questionAnswer.QID = passageQuestion.QID;
						val.Save((object)questionAnswer);
					}
					passageQuestion.QuestionLOs = BOLO.RemoveDupLO(passageQuestion.QuestionLOs);
					foreach (QuestionLO questionLO in passageQuestion.QuestionLOs)
					{
						questionLO.QID = passageQuestion.QID;
						questionLO.QType = passageQuestion.QType;
						val.Save((object)questionLO);
					}
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
}
