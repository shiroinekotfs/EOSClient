using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NAudio.Utils;

namespace NAudio.Wave;

public class Id3v2Tag
{
	private long tagStartPosition;

	private long tagEndPosition;

	private byte[] rawData;

	public byte[] RawData => rawData;

	public static Id3v2Tag ReadTag(Stream input)
	{
		try
		{
			return new Id3v2Tag(input);
		}
		catch (FormatException)
		{
			return null;
		}
	}

	public static Id3v2Tag Create(IEnumerable<KeyValuePair<string, string>> tags)
	{
		return ReadTag(CreateId3v2TagStream(tags));
	}

	private static byte[] FrameSizeToBytes(int n)
	{
		byte[] bytes = BitConverter.GetBytes(n);
		Array.Reverse(bytes);
		return bytes;
	}

	private static byte[] CreateId3v2Frame(string key, string value)
	{
		if (string.IsNullOrEmpty(key))
		{
			throw new ArgumentNullException("key");
		}
		if (string.IsNullOrEmpty(value))
		{
			throw new ArgumentNullException("value");
		}
		if (key.Length != 4)
		{
			throw new ArgumentOutOfRangeException("key", "key " + key + " must be 4 characters long");
		}
		byte[] array = new byte[2] { 255, 254 };
		byte[] array2 = new byte[3];
		byte[] array3 = array2;
		byte[] array4 = new byte[2];
		byte[] array5 = array4;
		byte[] array6 = ((!(key == "COMM")) ? ByteArrayExtensions.Concat(new byte[1] { 1 }, array, Encoding.Unicode.GetBytes(value)) : ByteArrayExtensions.Concat(new byte[1] { 1 }, array3, array5, array, Encoding.Unicode.GetBytes(value)));
		byte[][] array7 = new byte[4][]
		{
			Encoding.UTF8.GetBytes(key),
			FrameSizeToBytes(array6.Length),
			null,
			null
		};
		byte[] array8 = new byte[2];
		array7[2] = array8;
		array7[3] = array6;
		return ByteArrayExtensions.Concat(array7);
	}

	private static byte[] GetId3TagHeaderSize(int size)
	{
		byte[] array = new byte[4];
		for (int num = array.Length - 1; num >= 0; num--)
		{
			array[num] = (byte)(size % 128);
			size /= 128;
		}
		return array;
	}

	private static byte[] CreateId3v2TagHeader(IEnumerable<byte[]> frames)
	{
		int num = 0;
		foreach (byte[] frame in frames)
		{
			num += frame.Length;
		}
		byte[][] array = new byte[4][]
		{
			Encoding.UTF8.GetBytes("ID3"),
			new byte[2] { 3, 0 },
			null,
			null
		};
		byte[] array2 = new byte[1];
		array[2] = array2;
		array[3] = GetId3TagHeaderSize(num);
		return ByteArrayExtensions.Concat(array);
	}

	private static Stream CreateId3v2TagStream(IEnumerable<KeyValuePair<string, string>> tags)
	{
		List<byte[]> list = new List<byte[]>();
		foreach (KeyValuePair<string, string> tag in tags)
		{
			list.Add(CreateId3v2Frame(tag.Key, tag.Value));
		}
		byte[] array = CreateId3v2TagHeader(list);
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(array, 0, array.Length);
		foreach (byte[] item in list)
		{
			memoryStream.Write(item, 0, item.Length);
		}
		memoryStream.Position = 0L;
		return memoryStream;
	}

	private Id3v2Tag(Stream input)
	{
		tagStartPosition = input.Position;
		BinaryReader binaryReader = new BinaryReader(input);
		byte[] array = binaryReader.ReadBytes(10);
		if (array.Length >= 3 && array[0] == 73 && array[1] == 68 && array[2] == 51)
		{
			if ((array[5] & 0x40) == 64)
			{
				byte[] array2 = binaryReader.ReadBytes(4);
				int num = array2[0] * 2097152;
				num += array2[1] * 16384;
				num += array2[2] * 128;
				num += array2[3];
			}
			int num2 = array[6] * 2097152;
			num2 += array[7] * 16384;
			num2 += array[8] * 128;
			num2 += array[9];
			binaryReader.ReadBytes(num2);
			if ((array[5] & 0x10) == 16)
			{
				binaryReader.ReadBytes(10);
			}
			tagEndPosition = input.Position;
			input.Position = tagStartPosition;
			rawData = binaryReader.ReadBytes((int)(tagEndPosition - tagStartPosition));
			return;
		}
		input.Position = tagStartPosition;
		throw new FormatException("Not an ID3v2 tag");
	}
}
