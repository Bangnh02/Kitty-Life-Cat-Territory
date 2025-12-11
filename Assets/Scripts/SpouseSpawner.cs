using Avelog;
using UnityEngine;
using UnityEngine.AI;

public class SpouseSpawner : Singleton<SpouseSpawner>
{
	public delegate void SpawnHandler(PotentialSpouseController potentialSpouse);

	[SerializeField]
	private GameObject spousePrefab;

	[SerializeField]
	private Transform spawnedObjsParent;

	[SerializeField]
	private float processingDistance = 200f;

	[SerializeField]
	private float infoPanelProcessingDistance = 230f;

	private SpouseSpawnPoint spawnPoint;

	private PotentialSpouseController potentialSpouse;

	public float ProcessingDistance => processingDistance;

	public float InfoPanelProcessingDistance => infoPanelProcessingDistance;

	public Vector3 SpawnPoint
	{
		get
		{
			if (spawnPoint == null)
			{
				spawnPoint = Object.FindObjectOfType<SpouseSpawnPoint>();
			}
			return spawnPoint.transform.position;
		}
	}

	public PotentialSpouseController PotentialSpouse => potentialSpouse;

	private FamilyManager FamilyManager => ManagerBase<FamilyManager>.Instance;

	private PlayerManager PlayerManager => ManagerBase<PlayerManager>.Instance;

	public static event SpawnHandler potentialSpouseSpawnEvent;

	protected override void OnInit()
	{
		if (!FamilyManager.HaveSpouse)
		{
			if (PlayerManager.Level < FamilyManager.SpouseLevelNeed)
			{
				PlayerManager.levelChangeEvent += OnLevelChange;
			}
			else
			{
				PotentialSpousSpawn();
			}
		}
	}

	private void OnDestroy()
	{
		PlayerManager.levelChangeEvent -= OnLevelChange;
	}

	private void OnLevelChange()
	{
		if (PlayerManager.Level >= FamilyManager.SpouseLevelNeed)
		{
			PotentialSpousSpawn();
			PlayerManager.levelChangeEvent -= OnLevelChange;
		}
	}

	private void PotentialSpousSpawn()
	{
		NavMeshPath navMeshPath = new NavMeshPath();
		NavMeshUtils.SamplePositionIterative(SpawnPoint, out NavMeshHit navMeshHit, 5f, 100f, 5, -1);
		if (float.IsInfinity(navMeshHit.position.x))
		{
			UnityEngine.Debug.LogError("Навмеш не построен либо точка спавна " + spawnPoint.gameObject.name + " находится очень далеко от навмеша");
			return;
		}
		Vector3 zero = Vector3.zero;
		do
		{
			NavMeshUtils.SamplePositionIterative(SpawnPoint, out NavMeshHit navMeshHit2, 5f, 100f, 5, -1);
			zero = navMeshHit2.position;
			NavMesh.CalculatePath(navMeshHit.position, zero, -1, navMeshPath);
		}
		while (navMeshPath.status != 0);
		potentialSpouse = Object.Instantiate(spousePrefab, zero, Quaternion.identity, spawnedObjsParent).GetComponent<PotentialSpouseController>();
		potentialSpouse.Spawn(SpawnPoint);
		SpouseSpawner.potentialSpouseSpawnEvent?.Invoke(potentialSpouse);
	}
}
