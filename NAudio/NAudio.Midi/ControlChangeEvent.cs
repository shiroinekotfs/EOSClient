using System;
using System.IO;

namespace NAudio.Midi;

public class ControlChangeEvent : MidiEvent
{
	private MidiController controller;

	private byte controllerValue;

	public MidiController Controller
	{
		get
		{
			return controller;
		}
		set
		{
			if ((int)value < 0 || (int)value > 127)
			{
				throw new ArgumentOutOfRangeException("value", "Controller number must be in the range 0-127");
			}
			controller = value;
		}
	}

	public int ControllerValue
	{
		get
		{
			return controllerValue;
		}
		set
		{
			if (value < 0 || value > 127)
			{
				throw new ArgumentOutOfRangeException("value", "Controller Value must be in the range 0-127");
			}
			controllerValue = (byte)value;
		}
	}

	public ControlChangeEvent(BinaryReader br)
	{
		byte b = br.ReadByte();
		controllerValue = br.ReadByte();
		if ((b & 0x80u) != 0)
		{
			throw new InvalidDataException("Invalid controller");
		}
		controller = (MidiController)b;
		if ((controllerValue & 0x80u) != 0)
		{
			throw new InvalidDataException($"Invalid controllerValue {controllerValue} for controller {controller}, Pos 0x{br.BaseStream.Position:X}");
		}
	}

	public ControlChangeEvent(long absoluteTime, int channel, MidiController controller, int controllerValue)
		: base(absoluteTime, channel, MidiCommandCode.ControlChange)
	{
		Controller = controller;
		ControllerValue = controllerValue;
	}

	public override string ToString()
	{
		return $"{base.ToString()} Controller {controller} Value {controllerValue}";
	}

	public override int GetAsShortMessage()
	{
		byte b = (byte)controller;
		return base.GetAsShortMessage() + (b << 8) + (controllerValue << 16);
	}

	public override void Export(ref long absoluteTime, BinaryWriter writer)
	{
		base.Export(ref absoluteTime, writer);
		writer.Write((byte)controller);
		writer.Write(controllerValue);
	}
}
