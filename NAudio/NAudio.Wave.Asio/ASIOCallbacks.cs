using System;
using System.Runtime.InteropServices;

namespace NAudio.Wave.Asio;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal struct ASIOCallbacks
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void ASIOBufferSwitchCallBack(int doubleBufferIndex, bool directProcess);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void ASIOSampleRateDidChangeCallBack(double sRate);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate int ASIOAsioMessageCallBack(ASIOMessageSelector selector, int value, IntPtr message, IntPtr opt);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate IntPtr ASIOBufferSwitchTimeInfoCallBack(IntPtr asioTimeParam, int doubleBufferIndex, bool directProcess);

	public ASIOBufferSwitchCallBack pbufferSwitch;

	public ASIOSampleRateDidChangeCallBack psampleRateDidChange;

	public ASIOAsioMessageCallBack pasioMessage;

	public ASIOBufferSwitchTimeInfoCallBack pbufferSwitchTimeInfo;
}
