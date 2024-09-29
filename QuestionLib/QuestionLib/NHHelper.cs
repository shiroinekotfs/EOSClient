using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace QuestionLib;

public class NHHelper
{
	public static string ConnectionString = "";

	public ISessionFactory SessionFactory;

	public void Configure()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Configuration val = new Configuration().Configure();
		val.AddAssembly("QuestionLib");
		val.Properties["hibernate.connection.connection_string"] = ConnectionString;
		val.Properties["connection.connection_string"] = ConnectionString;
		SessionFactory = val.BuildSessionFactory();
	}

	public void ExportTables()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Configuration val = new Configuration().Configure();
		val.AddAssembly("QuestionLib");
		new SchemaExport(val).Create(true, true);
	}

	public static ISessionFactory GetSessionFactory()
	{
		NHHelper nHHelper = new NHHelper();
		nHHelper.Configure();
		return nHHelper.SessionFactory;
	}
}
