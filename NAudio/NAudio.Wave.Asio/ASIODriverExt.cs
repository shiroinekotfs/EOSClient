using System;

namespace NAudio.Wave.Asio;

internal class ASIODriverExt
{
	private ASIODriver driver;

	private ASIOCallbacks callbacks;

	private AsioDriverCapability capability;

	private ASIOBufferInfo[] bufferInfos;

	private bool isOutputReadySupported;

	private IntPtr[] currentOutputBuffers;

	private IntPtr[] currentInputBuffers;

	private int numberOfOutputChannels;

	private int numberOfInputChannels;

	private ASIOFillBufferCallback fillBufferCallback;

	private int bufferSize;

	private int outputChannelOffset;

	private int inputChannelOffset;

	public ASIODriver Driver => driver;

	public ASIOFillBufferCallback FillBufferCallback
	{
		get
		{
			return fillBufferCallback;
		}
		set
		{
			fillBufferCallback = value;
		}
	}

	public AsioDriverCapability Capabilities => capability;

	public ASIODriverExt(ASIODriver driver)
	{
		this.driver = driver;
		if (!driver.init(IntPtr.Zero))
		{
			throw new InvalidOperationException(driver.getErrorMessage());
		}
		callbacks = default(ASIOCallbacks);
		callbacks.pasioMessage = AsioMessageCallBack;
		callbacks.pbufferSwitch = BufferSwitchCallBack;
		callbacks.pbufferSwitchTimeInfo = BufferSwitchTimeInfoCallBack;
		callbacks.psampleRateDidChange = SampleRateDidChangeCallBack;
		BuildCapabilities();
	}

	public void SetChannelOffset(int outputChannelOffset, int inputChannelOffset)
	{
		if (outputChannelOffset + numberOfOutputChannels <= Capabilities.NbOutputChannels)
		{
			this.outputChannelOffset = outputChannelOffset;
			if (inputChannelOffset + numberOfInputChannels <= Capabilities.NbInputChannels)
			{
				this.inputChannelOffset = inputChannelOffset;
				return;
			}
			throw new ArgumentException("Invalid channel offset");
		}
		throw new ArgumentException("Invalid channel offset");
	}

	public void Start()
	{
		driver.start();
	}

	public void Stop()
	{
		driver.stop();
	}

	public void ShowControlPanel()
	{
		driver.controlPanel();
	}

	public void ReleaseDriver()
	{
		try
		{
			driver.disposeBuffers();
		}
		catch (Exception ex)
		{
			Console.Out.WriteLine(ex.ToString());
		}
		driver.ReleaseComASIODriver();
	}

	public bool IsSampleRateSupported(double sampleRate)
	{
		return driver.canSampleRate(sampleRate);
	}

	public void SetSampleRate(double sampleRate)
	{
		driver.setSampleRate(sampleRate);
		BuildCapabilities();
	}

	public unsafe int CreateBuffers(int numberOfOutputChannels, int numberOfInputChannels, bool useMaxBufferSize)
	{
		if (numberOfOutputChannels < 0 || numberOfOutputChannels > capability.NbOutputChannels)
		{
			throw new ArgumentException($"Invalid number of channels {numberOfOutputChannels}, must be in the range [0,{capability.NbOutputChannels}]");
		}
		if (numberOfInputChannels < 0 || numberOfInputChannels > capability.NbInputChannels)
		{
			throw new ArgumentException("numberOfInputChannels", $"Invalid number of input channels {numberOfInputChannels}, must be in the range [0,{capability.NbInputChannels}]");
		}
		this.numberOfOutputChannels = numberOfOutputChannels;
		this.numberOfInputChannels = numberOfInputChannels;
		int num = capability.NbInputChannels + capability.NbOutputChannels;
		bufferInfos = new ASIOBufferInfo[num];
		currentOutputBuffers = new IntPtr[numberOfOutputChannels];
		currentInputBuffers = new IntPtr[numberOfInputChannels];
		int num2 = 0;
		int num3 = 0;
		while (num3 < capability.NbInputChannels)
		{
			bufferInfos[num2].isInput = true;
			bufferInfos[num2].channelNum = num3;
			bufferInfos[num2].pBuffer0 = IntPtr.Zero;
			bufferInfos[num2].pBuffer1 = IntPtr.Zero;
			num3++;
			num2++;
		}
		int num4 = 0;
		while (num4 < capability.NbOutputChannels)
		{
			bufferInfos[num2].isInput = false;
			bufferInfos[num2].channelNum = num4;
			bufferInfos[num2].pBuffer0 = IntPtr.Zero;
			bufferInfos[num2].pBuffer1 = IntPtr.Zero;
			num4++;
			num2++;
		}
		if (useMaxBufferSize)
		{
			bufferSize = capability.BufferMaxSize;
		}
		else
		{
			bufferSize = capability.BufferPreferredSize;
		}
		fixed (ASIOBufferInfo* value = &bufferInfos[0])
		{
			IntPtr intPtr = new IntPtr(value);
			driver.createBuffers(intPtr, num, bufferSize, ref callbacks);
		}
		isOutputReadySupported = driver.outputReady() == ASIOError.ASE_OK;
		return bufferSize;
	}

	private void BuildCapabilities()
	{
		capability = new AsioDriverCapability();
		capability.DriverName = driver.getDriverName();
		driver.getChannels(out capability.NbInputChannels, out capability.NbOutputChannels);
		capability.InputChannelInfos = new ASIOChannelInfo[capability.NbInputChannels];
		capability.OutputChannelInfos = new ASIOChannelInfo[capability.NbOutputChannels];
		for (int i = 0; i < capability.NbInputChannels; i++)
		{
			ref ASIOChannelInfo reference = ref capability.InputChannelInfos[i];
			reference = driver.getChannelInfo(i, trueForInputInfo: true);
		}
		for (int j = 0; j < capability.NbOutputChannels; j++)
		{
			ref ASIOChannelInfo reference2 = ref capability.OutputChannelInfos[j];
			reference2 = driver.getChannelInfo(j, trueForInputInfo: false);
		}
		capability.SampleRate = driver.getSampleRate();
		ASIOError latencies = driver.GetLatencies(out capability.InputLatency, out capability.OutputLatency);
		if (latencies != 0 && latencies != ASIOError.ASE_NotPresent)
		{
			ASIOException ex = new ASIOException("ASIOgetLatencies");
			ex.Error = latencies;
			throw ex;
		}
		driver.getBufferSize(out capability.BufferMinSize, out capability.BufferMaxSize, out capability.BufferPreferredSize, out capability.BufferGranularity);
	}

	private void BufferSwitchCallBack(int doubleBufferIndex, bool directProcess)
	{
		for (int i = 0; i < numberOfInputChannels; i++)
		{
			ref IntPtr reference = ref currentInputBuffers[i];
			reference = bufferInfos[i + inputChannelOffset].Buffer(doubleBufferIndex);
		}
		for (int j = 0; j < numberOfOutputChannels; j++)
		{
			ref IntPtr reference2 = ref currentOutputBuffers[j];
			reference2 = bufferInfos[j + outputChannelOffset + capability.NbInputChannels].Buffer(doubleBufferIndex);
		}
		if (fillBufferCallback != null)
		{
			fillBufferCallback(currentInputBuffers, currentOutputBuffers);
		}
		if (isOutputReadySupported)
		{
			driver.outputReady();
		}
	}

	private void SampleRateDidChangeCallBack(double sRate)
	{
		capability.SampleRate = sRate;
	}

	private int AsioMessageCallBack(ASIOMessageSelector selector, int value, IntPtr message, IntPtr opt)
	{
		switch (selector)
		{
		case ASIOMessageSelector.kAsioSelectorSupported:
			switch ((ASIOMessageSelector)Enum.ToObject(typeof(ASIOMessageSelector), value))
			{
			case ASIOMessageSelector.kAsioEngineVersion:
				return 1;
			case ASIOMessageSelector.kAsioResetRequest:
				return 0;
			case ASIOMessageSelector.kAsioBufferSizeChange:
				return 0;
			case ASIOMessageSelector.kAsioResyncRequest:
				return 0;
			case ASIOMessageSelector.kAsioLatenciesChanged:
				return 0;
			case ASIOMessageSelector.kAsioSupportsTimeInfo:
				return 0;
			case ASIOMessageSelector.kAsioSupportsTimeCode:
				return 0;
			}
			break;
		case ASIOMessageSelector.kAsioEngineVersion:
			return 2;
		case ASIOMessageSelector.kAsioResetRequest:
			return 1;
		case ASIOMessageSelector.kAsioBufferSizeChange:
			return 0;
		case ASIOMessageSelector.kAsioResyncRequest:
			return 0;
		case ASIOMessageSelector.kAsioLatenciesChanged:
			return 0;
		case ASIOMessageSelector.kAsioSupportsTimeInfo:
			return 0;
		case ASIOMessageSelector.kAsioSupportsTimeCode:
			return 0;
		}
		return 0;
	}

	private IntPtr BufferSwitchTimeInfoCallBack(IntPtr asioTimeParam, int doubleBufferIndex, bool directProcess)
	{
		return IntPtr.Zero;
	}
}
