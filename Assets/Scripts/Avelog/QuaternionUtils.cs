using System;
using UnityEngine;

namespace Avelog
{
	public static class QuaternionUtils
	{
		public static Quaternion GetRotation(Vector3 startDir, Vector3 endDir, float rotAngle)
		{
			if (rotAngle <= 0f)
			{
				return Quaternion.identity;
			}
			startDir.Normalize();
			endDir.Normalize();
			if (Vector3.Dot(startDir, endDir) == 1f)
			{
				return Quaternion.identity;
			}
			if (rotAngle >= 180f)
			{
				return Quaternion.FromToRotation(startDir, endDir);
			}
			Vector3 toDirection = Vector3.RotateTowards(startDir, endDir, rotAngle * ((float)Math.PI / 180f), 0f);
			return Quaternion.FromToRotation(startDir, toDirection);
		}

		public static Quaternion GetRotation(Vector3 startDir, Vector3 endDir, float rotAngle, Vector3 collinearVectorsRotAxis)
		{
			if (rotAngle <= 0f)
			{
				return Quaternion.identity;
			}
			if (rotAngle >= 180f)
			{
				return Quaternion.FromToRotation(startDir, endDir);
			}
			startDir.Normalize();
			endDir.Normalize();
			float num = Vector3.Dot(startDir, endDir);
			Vector3 toDirection;
			if (num == 1f || num == -1f)
			{
				if (num == 1f)
				{
					return Quaternion.identity;
				}
				if (num == -1f)
				{
					rotAngle = Mathf.Clamp(rotAngle, 0f, 180f);
					toDirection = startDir.Rotate(rotAngle, collinearVectorsRotAxis);
					return Quaternion.FromToRotation(startDir, toDirection);
				}
			}
			toDirection = Vector3.RotateTowards(startDir, endDir, rotAngle * ((float)Math.PI / 180f), 0f);
			return Quaternion.FromToRotation(startDir, toDirection);
		}
	}
}
