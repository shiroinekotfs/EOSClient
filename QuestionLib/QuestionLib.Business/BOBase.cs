using System;
using System.Collections;
using System.Data.SqlClient;
using NHibernate;
using QuestionLib.Entity;

namespace QuestionLib.Business;

public class BOBase
{
	protected ISessionFactory sessionFactory;

	protected ISession session;

	public BOBase()
	{
	}

	public BOBase(ISessionFactory sessionFactory)
	{
		this.sessionFactory = sessionFactory;
	}

	public object SaveOrUpdate(object obj)
	{
		session = sessionFactory.OpenSession();
		ITransaction val = session.BeginTransaction();
		try
		{
			session.SaveOrUpdate(obj);
			session.Flush();
			val.Commit();
			session.Close();
			return obj;
		}
		catch (Exception ex)
		{
			val.Rollback();
			session.Close();
			throw ex;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public object Save(object obj)
	{
		session = sessionFactory.OpenSession();
		ITransaction val = session.BeginTransaction();
		try
		{
			session.Save(obj);
			session.Flush();
			val.Commit();
			session.Close();
			return obj;
		}
		catch (Exception ex)
		{
			val.Rollback();
			session.Close();
			throw ex;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public object Save(object obj, ISession mySession)
	{
		try
		{
			mySession.Save(obj);
			mySession.Flush();
			return obj;
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public object Update(object obj)
	{
		session = sessionFactory.OpenSession();
		ITransaction val = session.BeginTransaction();
		try
		{
			session.Update(obj);
			session.Flush();
			val.Commit();
			session.Close();
			return obj;
		}
		catch (Exception ex)
		{
			val.Rollback();
			session.Close();
			throw ex;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public object Update(object obj, ISession mySession)
	{
		try
		{
			mySession.Update(obj);
			mySession.Flush();
			return obj;
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public void Load(object obj, object id)
	{
		session = sessionFactory.OpenSession();
		try
		{
			session.Load(obj, id);
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
	}

	public void Delete(object obj)
	{
		session = sessionFactory.OpenSession();
		ITransaction val = session.BeginTransaction();
		try
		{
			session.Delete(obj);
			session.Flush();
			val.Commit();
			session.Close();
		}
		catch (Exception ex)
		{
			val.Rollback();
			session.Close();
			throw ex;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public IList List(string typeName)
	{
		IList result = null;
		session = sessionFactory.OpenSession();
		ITransaction val = session.BeginTransaction();
		try
		{
			IQuery val2 = session.CreateQuery("from " + typeName);
			result = val2.List();
			val.Commit();
			session.Close();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return result;
	}

	public IList ListID(string typeName, QuestionType qt, int chapterID)
	{
		IList result = null;
		string text = "";
		string text2 = "";
		switch (qt)
		{
		case QuestionType.READING:
			text = "pid";
			text2 = "=0";
			break;
		case QuestionType.MULTIPLE_CHOICE:
			text = "qid";
			text2 = "=1";
			break;
		case QuestionType.INDICATE_MISTAKE:
			text = "qid";
			text2 = "=2";
			break;
		case QuestionType.MATCH:
			text = "mid";
			text2 = "=3";
			break;
		default:
			text = "qid";
			text2 = ">3";
			break;
		}
		string text3 = "SELECT " + text + " FROM " + typeName + " WHERE chapterId=" + chapterID + " AND qType=" + text2;
		SqlConnection sqlConnection = (SqlConnection)sessionFactory.ConnectionProvider.GetConnection();
		return result;
	}

	public IList ListID(string typeName, QuestionType qt, string courseID)
	{
		return null;
	}
}
