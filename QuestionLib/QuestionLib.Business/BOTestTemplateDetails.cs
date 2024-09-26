using System.Data;
using System.Data.SqlClient;
using NHibernate;
using QuestionLib.Entity;

namespace QuestionLib.Business;

public class BOTestTemplateDetails : BOBase
{
	public BOTestTemplateDetails(ISessionFactory sessionFactory)
		: base(sessionFactory)
	{
	}

	public static DataTable LoadTestTemplateDetails(string testTemplateID, string conStr)
	{
		SqlConnection sqlConnection = new SqlConnection(conStr);
		sqlConnection.Open();
		string cmdText = "SELECT tt.TestTemplateName AS 'Test template name', tt.CID, ch.Name AS 'Chapter', ttd.NoQInTest, tmp.QString AS 'Question type' FROM TestTemplateDetails AS ttd INNER JOIN TestTemplate AS tt ON tt.TestTemplateID = ttd.TestTemplateID INNER JOIN Chapter AS ch ON ch.ChID = ttd.ChapterID INNER JOIN (SELECT 0 AS QType, 'Reading' AS QString UNION SELECT 1 AS QType, 'Multiple choice' AS QString UNION SELECT 2 AS QType, 'Indicate mistake' AS QString UNION SELECT 3 AS QType, 'Match' AS QString UNION SELECT 4 AS QType, 'Fill blank' AS QString ) AS tmp ON ttd.QuestionType = tmp.QType WHERE tt.TestTemplateID = @testTemplateID";
		SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
		sqlCommand.Parameters.Add("testTemplateID", SqlDbType.Int);
		sqlCommand.Parameters["testTemplateID"].Value = testTemplateID;
		SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
		DataTable dataTable = new DataTable();
		sqlDataAdapter.Fill(dataTable);
		sqlConnection.Close();
		return dataTable;
	}

	public static DataTable LoadTestTemplateDetails(QuestionType questionType, int testTemplateID, string conStr)
	{
		SqlConnection sqlConnection = new SqlConnection(conStr);
		sqlConnection.Open();
		string cmdText = "SELECT ChapterId,QuestionType,NoQInTest FROM TestTemplateDetails WHERE TestTemplateID = @testTemplateID AND QuestionType = @questionType ";
		SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
		sqlCommand.Parameters.Add("testTemplateID", SqlDbType.Int);
		sqlCommand.Parameters["testTemplateID"].Value = testTemplateID;
		sqlCommand.Parameters.Add("questionType", SqlDbType.Int);
		sqlCommand.Parameters["questionType"].Value = questionType;
		SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
		DataTable dataTable = new DataTable();
		sqlDataAdapter.Fill(dataTable);
		sqlConnection.Close();
		return dataTable;
	}
}
