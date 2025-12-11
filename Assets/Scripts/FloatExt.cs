using System;

public static class FloatExt
{
	public static float Cutoff(this float value, int digits)
	{
		return (float)Math.Round(value, digits);
	}

	public static bool NearlyEqual(this float value, float otherValue)
	{
		float num = Math.Abs(value);
		float num2 = Math.Abs(otherValue);
		float num3 = Math.Abs(value - otherValue);
		if (value == otherValue)
		{
			return true;
		}
		if (value == 0f || otherValue == 0f)
		{
			return value == otherValue;
		}
		return num3 / (num + num2) < float.Epsilon;
	}
}
