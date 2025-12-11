using System;
using UnityEngine;

namespace Avelog
{
	public class VectorUtils
	{
		public enum Vector2Plane
		{
			XY,
			XZ
		}

		public static Vector3 GetVectorFromAngle(float angle, Vector2Plane vectorPlane = Vector2Plane.XZ)
		{
			float f = angle * ((float)Math.PI / 180f);
			return new Vector3((vectorPlane == Vector2Plane.XZ) ? Mathf.Sin(f) : 0f, (vectorPlane == Vector2Plane.XY) ? Mathf.Sin(f) : 0f, Mathf.Cos(f));
		}
	}
}
