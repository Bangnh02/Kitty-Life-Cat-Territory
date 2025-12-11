using Avelog;
using UnityEngine;
using UnityEngine.AI;

public class NewBehaviourScript2 : MonoBehaviour
{
	[SerializeField]
	private Vector3 spawnPosition;

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.B))
		{
			NavMeshHit navMeshHit;
			bool flag = NavMeshUtils.SamplePositionIterative(base.transform.position, out navMeshHit, 5f, 100f, 10, -1);
			UnityEngine.Debug.Log($"Nav mesh hit = {flag}, navMeshHit.position {navMeshHit.position}");
		}
	}
}
