using System.Runtime.InteropServices;
using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi;

public class PropertyStore
{
	private readonly IPropertyStore storeInterface;

	public int Count
	{
		get
		{
			Marshal.ThrowExceptionForHR(storeInterface.GetCount(out var propCount));
			return propCount;
		}
	}

	public PropertyStoreProperty this[int index]
	{
		get
		{
			PropertyKey key = Get(index);
			Marshal.ThrowExceptionForHR(storeInterface.GetValue(ref key, out var value));
			return new PropertyStoreProperty(key, value);
		}
	}

	public PropertyStoreProperty this[PropertyKey key]
	{
		get
		{
			for (int i = 0; i < Count; i++)
			{
				PropertyKey key2 = Get(i);
				if (key2.formatId == key.formatId && key2.propertyId == key.propertyId)
				{
					Marshal.ThrowExceptionForHR(storeInterface.GetValue(ref key2, out var value));
					return new PropertyStoreProperty(key2, value);
				}
			}
			return null;
		}
	}

	public bool Contains(PropertyKey key)
	{
		for (int i = 0; i < Count; i++)
		{
			PropertyKey propertyKey = Get(i);
			if (propertyKey.formatId == key.formatId && propertyKey.propertyId == key.propertyId)
			{
				return true;
			}
		}
		return false;
	}

	public PropertyKey Get(int index)
	{
		Marshal.ThrowExceptionForHR(storeInterface.GetAt(index, out var key));
		return key;
	}

	public PropVariant GetValue(int index)
	{
		PropertyKey key = Get(index);
		Marshal.ThrowExceptionForHR(storeInterface.GetValue(ref key, out var value));
		return value;
	}

	internal PropertyStore(IPropertyStore store)
	{
		storeInterface = store;
	}
}
