using UnityEngine;

namespace Avelog
{
	public static class GeometryUtils
	{
		public static Vector3 GetRandomPointInBox(Vector3 boxPosition, Quaternion boxRotation, Vector3 boxSize)
		{
			float x = Random.Range((0f - boxSize.x) / 2f, boxSize.x / 2f);
			float y = Random.Range((0f - boxSize.y) / 2f, boxSize.y / 2f);
			float z = Random.Range((0f - boxSize.z) / 2f, boxSize.z / 2f);
			Vector3 point = new Vector3(x, y, z);
			point = boxRotation * point;
			return point + boxPosition;
		}

		public static bool IsInBox(Vector3 sourcePosition, Vector3 boxPosition, Quaternion boxRotation, Vector3 boxSize)
		{
			return new Bounds(boxPosition, boxSize).Contains(boxRotation * sourcePosition);
		}
	}
}
