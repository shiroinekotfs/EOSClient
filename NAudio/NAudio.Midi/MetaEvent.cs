using System;
using System.IO;
using System.Text;

namespace NAudio.Midi;

public class MetaEvent : MidiEvent
{
	private MetaEventType metaEvent;

	internal int metaDataLength;

	private byte[] data;

	public MetaEventType MetaEventType => metaEvent;

	protected MetaEvent()
	{
	}

	public MetaEvent(MetaEventType metaEventType, int metaDataLength, long absoluteTime)
		: base(absoluteTime, 1, MidiCommandCode.MetaEvent)
	{
		metaEvent = metaEventType;
		this.metaDataLength = metaDataLength;
	}

	public static MetaEvent ReadMetaEvent(BinaryReader br)
	{
		MetaEventType metaEventType = (MetaEventType)br.ReadByte();
		int num = MidiEvent.ReadVarInt(br);
		MetaEvent metaEvent = new MetaEvent();
		switch (metaEventType)
		{
		case MetaEventType.TrackSequenceNumber:
			metaEvent = new TrackSequenceNumberEvent(br, num);
			break;
		case MetaEventType.TextEvent:
		case MetaEventType.Copyright:
		case MetaEventType.SequenceTrackName:
		case MetaEventType.TrackInstrumentName:
		case MetaEventType.Lyric:
		case MetaEventType.Marker:
		case MetaEventType.CuePoint:
		case MetaEventType.ProgramName:
		case MetaEventType.DeviceName:
			metaEvent = new TextEvent(br, num);
			break;
		case MetaEventType.EndTrack:
			if (num != 0)
			{
				throw new FormatException("End track length");
			}
			break;
		case MetaEventType.SetTempo:
			metaEvent = new TempoEvent(br, num);
			break;
		case MetaEventType.TimeSignature:
			metaEvent = new TimeSignatureEvent(br, num);
			break;
		case MetaEventType.KeySignature:
			metaEvent = new KeySignatureEvent(br, num);
			break;
		case MetaEventType.SequencerSpecific:
			metaEvent = new SequencerSpecificEvent(br, num);
			break;
		case MetaEventType.SmpteOffset:
			metaEvent = new SmpteOffsetEvent(br, num);
			break;
		default:
			metaEvent.data = br.ReadBytes(num);
			if (metaEvent.data.Length != num)
			{
				throw new FormatException("Failed to read metaevent's data fully");
			}
			break;
		}
		metaEvent.metaEvent = metaEventType;
		metaEvent.metaDataLength = num;
		return metaEvent;
	}

	public override string ToString()
	{
		if (data == null)
		{
			return $"{base.AbsoluteTime} {metaEvent}";
		}
		StringBuilder stringBuilder = new StringBuilder();
		byte[] array = data;
		foreach (byte b in array)
		{
			stringBuilder.AppendFormat("{0:X2} ", b);
		}
		return $"{base.AbsoluteTime} {metaEvent}\r\n{stringBuilder.ToString()}";
	}

	public override void Export(ref long absoluteTime, BinaryWriter writer)
	{
		base.Export(ref absoluteTime, writer);
		writer.Write((byte)metaEvent);
		MidiEvent.WriteVarInt(writer, metaDataLength);
		if (data != null)
		{
			writer.Write(data, 0, data.Length);
		}
	}
}
