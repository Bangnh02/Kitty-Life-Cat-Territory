using Avelog;
using UnityEngine;
using UnityEngine.AI;

public class NewBehaviourScript4 : MonoBehaviour
{
	[SerializeField]
	private NavMeshAgent navAgent;

	[SerializeField]
	private Transform destination;

	private Vector3 prevDestination;

	private void Start()
	{
		base.transform.parent = PlayerSpawner.PlayerInstance.transform.parent;
		NavMeshUtils.SamplePositionIterative(destination.position, out NavMeshHit navMeshHit, 5f, 100f, 4, -1);
		navAgent.enabled = false;
		base.transform.position = navMeshHit.position;
		navAgent.enabled = true;
		navAgent.Warp(navMeshHit.position);
	}

	private void Update()
	{
		navAgent.acceleration = ManagerBase<PlayerManager>.Instance.SpeedAcceleration;
		NavMeshUtils.SamplePositionIterative(destination.position, out NavMeshHit navMeshHit, 5f, 100f, 4, -1);
		navAgent.SetDestination(navMeshHit.position);
		if (Time.deltaTime != 0f)
		{
			float f = (navAgent.destination - prevDestination).magnitude / Time.deltaTime;
			navAgent.speed = Mathf.Abs(f);
		}
		prevDestination = navAgent.destination;
	}
}
