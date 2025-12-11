using System;
using UnityEngine;

[Serializable]
public class IntPBI
{
	[SerializeField]
	private int minimum;

	[SerializeField]
	private int maximum;

	private int LimitLevel
	{
		get
		{
			if (!(Managers.Instance != null))
			{
				return 0;
			}
			return Managers.Instance.PBILimitLevel;
		}
	}

	public int GetValue(int level, int startLevel = 1)
	{
		return Calculate(level, LimitLevel, minimum, maximum, startLevel);
	}

	public static int Calculate(int level, int LimitLevel, float minimum, float maximum, int startLevel = 1)
	{
		float num = Mathf.InverseLerp(startLevel, LimitLevel, level);
		int num2 = Mathf.Abs(Mathf.RoundToInt(maximum) - Mathf.RoundToInt(minimum)) + 1;
		int num3 = (int)Mathf.Sign(maximum - minimum);
		float num4 = 100f / (float)num2 / 100f;
		int value = (int)(num / num4);
		value = Mathf.Clamp(value, 0, num2 - 1);
		return (int)minimum + value * num3;
	}
}
