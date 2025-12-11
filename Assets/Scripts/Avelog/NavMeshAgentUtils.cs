using System;
using UnityEngine;
using UnityEngine.AI;

namespace Avelog
{
	public class NavMeshAgentUtils
	{
		public static void UpdateVelocity(NavMeshAgent navAgent, ref Vector3 curVelocity, ref float curSpeed, float deltaTime, float rotationSpeed, float corneringSlowingPerc, bool clampSpeedOnCloseDistance = true)
		{
			if (deltaTime == 0f)
			{
				return;
			}
			if (navAgent.acceleration == 0f || rotationSpeed == 0f)
			{
				UnityEngine.Debug.LogError("CalculateVelocity невалидные аргументы");
				return;
			}
			if (navAgent.velocity == Vector3.zero && curSpeed > 0f)
			{
				curSpeed = 0f;
				curVelocity = Vector3.zero;
			}
			Vector3 desiredVelocity = navAgent.desiredVelocity;
			if (curVelocity != desiredVelocity)
			{
				float magnitude = desiredVelocity.magnitude;
				float minSpeed = Mathf.Min(magnitude, curSpeed);
				float maxSpeed = Mathf.Max(magnitude, curSpeed);
				Vector3 vector = (curVelocity != Vector3.zero) ? curVelocity : navAgent.transform.forward.normalized;
				bool flag = false;
				if (desiredVelocity != Vector3.zero)
				{
					vector = Vector3.RotateTowards(vector, desiredVelocity, rotationSpeed * ((float)Math.PI / 180f) * deltaTime, 0f);
					flag = (Vector3.Dot(vector.normalized, desiredVelocity.normalized) == 1f);
				}
				if (clampSpeedOnCloseDistance && vector != Vector3.zero && !flag && navAgent.enabled && !float.IsInfinity(navAgent.remainingDistance) && navAgent.remainingDistance < navAgent.radius)
				{
					Vector3 b = (desiredVelocity - curVelocity).normalized * navAgent.acceleration * Time.deltaTime;
					vector = curVelocity + b;
					curSpeed = Mathf.Clamp(vector.magnitude, 0f, magnitude);
					vector = vector.normalized * curSpeed;
					return;
				}
				float num = Mathf.Sign(magnitude - curSpeed);
				MovementUtils.CalculateSpeed(ref curSpeed, out float curFrameSpeed, deltaTime, navAgent.acceleration * num, minSpeed, maxSpeed);
				Vector3 from = new Vector3(vector.x, 0f, vector.z);
				Vector3 to = new Vector3(desiredVelocity.x, 0f, desiredVelocity.z);
				float t = Mathf.Clamp(Vector3.Angle(from, to) / 180f, 0f, 1f) * (corneringSlowingPerc / 100f) * Time.deltaTime * 22.222f;
				curSpeed = Mathf.Lerp(curSpeed, 0f, t);
				curFrameSpeed = Mathf.Lerp(curFrameSpeed, 0f, t);
				navAgent.velocity = vector.normalized * curFrameSpeed;
				curVelocity = vector.normalized * curSpeed;
			}
			else
			{
				navAgent.velocity = curVelocity;
			}
		}

		public static bool HasPath(NavMeshAgent navAgent)
		{
			if (navAgent.isOnNavMesh || navAgent.isOnOffMeshLink)
			{
				if (!navAgent.pathPending && ((!navAgent.hasPath && !(navAgent.transform.position != navAgent.destination)) || !(navAgent.remainingDistance > navAgent.stoppingDistance)))
				{
					return navAgent.pathStatus == NavMeshPathStatus.PathInvalid;
				}
				return true;
			}
			return false;
		}
	}
}
