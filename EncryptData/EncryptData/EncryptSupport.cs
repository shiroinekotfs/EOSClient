using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace EncryptData;

public class EncryptSupport
{
	public static byte[] ObjectToByteArray(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream memoryStream = new MemoryStream();
		binaryFormatter.Serialize(memoryStream, obj);
		return memoryStream.ToArray();
	}

	public static object ByteArrayToObject(byte[] arrBytes)
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		memoryStream.Write(arrBytes, 0, arrBytes.Length);
		memoryStream.Seek(0L, SeekOrigin.Begin);
		return binaryFormatter.Deserialize(memoryStream);
	}

	public static bool EncryptQuestions_SaveToFile(string fname, byte[] data, string key)
	{
		try
		{
			FileStream fileStream = new FileStream(fname, FileMode.Create, FileAccess.Write);
			DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
			dESCryptoServiceProvider.Key = Encoding.ASCII.GetBytes(key);
			dESCryptoServiceProvider.IV = Encoding.ASCII.GetBytes(key);
			CryptoStream cryptoStream = new CryptoStream(fileStream, dESCryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(data, 0, data.Length);
			cryptoStream.Close();
			fileStream.Close();
			return true;
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static byte[] DecryptQuestions_FromFile(string fname, string key)
	{
		try
		{
			FileStream fileStream = new FileStream(fname, FileMode.Open, FileAccess.Read);
			DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
			dESCryptoServiceProvider.Key = Encoding.ASCII.GetBytes(key);
			dESCryptoServiceProvider.IV = Encoding.ASCII.GetBytes(key);
			CryptoStream cryptoStream = new CryptoStream(fileStream, dESCryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Read);
			byte[] array = new byte[fileStream.Length];
			int num = cryptoStream.Read(array, 0, (int)fileStream.Length);
			cryptoStream.Close();
			fileStream.Close();
			return array;
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static string Encryption(byte[] data, string key)
	{
		DES dES = new DESCryptoServiceProvider();
		dES.Key = Encoding.ASCII.GetBytes(key);
		dES.IV = dES.Key;
		dES.Padding = PaddingMode.PKCS7;
		MemoryStream memoryStream = new MemoryStream();
		ICryptoTransform transform = dES.CreateEncryptor();
		CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
		cryptoStream.Write(data, 0, data.Length);
		cryptoStream.FlushFinalBlock();
		memoryStream.Position = 0L;
		string empty = string.Empty;
		empty = Convert.ToBase64String(memoryStream.ToArray());
		cryptoStream.Close();
		return empty;
	}

	public static string Decryption(byte[] data, string key)
	{
		DES dES = new DESCryptoServiceProvider();
		dES.Key = Encoding.ASCII.GetBytes(key);
		dES.IV = dES.Key;
		dES.Padding = PaddingMode.PKCS7;
		MemoryStream stream = new MemoryStream(data);
		CryptoStream cryptoStream = new CryptoStream(stream, dES.CreateDecryptor(), CryptoStreamMode.Read);
		byte[] array = new byte[data.Length];
		cryptoStream.Read(array, 0, array.Length);
		cryptoStream.Close();
		return Encoding.Unicode.GetString(array);
	}

	public static string GetMD5(string msg)
	{
		MD5 mD = new MD5CryptoServiceProvider();
		byte[] bytes = mD.ComputeHash(Encoding.Unicode.GetBytes(msg));
		return Encoding.Unicode.GetString(bytes);
	}
}
