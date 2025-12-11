using UnityEngine;

namespace Avelog
{
	public class LoadBalancingUtils
	{
		public static bool ByFrames(int elemIndex, float duration)
		{
			int num = (int)(30f * duration);
			if (num == 0)
			{
				num = 1;
			}
			int num2 = Time.frameCount % num;
			int num3 = elemIndex % num;
			return num2 == num3;
		}
	}
}
