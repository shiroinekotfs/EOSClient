using System.IO;

namespace NAudio.SoundFont;

public class InfoChunk
{
	private SFVersion verSoundFont;

	private string waveTableSoundEngine;

	private string bankName;

	private string dataROM;

	private string creationDate;

	private string author;

	private string targetProduct;

	private string copyright;

	private string comments;

	private string tools;

	private SFVersion verROM;

	public SFVersion SoundFontVersion => verSoundFont;

	public string WaveTableSoundEngine
	{
		get
		{
			return waveTableSoundEngine;
		}
		set
		{
			waveTableSoundEngine = value;
		}
	}

	public string BankName
	{
		get
		{
			return bankName;
		}
		set
		{
			bankName = value;
		}
	}

	public string DataROM
	{
		get
		{
			return dataROM;
		}
		set
		{
			dataROM = value;
		}
	}

	public string CreationDate
	{
		get
		{
			return creationDate;
		}
		set
		{
			creationDate = value;
		}
	}

	public string Author
	{
		get
		{
			return author;
		}
		set
		{
			author = value;
		}
	}

	public string TargetProduct
	{
		get
		{
			return targetProduct;
		}
		set
		{
			targetProduct = value;
		}
	}

	public string Copyright
	{
		get
		{
			return copyright;
		}
		set
		{
			copyright = value;
		}
	}

	public string Comments
	{
		get
		{
			return comments;
		}
		set
		{
			comments = value;
		}
	}

	public string Tools
	{
		get
		{
			return tools;
		}
		set
		{
			tools = value;
		}
	}

	public SFVersion ROMVersion
	{
		get
		{
			return verROM;
		}
		set
		{
			verROM = value;
		}
	}

	internal InfoChunk(RiffChunk chunk)
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		if (chunk.ReadChunkID() != "INFO")
		{
			throw new InvalidDataException("Not an INFO chunk");
		}
		RiffChunk nextSubChunk;
		while ((nextSubChunk = chunk.GetNextSubChunk()) != null)
		{
			switch (nextSubChunk.ChunkID)
			{
			case "ifil":
				flag = true;
				verSoundFont = nextSubChunk.GetDataAsStructure(new SFVersionBuilder());
				break;
			case "isng":
				flag2 = true;
				waveTableSoundEngine = nextSubChunk.GetDataAsString();
				break;
			case "INAM":
				flag3 = true;
				bankName = nextSubChunk.GetDataAsString();
				break;
			case "irom":
				dataROM = nextSubChunk.GetDataAsString();
				break;
			case "iver":
				verROM = nextSubChunk.GetDataAsStructure(new SFVersionBuilder());
				break;
			case "ICRD":
				creationDate = nextSubChunk.GetDataAsString();
				break;
			case "IENG":
				author = nextSubChunk.GetDataAsString();
				break;
			case "IPRD":
				targetProduct = nextSubChunk.GetDataAsString();
				break;
			case "ICOP":
				copyright = nextSubChunk.GetDataAsString();
				break;
			case "ICMT":
				comments = nextSubChunk.GetDataAsString();
				break;
			case "ISFT":
				tools = nextSubChunk.GetDataAsString();
				break;
			default:
				throw new InvalidDataException($"Unknown chunk type {nextSubChunk.ChunkID}");
			}
		}
		if (!flag)
		{
			throw new InvalidDataException("Missing SoundFont version information");
		}
		if (!flag2)
		{
			throw new InvalidDataException("Missing wavetable sound engine information");
		}
		if (!flag3)
		{
			throw new InvalidDataException("Missing SoundFont name information");
		}
	}

	public override string ToString()
	{
		return string.Format("Bank Name: {0}\r\nAuthor: {1}\r\nCopyright: {2}\r\nCreation Date: {3}\r\nTools: {4}\r\nComments: {5}\r\nSound Engine: {6}\r\nSoundFont Version: {7}\r\nTarget Product: {8}\r\nData ROM: {9}\r\nROM Version: {10}", BankName, Author, Copyright, CreationDate, Tools, "TODO-fix comments", WaveTableSoundEngine, SoundFontVersion, TargetProduct, DataROM, ROMVersion);
	}
}
