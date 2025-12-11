using UnityEngine;
using UnityEngine.AI;

namespace Avelog
{
	public static class NavMeshUtils
	{
		public static bool SamplePositionIterative(Vector3 sourcePosition, out NavMeshHit navMeshHit, float minDistance, float maxDistance, int maxIterations, int areaMask)
		{
			navMeshHit = default(NavMeshHit);
			if (minDistance < 0f || maxDistance < 0f || maxIterations == 0 || areaMask == 0)
			{
				UnityEngine.Debug.LogError("SamplePositionIterative(). Invalid args");
				return false;
			}
			float num = 0f;
			if (maxIterations > 1)
			{
				num = (maxDistance - minDistance) / (float)(maxIterations - 1);
			}
			for (int i = 0; i < maxIterations; i++)
			{
				float maxDistance2 = minDistance + num * (float)i;
				if (NavMesh.SamplePosition(sourcePosition, out navMeshHit, maxDistance2, areaMask))
				{
					return true;
				}
			}
			return false;
		}

		public static bool SamplePositionIterative(Vector3 sourcePosition, out NavMeshHit navMeshHit, float minDistance, float maxDistance, int maxIterations, NavMeshQueryFilter filter)
		{
			navMeshHit = default(NavMeshHit);
			if (minDistance < 0f || maxDistance < 0f || maxIterations == 0 || filter.areaMask == 0)
			{
				UnityEngine.Debug.LogError("SamplePositionIterative(). Invalid args");
				return false;
			}
			float num = (maxDistance - minDistance) / (float)(maxIterations - 1);
			for (int i = 0; i < maxIterations; i++)
			{
				float maxDistance2 = minDistance + num * (float)i;
				if (NavMesh.SamplePosition(sourcePosition, out navMeshHit, maxDistance2, filter))
				{
					return true;
				}
			}
			return false;
		}
	}
}
