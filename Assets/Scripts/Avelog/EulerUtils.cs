using UnityEngine;

namespace Avelog
{
	public static class EulerUtils
	{
		public static Vector3 ToLowestEuler(Vector3 euler)
		{
			return new Vector3(ToLowestAngle(euler.x), ToLowestAngle(euler.y), ToLowestAngle(euler.z));
		}

		public static float ToLowestAngle(float angle)
		{
			angle %= 360f;
			float num = 0f;
			num = ((!(angle > 0f)) ? (360f + angle) : (angle - 360f));
			if (Mathf.Abs(num) < Mathf.Abs(angle))
			{
				return num;
			}
			return angle;
		}
	}
}
