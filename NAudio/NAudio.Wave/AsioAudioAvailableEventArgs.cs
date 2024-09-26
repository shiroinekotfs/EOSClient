using System;
using NAudio.Wave.Asio;

namespace NAudio.Wave;

public class AsioAudioAvailableEventArgs : EventArgs
{
	public IntPtr[] InputBuffers { get; private set; }

	public IntPtr[] OutputBuffers { get; private set; }

	public bool WrittenToOutputBuffers { get; set; }

	public int SamplesPerBuffer { get; private set; }

	public AsioSampleType AsioSampleType { get; private set; }

	public AsioAudioAvailableEventArgs(IntPtr[] inputBuffers, IntPtr[] outputBuffers, int samplesPerBuffer, AsioSampleType asioSampleType)
	{
		InputBuffers = inputBuffers;
		OutputBuffers = outputBuffers;
		SamplesPerBuffer = samplesPerBuffer;
		AsioSampleType = asioSampleType;
	}

	public unsafe int GetAsInterleavedSamples(float[] samples)
	{
		int num = InputBuffers.Length;
		if (samples.Length < SamplesPerBuffer * num)
		{
			throw new ArgumentException("Buffer not big enough");
		}
		int num2 = 0;
		if (AsioSampleType == AsioSampleType.Int32LSB)
		{
			for (int i = 0; i < SamplesPerBuffer; i++)
			{
				for (int j = 0; j < num; j++)
				{
					samples[num2++] = (float)(*(int*)((byte*)(void*)InputBuffers[j] + (nint)i * (nint)4)) / 2.1474836E+09f;
				}
			}
		}
		else if (AsioSampleType == AsioSampleType.Int16LSB)
		{
			for (int k = 0; k < SamplesPerBuffer; k++)
			{
				for (int l = 0; l < num; l++)
				{
					samples[num2++] = (float)(*(short*)((byte*)(void*)InputBuffers[l] + (nint)k * (nint)2)) / 32767f;
				}
			}
		}
		else if (AsioSampleType == AsioSampleType.Int24LSB)
		{
			for (int m = 0; m < SamplesPerBuffer; m++)
			{
				for (int n = 0; n < num; n++)
				{
					byte* ptr = (byte*)(void*)InputBuffers[n] + (nint)m * (nint)3;
					int num3 = *ptr | (ptr[1] << 8) | ((sbyte)ptr[2] << 16);
					samples[num2++] = (float)num3 / 8388608f;
				}
			}
		}
		else
		{
			if (AsioSampleType != AsioSampleType.Float32LSB)
			{
				throw new NotImplementedException($"ASIO Sample Type {AsioSampleType} not supported");
			}
			for (int num4 = 0; num4 < SamplesPerBuffer; num4++)
			{
				for (int num5 = 0; num5 < num; num5++)
				{
					samples[num2++] = *(float*)((byte*)(void*)InputBuffers[num5] + (nint)num4 * (nint)4);
				}
			}
		}
		return SamplesPerBuffer * num;
	}

	[Obsolete("Better performance if you use the overload that takes an array, and reuse the same one")]
	public float[] GetAsInterleavedSamples()
	{
		int num = InputBuffers.Length;
		float[] array = new float[SamplesPerBuffer * num];
		GetAsInterleavedSamples(array);
		return array;
	}
}
