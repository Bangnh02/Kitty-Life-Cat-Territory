using UnityEngine;

public class BossAssistantSpawnPoint : MonoBehaviour
{
	public EnemyController CurrentAssistant
	{
		get;
		set;
	}

	public bool IsBusy => CurrentAssistant != null;

	public Vector3 Position => base.transform.position;

	public Quaternion Rotation => base.transform.rotation;

	private void Start()
	{
		EnemyController.unspawnEvent += OnEnemyUnspawn;
	}

	private void OnDestroy()
	{
		EnemyController.unspawnEvent -= OnEnemyUnspawn;
	}

	private void OnEnemyUnspawn(EnemyController enemyController)
	{
		if (enemyController == CurrentAssistant)
		{
			CurrentAssistant = null;
		}
	}
}
