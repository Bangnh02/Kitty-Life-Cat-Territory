using Avelog;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class FarmResidentSpawner : Singleton<FarmResidentSpawner>
{
	public delegate void SpawnHandler(FarmResident farmResident);

	private Transform spawnPoint;

	[SerializeField]
	private Transform spawnedObjsParent;

	[SerializeField]
	private float processingDistance = 200f;

	[SerializeField]
	private float infoPanelProcessingDistance = 230f;

	public List<FarmResidentHabitatArea> Habitats
	{
		get;
		private set;
	}

	public float ProcessingDistance => processingDistance;

	public float InfoPanelProcessingDistance => infoPanelProcessingDistance;

	public List<FarmResident> SpawnedFarmResidents
	{
		get;
		private set;
	} = new List<FarmResident>();


	public bool InitializeSpawnCompleted
	{
		get;
		private set;
	}

	public static event SpawnHandler spawnEvent;

	public event Action initializeSpawnCompletedEvent;

	protected override void OnInit()
	{
		spawnPoint = UnityEngine.Object.FindObjectOfType<FarmResidentSpawnPoint>().transform;
		Habitats = new List<FarmResidentHabitatArea>(UnityEngine.Object.FindObjectsOfType<FarmResidentHabitatArea>());
		Habitats.Sort((FarmResidentHabitatArea x, FarmResidentHabitatArea y) => x.HabitatId.CompareTo(y.HabitatId));
		PlayerSpawner.spawnPlayerEvent += TryInitializeSpawn;
		if (Singleton<EnemySpawner>.Instance != null)
		{
			Singleton<EnemySpawner>.Instance.initializeSpawnCompletedEvent += TryInitializeSpawn;
		}
		TryInitializeSpawn();
	}

	private void OnDestroy()
	{
		PlayerSpawner.spawnPlayerEvent -= TryInitializeSpawn;
		if (Singleton<EnemySpawner>.Instance != null)
		{
			Singleton<EnemySpawner>.Instance.initializeSpawnCompletedEvent -= TryInitializeSpawn;
		}
	}

	private void TryInitializeSpawn()
	{
		if ((Singleton<EnemySpawner>.Instance == null || Singleton<EnemySpawner>.Instance.InitializeSpawnCompleted) && PlayerSpawner.IsPlayerSpawned && !InitializeSpawnCompleted)
		{
			PlayerSpawner.spawnPlayerEvent -= TryInitializeSpawn;
			if (Singleton<EnemySpawner>.Instance != null)
			{
				Singleton<EnemySpawner>.Instance.initializeSpawnCompletedEvent -= TryInitializeSpawn;
			}
			ManagerBase<FarmResidentManager>.Instance.FarmResidentsParams.ForEach(delegate(FarmResidentManager.FarmResidentParams x)
			{
				Spawn(x);
			});
			InitializeSpawnCompleted = true;
			this.initializeSpawnCompletedEvent?.Invoke();
		}
	}

	private void Spawn(FarmResidentManager.FarmResidentParams farmResidentParams)
	{
        bool IsHabitatValid(int id) => Habitats.Any(h => h.HabitatId == id);

        FarmResidentHabitatArea farmResidentHabitatArea = null;
        if (farmResidentParams.FarmResidentData.curHabitatId != -1 && IsHabitatValid(farmResidentParams.FarmResidentData.curHabitatId))
		{
			farmResidentHabitatArea = Habitats.Find((FarmResidentHabitatArea x) => x.AllowedFarmResidents.Contains(farmResidentParams.farmResidentId) && x.ZoneId == farmResidentParams.FarmResidentData.curZoneId && x.HabitatId == farmResidentParams.FarmResidentData.curHabitatId);
		}
		else if (farmResidentParams.farmResidentId == FarmResidentId.Farmer)
		{
			farmResidentHabitatArea = FarmResidentHabitats.Instance.FarmerStartHabitat;
		}
		else if (farmResidentParams.farmResidentId == FarmResidentId.Goat)
		{
			farmResidentHabitatArea = FarmResidentHabitats.Instance.GoatStartHabitat;
		}
		else if (farmResidentParams.farmResidentId == FarmResidentId.Pig)
		{
			farmResidentHabitatArea = FarmResidentHabitats.Instance.PigStartHabitat;
		}
		if (farmResidentHabitatArea == null)
		{
			farmResidentHabitatArea = Habitats.FindAll((FarmResidentHabitatArea x) => x.Inhabitant == null && x.AllowedFarmResidents.Contains(farmResidentParams.farmResidentId)).Random();
		}
		NavMeshPath navMeshPath = new NavMeshPath();
		NavMeshUtils.SamplePositionIterative(spawnPoint.transform.position, out NavMeshHit navMeshHit, 5f, 100f, 5, -1);
		if (float.IsInfinity(navMeshHit.position.x))
		{
			UnityEngine.Debug.LogError("Навмеш не построен либо точка спавна " + spawnPoint.gameObject.name + " находится очень далеко от навмеша");
			return;
		}
		Vector3 zero = Vector3.zero;
		do
		{
			zero = farmResidentHabitatArea.PatrolZones.Random().GetRandomPoint();
			NavMeshUtils.SamplePositionIterative(zero, out NavMeshHit navMeshHit2, 5f, 100f, 5, -1);
			zero = navMeshHit2.position;
			NavMesh.CalculatePath(navMeshHit.position, zero, -1, navMeshPath);
		}
		while (navMeshPath.status != 0);
		FarmResident component = UnityEngine.Object.Instantiate(farmResidentParams.prefab, zero, Quaternion.identity, spawnedObjsParent).GetComponent<FarmResident>();
		component.Spawn(farmResidentParams, farmResidentHabitatArea);
		SpawnedFarmResidents.Add(component);
		FarmResidentSpawner.spawnEvent?.Invoke(component);
	}

	public FarmResidentHabitatArea GetNewHabitat(FarmResidentId farmResidentId)
	{
		ManagerBase<FarmResidentManager>.Instance.ZonesPriorities.Sort((FarmResidentManager.ZonePriority x, FarmResidentManager.ZonePriority y) => x.useCount.CompareTo(y.useCount));
		foreach (FarmResidentManager.ZonePriority zonePriority in ManagerBase<FarmResidentManager>.Instance.ZonesPriorities)
		{
			List<FarmResidentHabitatArea> list = Habitats.FindAll((FarmResidentHabitatArea x) => x.Inhabitant == null && x.ZoneId == zonePriority.zoneId && x.AllowedFarmResidents.Contains(farmResidentId));
			if (list.Count > 0)
			{
				return list.Random();
			}
		}
		return Singleton<FarmResidentSpawner>.Instance.Habitats.FindAll((FarmResidentHabitatArea x) => x.Inhabitant == null && x.AllowedFarmResidents.Contains(farmResidentId)).Random();
	}
}
