using System;

namespace NAudio.Utils;

public class CircularBuffer
{
	private readonly byte[] buffer;

	private readonly object lockObject;

	private int writePosition;

	private int readPosition;

	private int byteCount;

	public int MaxLength => buffer.Length;

	public int Count => byteCount;

	public CircularBuffer(int size)
	{
		buffer = new byte[size];
		lockObject = new object();
	}

	public int Write(byte[] data, int offset, int count)
	{
		lock (lockObject)
		{
			int num = 0;
			if (count > buffer.Length - byteCount)
			{
				count = buffer.Length - byteCount;
			}
			int num2 = Math.Min(buffer.Length - writePosition, count);
			Array.Copy(data, offset, buffer, writePosition, num2);
			writePosition += num2;
			writePosition %= buffer.Length;
			num += num2;
			if (num < count)
			{
				Array.Copy(data, offset + num, buffer, writePosition, count - num);
				writePosition += count - num;
				num = count;
			}
			byteCount += num;
			return num;
		}
	}

	public int Read(byte[] data, int offset, int count)
	{
		lock (lockObject)
		{
			if (count > byteCount)
			{
				count = byteCount;
			}
			int num = 0;
			int num2 = Math.Min(buffer.Length - readPosition, count);
			Array.Copy(buffer, readPosition, data, offset, num2);
			num += num2;
			readPosition += num2;
			readPosition %= buffer.Length;
			if (num < count)
			{
				Array.Copy(buffer, readPosition, data, offset + num, count - num);
				readPosition += count - num;
				num = count;
			}
			byteCount -= num;
			return num;
		}
	}

	public void Reset()
	{
		byteCount = 0;
		readPosition = 0;
		writePosition = 0;
	}

	public void Advance(int count)
	{
		if (count >= byteCount)
		{
			Reset();
			return;
		}
		byteCount -= count;
		readPosition += count;
		readPosition %= MaxLength;
	}
}
