using System;

namespace NAudio.Wave.Asio;

internal class ASIOSampleConvertor
{
	public delegate void SampleConvertor(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples);

	public static SampleConvertor SelectSampleConvertor(WaveFormat waveFormat, AsioSampleType asioType)
	{
		SampleConvertor result = null;
		bool flag = waveFormat.Channels == 2;
		switch (asioType)
		{
		case AsioSampleType.Int32LSB:
			switch (waveFormat.BitsPerSample)
			{
			case 16:
				result = (flag ? new SampleConvertor(ConvertorShortToInt2Channels) : new SampleConvertor(ConvertorShortToIntGeneric));
				break;
			case 32:
				result = (flag ? new SampleConvertor(ConvertorFloatToInt2Channels) : new SampleConvertor(ConvertorFloatToIntGeneric));
				break;
			}
			break;
		case AsioSampleType.Int16LSB:
			switch (waveFormat.BitsPerSample)
			{
			case 16:
				result = (flag ? new SampleConvertor(ConvertorShortToShort2Channels) : new SampleConvertor(ConvertorShortToShortGeneric));
				break;
			case 32:
				result = (flag ? new SampleConvertor(ConvertorFloatToShort2Channels) : new SampleConvertor(ConvertorFloatToShortGeneric));
				break;
			}
			break;
		case AsioSampleType.Int24LSB:
			switch (waveFormat.BitsPerSample)
			{
			case 16:
				throw new ArgumentException("Not a supported conversion");
			case 32:
				result = ConverterFloatTo24LSBGeneric;
				break;
			}
			break;
		case AsioSampleType.Float32LSB:
			switch (waveFormat.BitsPerSample)
			{
			case 16:
				throw new ArgumentException("Not a supported conversion");
			case 32:
				result = ConverterFloatToFloatGeneric;
				break;
			}
			break;
		default:
			throw new ArgumentException($"ASIO Buffer Type {Enum.GetName(typeof(AsioSampleType), asioType)} is not yet supported.");
		}
		return result;
	}

	public unsafe static void ConvertorShortToInt2Channels(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
	{
		short* ptr = (short*)(void*)inputInterleavedBuffer;
		short* ptr2 = (short*)(void*)asioOutputBuffers[0];
		short* ptr3 = (short*)(void*)asioOutputBuffers[1];
		ptr2++;
		ptr3++;
		for (int i = 0; i < nbSamples; i++)
		{
			*ptr2 = *ptr;
			*ptr3 = ptr[1];
			ptr += 2;
			ptr2 += 2;
			ptr3 += 2;
		}
	}

	public unsafe static void ConvertorShortToIntGeneric(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
	{
		short* ptr = (short*)(void*)inputInterleavedBuffer;
		short*[] array = new short*[nbChannels];
		for (int i = 0; i < nbChannels; i++)
		{
			array[i] = (short*)(void*)asioOutputBuffers[i];
			short*[] array2;
			short*[] array3 = (array2 = array);
			int num = i;
			nint num2 = num;
			array3[num] = array2[num2] + 1;
		}
		for (int j = 0; j < nbSamples; j++)
		{
			for (int k = 0; k < nbChannels; k++)
			{
				*array[k] = *(ptr++);
				short*[] array4;
				short*[] array5 = (array4 = array);
				int num3 = k;
				nint num4 = num3;
				array5[num3] = array4[num4] + 2;
			}
		}
	}

	public unsafe static void ConvertorFloatToInt2Channels(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
	{
		float* ptr = (float*)(void*)inputInterleavedBuffer;
		int* ptr2 = (int*)(void*)asioOutputBuffers[0];
		int* ptr3 = (int*)(void*)asioOutputBuffers[1];
		for (int i = 0; i < nbSamples; i++)
		{
			*(ptr2++) = clampToInt(*ptr);
			*(ptr3++) = clampToInt(ptr[1]);
			ptr += 2;
		}
	}

	public unsafe static void ConvertorFloatToIntGeneric(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
	{
		float* ptr = (float*)(void*)inputInterleavedBuffer;
		int*[] array = new int*[nbChannels];
		for (int i = 0; i < nbChannels; i++)
		{
			array[i] = (int*)(void*)asioOutputBuffers[i];
		}
		for (int j = 0; j < nbSamples; j++)
		{
			for (int k = 0; k < nbChannels; k++)
			{
				int*[] array2;
				int*[] array3 = (array2 = array);
				int num = k;
				nint num2 = num;
				int* ptr2;
				array3[num] = (ptr2 = array2[num2]) + 1;
				*ptr2 = clampToInt(*(ptr++));
			}
		}
	}

	public unsafe static void ConvertorShortToShort2Channels(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
	{
		short* ptr = (short*)(void*)inputInterleavedBuffer;
		short* ptr2 = (short*)(void*)asioOutputBuffers[0];
		short* ptr3 = (short*)(void*)asioOutputBuffers[1];
		for (int i = 0; i < nbSamples; i++)
		{
			*(ptr2++) = *ptr;
			*(ptr3++) = ptr[1];
			ptr += 2;
		}
	}

	public unsafe static void ConvertorShortToShortGeneric(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
	{
		short* ptr = (short*)(void*)inputInterleavedBuffer;
		short*[] array = new short*[nbChannels];
		for (int i = 0; i < nbChannels; i++)
		{
			array[i] = (short*)(void*)asioOutputBuffers[i];
		}
		for (int j = 0; j < nbSamples; j++)
		{
			for (int k = 0; k < nbChannels; k++)
			{
				short*[] array2;
				short*[] array3 = (array2 = array);
				int num = k;
				nint num2 = num;
				short* ptr2;
				array3[num] = (ptr2 = array2[num2]) + 1;
				*ptr2 = *(ptr++);
			}
		}
	}

	public unsafe static void ConvertorFloatToShort2Channels(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
	{
		float* ptr = (float*)(void*)inputInterleavedBuffer;
		short* ptr2 = (short*)(void*)asioOutputBuffers[0];
		short* ptr3 = (short*)(void*)asioOutputBuffers[1];
		for (int i = 0; i < nbSamples; i++)
		{
			*(ptr2++) = clampToShort(*ptr);
			*(ptr3++) = clampToShort(ptr[1]);
			ptr += 2;
		}
	}

	public unsafe static void ConvertorFloatToShortGeneric(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
	{
		float* ptr = (float*)(void*)inputInterleavedBuffer;
		short*[] array = new short*[nbChannels];
		for (int i = 0; i < nbChannels; i++)
		{
			array[i] = (short*)(void*)asioOutputBuffers[i];
		}
		for (int j = 0; j < nbSamples; j++)
		{
			for (int k = 0; k < nbChannels; k++)
			{
				short*[] array2;
				short*[] array3 = (array2 = array);
				int num = k;
				nint num2 = num;
				short* ptr2;
				array3[num] = (ptr2 = array2[num2]) + 1;
				*ptr2 = clampToShort(*(ptr++));
			}
		}
	}

	public unsafe static void ConverterFloatTo24LSBGeneric(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
	{
		float* ptr = (float*)(void*)inputInterleavedBuffer;
		byte*[] array = new byte*[nbChannels];
		for (int i = 0; i < nbChannels; i++)
		{
			array[i] = (byte*)(void*)asioOutputBuffers[i];
		}
		for (int j = 0; j < nbSamples; j++)
		{
			for (int k = 0; k < nbChannels; k++)
			{
				int num = clampTo24Bit(*(ptr++));
				byte*[] array2;
				byte*[] array3 = (array2 = array);
				int num2 = k;
				nint num3 = num2;
				byte* ptr2;
				array3[num2] = (ptr2 = array2[num3]) + 1;
				*ptr2 = (byte)num;
				byte*[] array4;
				byte*[] array5 = (array4 = array);
				int num4 = k;
				nint num5 = num4;
				byte* ptr3;
				array5[num4] = (ptr3 = array4[num5]) + 1;
				*ptr3 = (byte)(num >> 8);
				byte*[] array6;
				byte*[] array7 = (array6 = array);
				int num6 = k;
				nint num7 = num6;
				byte* ptr4;
				array7[num6] = (ptr4 = array6[num7]) + 1;
				*ptr4 = (byte)(num >> 16);
			}
		}
	}

	public unsafe static void ConverterFloatToFloatGeneric(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
	{
		float* ptr = (float*)(void*)inputInterleavedBuffer;
		float*[] array = new float*[nbChannels];
		for (int i = 0; i < nbChannels; i++)
		{
			array[i] = (float*)(void*)asioOutputBuffers[i];
		}
		for (int j = 0; j < nbSamples; j++)
		{
			for (int k = 0; k < nbChannels; k++)
			{
				float*[] array2;
				float*[] array3 = (array2 = array);
				int num = k;
				nint num2 = num;
				float* ptr2;
				array3[num] = (ptr2 = array2[num2]) + 1;
				*ptr2 = *(ptr++);
			}
		}
	}

	private static int clampTo24Bit(double sampleValue)
	{
		sampleValue = ((sampleValue < -1.0) ? (-1.0) : ((sampleValue > 1.0) ? 1.0 : sampleValue));
		return (int)(sampleValue * 8388607.0);
	}

	private static int clampToInt(double sampleValue)
	{
		sampleValue = ((sampleValue < -1.0) ? (-1.0) : ((sampleValue > 1.0) ? 1.0 : sampleValue));
		return (int)(sampleValue * 2147483647.0);
	}

	private static short clampToShort(double sampleValue)
	{
		sampleValue = ((sampleValue < -1.0) ? (-1.0) : ((sampleValue > 1.0) ? 1.0 : sampleValue));
		return (short)(sampleValue * 32767.0);
	}
}
