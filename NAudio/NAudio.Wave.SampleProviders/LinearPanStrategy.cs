namespace NAudio.Wave.SampleProviders;

public class LinearPanStrategy : IPanStrategy
{
	public StereoSamplePair GetMultipliers(float pan)
	{
		float num = (0f - pan + 1f) / 2f;
		float left = num;
		float right = 1f - num;
		StereoSamplePair result = default(StereoSamplePair);
		result.Left = left;
		result.Right = right;
		return result;
	}
}
