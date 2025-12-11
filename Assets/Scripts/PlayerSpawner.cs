using Avelog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : Singleton<PlayerSpawner>
{
	public delegate void SpawnPlayerHandler();

	public delegate void BlockHandler(float duration);

	[SerializeField]
	private GameObject playerPrefab;

	private GameObject spawnPoint;

	[SerializeField]
	private Transform spawnedObjsParent;

	[SerializeField]
	private float waitBeforeBlocking = 2f;

	[SerializeField]
	private float blockingDuration = 0.18f;

	[SerializeField]
	private float unblockingDuration = 1.5f;

	public Transform SpawnedObjsParent => spawnedObjsParent;

	public static bool IsPlayerSpawned => PlayerInstance != null;

	public static PlayerBrain PlayerInstance
	{
		get;
		private set;
	}

	public bool IsRespawing
	{
		get;
		private set;
	}

	public static event SpawnPlayerHandler spawnPlayerEvent;

	public static event SpawnPlayerHandler respawnPlayerEvent;

	public static event SpawnPlayerHandler beforeRespawnPlayerEvent;

	public static event BlockHandler startBlockingGameEvent;

	public static event BlockHandler startUnblockingGameEvent;

	protected override void OnInit()
	{
		spawnPoint = Object.FindObjectOfType<PlayerSpawnPoint>().gameObject;
		Spawn();
	}

	private void Spawn()
	{
		if (!IsPlayerSpawned)
		{
			float maxDistance = 1000f;
			Physics.Raycast(spawnPoint.transform.position, Vector3.down, out RaycastHit hitInfo, maxDistance, 1 << Layers.ColliderLayer);
			PlayerInstance = Object.Instantiate(playerPrefab, hitInfo.point, spawnPoint.transform.rotation, spawnedObjsParent).GetComponent<PlayerBrain>();
			new List<IInitializablePlayerComponent>(PlayerInstance.GetComponentsInChildren<IInitializablePlayerComponent>(includeInactive: true)).ForEach(delegate(IInitializablePlayerComponent x)
			{
				x.Initialize();
			});
			PlayerInstance.PlayerCombat.changeLifeStateEvent += OnPlayerChangeLifeState;
			PlayerSpawner.spawnPlayerEvent?.Invoke();
		}
	}

	private void OnDestroy()
	{
	}

	private void OnPlayerChangeLifeState(ActorCombat.LifeState state)
	{
		if (state == ActorCombat.LifeState.Dead)
		{
			StartCoroutine(Respawn());
		}
	}

	private IEnumerator Respawn()
	{
		IsRespawing = true;
		PlayerInstance.PlayerPicker.Drop();
		yield return new WaitForSecondsRealtime(waitBeforeBlocking);
		PlayerSpawner.startBlockingGameEvent?.Invoke(blockingDuration);
		yield return new WaitForSecondsRealtime(blockingDuration);
		PlayerSpawner.beforeRespawnPlayerEvent?.Invoke();
		float maxDistance = 1000f;
		Physics.Raycast(spawnPoint.transform.position, Vector3.down, out RaycastHit hitInfo, maxDistance, 1 << Layers.ColliderLayer);
		PlayerInstance.PlayerMovement.Teleport(hitInfo.point, spawnPoint.transform.rotation);
		PlayerInstance.PlayerEating.Respawn();
		PlayerInstance.PlayerPicker.Respawn();
		PlayerInstance.PlayerCombat.Respawn();
		PlayerSpawner.respawnPlayerEvent?.Invoke();
		ManagerBase<SaveManager>.Instance.SaveToLocal();
		PlayerSpawner.startUnblockingGameEvent?.Invoke(unblockingDuration);
		yield return new WaitForSecondsRealtime(unblockingDuration);
		IsRespawing = false;
		
	}
}
