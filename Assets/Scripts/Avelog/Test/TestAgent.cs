using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Avelog.Test
{
	public class TestAgent : MonoBehaviour
	{
		[SerializeField]
		private bool move = true;

		[SerializeField]
		private bool customUpdateVelocity = true;

		[SerializeField]
		private bool correctSpeed = true;

		[SerializeField]
		private float speed = 15f;

		[SerializeField]
		[ReadonlyInspector]
		private float curSpeedMulty;

		private Vector3 curVelocity = Vector3.zero;

		[SerializeField]
		[ReadonlyInspector]
		private float curSpeed;

		[SerializeField]
		private AnimationCurve corneringCurve;

		[SerializeField]
		private bool usePow = true;

		[SerializeField]
		private float pow = 0.5f;

		[SerializeField]
		private float minSpeedPart = 0.2f;

		[SerializeField]
		private float acceleration = 25f;

		[SerializeField]
		private float corneringSlowing = 10f;

		[SerializeField]
		private int fps = 60;

		[SerializeField]
		private NavMeshAgent navAgent;

		[SerializeField]
		private GameObject waypointsParent;

		private List<GameObject> waypoints;

		private GameObject curWaypoint;

		private void Start()
		{
			waypoints = new List<GameObject>();
			for (int i = 0; i < waypointsParent.transform.childCount; i++)
			{
				waypoints.Add(waypointsParent.transform.GetChild(i).gameObject);
			}
			NavMeshUtils.SamplePositionIterative(base.transform.position, out NavMeshHit navMeshHit, 5f, 100f, 10, -1);
			navAgent.Warp(navMeshHit.position);
		}

		private void Update()
		{
			navAgent.isStopped = !move;
			Application.targetFrameRate = fps;
			if (!HasPath())
			{
				int num = 0;
				if (curWaypoint != null)
				{
					num = waypoints.IndexOf(curWaypoint);
				}
				int num2 = num + 1;
				if (num2 >= waypoints.Count)
				{
					num2 = 0;
				}
				curWaypoint = waypoints[num2];
				NavMeshUtils.SamplePositionIterative(curWaypoint.transform.position, out NavMeshHit navMeshHit, 5f, 100f, 10, -1);
				NavMeshPath path = new NavMeshPath();
				navAgent.CalculatePath(navMeshHit.position, path);
				navAgent.SetPath(path);
			}
			if (correctSpeed)
			{
				float num3 = Mathf.Clamp(Vector3.Angle(base.transform.forward, navAgent.desiredVelocity) / 180f, 0f, 1f);
				if (usePow)
				{
					curSpeedMulty = Mathf.Lerp(1f, minSpeedPart, Mathf.Pow(num3, pow));
				}
				else
				{
					curSpeedMulty = Mathf.Lerp(1f, minSpeedPart, corneringCurve.Evaluate(num3));
				}
				navAgent.speed = curSpeedMulty * speed;
				navAgent.acceleration = acceleration + (1f - curSpeedMulty) * acceleration;
			}
			else
			{
				navAgent.speed = speed;
				navAgent.acceleration = acceleration;
			}
			if (customUpdateVelocity)
			{
				NavMeshAgentUtils.UpdateVelocity(navAgent, ref curVelocity, ref curSpeed, Time.deltaTime, navAgent.angularSpeed, corneringSlowing);
				return;
			}
			curSpeed = navAgent.velocity.magnitude;
			curVelocity = navAgent.velocity;
		}

		private bool HasPath()
		{
			return NavMeshAgentUtils.HasPath(navAgent);
		}
	}
}
