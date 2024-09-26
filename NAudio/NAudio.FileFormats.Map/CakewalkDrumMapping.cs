namespace NAudio.FileFormats.Map;

public class CakewalkDrumMapping
{
	public string NoteName { get; set; }

	public int InNote { get; set; }

	public int OutNote { get; set; }

	public int OutPort { get; set; }

	public int Channel { get; set; }

	public int VelocityAdjust { get; set; }

	public float VelocityScale { get; set; }

	public override string ToString()
	{
		return $"{NoteName} In:{InNote} Out:{OutNote} Ch:{Channel} Port:{OutPort} Vel+:{VelocityAdjust} Vel:{VelocityScale * 100f}%";
	}
}
