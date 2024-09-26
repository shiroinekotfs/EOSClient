namespace NAudio.Wave.Compression;

public class AcmFormat
{
	private readonly AcmFormatDetails formatDetails;

	private readonly WaveFormat waveFormat;

	public int FormatIndex => formatDetails.formatIndex;

	public WaveFormatEncoding FormatTag => (WaveFormatEncoding)formatDetails.formatTag;

	public AcmDriverDetailsSupportFlags SupportFlags => formatDetails.supportFlags;

	public WaveFormat WaveFormat => waveFormat;

	public int WaveFormatByteSize => formatDetails.waveFormatByteSize;

	public string FormatDescription => formatDetails.formatDescription;

	internal AcmFormat(AcmFormatDetails formatDetails)
	{
		this.formatDetails = formatDetails;
		waveFormat = WaveFormat.MarshalFromPtr(formatDetails.waveFormatPointer);
	}
}
