using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi;

public class PropertyStoreProperty
{
	private readonly PropertyKey propertyKey;

	private PropVariant propertyValue;

	public PropertyKey Key => propertyKey;

	public object Value => propertyValue.Value;

	internal PropertyStoreProperty(PropertyKey key, PropVariant value)
	{
		propertyKey = key;
		propertyValue = value;
	}
}
