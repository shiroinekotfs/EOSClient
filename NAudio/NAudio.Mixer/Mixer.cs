using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NAudio.Mixer;

public class Mixer
{
	private MixerInterop.MIXERCAPS caps;

	private IntPtr mixerHandle;

	private MixerFlags mixerHandleType;

	public static int NumberOfDevices => MixerInterop.mixerGetNumDevs();

	public int DestinationCount => (int)caps.cDestinations;

	public string Name => caps.szPname;

	public Manufacturers Manufacturer => (Manufacturers)caps.wMid;

	public int ProductID => caps.wPid;

	public IEnumerable<MixerLine> Destinations
	{
		get
		{
			for (int destination = 0; destination < DestinationCount; destination++)
			{
				yield return GetDestination(destination);
			}
		}
	}

	public static IEnumerable<Mixer> Mixers
	{
		get
		{
			for (int device = 0; device < NumberOfDevices; device++)
			{
				yield return new Mixer(device);
			}
		}
	}

	public Mixer(int mixerIndex)
	{
		if (mixerIndex < 0 || mixerIndex >= NumberOfDevices)
		{
			throw new ArgumentOutOfRangeException("mixerID");
		}
		caps = default(MixerInterop.MIXERCAPS);
		MmException.Try(MixerInterop.mixerGetDevCaps((IntPtr)mixerIndex, ref caps, Marshal.SizeOf(caps)), "mixerGetDevCaps");
		mixerHandle = (IntPtr)mixerIndex;
		mixerHandleType = MixerFlags.Mixer;
	}

	public MixerLine GetDestination(int destinationIndex)
	{
		if (destinationIndex < 0 || destinationIndex >= DestinationCount)
		{
			throw new ArgumentOutOfRangeException("destinationIndex");
		}
		return new MixerLine(mixerHandle, destinationIndex, mixerHandleType);
	}
}
