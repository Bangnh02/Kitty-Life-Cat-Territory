using Avelog;
using Avelog.Spawn;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoinSpawner : Singleton<CoinSpawner>
{
	[Serializable]
	private class SpawnPointDistance
	{
		public CoinSpawnPoint spawnPoint;

		public float sqrDistance;
	}

	[Serializable]
	private class SpawnPointRelativeDistances
	{
		public CoinSpawnPoint spawnPoint;

		public List<SpawnPointDistance> nearSpawnPoints;

		public float NearestBusyPointSqrDistance
		{
			get;
			private set;
		} = float.MaxValue;


		public void RecalculateDistances()
		{
			NearestBusyPointSqrDistance = float.MaxValue;
			foreach (SpawnPointDistance nearSpawnPoint in nearSpawnPoints)
			{
				if (nearSpawnPoint.spawnPoint.IsBusy && NearestBusyPointSqrDistance > nearSpawnPoint.sqrDistance)
				{
					NearestBusyPointSqrDistance = nearSpawnPoint.sqrDistance;
				}
			}
		}

		public bool Contains(CoinSpawnPoint spawnPoint)
		{
			return nearSpawnPoints.Any((SpawnPointDistance x) => x.spawnPoint == spawnPoint);
		}
	}

	[Header("Монета")]
	[SerializeField]
	private float coinPickAcceleration = 20f;

	[SerializeField]
	private float coinPickStartSpeed = 10f;

	[Tooltip("Дистанция, на которой начинается полёт монеты к игроку")]
	[SerializeField]
	private float coinStartPickDistance = 5f;

	[Tooltip("Дистанция, на которой заканчивается полёт монеты к игроку и засчитывается подбор монеты")]
	[SerializeField]
	private float coinCompletePickDistance = 2f;

	[SerializeField]
	private bool scaleOnPick;

	[Range(0f, 1f)]
	[Tooltip("Масштаб, который будет у монеты в конечной фазе подбора при подлёте к игроку")]
	[SerializeField]
	private float pickEndScale = 0.3f;

	[Header("Параметры вылета монет при спавне")]
	[SerializeField]
	private float dropForceVertAngle = 80f;

	[SerializeField]
	private float dropForcePowerMinimum = 20f;

	[SerializeField]
	private float dropForcePerCoin = 1f;

	[SerializeField]
	private float dropForcePowerMaximum = 40f;

	[Tooltip("Высота отсупа монет при дропе")]
	[SerializeField]
	private float _dropHeightOffset = 1f;

	[Header("Спавнер")]
	[SerializeField]
	private int coinsMaximum = 10;

	[SerializeField]
	private float spawnFrequency = 1f;

	private float spawnTimer;

	[SerializeField]
	private float spawnDistanceToPlayerMinimum = 15f;

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

	private SpawnDistributor<CoinSpawnPoint> spawnDistributor;

	private List<CoinSpawnPoint> spawnPoints;

	private List<SpawnPointRelativeDistances> spawnPointsRelativeDistances;

	private List<SpawnPointRelativeDistances> bestSpawnPointsDistances = new List<SpawnPointRelativeDistances>();

	[Header("Ссылки")]
	[SerializeField]
	private GameObject coinPrefab;

	[SerializeField]
	private Transform poolTransform;

	private List<Coin> coinPool = new List<Coin>();

	[SerializeField]
	private Transform spawnedCoinsTransform;

	private List<Coin> spawnedCoins = new List<Coin>();

	private int scottishHitsCount;

	public float CoinPickAcceleration => coinPickAcceleration;

	public float CoinPickStartSpeed => coinPickStartSpeed;

	public float CoinStartPickDistance
	{
		get
		{
			float scottishCoinsPickDistanceBonus = coinStartPickDistance;
			if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Scottish)
			{
				scottishCoinsPickDistanceBonus = ManagerBase<SkinManager>.Instance.ScottishCoinsPickDistanceBonus;
			}
			return scottishCoinsPickDistanceBonus;
		}
	}

	public float CoinCompletePickDistance => coinCompletePickDistance;

	public bool ScaleOnPick => scaleOnPick;

	public float PickEndScale => pickEndScale;

	public float DropHeightOffset => _dropHeightOffset;

	public float SpawnDistanceToPlayerMinimum => spawnDistanceToPlayerMinimum;

	public int NearSpawnPointsCount => nearSpawnPointsCount;

	public List<Coin> SpawnedCoins => spawnedCoins;

	private event SpawnDistributor<CoinSpawnPoint>.EventHandler OccupyPointEvent;

	private event SpawnDistributor<CoinSpawnPoint>.EventHandler FreePointEvent;

	protected override void OnInit()
	{
		PlayerSpawner.spawnPlayerEvent += OnSpawnPlayer;
		if (PlayerSpawner.IsPlayerSpawned)
		{
			OnSpawnPlayer();
		}
	}

	private void OnDestroy()
	{
		PlayerSpawner.spawnPlayerEvent -= OnSpawnPlayer;
		ActorCombat.killEvent -= OnKill;
		if (PlayerSpawner.IsPlayerSpawned)
		{
			PlayerSpawner.PlayerInstance.PlayerCombat.hitEvent -= OnHit;
		}
	}

	private void OnSpawnPlayer()
	{
		spawnPoints = new List<CoinSpawnPoint>(UnityEngine.Object.FindObjectsOfType<CoinSpawnPoint>());
		spawnDistributor = new SpawnDistributor<CoinSpawnPoint>(spawnPoints, PlayerSpawner.PlayerInstance.gameObject, ref this.OccupyPointEvent, ref this.FreePointEvent, nearSpawnPointsCount, spawnDistanceToPlayerMinimum, timeWeight, occupancyWeight, startTimeWeight);
		InitialSpawn();
		ActorCombat.killEvent += OnKill;
		PlayerSpawner.PlayerInstance.PlayerCombat.hitEvent += OnHit;
	}

	private void OnKill(ActorCombat killer, ActorCombat target)
	{
		if (!(killer is PlayerCombat) && !(killer is FamilyMemberCombat))
		{
			return;
		}
		EnemyCurrentScheme currentScheme = ((EnemyCombat)target).EnemyModel.EnemyController.CurrentScheme;
		if (ManagerBase<FarmResidentManager>.Instance.SuperBonusesData.Find((FarmResidentManager.SuperBonusData x) => x.id == SuperBonus.Id.Acorn).IsActive)
		{
			DropCoins(target.transform.position, target.transform.position - PlayerSpawner.PlayerInstance.transform.position, currentScheme.Scheme.coinsByBonus);
		}
		else if (currentScheme.Scheme.Type != 0)
		{
			int num = currentScheme.Scheme.Coins;
			if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Mainecoon && currentScheme.Scheme.Type == EnemyScheme.SchemeType.MiniBoss)
			{
				num += ManagerBase<SkinManager>.Instance.MainecoonCoinsFromMiniBossBonus;
			}
			DropCoins(target.transform.position, target.transform.position - PlayerSpawner.PlayerInstance.transform.position, num);
		}
	}

	private void OnHit(ActorCombat attacker, ActorCombat target)
	{
		if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Scottish && target != null)
		{
			scottishHitsCount++;
			if (scottishHitsCount == ManagerBase<SkinManager>.Instance.ScottishHitCountsPerCoinDropBonus)
			{
				scottishHitsCount = 0;
				DropCoins(target.transform.position, target.transform.position - PlayerSpawner.PlayerInstance.transform.position, 1);
			}
		}
	}

	public void DropCoins(Vector3 dropSource, Vector3 dropDirection, int coinsObjsCount, float forceSectorAngle = 180f)
	{
		List<Vector3> dropForces = SpawnUtils.GetDropForces(dropDirection, coinsObjsCount, dropForceVertAngle, forceSectorAngle);
		Mathf.Clamp(coinsObjsCount / 10, 0f, 1f);
		float maxInclusive = Mathf.Clamp(dropForcePowerMinimum + dropForcePerCoin * (float)dropForces.Count, dropForcePowerMinimum, dropForcePowerMaximum);
		for (int i = 0; i < dropForces.Count; i++)
		{
			List<Vector3> list = dropForces;
			int index = i;
			list[index] *= UnityEngine.Random.Range(dropForcePowerMinimum, maxInclusive);
		}
		foreach (Vector3 item in dropForces)
		{
			Coin coin = SpawnCoin(dropSource, ManagerBase<PlayerManager>.Instance.CoinValue, isSpawnedCoin: false);
			coin.AttachToGround();
			coin.ApplyForce(item, null);
		}
	}

	private void InitialSpawn()
	{
		int num = coinsMaximum;
		if (ManagerBase<SaveManager>.Instance.IsDataLoaded && ManagerBase<PlayerManager>.Instance.lastCoinSpawnTime != 0L)
		{
			num = ManagerBase<PlayerManager>.Instance.coinsOnSpawnPoints;
			int num2 = (int)((float)(TimeUtils.GetDeviceTime(TimeUtils.TimeType.System) - ManagerBase<PlayerManager>.Instance.lastCoinSpawnTime) / spawnFrequency);
			num += num2;
			num = Mathf.Clamp(num, 0, coinsMaximum);
		}
		ManagerBase<PlayerManager>.Instance.coinsOnSpawnPoints = 0;
		while (spawnedCoins.Count < num)
		{
			CoinSpawnPoint coinSpawnPoint = spawnDistributor.SelectSpawnPoint();
			if (!(coinSpawnPoint == null))
			{
				SpawnCoin(coinSpawnPoint, ManagerBase<PlayerManager>.Instance.CoinValue, isSpawnedCoin: true);
				continue;
			}
			break;
		}
	}

	private Coin SpawnCoin(Vector3 position, int coinValue, bool isSpawnedCoin)
	{
		Coin coinFromPool = GetCoinFromPool();
		coinFromPool.Position = position;
		SpawnCoin(null, coinValue, isSpawnedCoin, coinFromPool);
		return coinFromPool;
	}

	private Coin SpawnCoin(CoinSpawnPoint spawnPoint, int coinValue, bool isSpawnedCoin, Coin coin = null)
	{
		if (coin == null)
		{
			coin = GetCoinFromPool();
		}
		if (spawnPoint != null)
		{
			spawnPoint.Occupy(coin);
			this.OccupyPointEvent?.Invoke(spawnPoint);
			coin.Position = spawnPoint.transform.position;
			ManagerBase<PlayerManager>.Instance.coinsOnSpawnPoints++;
			ManagerBase<PlayerManager>.Instance.lastCoinSpawnTime = TimeUtils.GetDeviceTime(TimeUtils.TimeType.System);
		}
		coin.transform.parent = spawnedCoinsTransform;
		coin.gameObject.SetActive(value: true);
		spawnedCoins.Add(coin);
		coin.Spawn(delegate
		{
			OnUnspawn(coin, spawnPoint);
		}, coinValue, isSpawnedCoin);
		return coin;
	}

	private void OnUnspawn(Coin coin, CoinSpawnPoint spawnPoint)
	{
		spawnedCoins.Remove(coin);
		coin.gameObject.SetActive(value: false);
		coin.transform.parent = poolTransform;
		coinPool.Add(coin);
		if (spawnPoint != null)
		{
			spawnPoint.Free();
			this.FreePointEvent?.Invoke(spawnPoint);
			ManagerBase<PlayerManager>.Instance.coinsOnSpawnPoints--;
		}
	}

	private Coin GetCoinFromPool()
	{
		if (coinPool.Count > 0)
		{
			Coin coin = coinPool[0];
			coinPool.Remove(coin);
			return coin;
		}
		return UnityEngine.Object.Instantiate(coinPrefab).GetComponent<Coin>();
	}

	private void Update()
	{
		if (!PlayerSpawner.IsPlayerSpawned)
		{
			return;
		}
		if (spawnTimer <= 0f)
		{
			spawnTimer = spawnFrequency;
			if (spawnedCoins.Count < coinsMaximum)
			{
				CoinSpawnPoint coinSpawnPoint = spawnDistributor.SelectSpawnPoint();
				if (coinSpawnPoint != null)
				{
					SpawnCoin(coinSpawnPoint, ManagerBase<PlayerManager>.Instance.CoinValue, isSpawnedCoin: true);
				}
			}
		}
		else
		{
			spawnTimer -= Time.deltaTime;
		}
	}
}
