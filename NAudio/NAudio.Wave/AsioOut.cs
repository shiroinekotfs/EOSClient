using System;
using System.Runtime.CompilerServices;
using System.Threading;
using NAudio.Wave.Asio;

namespace NAudio.Wave;

public class AsioOut : IWavePlayer, IDisposable
{
	private ASIODriverExt driver;

	private IWaveProvider sourceStream;

	private PlaybackState playbackState;

	private int nbSamples;

	private byte[] waveBuffer;

	private ASIOSampleConvertor.SampleConvertor convertor;

	private readonly string driverName;

	private readonly SynchronizationContext syncContext;

	public int PlaybackLatency
	{
		get
		{
			driver.Driver.GetLatencies(out var _, out var outputLatency);
			return outputLatency;
		}
	}

	public PlaybackState PlaybackState => playbackState;

	public string DriverName => driverName;

	public int NumberOfOutputChannels { get; private set; }

	public int NumberOfInputChannels { get; private set; }

	public int DriverInputChannelCount => driver.Capabilities.NbInputChannels;

	public int DriverOutputChannelCount => driver.Capabilities.NbOutputChannels;

	public int ChannelOffset { get; set; }

	public int InputChannelOffset { get; set; }

	[Obsolete("this function will be removed in a future NAudio as ASIO does not support setting the volume on the device")]
	public float Volume
	{
		get
		{
			return 1f;
		}
		set
		{
			if (value != 1f)
			{
				throw new InvalidOperationException("AsioOut does not support setting the device volume");
			}
		}
	}

	public event EventHandler<StoppedEventArgs> PlaybackStopped;

	public event EventHandler<AsioAudioAvailableEventArgs> AudioAvailable;

	public AsioOut()
		: this(0)
	{
	}

	public AsioOut(string driverName)
	{
		syncContext = SynchronizationContext.Current;
		InitFromName(driverName);
	}

	public AsioOut(int driverIndex)
	{
		syncContext = SynchronizationContext.Current;
		string[] driverNames = GetDriverNames();
		if (driverNames.Length == 0)
		{
			throw new ArgumentException("There is no ASIO Driver installed on your system");
		}
		if (driverIndex < 0 || driverIndex > driverNames.Length)
		{
			throw new ArgumentException($"Invalid device number. Must be in the range [0,{driverNames.Length}]");
		}
		driverName = driverNames[driverIndex];
		InitFromName(driverName);
	}

	~AsioOut()
	{
		Dispose();
	}

	public void Dispose()
	{
		if (driver != null)
		{
			if (playbackState != 0)
			{
				driver.Stop();
			}
			driver.ReleaseDriver();
			driver = null;
		}
	}

	public static string[] GetDriverNames()
	{
		return ASIODriver.GetASIODriverNames();
	}

	public static bool isSupported()
	{
		return GetDriverNames().Length > 0;
	}

	private void InitFromName(string driverName)
	{
		ASIODriver aSIODriverByName = ASIODriver.GetASIODriverByName(driverName);
		driver = new ASIODriverExt(aSIODriverByName);
		ChannelOffset = 0;
	}

	public void ShowControlPanel()
	{
		driver.ShowControlPanel();
	}

	public void Play()
	{
		if (playbackState != PlaybackState.Playing)
		{
			playbackState = PlaybackState.Playing;
			driver.Start();
		}
	}

	public void Stop()
	{
		playbackState = PlaybackState.Stopped;
		driver.Stop();
		RaisePlaybackStopped(null);
	}

	public void Pause()
	{
		playbackState = PlaybackState.Paused;
		driver.Stop();
	}

	public void Init(IWaveProvider waveProvider)
	{
		InitRecordAndPlayback(waveProvider, 0, -1);
	}

	public void InitRecordAndPlayback(IWaveProvider waveProvider, int recordChannels, int recordOnlySampleRate)
	{
		if (sourceStream != null)
		{
			throw new InvalidOperationException("Already initialised this instance of AsioOut - dispose and create a new one");
		}
		int num = waveProvider?.WaveFormat.SampleRate ?? recordOnlySampleRate;
		if (waveProvider != null)
		{
			sourceStream = waveProvider;
			NumberOfOutputChannels = waveProvider.WaveFormat.Channels;
			convertor = ASIOSampleConvertor.SelectSampleConvertor(waveProvider.WaveFormat, driver.Capabilities.OutputChannelInfos[0].type);
		}
		else
		{
			NumberOfOutputChannels = 0;
		}
		if (!driver.IsSampleRateSupported(num))
		{
			throw new ArgumentException("SampleRate is not supported");
		}
		if (driver.Capabilities.SampleRate != (double)num)
		{
			driver.SetSampleRate(num);
		}
		driver.FillBufferCallback = driver_BufferUpdate;
		NumberOfInputChannels = recordChannels;
		nbSamples = driver.CreateBuffers(NumberOfOutputChannels, NumberOfInputChannels, useMaxBufferSize: false);
		driver.SetChannelOffset(ChannelOffset, InputChannelOffset);
		if (waveProvider != null)
		{
			waveBuffer = new byte[nbSamples * NumberOfOutputChannels * waveProvider.WaveFormat.BitsPerSample / 8];
		}
	}

	private unsafe void driver_BufferUpdate(IntPtr[] inputChannels, IntPtr[] outputChannels)
	{
		if (NumberOfInputChannels > 0)
		{
			EventHandler<AsioAudioAvailableEventArgs> audioAvailable = this.AudioAvailable;
			if (audioAvailable != null)
			{
				AsioAudioAvailableEventArgs asioAudioAvailableEventArgs = new AsioAudioAvailableEventArgs(inputChannels, outputChannels, nbSamples, driver.Capabilities.InputChannelInfos[0].type);
				audioAvailable(this, asioAudioAvailableEventArgs);
				if (asioAudioAvailableEventArgs.WrittenToOutputBuffers)
				{
					return;
				}
			}
		}
		if (NumberOfOutputChannels > 0)
		{
			int num = sourceStream.Read(waveBuffer, 0, waveBuffer.Length);
			_ = waveBuffer.Length;
			fixed (IntPtr* value = &System.Runtime.CompilerServices.Unsafe.As<byte, IntPtr>(ref waveBuffer[0]))
			{
				convertor(new IntPtr(value), outputChannels, NumberOfOutputChannels, nbSamples);
			}
			if (num == 0)
			{
				Stop();
			}
		}
	}

	private void RaisePlaybackStopped(Exception e)
	{
		EventHandler<StoppedEventArgs> handler = this.PlaybackStopped;
		if (handler == null)
		{
			return;
		}
		if (syncContext == null)
		{
			handler(this, new StoppedEventArgs(e));
			return;
		}
		syncContext.Post(delegate
		{
			handler(this, new StoppedEventArgs(e));
		}, null);
	}

	public string AsioInputChannelName(int channel)
	{
		if (channel <= DriverInputChannelCount)
		{
			return driver.Capabilities.InputChannelInfos[channel].name;
		}
		return "";
	}

	public string AsioOutputChannelName(int channel)
	{
		if (channel <= DriverOutputChannelCount)
		{
			return driver.Capabilities.OutputChannelInfos[channel].name;
		}
		return "";
	}
}
