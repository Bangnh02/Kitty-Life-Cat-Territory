using System;
using UnityEngine;

[Serializable]
public class FloatPBS
{
	[SerializeField]
	private float baseValue;

	[Tooltip("На сколько процентов повышается базовое значение за каждый дополнительный уровень")]
	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private float incPercent = 10f;

	public float GetValue(int level, int startLevel = 1)
	{
		return baseValue + baseValue / 100f * incPercent * (float)(level - startLevel);
	}
}
