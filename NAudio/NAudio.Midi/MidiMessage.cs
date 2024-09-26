namespace NAudio.Midi;

public class MidiMessage
{
	private int rawData;

	public int RawData => rawData;

	public MidiMessage(int status, int data1, int data2)
	{
		rawData = status + (data1 << 8) + (data2 << 16);
	}

	public MidiMessage(int rawData)
	{
		this.rawData = rawData;
	}

	public static MidiMessage StartNote(int note, int volume, int channel)
	{
		return new MidiMessage(144 + channel - 1, note, volume);
	}

	public static MidiMessage StopNote(int note, int volume, int channel)
	{
		return new MidiMessage(128 + channel - 1, note, volume);
	}

	public static MidiMessage ChangePatch(int patch, int channel)
	{
		return new MidiMessage(192 + channel - 1, patch, 0);
	}

	public static MidiMessage ChangeControl(int controller, int value, int channel)
	{
		return new MidiMessage(176 + channel - 1, controller, value);
	}
}
