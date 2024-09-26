using System;
using System.IO;

namespace NAudio.Midi;

public class MidiEvent
{
	private MidiCommandCode commandCode;

	private int channel;

	private int deltaTime;

	private long absoluteTime;

	public virtual int Channel
	{
		get
		{
			return channel;
		}
		set
		{
			if (value < 1 || value > 16)
			{
				throw new ArgumentOutOfRangeException("value", value, $"Channel must be 1-16 (Got {value})");
			}
			channel = value;
		}
	}

	public int DeltaTime => deltaTime;

	public long AbsoluteTime
	{
		get
		{
			return absoluteTime;
		}
		set
		{
			absoluteTime = value;
		}
	}

	public MidiCommandCode CommandCode => commandCode;

	public static MidiEvent FromRawMessage(int rawMessage)
	{
		long num = 0L;
		int num2 = rawMessage & 0xFF;
		int num3 = (rawMessage >> 8) & 0xFF;
		int num4 = (rawMessage >> 16) & 0xFF;
		int num5 = 1;
		MidiCommandCode midiCommandCode;
		if ((num2 & 0xF0) == 240)
		{
			midiCommandCode = (MidiCommandCode)num2;
		}
		else
		{
			midiCommandCode = (MidiCommandCode)((uint)num2 & 0xF0u);
			num5 = (num2 & 0xF) + 1;
		}
		switch (midiCommandCode)
		{
		case MidiCommandCode.NoteOff:
		case MidiCommandCode.NoteOn:
		case MidiCommandCode.KeyAfterTouch:
			if (num4 > 0 && midiCommandCode == MidiCommandCode.NoteOn)
			{
				return new NoteOnEvent(num, num5, num3, num4, 0);
			}
			return new NoteEvent(num, num5, midiCommandCode, num3, num4);
		case MidiCommandCode.ControlChange:
			return new ControlChangeEvent(num, num5, (MidiController)num3, num4);
		case MidiCommandCode.PatchChange:
			return new PatchChangeEvent(num, num5, num3);
		case MidiCommandCode.ChannelAfterTouch:
			return new ChannelAfterTouchEvent(num, num5, num3);
		case MidiCommandCode.PitchWheelChange:
			return new PitchWheelChangeEvent(num, num5, num3 + (num4 << 7));
		case MidiCommandCode.TimingClock:
		case MidiCommandCode.StartSequence:
		case MidiCommandCode.ContinueSequence:
		case MidiCommandCode.StopSequence:
		case MidiCommandCode.AutoSensing:
			return new MidiEvent(num, num5, midiCommandCode);
		default:
			throw new FormatException($"Unsupported MIDI Command Code for Raw Message {midiCommandCode}");
		}
	}

	public static MidiEvent ReadNextEvent(BinaryReader br, MidiEvent previous)
	{
		int num = ReadVarInt(br);
		int num2 = 1;
		byte b = br.ReadByte();
		MidiCommandCode midiCommandCode;
		if ((b & 0x80) == 0)
		{
			midiCommandCode = previous.CommandCode;
			num2 = previous.Channel;
			br.BaseStream.Position--;
		}
		else if ((b & 0xF0) == 240)
		{
			midiCommandCode = (MidiCommandCode)b;
		}
		else
		{
			midiCommandCode = (MidiCommandCode)(b & 0xF0u);
			num2 = (b & 0xF) + 1;
		}
		MidiEvent midiEvent;
		switch (midiCommandCode)
		{
		case MidiCommandCode.NoteOn:
			midiEvent = new NoteOnEvent(br);
			break;
		case MidiCommandCode.NoteOff:
		case MidiCommandCode.KeyAfterTouch:
			midiEvent = new NoteEvent(br);
			break;
		case MidiCommandCode.ControlChange:
			midiEvent = new ControlChangeEvent(br);
			break;
		case MidiCommandCode.PatchChange:
			midiEvent = new PatchChangeEvent(br);
			break;
		case MidiCommandCode.ChannelAfterTouch:
			midiEvent = new ChannelAfterTouchEvent(br);
			break;
		case MidiCommandCode.PitchWheelChange:
			midiEvent = new PitchWheelChangeEvent(br);
			break;
		case MidiCommandCode.TimingClock:
		case MidiCommandCode.StartSequence:
		case MidiCommandCode.ContinueSequence:
		case MidiCommandCode.StopSequence:
			midiEvent = new MidiEvent();
			break;
		case MidiCommandCode.Sysex:
			midiEvent = SysexEvent.ReadSysexEvent(br);
			break;
		case MidiCommandCode.MetaEvent:
			midiEvent = MetaEvent.ReadMetaEvent(br);
			break;
		default:
			throw new FormatException($"Unsupported MIDI Command Code {(byte)midiCommandCode:X2}");
		}
		midiEvent.channel = num2;
		midiEvent.deltaTime = num;
		midiEvent.commandCode = midiCommandCode;
		return midiEvent;
	}

	public virtual int GetAsShortMessage()
	{
		return channel - 1 + (int)commandCode;
	}

	protected MidiEvent()
	{
	}

	public MidiEvent(long absoluteTime, int channel, MidiCommandCode commandCode)
	{
		this.absoluteTime = absoluteTime;
		Channel = channel;
		this.commandCode = commandCode;
	}

	public static bool IsNoteOff(MidiEvent midiEvent)
	{
		if (midiEvent != null)
		{
			if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
			{
				NoteEvent noteEvent = (NoteEvent)midiEvent;
				return noteEvent.Velocity == 0;
			}
			return midiEvent.CommandCode == MidiCommandCode.NoteOff;
		}
		return false;
	}

	public static bool IsNoteOn(MidiEvent midiEvent)
	{
		if (midiEvent != null && midiEvent.CommandCode == MidiCommandCode.NoteOn)
		{
			NoteEvent noteEvent = (NoteEvent)midiEvent;
			return noteEvent.Velocity > 0;
		}
		return false;
	}

	public static bool IsEndTrack(MidiEvent midiEvent)
	{
		if (midiEvent != null && midiEvent is MetaEvent metaEvent)
		{
			return metaEvent.MetaEventType == MetaEventType.EndTrack;
		}
		return false;
	}

	public override string ToString()
	{
		if ((int)commandCode >= 240)
		{
			return $"{absoluteTime} {commandCode}";
		}
		return $"{absoluteTime} {commandCode} Ch: {channel}";
	}

	public static int ReadVarInt(BinaryReader br)
	{
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			byte b = br.ReadByte();
			num <<= 7;
			num += b & 0x7F;
			if ((b & 0x80) == 0)
			{
				return num;
			}
		}
		throw new FormatException("Invalid Var Int");
	}

	public static void WriteVarInt(BinaryWriter writer, int value)
	{
		if (value < 0)
		{
			throw new ArgumentOutOfRangeException("value", value, "Cannot write a negative Var Int");
		}
		if (value > 268435455)
		{
			throw new ArgumentOutOfRangeException("value", value, "Maximum allowed Var Int is 0x0FFFFFFF");
		}
		int num = 0;
		byte[] array = new byte[4];
		do
		{
			array[num++] = (byte)((uint)value & 0x7Fu);
			value >>= 7;
		}
		while (value > 0);
		while (num > 0)
		{
			num--;
			if (num > 0)
			{
				writer.Write((byte)(array[num] | 0x80u));
			}
			else
			{
				writer.Write(array[num]);
			}
		}
	}

	public virtual void Export(ref long absoluteTime, BinaryWriter writer)
	{
		if (this.absoluteTime < absoluteTime)
		{
			throw new FormatException("Can't export unsorted MIDI events");
		}
		WriteVarInt(writer, (int)(this.absoluteTime - absoluteTime));
		absoluteTime = this.absoluteTime;
		int num = (int)commandCode;
		if (commandCode != MidiCommandCode.MetaEvent)
		{
			num += channel - 1;
		}
		writer.Write((byte)num);
	}
}
