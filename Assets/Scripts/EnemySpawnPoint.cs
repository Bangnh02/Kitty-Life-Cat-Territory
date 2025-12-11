using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
	[SerializeField]
	private float maxDistanceToPlayer = 100f;

	public bool CanSpawn()
	{
		return (base.transform.position - PlayerSpawner.PlayerInstance.transform.position).IsLonger(maxDistanceToPlayer);
	}

	private void OnDrawGizmos()
	{
	}
}
