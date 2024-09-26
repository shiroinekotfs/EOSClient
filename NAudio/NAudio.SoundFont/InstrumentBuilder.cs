using System;
using System.IO;
using System.Text;

namespace NAudio.SoundFont;

internal class InstrumentBuilder : StructureBuilder<Instrument>
{
	private Instrument lastInstrument;

	public override int Length => 22;

	public Instrument[] Instruments => data.ToArray();

	public override Instrument Read(BinaryReader br)
	{
		Instrument instrument = new Instrument();
		string text = Encoding.UTF8.GetString(br.ReadBytes(20), 0, 20);
		if (text.IndexOf('\0') >= 0)
		{
			text = text.Substring(0, text.IndexOf('\0'));
		}
		instrument.Name = text;
		instrument.startInstrumentZoneIndex = br.ReadUInt16();
		if (lastInstrument != null)
		{
			lastInstrument.endInstrumentZoneIndex = (ushort)(instrument.startInstrumentZoneIndex - 1);
		}
		data.Add(instrument);
		lastInstrument = instrument;
		return instrument;
	}

	public override void Write(BinaryWriter bw, Instrument instrument)
	{
	}

	public void LoadZones(Zone[] zones)
	{
		for (int i = 0; i < data.Count - 1; i++)
		{
			Instrument instrument = data[i];
			instrument.Zones = new Zone[instrument.endInstrumentZoneIndex - instrument.startInstrumentZoneIndex + 1];
			Array.Copy(zones, instrument.startInstrumentZoneIndex, instrument.Zones, 0, instrument.Zones.Length);
		}
		data.RemoveAt(data.Count - 1);
	}
}
