using UnityEngine;

namespace Avelog
{
	public static class CalculationsHelpUtils
	{
		public static float CalculateProp(float x2, float y1, float y2)
		{
			if (y2 == 0f)
			{
				UnityEngine.Debug.LogError("Деление на 0");
			}
			return y1 / y2 * x2;
		}

		public static float CalculcatePBF(float value, float pbf, int level)
		{
			level = Mathf.Clamp(level, 1, int.MaxValue);
			float num = pbf * (float)(level - 1);
			return value + value / 100f * num;
		}
	}
}
