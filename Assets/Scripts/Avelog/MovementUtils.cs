using UnityEngine;

namespace Avelog
{
	public static class MovementUtils
	{
		public static void CalculateSpeed(ref float curSpeed, out float curFrameSpeed, float deltaTime, float acceleration, float minSpeed, float maxSpeed)
		{
			if (deltaTime == 0f)
			{
				curFrameSpeed = curSpeed;
				return;
			}
			curSpeed = Mathf.Clamp(curSpeed, minSpeed, maxSpeed);
			float num = curSpeed;
			curSpeed = Mathf.Clamp(curSpeed + acceleration * deltaTime, minSpeed, maxSpeed);
			float num2 = curSpeed - num;
			float num3 = num2 / acceleration;
			float num4 = num + num2 / 2f;
			curFrameSpeed = num4 * num3 / deltaTime + curSpeed * (deltaTime - num3) / deltaTime;
		}
	}
}
