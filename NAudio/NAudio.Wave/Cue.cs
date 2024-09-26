using System.Text.RegularExpressions;

namespace NAudio.Wave;

public class Cue
{
	public int Position { get; private set; }

	public string Label { get; private set; }

	public Cue(int position, string label)
	{
		Position = position;
		if (label == null)
		{
			label = "";
		}
		Label = Regex.Replace(label, "[^\\u0000-\\u00FF]", "");
	}
}
