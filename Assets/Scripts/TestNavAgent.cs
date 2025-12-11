using Avelog;
using UnityEngine;
using UnityEngine.AI;

public class TestNavAgent : MonoBehaviour
{
	[SerializeField]
	private GameObject point;

	[SerializeField]
	private NavMeshAgent agent;

	private Vector3 velocity = Vector3.zero;

	private float curSpeed;

	private void Start()
	{
		NavMesh.SamplePosition(point.transform.position, out NavMeshHit hit, 10f, -1);
		agent.SetDestination(hit.position);
	}

	private void Update()
	{
		NavMeshAgentUtils.UpdateVelocity(agent, ref velocity, ref curSpeed, Time.deltaTime, agent.angularSpeed, 20f);
	}
}
