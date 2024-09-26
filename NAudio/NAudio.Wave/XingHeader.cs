using System;

namespace NAudio.Wave;

public class XingHeader
{
	[Flags]
	private enum XingHeaderOptions
	{
		Frames = 1,
		Bytes = 2,
		Toc = 4,
		VbrScale = 8
	}

	private static int[] sr_table = new int[4] { 44100, 48000, 32000, 99999 };

	private int vbrScale = -1;

	private int startOffset;

	private int endOffset;

	private int tocOffset = -1;

	private int framesOffset = -1;

	private int bytesOffset = -1;

	private Mp3Frame frame;

	public int Frames
	{
		get
		{
			if (framesOffset == -1)
			{
				return -1;
			}
			return ReadBigEndian(frame.RawData, framesOffset);
		}
		set
		{
			if (framesOffset == -1)
			{
				throw new InvalidOperationException("Frames flag is not set");
			}
			WriteBigEndian(frame.RawData, framesOffset, value);
		}
	}

	public int Bytes
	{
		get
		{
			if (bytesOffset == -1)
			{
				return -1;
			}
			return ReadBigEndian(frame.RawData, bytesOffset);
		}
		set
		{
			if (framesOffset == -1)
			{
				throw new InvalidOperationException("Bytes flag is not set");
			}
			WriteBigEndian(frame.RawData, bytesOffset, value);
		}
	}

	public int VbrScale => vbrScale;

	public Mp3Frame Mp3Frame => frame;

	private static int ReadBigEndian(byte[] buffer, int offset)
	{
		int num = buffer[offset];
		num <<= 8;
		num |= buffer[offset + 1];
		num <<= 8;
		num |= buffer[offset + 2];
		num <<= 8;
		return num | buffer[offset + 3];
	}

	private void WriteBigEndian(byte[] buffer, int offset, int value)
	{
		byte[] bytes = BitConverter.GetBytes(value);
		for (int i = 0; i < 4; i++)
		{
			buffer[offset + 4 - i] = bytes[i];
		}
	}

	public static XingHeader LoadXingHeader(Mp3Frame frame)
	{
		XingHeader xingHeader = new XingHeader();
		xingHeader.frame = frame;
		int num = 0;
		if (frame.MpegVersion == MpegVersion.Version1)
		{
			num = ((frame.ChannelMode == ChannelMode.Mono) ? 21 : 36);
		}
		else
		{
			if (frame.MpegVersion != MpegVersion.Version2)
			{
				return null;
			}
			num = ((frame.ChannelMode == ChannelMode.Mono) ? 13 : 21);
		}
		if (frame.RawData[num] == 88 && frame.RawData[num + 1] == 105 && frame.RawData[num + 2] == 110 && frame.RawData[num + 3] == 103)
		{
			xingHeader.startOffset = num;
			num += 4;
			XingHeaderOptions xingHeaderOptions = (XingHeaderOptions)ReadBigEndian(frame.RawData, num);
			num += 4;
			if ((xingHeaderOptions & XingHeaderOptions.Frames) != 0)
			{
				xingHeader.framesOffset = num;
				num += 4;
			}
			if ((xingHeaderOptions & XingHeaderOptions.Bytes) != 0)
			{
				xingHeader.bytesOffset = num;
				num += 4;
			}
			if ((xingHeaderOptions & XingHeaderOptions.Toc) != 0)
			{
				xingHeader.tocOffset = num;
				num += 100;
			}
			if ((xingHeaderOptions & XingHeaderOptions.VbrScale) != 0)
			{
				xingHeader.vbrScale = ReadBigEndian(frame.RawData, num);
				num += 4;
			}
			xingHeader.endOffset = num;
			return xingHeader;
		}
		return null;
	}

	private XingHeader()
	{
	}
}
