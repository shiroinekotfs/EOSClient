using System;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.Dmo;

public class DmoResampler : IDisposable
{
	private MediaObject mediaObject;

	private IPropertyStore propertyStoreInterface;

	private IWMResamplerProps resamplerPropsInterface;

	private ResamplerMediaComObject mediaComObject;

	public MediaObject MediaObject => mediaObject;

	public DmoResampler()
	{
		mediaComObject = new ResamplerMediaComObject();
		mediaObject = new MediaObject((IMediaObject)mediaComObject);
		propertyStoreInterface = (IPropertyStore)mediaComObject;
		resamplerPropsInterface = (IWMResamplerProps)mediaComObject;
	}

	public void Dispose()
	{
		if (propertyStoreInterface != null)
		{
			Marshal.ReleaseComObject(propertyStoreInterface);
			propertyStoreInterface = null;
		}
		if (resamplerPropsInterface != null)
		{
			Marshal.ReleaseComObject(resamplerPropsInterface);
			resamplerPropsInterface = null;
		}
		if (mediaObject != null)
		{
			mediaObject.Dispose();
			mediaObject = null;
		}
		if (mediaComObject != null)
		{
			Marshal.ReleaseComObject(mediaComObject);
			mediaComObject = null;
		}
	}
}
