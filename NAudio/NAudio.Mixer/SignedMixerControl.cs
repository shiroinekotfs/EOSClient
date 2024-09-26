using System;
using System.Runtime.InteropServices;

namespace NAudio.Mixer;

public class SignedMixerControl : MixerControl
{
	private MixerInterop.MIXERCONTROLDETAILS_SIGNED signedDetails;

	public int Value
	{
		get
		{
			GetControlDetails();
			return signedDetails.lValue;
		}
		set
		{
			signedDetails.lValue = value;
			mixerControlDetails.paDetails = Marshal.AllocHGlobal(Marshal.SizeOf(signedDetails));
			Marshal.StructureToPtr(signedDetails, mixerControlDetails.paDetails, fDeleteOld: false);
			MmException.Try(MixerInterop.mixerSetControlDetails(mixerHandle, ref mixerControlDetails, mixerHandleType), "mixerSetControlDetails");
			Marshal.FreeHGlobal(mixerControlDetails.paDetails);
		}
	}

	public int MinValue => mixerControl.Bounds.minimum;

	public int MaxValue => mixerControl.Bounds.maximum;

	public double Percent
	{
		get
		{
			return 100.0 * (double)(Value - MinValue) / (double)(MaxValue - MinValue);
		}
		set
		{
			Value = (int)((double)MinValue + value / 100.0 * (double)(MaxValue - MinValue));
		}
	}

	internal SignedMixerControl(MixerInterop.MIXERCONTROL mixerControl, IntPtr mixerHandle, MixerFlags mixerHandleType, int nChannels)
	{
		base.mixerControl = mixerControl;
		base.mixerHandle = mixerHandle;
		base.mixerHandleType = mixerHandleType;
		base.nChannels = nChannels;
		mixerControlDetails = default(MixerInterop.MIXERCONTROLDETAILS);
		GetControlDetails();
	}

	protected override void GetDetails(IntPtr pDetails)
	{
		signedDetails = (MixerInterop.MIXERCONTROLDETAILS_SIGNED)Marshal.PtrToStructure(mixerControlDetails.paDetails, typeof(MixerInterop.MIXERCONTROLDETAILS_SIGNED));
	}

	public override string ToString()
	{
		return $"{base.ToString()} {Percent}%";
	}
}
