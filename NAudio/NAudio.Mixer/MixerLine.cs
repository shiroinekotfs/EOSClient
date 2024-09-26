using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NAudio.Mixer;

public class MixerLine
{
	private MixerInterop.MIXERLINE mixerLine;

	private IntPtr mixerHandle;

	private MixerFlags mixerHandleType;

	public string Name => mixerLine.szName;

	public string ShortName => mixerLine.szShortName;

	public int LineId => mixerLine.dwLineID;

	public MixerLineComponentType ComponentType => mixerLine.dwComponentType;

	public string TypeDescription => mixerLine.dwComponentType switch
	{
		MixerLineComponentType.DestinationUndefined => "Undefined Destination", 
		MixerLineComponentType.DestinationDigital => "Digital Destination", 
		MixerLineComponentType.DestinationLine => "Line Level Destination", 
		MixerLineComponentType.DestinationMonitor => "Monitor Destination", 
		MixerLineComponentType.DestinationSpeakers => "Speakers Destination", 
		MixerLineComponentType.DestinationHeadphones => "Headphones Destination", 
		MixerLineComponentType.DestinationTelephone => "Telephone Destination", 
		MixerLineComponentType.DestinationWaveIn => "Wave Input Destination", 
		MixerLineComponentType.DestinationVoiceIn => "Voice Recognition Destination", 
		MixerLineComponentType.SourceUndefined => "Undefined Source", 
		MixerLineComponentType.SourceDigital => "Digital Source", 
		MixerLineComponentType.SourceLine => "Line Level Source", 
		MixerLineComponentType.SourceMicrophone => "Microphone Source", 
		MixerLineComponentType.SourceSynthesizer => "Synthesizer Source", 
		MixerLineComponentType.SourceCompactDisc => "Compact Disk Source", 
		MixerLineComponentType.SourceTelephone => "Telephone Source", 
		MixerLineComponentType.SourcePcSpeaker => "PC Speaker Source", 
		MixerLineComponentType.SourceWaveOut => "Wave Out Source", 
		MixerLineComponentType.SourceAuxiliary => "Auxiliary Source", 
		MixerLineComponentType.SourceAnalog => "Analog Source", 
		_ => "Invalid Component Type", 
	};

	public int Channels => mixerLine.cChannels;

	public int SourceCount => mixerLine.cConnections;

	public int ControlsCount => mixerLine.cControls;

	public bool IsActive => (mixerLine.fdwLine & MixerInterop.MIXERLINE_LINEF.MIXERLINE_LINEF_ACTIVE) != 0;

	public bool IsDisconnected => (mixerLine.fdwLine & MixerInterop.MIXERLINE_LINEF.MIXERLINE_LINEF_DISCONNECTED) != 0;

	public bool IsSource => (mixerLine.fdwLine & MixerInterop.MIXERLINE_LINEF.MIXERLINE_LINEF_SOURCE) != 0;

	public IEnumerable<MixerControl> Controls => MixerControl.GetMixerControls(mixerHandle, this, mixerHandleType);

	public IEnumerable<MixerLine> Sources
	{
		get
		{
			for (int source = 0; source < SourceCount; source++)
			{
				yield return GetSource(source);
			}
		}
	}

	public string TargetName => mixerLine.szPname;

	public MixerLine(IntPtr mixerHandle, int destinationIndex, MixerFlags mixerHandleType)
	{
		this.mixerHandle = mixerHandle;
		this.mixerHandleType = mixerHandleType;
		mixerLine = default(MixerInterop.MIXERLINE);
		mixerLine.cbStruct = Marshal.SizeOf(mixerLine);
		mixerLine.dwDestination = destinationIndex;
		MmException.Try(MixerInterop.mixerGetLineInfo(mixerHandle, ref mixerLine, mixerHandleType), "mixerGetLineInfo");
	}

	public MixerLine(IntPtr mixerHandle, int destinationIndex, int sourceIndex, MixerFlags mixerHandleType)
	{
		this.mixerHandle = mixerHandle;
		this.mixerHandleType = mixerHandleType;
		mixerLine = default(MixerInterop.MIXERLINE);
		mixerLine.cbStruct = Marshal.SizeOf(mixerLine);
		mixerLine.dwDestination = destinationIndex;
		mixerLine.dwSource = sourceIndex;
		MmException.Try(MixerInterop.mixerGetLineInfo(mixerHandle, ref mixerLine, mixerHandleType | MixerFlags.ListText), "mixerGetLineInfo");
	}

	public static int GetMixerIdForWaveIn(int waveInDevice)
	{
		int mixerID = -1;
		MmException.Try(MixerInterop.mixerGetID((IntPtr)waveInDevice, out mixerID, MixerFlags.WaveIn), "mixerGetID");
		return mixerID;
	}

	public MixerLine GetSource(int sourceIndex)
	{
		if (sourceIndex < 0 || sourceIndex >= SourceCount)
		{
			throw new ArgumentOutOfRangeException("sourceIndex");
		}
		return new MixerLine(mixerHandle, mixerLine.dwDestination, sourceIndex, mixerHandleType);
	}

	public override string ToString()
	{
		return $"{Name} {TypeDescription} ({ControlsCount} controls, ID={mixerLine.dwLineID})";
	}
}
