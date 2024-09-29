using System;
using System.Collections;
using NHibernate;
using QuestionLib.Entity;

namespace QuestionLib.Business;

public class BOLO : BOBase
{
	public BOLO(ISessionFactory sessionFactory)
		: base(sessionFactory)
	{
	}

	public LO Load(int loid)
	{
		IList list = null;
		session = sessionFactory.OpenSession();
		try
		{
			string text = null;
			text = "from LO lo Where lo.LOID=:loid";
			IQuery val = session.CreateQuery(text);
			val.SetParameter("loid", (object)loid);
			list = val.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		return (list.Count > 0) ? ((LO)list[0]) : null;
	}

	public int GetLOID(string lo_name, string CID)
	{
		IList list = null;
		session = sessionFactory.OpenSession();
		try
		{
			string text = null;
			text = "from LO lo Where lo.LO_Name=:lo_name And lo.CID=:CID";
			IQuery val = session.CreateQuery(text);
			val.SetParameter("lo_name", (object)lo_name.Trim());
			val.SetParameter("CID", (object)CID);
			list = val.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		LO lO = (LO)list[0];
		return lO.LOID;
	}

	public IList LoadLOByCourse(string courseId)
	{
		session = sessionFactory.OpenSession();
		IList result;
		try
		{
			string text = null;
			text = "from LO lo Where lo.CID=:courseId";
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

	public bool IsLOExists(string CID, string lo_name)
	{
		session = sessionFactory.OpenSession();
		IList list;
		try
		{
			string text = null;
			text = "from LO lo Where lo.CID=:CID And lo.LO_Name=:lo_name";
			IQuery val = session.CreateQuery(text);
			val.SetParameter("lo_name", (object)lo_name);
			val.SetParameter("CID", (object)CID);
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

	public bool IsLODescriptionExists(string CID, string lo_desc)
	{
		session = sessionFactory.OpenSession();
		IList list;
		try
		{
			string text = null;
			text = "from LO lo Where lo.CID=:CID And lo.LO_Desc=:lo_desc";
			IQuery val = session.CreateQuery(text);
			val.SetParameter("lo_desc", (object)lo_desc);
			val.SetParameter("CID", (object)CID);
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

	public static ArrayList RemoveDupLO(ArrayList listLO)
	{
		ArrayList arrayList = new ArrayList();
		foreach (QuestionLO item in listLO)
		{
			bool flag = false;
			foreach (QuestionLO item2 in arrayList)
			{
				if (item.LOID == item2.LOID)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				arrayList.Add(item);
			}
		}
		return arrayList;
	}
}
