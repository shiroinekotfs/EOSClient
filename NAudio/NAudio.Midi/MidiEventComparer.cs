using System.Collections.Generic;

namespace NAudio.Midi;

public class MidiEventComparer : IComparer<MidiEvent>
{
	public int Compare(MidiEvent x, MidiEvent y)
	{
		long num = x.AbsoluteTime;
		long num2 = y.AbsoluteTime;
		if (num == num2)
		{
			MetaEvent metaEvent = x as MetaEvent;
			MetaEvent metaEvent2 = y as MetaEvent;
			if (metaEvent != null)
			{
				num = ((metaEvent.MetaEventType != MetaEventType.EndTrack) ? long.MinValue : long.MaxValue);
			}
			if (metaEvent2 != null)
			{
				num2 = ((metaEvent2.MetaEventType != MetaEventType.EndTrack) ? long.MinValue : long.MaxValue);
			}
		}
		return num.CompareTo(num2);
	}
}
