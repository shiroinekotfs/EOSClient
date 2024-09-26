using System;
using System.Collections.Generic;
using NAudio.Utils;

namespace NAudio.Wave;

public class MultiplexingWaveProvider : IWaveProvider
{
	private readonly IList<IWaveProvider> inputs;

	private readonly WaveFormat waveFormat;

	private readonly int outputChannelCount;

	private readonly int inputChannelCount;

	private readonly List<int> mappings;

	private readonly int bytesPerSample;

	private byte[] inputBuffer;

	public WaveFormat WaveFormat => waveFormat;

	public int InputChannelCount => inputChannelCount;

	public int OutputChannelCount => outputChannelCount;

	public MultiplexingWaveProvider(IEnumerable<IWaveProvider> inputs, int numberOfOutputChannels)
	{
		this.inputs = new List<IWaveProvider>(inputs);
		outputChannelCount = numberOfOutputChannels;
		if (this.inputs.Count == 0)
		{
			throw new ArgumentException("You must provide at least one input");
		}
		if (numberOfOutputChannels < 1)
		{
			throw new ArgumentException("You must provide at least one output");
		}
		foreach (IWaveProvider input in this.inputs)
		{
			if (waveFormat == null)
			{
				if (input.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
				{
					waveFormat = new WaveFormat(input.WaveFormat.SampleRate, input.WaveFormat.BitsPerSample, numberOfOutputChannels);
				}
				else
				{
					if (input.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
					{
						throw new ArgumentException("Only PCM and 32 bit float are supported");
					}
					waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(input.WaveFormat.SampleRate, numberOfOutputChannels);
				}
			}
			else
			{
				if (input.WaveFormat.BitsPerSample != waveFormat.BitsPerSample)
				{
					throw new ArgumentException("All inputs must have the same bit depth");
				}
				if (input.WaveFormat.SampleRate != waveFormat.SampleRate)
				{
					throw new ArgumentException("All inputs must have the same sample rate");
				}
			}
			inputChannelCount += input.WaveFormat.Channels;
		}
		bytesPerSample = waveFormat.BitsPerSample / 8;
		mappings = new List<int>();
		for (int i = 0; i < outputChannelCount; i++)
		{
			mappings.Add(i % inputChannelCount);
		}
	}

	public int Read(byte[] buffer, int offset, int count)
	{
		int num = bytesPerSample * outputChannelCount;
		int num2 = count / num;
		int num3 = 0;
		int num4 = 0;
		foreach (IWaveProvider input in inputs)
		{
			int num5 = bytesPerSample * input.WaveFormat.Channels;
			int num6 = num2 * num5;
			inputBuffer = BufferHelpers.Ensure(inputBuffer, num6);
			int num7 = input.Read(inputBuffer, 0, num6);
			num4 = Math.Max(num4, num7 / num5);
			for (int i = 0; i < input.WaveFormat.Channels; i++)
			{
				int num8 = num3 + i;
				for (int j = 0; j < outputChannelCount; j++)
				{
					if (mappings[j] != num8)
					{
						continue;
					}
					int num9 = i * bytesPerSample;
					int num10 = offset + j * bytesPerSample;
					int k;
					for (k = 0; k < num2; k++)
					{
						if (num9 >= num7)
						{
							break;
						}
						Array.Copy(inputBuffer, num9, buffer, num10, bytesPerSample);
						num10 += num;
						num9 += num5;
					}
					for (; k < num2; k++)
					{
						Array.Clear(buffer, num10, bytesPerSample);
						num10 += num;
					}
				}
			}
			num3 += input.WaveFormat.Channels;
		}
		return num4 * num;
	}

	public void ConnectInputToOutput(int inputChannel, int outputChannel)
	{
		if (inputChannel < 0 || inputChannel >= InputChannelCount)
		{
			throw new ArgumentException("Invalid input channel");
		}
		if (outputChannel < 0 || outputChannel >= OutputChannelCount)
		{
			throw new ArgumentException("Invalid output channel");
		}
		mappings[outputChannel] = inputChannel;
	}
}
