using System;
using UnityEngine;

public static class Vector3Ext
{
	public static bool IsShorter(this Vector3 vector, float value)
	{
		return vector.sqrMagnitude < value * value;
	}

	public static bool IsLonger(this Vector3 vector, float value)
	{
		return vector.sqrMagnitude > value * value;
	}

	public static bool IsShorterOrEqual(this Vector3 vector, float value)
	{
		return vector.sqrMagnitude <= value * value;
	}

	public static bool IsLongerOrEqual(this Vector3 vector, float value)
	{
		return vector.sqrMagnitude >= value * value;
	}

	public static Vector3 Rotate(this Vector3 vector, float angle, Vector3 axis)
	{
		float num = Mathf.Cos((float)Math.PI / 180f * angle);
		float num2 = Mathf.Sin((float)Math.PI / 180f * angle);
		float x = axis.x;
		float y = axis.y;
		float z = axis.z;
		float[,] array = new float[3, 3]
		{
			{
				num + (1f - num) * x * x,
				(1f - num) * x * y - num2 * z,
				(1f - num) * x * z + num2 * y
			},
			{
				(1f - num) * y * x + num2 * z,
				num + (1f - num) * y * y,
				(1f - num) * y * z - num2 * x
			},
			{
				(1f - num) * z * x - num2 * y,
				(1f - num) * z * y + num2 * x,
				num + (1f - num) * z * z
			}
		};
		Vector3 zero = Vector3.zero;
		zero.x = array[0, 0] * vector.x + array[0, 1] * vector.y + array[0, 2] * vector.z;
		zero.y = array[1, 0] * vector.x + array[1, 1] * vector.y + array[1, 2] * vector.z;
		zero.z = array[2, 0] * vector.x + array[2, 1] * vector.y + array[2, 2] * vector.z;
		return zero;
	}

	public static Vector3 ProjectOnLineSegment(this Vector3 sourcePosition, Vector3 lineSegmentPoint1, Vector3 lineSegmentPoint2)
	{
		Vector3 onNormal = lineSegmentPoint2 - lineSegmentPoint1;
		Vector3 b = lineSegmentPoint1 - Vector3.Project(lineSegmentPoint1, onNormal);
		Vector3 vector = Vector3.Project(sourcePosition, onNormal) + b;
		if (vector.x < Mathf.Min(lineSegmentPoint1.x, lineSegmentPoint2.x) || vector.x > Mathf.Max(lineSegmentPoint1.x, lineSegmentPoint2.x) || vector.z < Mathf.Min(lineSegmentPoint1.z, lineSegmentPoint2.z) || vector.z > Mathf.Max(lineSegmentPoint1.z, lineSegmentPoint2.z) || vector.y < Mathf.Min(lineSegmentPoint1.y, lineSegmentPoint2.y) || vector.y > Mathf.Max(lineSegmentPoint1.y, lineSegmentPoint2.y))
		{
			float sqrMagnitude = (lineSegmentPoint1 - sourcePosition).sqrMagnitude;
			float sqrMagnitude2 = (lineSegmentPoint2 - sourcePosition).sqrMagnitude;
			if (sqrMagnitude < sqrMagnitude2)
			{
				return lineSegmentPoint1;
			}
			return lineSegmentPoint2;
		}
		return vector;
	}
}
