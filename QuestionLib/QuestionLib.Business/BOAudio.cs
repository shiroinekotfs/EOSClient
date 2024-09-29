using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using NHibernate;
using QuestionLib.Entity;

namespace QuestionLib.Business;

public class BOAudio : BOBase
{
	public BOAudio(ISessionFactory sessionFactory)
		: base(sessionFactory)
	{
	}

	public Audio Load(int auid)
	{
		IList list = null;
		session = sessionFactory.OpenSession();
		try
		{
			string text = null;
			text = "from Audio a Where a.AuID=:auid";
			IQuery val = session.CreateQuery(text);
			val.SetParameter("auid", (object)auid);
			list = val.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		return (list.Count > 0) ? ((Audio)list[0]) : null;
	}

	public DataTable LoadAllChapterAudio(int chid, string conStr)
	{
		IList list = new ArrayList();
		SqlConnection sqlConnection = new SqlConnection(conStr);
		string selectCommandText = "SELECT AuID, AudioFile, AudioSize, AudioLength, RepeatTime, PaddingTime, PlayOrder FROM Audio WHERE ChID = " + chid;
		SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(selectCommandText, sqlConnection);
		DataTable dataTable = new DataTable();
		sqlDataAdapter.Fill(dataTable);
		sqlConnection.Close();
		return dataTable;
	}

	public Audio LoadAudio(int auid, string conStr)
	{
		SqlConnection sqlConnection = new SqlConnection(conStr);
		sqlConnection.Open();
		string cmdText = "SELECT AudioFile, AudioSize, AudioLength, AudioData, RepeatTime, PaddingTime, PlayOrder FROM Audio WHERE AuID= " + auid;
		SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
		SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
		Audio audio = null;
		if (sqlDataReader.Read())
		{
			audio = new Audio();
			audio.AudioFile = sqlDataReader["AudioFile"].ToString();
			audio.AudioSize = (int)sqlDataReader["AudioSize"];
			audio.AudioData = new byte[audio.AudioSize];
			sqlDataReader.GetBytes(3, 0L, audio.AudioData, 0, audio.AudioSize);
			audio.AudioLength = (int)sqlDataReader["AudioLength"];
		}
		sqlDataReader.Close();
		sqlConnection.Close();
		return audio;
	}

	public List<AudioInPaper> LoadChapterAudio(int chapterID, string conStr)
	{
		List<AudioInPaper> list = new List<AudioInPaper>();
		SqlConnection sqlConnection = new SqlConnection(conStr);
		sqlConnection.Open();
		string cmdText = "SELECT AudioSize, AudioData, AudioLength, RepeatTime, PaddingTime, PlayOrder FROM Audio WHERE ChID= " + chapterID;
		SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
		SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
		AudioInPaper audioInPaper = null;
		while (sqlDataReader.Read())
		{
			audioInPaper = new AudioInPaper();
			audioInPaper.AudioSize = (int)sqlDataReader["AudioSize"];
			audioInPaper.AudioData = new byte[audioInPaper.AudioSize];
			sqlDataReader.GetBytes(1, 0L, audioInPaper.AudioData, 0, audioInPaper.AudioSize);
			audioInPaper.AudioLength = (int)sqlDataReader["AudioLength"];
			audioInPaper.RepeatTime = (byte)sqlDataReader["RepeatTime"];
			audioInPaper.PaddingTime = (int)sqlDataReader["PaddingTime"];
			audioInPaper.PlayOrder = (byte)sqlDataReader["PlayOrder"];
			list.Add(audioInPaper);
		}
		sqlDataReader.Close();
		sqlConnection.Close();
		return list;
	}

	public static bool Delete(int auid, string conStr)
	{
		SqlConnection sqlConnection = new SqlConnection(conStr);
		sqlConnection.Open();
		string cmdText = "DELETE FROM Audio WHERE AuID= " + auid;
		SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
		int num = sqlCommand.ExecuteNonQuery();
		sqlConnection.Close();
		return num > 0;
	}

	public static bool AudioExist(int auid, string conStr)
	{
		SqlConnection sqlConnection = new SqlConnection(conStr);
		sqlConnection.Open();
		string cmdText = "SELECT * FROM Audio WHERE AuID= " + auid;
		SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
		SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
		if (sqlDataReader.HasRows)
		{
			sqlDataReader.Close();
			sqlConnection.Close();
			return true;
		}
		sqlDataReader.Close();
		sqlConnection.Close();
		return false;
	}

	public static bool AudioFileExist(int chapterID, string audioFile, int audioSize, int audioLength, string conStr)
	{
		SqlConnection sqlConnection = new SqlConnection(conStr);
		sqlConnection.Open();
		string cmdText = "SELECT * FROM Audio WHERE ChID= @chapterID AND AudioFile=@audioFile AND AudioSize = @audioSize AND AudioLength = @audioLength";
		SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
		sqlCommand.Parameters.Add("chapterID", SqlDbType.Int);
		sqlCommand.Parameters["chapterID"].Value = chapterID;
		sqlCommand.Parameters.Add("audioFile", SqlDbType.NVarChar);
		sqlCommand.Parameters["audioFile"].Value = audioFile;
		sqlCommand.Parameters.Add("audioSize", SqlDbType.Int);
		sqlCommand.Parameters["audioSize"].Value = audioSize;
		sqlCommand.Parameters.Add("audioLength", SqlDbType.Int);
		sqlCommand.Parameters["audioLength"].Value = audioLength;
		SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
		if (sqlDataReader.HasRows)
		{
			sqlDataReader.Close();
			sqlConnection.Close();
			return true;
		}
		sqlDataReader.Close();
		sqlConnection.Close();
		return false;
	}

	public static string GetAudioFile(int auid, string conStr)
	{
		SqlConnection sqlConnection = new SqlConnection(conStr);
		sqlConnection.Open();
		string cmdText = "SELECT AudioFile FROM Audio WHERE AuID= " + auid;
		SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
		SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
		if (sqlDataReader.Read())
		{
			string result = sqlDataReader["AudioFile"].ToString();
			sqlDataReader.Close();
			sqlConnection.Close();
			return result;
		}
		sqlDataReader.Close();
		sqlConnection.Close();
		return null;
	}

	public static string GetChapterAudioFile(int chapterID, string conStr)
	{
		SqlConnection sqlConnection = new SqlConnection(conStr);
		sqlConnection.Open();
		string cmdText = "SELECT AudioFile FROM Audio WHERE ChID= " + chapterID;
		SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
		SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
		string text = null;
		while (sqlDataReader.Read())
		{
			text = ((text != null) ? (text + ", " + sqlDataReader["AudioFile"].ToString()) : sqlDataReader["AudioFile"].ToString());
		}
		sqlDataReader.Close();
		sqlConnection.Close();
		return text;
	}

	public static int GetAudioLength(int chapterID, string conStr)
	{
		SqlConnection sqlConnection = new SqlConnection(conStr);
		sqlConnection.Open();
		string cmdText = "SELECT AudioLength, RepeatTime, PaddingTime FROM Audio WHERE ChID= " + chapterID;
		SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
		SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
		int num = 0;
		while (sqlDataReader.Read())
		{
			int num2 = Convert.ToInt32(sqlDataReader["AudioLength"].ToString());
			int num3 = Convert.ToByte(sqlDataReader["RepeatTime"].ToString());
			int num4 = Convert.ToInt32(sqlDataReader["PaddingTime"].ToString());
			num += (num2 + num4) * num3;
		}
		sqlDataReader.Close();
		sqlConnection.Close();
		return num;
	}

	public static int GetFileAudioLength(int auid, string conStr)
	{
		SqlConnection sqlConnection = new SqlConnection(conStr);
		sqlConnection.Open();
		string cmdText = "SELECT AudioLength FROM Audio WHERE AuID= " + auid;
		SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
		SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
		int result = 0;
		if (sqlDataReader.Read())
		{
			result = Convert.ToInt32(sqlDataReader["AudioLength"].ToString());
		}
		sqlDataReader.Close();
		sqlConnection.Close();
		return result;
	}

	public static bool UpdateAudioPlayingInfo(List<Audio> listAudio, string conStr)
	{
		SqlConnection sqlConnection = new SqlConnection(conStr);
		sqlConnection.Open();
		string cmdText = "UPDATE Audio SET RepeatTime=@repeatTime, PaddingTime=@paddingTime, PlayOrder=@playOrder WHERE AuID= @auID";
		SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
		try
		{
			sqlCommand.Parameters.Add("repeatTime", SqlDbType.TinyInt);
			sqlCommand.Parameters.Add("paddingTime", SqlDbType.Int);
			sqlCommand.Parameters.Add("playOrder", SqlDbType.TinyInt);
			sqlCommand.Parameters.Add("auID", SqlDbType.Int);
			sqlCommand.Transaction = sqlConnection.BeginTransaction();
			foreach (Audio item in listAudio)
			{
				sqlCommand.Parameters["repeatTime"].Value = item.RepeatTime;
				sqlCommand.Parameters["paddingTime"].Value = item.PaddingTime;
				sqlCommand.Parameters["playOrder"].Value = item.PlayOrder;
				sqlCommand.Parameters["auID"].Value = item.AuID;
				sqlCommand.ExecuteNonQuery();
			}
			sqlCommand.Transaction.Commit();
			sqlConnection.Close();
			return true;
		}
		catch
		{
			sqlCommand.Transaction.Rollback();
			sqlConnection.Close();
			return false;
		}
	}

	public static bool CheckAudioPlayingInfo(int chapterID, string conStr)
	{
		bool result = true;
		SqlConnection sqlConnection = new SqlConnection(conStr);
		sqlConnection.Open();
		string cmdText = "SELECT AudioLength, RepeatTime, PaddingTime, PlayOrder FROM Audio WHERE ChID= " + chapterID;
		SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
		SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
		while (sqlDataReader.Read())
		{
			int num = Convert.ToInt32(sqlDataReader["AudioLength"].ToString());
			int num2 = Convert.ToByte(sqlDataReader["RepeatTime"].ToString());
			int num3 = Convert.ToInt32(sqlDataReader["PaddingTime"].ToString());
			int num4 = Convert.ToInt32(sqlDataReader["PlayOrder"].ToString());
			if (num * num2 * num3 * num4 == 0)
			{
				result = false;
				break;
			}
		}
		sqlDataReader.Close();
		sqlConnection.Close();
		return result;
	}
}
