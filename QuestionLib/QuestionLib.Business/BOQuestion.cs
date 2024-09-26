using System;
using System.Collections;
using System.Data.SqlClient;
using NHibernate;
using QuestionLib.Entity;

namespace QuestionLib.Business;

public class BOQuestion : BOBase
{
	public BOQuestion(ISessionFactory sessionFactory)
		: base(sessionFactory)
	{
	}

	public IList LoadPassageQuestion(int pid)
	{
		IList list = null;
		session = sessionFactory.OpenSession();
		try
		{
			string text = null;
			text = "from Question q Where q.PID=:pid";
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
		return list;
	}

	public Question Load(int qid)
	{
		IList list = null;
		session = sessionFactory.OpenSession();
		try
		{
			string text = null;
			text = "from Question q Where q.QID=:qid";
			IQuery val = session.CreateQuery(text);
			val.SetParameter("qid", (object)qid);
			list = val.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		return (list.Count > 0) ? ((Question)list[0]) : null;
	}

	public IList LoadByType(QuestionType type)
	{
		IList list = null;
		session = sessionFactory.OpenSession();
		try
		{
			string text = null;
			text = "from Question q Where q.QType=:type";
			IQuery val = session.CreateQuery(text);
			val.SetParameter("type", (object)type);
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

	public IList LoadByTypeOfCourse(QuestionType type, string courseId)
	{
		IList list = null;
		session = sessionFactory.OpenSession();
		try
		{
			string text = null;
			text = "from Question q Where q.QType=:type and CourseId=:courseId";
			IQuery val = session.CreateQuery(text);
			val.SetParameter("type", (object)type);
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

	public IList LoadFillBlankByTypeOfCourse(string courseId)
	{
		IList list = null;
		QuestionType questionType = QuestionType.FILL_BLANK_ALL;
		QuestionType questionType2 = QuestionType.FILL_BLANK_EMPTY;
		QuestionType questionType3 = QuestionType.FILL_BLANK_GROUP;
		session = sessionFactory.OpenSession();
		try
		{
			string text = null;
			text = "from Question q Where (q.QType=:type1 OR q.QType=:type2 OR q.QType=:type3) and CourseId=:courseId";
			IQuery val = session.CreateQuery(text);
			val.SetParameter("type1", (object)questionType);
			val.SetParameter("type2", (object)questionType2);
			val.SetParameter("type3", (object)questionType3);
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

	public void Delete(int qid, QuestionType qt)
	{
		session = sessionFactory.OpenSession();
		ITransaction val = session.BeginTransaction();
		try
		{
			string text = "from Question q Where q.QID=" + qid;
			session.Delete(text);
			text = "from QuestionAnswer qa Where qa.QID=" + qid;
			session.Delete(text);
			text = "from QuestionLO qlo Where qlo.QType=" + (int)qt + " and qlo.QID=" + qid;
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
			foreach (Question item in list)
			{
				val.Save((object)item);
				foreach (QuestionAnswer questionAnswer in item.QuestionAnswers)
				{
					questionAnswer.QID = item.QID;
					val.Save((object)questionAnswer);
				}
				item.QuestionLOs = BOLO.RemoveDupLO(item.QuestionLOs);
				foreach (QuestionLO questionLO in item.QuestionLOs)
				{
					questionLO.QID = item.QID;
					questionLO.QType = item.QType;
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

	public bool Delete(int chapterID, QuestionType qt, string conStr)
	{
		SqlConnection sqlConnection = new SqlConnection(conStr);
		sqlConnection.Open();
		SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
		string cmdText = "DELETE FROM QuestionAnswer WHERE qid in (SELECT qid FROM Question WHERE QType=" + (int)qt + " AND chapterId=" + chapterID + ")";
		string cmdText2 = "DELETE FROM QuestionLO WHERE qid in (SELECT qid FROM Question WHERE QType=" + (int)qt + " AND chapterId=" + chapterID + ")";
		string cmdText3 = "DELETE FROM Question WHERE QType=" + (int)qt + " AND chapterID=" + chapterID;
		SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
		sqlCommand.Transaction = sqlTransaction;
		SqlCommand sqlCommand2 = new SqlCommand(cmdText2, sqlConnection);
		sqlCommand2.Transaction = sqlTransaction;
		SqlCommand sqlCommand3 = new SqlCommand(cmdText3, sqlConnection);
		sqlCommand3.Transaction = sqlTransaction;
		try
		{
			sqlCommand.ExecuteNonQuery();
			sqlCommand2.ExecuteNonQuery();
			sqlCommand3.ExecuteNonQuery();
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
}
