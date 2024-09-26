namespace NAudio.SoundFont;

public class ModulatorType
{
	private bool polarity;

	private bool direction;

	private bool midiContinuousController;

	private ControllerSourceEnum controllerSource;

	private SourceTypeEnum sourceType;

	private ushort midiContinuousControllerNumber;

	internal ModulatorType(ushort raw)
	{
		polarity = (raw & 0x200) == 512;
		direction = (raw & 0x100) == 256;
		midiContinuousController = (raw & 0x80) == 128;
		sourceType = (SourceTypeEnum)((raw & 0xFC00) >> 10);
		controllerSource = (ControllerSourceEnum)(raw & 0x7F);
		midiContinuousControllerNumber = (ushort)(raw & 0x7Fu);
	}

	public override string ToString()
	{
		if (midiContinuousController)
		{
			return $"{sourceType} CC{midiContinuousControllerNumber}";
		}
		return $"{sourceType} {controllerSource}";
	}
}
