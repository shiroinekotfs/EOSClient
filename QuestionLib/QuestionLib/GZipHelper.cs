using System.IO;
using System.IO.Compression;

namespace QuestionLib;

public class GZipHelper
{
	public static byte[] Compress(byte[] bytData)
	{
		try
		{
			MemoryStream memoryStream = new MemoryStream();
			Stream stream = new GZipStream(memoryStream, CompressionMode.Compress);
			stream.Write(bytData, 0, bytData.Length);
			stream.Close();
			return memoryStream.ToArray();
		}
		catch
		{
			return null;
		}
	}

	public static byte[] DeCompress(byte[] bytInput, int originSize)
	{
		Stream stream = new GZipStream(new MemoryStream(bytInput), CompressionMode.Decompress);
		byte[] array = new byte[originSize];
		stream.Read(array, 0, originSize);
		return array;
	}
}
