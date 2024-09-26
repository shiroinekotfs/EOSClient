using System;
using System.Text;

namespace NAudio.Wave;

public class RiffChunk
{
	public int Identifier { get; private set; }

	public string IdentifierAsString => Encoding.UTF8.GetString(BitConverter.GetBytes(Identifier));

	public int Length { get; private set; }

	public long StreamPosition { get; private set; }

	public RiffChunk(int identifier, int length, long streamPosition)
	{
		Identifier = identifier;
		Length = length;
		StreamPosition = streamPosition;
	}
}
