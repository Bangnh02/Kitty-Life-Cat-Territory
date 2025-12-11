using Avelog.Spawn;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSpawner : Singleton<ItemSpawner>
{
	[Serializable]
	public class SpawnItem
	{
		public ItemId itemId;

		public GameObject itemPrefab;

		private List<SpawnPoint> spawnPoints;

		public int maxCountItems;

		public float spawnInterval;

		[ReadonlyInspector]
		public float spawnCooldown;

		[HideInInspector]
		public List<GameObject> nonSpawnedObjects = new List<GameObject>();

		public List<SpawnPoint> SpawnPoints
		{
			get
			{
				if (spawnPoints == null)
				{
					spawnPoints = new List<SpawnPoint>(from x in UnityEngine.Object.FindObjectsOfType<SpawnPoint>()
						where x.ItemId == itemId
						select x);
				}
				return spawnPoints;
			}
		}

		public int CurCountItems => SpawnPoints.Count((SpawnPoint x) => x.IsBusy);
	}

	public class EnemySpawnItem
	{
		public ItemId itemId;

		public List<GameObject> nonSpawnedObjects = new List<GameObject>();

		public List<GameObject> spawnedObjects = new List<GameObject>();

		public EnemySpawnItem(ItemId itemId)
		{
			this.itemId = itemId;
		}
	}

	public delegate void FoodSpawnHandler(Food food);

	[Header("Параметры распределения")]
	[SerializeField]
	private float spawnDistanceToPlayerMinimum = 50f;

	[Tooltip("Кол-во соседних точек (к какой либо), которые будут учитываться при определении занятости точки спавна")]
	[SerializeField]
	private int nearSpawnPointsCount = 5;

	[Header("Веса для алгоритма выбора спавн точки")]
	[Tooltip("Влияние от времени последнего использования на момент, когда время последнего использования у всех свободных точек одинаковое")]
	[Range(0f, 1f)]
	[SerializeField]
	private float startTimeWeight = 1f;

	[Tooltip("Влияние от времени последнего использования")]
	[Range(0f, 1f)]
	[SerializeField]
	private float timeWeight = 1f;

	[Tooltip("Влияние равномерного распределения")]
	[Range(0f, 1f)]
	[SerializeField]
	private float occupancyWeight = 1f;

	[Header("Прочее")]
	[SerializeField]
	private float processingDistance = 150f;

	[SerializeField]
	private float infoPanelProcessingDistance = 180f;

	[SerializeField]
	private GameObject enemyFoodPrefab;

	[SerializeField]
	private Vector3 pickedItemPanelOffset;

	[SerializeField]
	private List<SpawnItem> spawnItems;

	private EnemySpawnItem enemySpawnItem;

	private SpawnDistributor<SpawnPoint> spawnDistributor;

	public float SpawnDistanceToPlayerMinimum => spawnDistanceToPlayerMinimum;

	public int NearSpawnPointsCount => nearSpawnPointsCount;

	public float ProcessingDistance => processingDistance;

	public float InfoPanelProcessingDistance => infoPanelProcessingDistance;

	public Vector3 PickedItemPanelOffset => pickedItemPanelOffset;

	public List<SpawnItem> SpawnItems => spawnItems;

	public EnemySpawnItem EnemySpawnItems => enemySpawnItem;

	public bool InitializeSpawnCompleted
	{
		get;
		private set;
	}

	public static event FoodSpawnHandler foodSpawnEvent;

	public event Action initializeSpawnCompletedEvent;

	protected override void OnInit()
	{
		if (PlayerSpawner.IsPlayerSpawned)
		{
			OnSpawnPlayer();
		}
		PlayerSpawner.spawnPlayerEvent += OnSpawnPlayer;
	}

	private void OnDestroy()
	{
		PlayerSpawner.spawnPlayerEvent -= OnSpawnPlayer;
	}

	private void OnSpawnPlayer()
	{
		spawnDistributor = new SpawnDistributor<SpawnPoint>(UnityEngine.Object.FindObjectsOfType<SpawnPoint>().ToList(), PlayerSpawner.PlayerInstance.gameObject, ref SpawnPoint.occupyEvent, ref SpawnPoint.freeEvent, NearSpawnPointsCount, SpawnDistanceToPlayerMinimum, timeWeight, occupancyWeight, startTimeWeight);
		foreach (SpawnItem spawnItem in spawnItems)
		{
			Spawn(spawnItem);
		}
		InitializeSpawnCompleted = true;
		this.initializeSpawnCompletedEvent?.Invoke();
	}

	private void Update()
	{
		foreach (SpawnItem spawnItem in spawnItems)
		{
			if (spawnItem.spawnCooldown >= spawnItem.spawnInterval)
			{
				if (spawnItem.CurCountItems < spawnItem.maxCountItems)
				{
					Spawn(spawnItem);
				}
				spawnItem.spawnCooldown = 0f;
			}
			else
			{
				spawnItem.spawnCooldown += Time.deltaTime;
			}
		}
	}

	private SpawnPoint GetRandomMaxPriorityEmptyPoint(List<SpawnPoint> emptyPoints)
	{
		int maxPriority = emptyPoints.Max((SpawnPoint x) => x.CurPriority);
		emptyPoints = emptyPoints.FindAll((SpawnPoint x) => x.CurPriority == maxPriority);
		return emptyPoints.Random();
	}

	private void Spawn(SpawnItem item)
	{
		while (item.CurCountItems < item.maxCountItems)
		{
			SpawnPoint spawnPoint = spawnDistributor.SelectSpawnPoint((SpawnPoint x) => x.ItemId == item.itemId);
			if (spawnPoint == null)
			{
				break;
			}
			GameObject gameObject;
			if (item.nonSpawnedObjects.Count == 0)
			{
				gameObject = UnityEngine.Object.Instantiate(item.itemPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation, spawnPoint.transform);
			}
			else
			{
				gameObject = item.nonSpawnedObjects[0];
				ActivateNonSpawnedObject(gameObject, spawnPoint);
				item.nonSpawnedObjects.Remove(gameObject);
			}
			spawnPoint.IsBusy = true;
			Food component = gameObject.GetComponent<Food>();
			ItemSpawner.foodSpawnEvent?.Invoke(component);
		}
	}

	public void SpawnEnemyItem(EnemyArchetype enemyArchetype, bool edible, float scaleMod, Vector3 spawnPosition, Mesh mesh, Material material, Quaternion rotation)
	{
		GameObject gameObject;
		if (enemySpawnItem != null)
		{
			if (enemySpawnItem.nonSpawnedObjects.Count != 0)
			{
				gameObject = enemySpawnItem.nonSpawnedObjects[0];
				gameObject.transform.position = spawnPosition;
				gameObject.transform.rotation = rotation;
				gameObject.transform.parent = base.transform;
				gameObject.SetActive(value: true);
				enemySpawnItem.nonSpawnedObjects.Remove(gameObject);
			}
			else
			{
				gameObject = UnityEngine.Object.Instantiate(enemyFoodPrefab, spawnPosition, rotation, base.transform);
			}
		}
		else
		{
			enemySpawnItem = new EnemySpawnItem(ItemId.Enemy);
			gameObject = UnityEngine.Object.Instantiate(enemyFoodPrefab, spawnPosition, rotation, base.transform);
		}
		enemySpawnItem.spawnedObjects.Add(gameObject);
		EnemyFood component = gameObject.GetComponent<EnemyFood>();
		component.Setup(enemyArchetype, edible, scaleMod);
		component.MeshFilter.mesh = mesh;
		component.MeshRenderer.material = material;
		component.ShadowsMeshFilter.mesh = mesh;
		component.ShadowsMeshRenderer.material = material;
		component.Id = ItemId.Enemy;
		ItemSpawner.foodSpawnEvent?.Invoke(component);
	}

	private void ActivateNonSpawnedObject(GameObject obj, SpawnPoint point)
	{
		obj.transform.position = point.transform.position;
		obj.transform.rotation = point.transform.rotation;
		obj.transform.parent = point.transform;
		obj.SetActive(value: true);
	}

	public void AddNonSpawnedItem(GameObject obj, ItemId type)
	{
		spawnItems.Find((SpawnItem x) => x.itemId == type).nonSpawnedObjects.Add(obj);
	}

	public void AddNonSpawnedEnemyItem(GameObject obj, ItemId type)
	{
		enemySpawnItem.spawnedObjects.Remove(obj);
		enemySpawnItem.nonSpawnedObjects.Add(obj);
	}
}
