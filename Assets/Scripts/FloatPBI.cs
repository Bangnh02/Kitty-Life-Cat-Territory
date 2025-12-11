using System;
using UnityEngine;

[Serializable]
public class FloatPBI
{
	[SerializeField]
	private float minimum;

	[SerializeField]
	private float maximum;

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

	public float GetValue(int level, int startLevel = 1)
	{
		return Calculate(level, LimitLevel, minimum, maximum, startLevel);
	}

	public static float Calculate(int level, int LimitLevel, float minimum, float maximum, int startLevel = 1)
	{
		return Mathf.Lerp(minimum, maximum, (float)(level - startLevel) / (float)(LimitLevel - startLevel));
	}
}
