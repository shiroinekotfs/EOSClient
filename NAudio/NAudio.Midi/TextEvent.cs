using System.IO;
using System.Text;
using NAudio.Utils;

namespace NAudio.Midi;

public class TextEvent : MetaEvent
{
	private string text;

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			text = value;
			metaDataLength = text.Length;
		}
	}

	public TextEvent(BinaryReader br, int length)
	{
		Encoding instance = ByteEncoding.Instance;
		text = instance.GetString(br.ReadBytes(length));
	}

	public TextEvent(string text, MetaEventType metaEventType, long absoluteTime)
		: base(metaEventType, text.Length, absoluteTime)
	{
		this.text = text;
	}

	public override string ToString()
	{
		return $"{base.ToString()} {text}";
	}

	public override void Export(ref long absoluteTime, BinaryWriter writer)
	{
		base.Export(ref absoluteTime, writer);
		Encoding instance = ByteEncoding.Instance;
		byte[] bytes = instance.GetBytes(text);
		writer.Write(bytes);
	}
}
