using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemySpawner : Singleton<EnemySpawner>
{
	[Serializable]
	public class EnemyLogic
	{
		public EnemyController controller;

		public EnemyCurrentScheme currentScheme;

		public EnemyLogic(EnemyController controller, EnemyCurrentScheme currentScheme)
		{
			this.controller = controller;
			this.currentScheme = currentScheme;
		}
	}

	[Serializable]
	public class EnemyInstance
	{
		public EnemyLogic logic;

		public EnemyModel model;

		public EnemyInstance(EnemyLogic logic, EnemyModel model)
		{
			this.logic = logic;
			this.model = model;
		}
	}

	[Serializable]
	public class EnemyMatrixElement
	{
		public EnemyArchetype archetype;

		public int zoneId;

		public int value;
	}

	[Serializable]
	public class PriorityMaxtrixPBI
	{
		public List<EnemyMatrixElement> priorityMatrixMinimumPBI = new List<EnemyMatrixElement>();

		public List<EnemyMatrixElement> priorityMatrixMaximumPBI = new List<EnemyMatrixElement>();

		private int LimitLevel
		{
			get
			{
				if (!(Managers.Instance != null))
				{
					return 0;
				}
				return Managers.Instance.PBILimitLevel;
			}
		}

		public void UpdateMatrix(int level, List<EnemyMatrixElement> priorityMatrix)
		{
			foreach (EnemyMatrixElement matrixElem in priorityMatrix)
			{
				EnemyMatrixElement enemyMatrixElement = priorityMatrixMaximumPBI.Find((EnemyMatrixElement x) => x.archetype == matrixElem.archetype && x.zoneId == matrixElem.zoneId);
				EnemyMatrixElement enemyMatrixElement2 = priorityMatrixMinimumPBI.Find((EnemyMatrixElement x) => x.archetype == matrixElem.archetype && x.zoneId == matrixElem.zoneId);
				matrixElem.value = IntPBI.Calculate(level, Managers.Instance.PBILimitLevel, enemyMatrixElement2.value, enemyMatrixElement.value);
			}
		}

		public void ResolveEditorChanges(int zonesCount, List<EnemyArchetype> enemyArchetypes)
		{
			priorityMatrixMinimumPBI.RemoveAll((EnemyMatrixElement x) => (!(x.archetype == null) && x.archetype.gameObject.activeSelf) ? (x.zoneId >= zonesCount) : true);
			priorityMatrixMaximumPBI.RemoveAll((EnemyMatrixElement x) => (!(x.archetype == null) && x.archetype.gameObject.activeSelf) ? (x.zoneId >= zonesCount) : true);
			int zoneId = 0;
			while (zoneId < zonesCount)
			{
				foreach (EnemyArchetype enemyArchetype in enemyArchetypes)
				{
					EnemyMatrixElement enemyMatrixElement = priorityMatrixMinimumPBI.Find((EnemyMatrixElement x) => x.archetype == enemyArchetype && x.zoneId == zoneId);
					if (enemyMatrixElement == null)
					{
						enemyMatrixElement = new EnemyMatrixElement
						{
							archetype = enemyArchetype,
							zoneId = zoneId,
							value = 0
						};
						priorityMatrixMinimumPBI.Add(enemyMatrixElement);
					}
					enemyMatrixElement = priorityMatrixMaximumPBI.Find((EnemyMatrixElement x) => x.archetype == enemyArchetype && x.zoneId == zoneId);
					if (enemyMatrixElement == null)
					{
						enemyMatrixElement = new EnemyMatrixElement
						{
							archetype = enemyArchetype,
							zoneId = zoneId,
							value = 0
						};
						priorityMatrixMaximumPBI.Add(enemyMatrixElement);
					}
				}
				int num = ++zoneId;
			}
		}
	}

	[Serializable]
	public class GlobalEnemyParamsType
	{
		[Header("Основные параметры")]
		[Postfix(PostfixAttribute.Id.PBI)]
		public FloatPBI visionDistance;

		[Postfix(PostfixAttribute.Id.PBI)]
		public FloatPBI visionAngle;

		[Postfix(PostfixAttribute.Id.PBI)]
		public FloatPBI visionHeight;

		[Postfix(PostfixAttribute.Id.PBI)]
		public FloatPBI visionStealthDistance;

		[Postfix(PostfixAttribute.Id.PBI)]
		public FloatPBI visionStealthAngle;

		[Postfix(PostfixAttribute.Id.PBI)]
		public FloatPBI visionStealthHeight;

		[Postfix(PostfixAttribute.Id.PBI)]
		public FloatPBI hearing;

		public float hearingMinimum;

		[Postfix(PostfixAttribute.Id.PBI)]
		public FloatPBI hearingMaximum;

		public float hitDistance = 5f;

		[Postfix(PostfixAttribute.Id.PBI)]
		public FloatPBI endurance;

		[Postfix(PostfixAttribute.Id.Seconds)]
		public float recoveryTime = 180f;

		[Postfix(PostfixAttribute.Id.PBI)]
		public FloatPBI acceleration;

		[Postfix(PostfixAttribute.Id.Percents)]
		public float escapeMediumSpeed = 80f;

		[Postfix(PostfixAttribute.Id.Percents)]
		public float hitSlowingSpeed;

		[Postfix(PostfixAttribute.Id.Seconds)]
		public float hitSlowingTime;

		[Tooltip("Замедление на поворотах")]
		[Postfix(PostfixAttribute.Id.Percents)]
		public float corneringSlowing = 90f;

		[Postfix(PostfixAttribute.Id.Percents)]
		public float healthLevelMod = 10f;

		[Postfix(PostfixAttribute.Id.Percents)]
		public float hitPowerLevelMod = 10f;

		[Header("Остальные параметры")]
		[Tooltip("Минимальная скорость при преследовании, для того что бы моб мог догнать игрока при движении назад.")]
		public float minPursuitSpeed = 8f;

		public float helpRadius;

		public float allyAngle;

		public float rotationSpeed;

		public float rotationSpeedFight;

		[Postfix(PostfixAttribute.Id.Seconds)]
		public float jumpTime = 0.5f;

		[Postfix(PostfixAttribute.Id.Percents)]
		public float experienceDec = 10f;

		public float processingDistance;

		public float distanceToSwitchMoveMode;

		public float bossProcessingDistance;

		public float bossDistanceToSwitchMoveMode;

		public float walkDistanceMaximum;

		[Postfix(PostfixAttribute.Id.Percents)]
		public float idleChance;

		[Postfix(PostfixAttribute.Id.Seconds)]
		public float idleTimeMin = 3f;

		[Postfix(PostfixAttribute.Id.Seconds)]
		public float idleTimeMax = 15f;

		[Postfix(PostfixAttribute.Id.Percents)]
		public float enduranceHealthFallMod;

		[Postfix(PostfixAttribute.Id.Seconds)]
		public float checkHearAndViewInterval;

		[Postfix(PostfixAttribute.Id.Seconds)]
		public float checkPosAndRotBossInterval = 5f;
	}

	[Serializable]
	public class ArchetypeAllies
	{
		public EnemyArchetype archetype;

		public List<EnemyArchetype> allies = new List<EnemyArchetype>();
	}

	private class ArchetypeOccupancyRatio
	{
		public EnemyArchetype archetype;

		public float occupancyRatio;

		public ArchetypeOccupancyRatio(EnemyArchetype archetype, float occupancyRatio)
		{
			this.archetype = archetype;
			this.occupancyRatio = occupancyRatio;
		}
	}

	private class ZoneOccupancyRatio
	{
		public int zoneId;

		public float occupancyRatio;
	}

	public delegate void SpawnHandler(EnemyController enemyController);

	[SerializeField]
	private GlobalEnemyParamsType globalEnemyParams;

	[SerializeField]
	private List<EnemyArchetype> enemyArchetypes;

	[SerializeField]
	private List<EnemyScheme> enemySchemes;

	[SerializeField]
	private FloatPBI simpleMinimum;

	[SerializeField]
	private FloatPBI simpleMaximum;

	[SerializeField]
	private FloatPBI simpleFrequency;

	private float simpleTimer;

	[SerializeField]
	private int levelLag = 2;

	[Tooltip("Вероятность (в процентах) получения врагом уровня игрока при корректировки схемы")]
	[SerializeField]
	private FloatPBI assignPlayerLevelChance;

	[SerializeField]
	private int zonesCount = 2;

	[SerializeField]
	private PriorityMaxtrixPBI priorityMatrixPBI = new PriorityMaxtrixPBI();

	[SerializeField]
	private List<EnemyMatrixElement> priorityMatrix = new List<EnemyMatrixElement>();

	[SerializeField]
	private List<EnemyMatrixElement> quantityMatrix = new List<EnemyMatrixElement>();

	[SerializeField]
	private List<ArchetypeAllies> allyMatrix;

	private List<EnemySpawnPoint> enemySpawnPoints;

	[Header("Отладка")]
	private EnemyInstance spawnedBoss;

	[SerializeField]
	private List<EnemyInstance> bossAssistantsSchemes = new List<EnemyInstance>();

	[SerializeField]
	private List<EnemyInstance> simpleSchemes = new List<EnemyInstance>();

	[SerializeField]
	private List<EnemyInstance> miniBossSchemes = new List<EnemyInstance>();

	[Header("Ссылки")]
	[SerializeField]
	private GameObject archetypesGO;

	[SerializeField]
	private Transform activeEnemiesTransform;

	[SerializeField]
	private Transform activeBossTransform;

	[SerializeField]
	private Transform enemyLogicPoolTransform;

	[SerializeField]
	private Transform enemyModelPoolTransform;

	[SerializeField]
	private GameObject enemyLogicPrefab;

	private BossSpawnPoint bossSpawnPoint;

	private List<BossAssistantSpawnPoint> bossAssistantSpawnPoints = new List<BossAssistantSpawnPoint>();

	private List<EnemyInstance> spawnedEnemies = new List<EnemyInstance>();

	private List<ArchetypeOccupancyRatio> archetypesOccupancyRatio = new List<ArchetypeOccupancyRatio>();

	private List<ZoneOccupancyRatio> zonesOccupancy = new List<ZoneOccupancyRatio>();

	private List<EnemyModel> enemyModelPool = new List<EnemyModel>();

	private List<EnemyLogic> enemyLogicPool = new List<EnemyLogic>();

	public GlobalEnemyParamsType GlobalEnemyParams => globalEnemyParams;

	public List<EnemyArchetype> Archetypes => enemyArchetypes;

	public int LevelLag => levelLag;

	public List<ArchetypeAllies> AllyMatrix => allyMatrix;

	public EnemyInstance SpawnedBoss => spawnedBoss;

	public BossSpawnPoint BossSpawnPoint => bossSpawnPoint;

	public List<EnemyHabitatArea> EnemyHabitats
	{
		get;
		private set;
	}

	public List<EnemyInstance> SpawnedEnemies => spawnedEnemies;

	private List<EnemyCurrentScheme> SaveSchemes => ManagerBase<EnemyManager>.Instance.saveSchemes;

	private int PlayerLevel => ManagerBase<PlayerManager>.Instance.Level;

	public bool InitializeSpawnCompleted
	{
		get;
		private set;
	}

	public static event SpawnHandler spawnEvent;

	public static event SpawnHandler unspawnEvent;

	public event Action initializeSpawnCompletedEvent;

	protected override void OnInit()
	{
		EnemyHabitats = UnityEngine.Object.FindObjectsOfType<EnemyHabitatArea>().ToList();
		enemySpawnPoints = UnityEngine.Object.FindObjectsOfType<EnemySpawnPoint>().ToList();
		bossSpawnPoint = UnityEngine.Object.FindObjectOfType<BossSpawnPoint>();
		bossAssistantSpawnPoints = UnityEngine.Object.FindObjectsOfType<BossAssistantSpawnPoint>().ToList();
		ResolveEditorChanges();
		priorityMatrixPBI.UpdateMatrix(PlayerLevel, priorityMatrix);
		quantityMatrix = new List<EnemyMatrixElement>();
		priorityMatrix.ForEach(delegate(EnemyMatrixElement elem)
		{
			quantityMatrix.Add(new EnemyMatrixElement
			{
				archetype = elem.archetype,
				zoneId = elem.zoneId,
				value = 0
			});
		});
		PlayerSpawner.spawnPlayerEvent += OnSpawnPlayer;
		if (PlayerSpawner.IsPlayerSpawned)
		{
			OnSpawnPlayer();
		}
		PlayerSleepController.sleepEvent += OnPlayerSleep;
		PlayerManager.levelChangeEvent += OnPlayerChangeLevel;
	}

	private void OnDestroy()
	{
		PlayerSpawner.spawnPlayerEvent -= OnSpawnPlayer;
		PlayerSleepController.sleepEvent -= OnPlayerSleep;
		PlayerManager.levelChangeEvent -= OnPlayerChangeLevel;
		ActorCombat.killEvent -= OnKill;
	}

	private void OnSpawnPlayer()
	{
		ActorCombat.killEvent += OnKill;
		InitializeSpawn();
	}

	private void OnPlayerSleep()
	{
		InitializeSpawn();
	}

	private void OnPlayerChangeLevel()
	{
		priorityMatrixPBI.UpdateMatrix(PlayerLevel, priorityMatrix);
		foreach (EnemyInstance spawnedEnemy in SpawnedEnemies)
		{
			CorrectEnemyLevel(spawnedEnemy.logic.currentScheme);
		}
	}

	public void ResolveEditorChanges()
	{
		if (!(archetypesGO == null))
		{
			enemyArchetypes = archetypesGO.GetComponentsInChildren<EnemyArchetype>().ToList();
			enemySchemes = archetypesGO.GetComponentsInChildren<EnemyScheme>().ToList();
			priorityMatrix.RemoveAll((EnemyMatrixElement x) => (!(x.archetype == null) && x.archetype.gameObject.activeSelf) ? (x.zoneId >= zonesCount) : true);
			quantityMatrix.RemoveAll((EnemyMatrixElement x) => (!(x.archetype == null) && x.archetype.gameObject.activeSelf) ? (x.zoneId >= zonesCount) : true);
			int zoneId = 0;
			while (zoneId < zonesCount)
			{
				foreach (EnemyArchetype enemyArchetype in enemyArchetypes)
				{
					EnemyMatrixElement enemyMatrixElement = quantityMatrix.Find((EnemyMatrixElement x) => x.archetype == enemyArchetype && x.zoneId == zoneId);
					if (enemyMatrixElement == null)
					{
						enemyMatrixElement = new EnemyMatrixElement
						{
							archetype = enemyArchetype,
							zoneId = zoneId,
							value = 0
						};
						quantityMatrix.Add(enemyMatrixElement);
					}
					enemyMatrixElement = priorityMatrix.Find((EnemyMatrixElement x) => x.archetype == enemyArchetype && x.zoneId == zoneId);
					if (enemyMatrixElement == null)
					{
						enemyMatrixElement = new EnemyMatrixElement
						{
							archetype = enemyArchetype,
							zoneId = zoneId,
							value = 0
						};
						priorityMatrix.Add(enemyMatrixElement);
					}
				}
				int num = ++zoneId;
			}
			priorityMatrixPBI.ResolveEditorChanges(zonesCount, enemyArchetypes);
			foreach (ArchetypeAllies item in allyMatrix)
			{
				item.allies.RemoveAll((EnemyArchetype x) => x == null);
			}
			allyMatrix.RemoveAll((ArchetypeAllies x) => (!(x.archetype == null)) ? (!x.archetype.isActiveAndEnabled) : true);
			foreach (EnemyArchetype enemyArchetype2 in enemyArchetypes)
			{
				if (!allyMatrix.Any((ArchetypeAllies x) => x.archetype == enemyArchetype2))
				{
					ArchetypeAllies archetypeAllies = new ArchetypeAllies
					{
						archetype = enemyArchetype2,
						allies = new List<EnemyArchetype>()
					};
					archetypeAllies.allies.Add(enemyArchetype2);
					allyMatrix.Add(archetypeAllies);
				}
			}
		}
	}

	private void OnKill(ActorCombat killer, ActorCombat actorCombat)
	{
		if (!(killer is PlayerCombat) && !(killer is FamilyMemberCombat))
		{
			return;
		}
		EnemyController enemyController = actorCombat.GetComponentInParent<EnemyController>();
		if (!(enemyController != null))
		{
			return;
		}
		EnemyInstance enemy = spawnedEnemies.Find((EnemyInstance x) => x.logic.controller == enemyController);
		if (enemy == null)
		{
			return;
		}
		if (enemy.logic.currentScheme.Scheme.Type == EnemyScheme.SchemeType.Simple)
		{
			EnemyManager.MinibossData minibossData = ManagerBase<EnemyManager>.Instance.GetMinibossData(enemy.logic.currentScheme.Archetype);
			if (!miniBossSchemes.Any((EnemyInstance x) => x.logic.currentScheme.Archetype == enemy.logic.currentScheme.Archetype))
			{
				minibossData.simpleSchemeDeaths++;
			}
		}
		else if (enemy.logic.currentScheme.Scheme.Type == EnemyScheme.SchemeType.Boss)
		{
			ManagerBase<EnemyManager>.Instance.killedBosses++;
		}
	}

	private void InitializeSpawn()
	{
		if (!InitializeSpawnCompleted && SaveSchemes.Count == 0)
		{
			while ((float)simpleSchemes.Count < simpleMaximum.GetValue(PlayerLevel))
			{
				EnemyCurrentScheme enemyCurrentScheme = CreateScheme(EnemyScheme.SchemeType.Simple);
				SpawnEnemy(enemyCurrentScheme, isInitializeSpawn: true);
			}
			if (!SpawnedEnemies.Any((EnemyInstance x) => x.logic.currentScheme.Scheme.Type == EnemyScheme.SchemeType.Boss))
			{
				EnemyCurrentScheme enemyCurrentScheme2 = CreateScheme(EnemyScheme.SchemeType.Boss);
				SpawnEnemy(enemyCurrentScheme2, isInitializeSpawn: true);
				_003CInitializeSpawn_003Eg__TrySpawnBossAssistant_007C83_1();
			}
		}
		else if (!InitializeSpawnCompleted && SaveSchemes.Count > 0)
		{
			List<EnemyCurrentScheme> list = new List<EnemyCurrentScheme>(SaveSchemes);
			SaveSchemes.Clear();
			foreach (EnemyCurrentScheme item in list)
			{
				SpawnEnemy(item, isInitializeSpawn: true);
				if (item.Scheme.Type == EnemyScheme.SchemeType.Boss)
				{
					_003CInitializeSpawn_003Eg__TrySpawnBossAssistant_007C83_1();
				}
			}
			while ((float)simpleSchemes.Count < simpleMaximum.GetValue(PlayerLevel))
			{
				EnemyCurrentScheme enemyCurrentScheme3 = CreateScheme(EnemyScheme.SchemeType.Simple);
				enemyCurrentScheme3.level = Mathf.Clamp(PlayerLevel - LevelLag, 1, PlayerLevel);
				SpawnEnemy(enemyCurrentScheme3, isInitializeSpawn: true);
			}
		}
		else if (ManagerBase<PlayerManager>.Instance.isSleeping)
		{
			if (SpawnedBoss == null)
			{
				EnemyCurrentScheme enemyCurrentScheme4 = CreateScheme(EnemyScheme.SchemeType.Boss);
				SpawnEnemy(enemyCurrentScheme4, isInitializeSpawn: true);
				if (bossAssistantsSchemes.Count > 0)
				{
					List<EnemyInstance> list2 = new List<EnemyInstance>(bossAssistantsSchemes);
					for (int i = 0; i < list2.Count; i++)
					{
						list2[i].logic.controller.Unspawn();
					}
				}
				_003CInitializeSpawn_003Eg__TrySpawnBossAssistant_007C83_1();
			}
			while ((float)simpleSchemes.Count < simpleMaximum.GetValue(PlayerLevel))
			{
				EnemyCurrentScheme enemyCurrentScheme5 = CreateScheme(EnemyScheme.SchemeType.Simple);
				SpawnEnemy(enemyCurrentScheme5, isInitializeSpawn: true);
			}
		}
		foreach (EnemyInstance enemyInstance in SpawnedEnemies)
		{
			int newZone = -1;
			if (enemyInstance.logic.currentScheme.Scheme.Type != EnemyScheme.SchemeType.Boss && enemyInstance.logic.currentScheme.Scheme.Type != EnemyScheme.SchemeType.BossAssistant)
			{
				quantityMatrix.Find((EnemyMatrixElement x) => x.archetype == enemyInstance.logic.currentScheme.Scheme.Archetype && x.zoneId == enemyInstance.logic.currentScheme.zone).value--;
				newZone = SelectZone(enemyInstance.logic.currentScheme.Scheme.Archetype);
				quantityMatrix.Find((EnemyMatrixElement x) => x.archetype == enemyInstance.logic.currentScheme.Scheme.Archetype && x.zoneId == newZone).value++;
			}
			enemyInstance.logic.currentScheme.CorrectZone(newZone);
			CorrectEnemyLevel(enemyInstance.logic.currentScheme);
		}
		if (!InitializeSpawnCompleted)
		{
			InitializeSpawnCompleted = true;
			this.initializeSpawnCompletedEvent?.Invoke();
		}
	}

	private void CorrectEnemyLevel(EnemyCurrentScheme enemyCurrentScheme)
	{
		int level = Mathf.Clamp(enemyCurrentScheme.level, PlayerLevel - levelLag, enemyCurrentScheme.level);
		if (enemyCurrentScheme.Scheme.Type != EnemyScheme.SchemeType.Boss && UnityEngine.Random.Range(0f, 100f) <= assignPlayerLevelChance.GetValue(PlayerLevel))
		{
			level = PlayerLevel;
		}
		enemyCurrentScheme.CorrectLevel(level);
	}

	private EnemyCurrentScheme CreateScheme(EnemyScheme.SchemeType schemeType)
	{
		EnemyCurrentScheme enemyCurrentScheme = null;
		switch (schemeType)
		{
		case EnemyScheme.SchemeType.Boss:
		{
			EnemyScheme bossScheme = enemyArchetypes[ManagerBase<EnemyManager>.Instance.killedBosses % enemyArchetypes.Count].BossScheme;
			EnemyArchetype archetype = bossScheme.Archetype;
			return new EnemyCurrentScheme(level: PlayerLevel, scheme: bossScheme, archetype: bossScheme.Archetype, zone: -1);
		}
		case EnemyScheme.SchemeType.BossAssistant:
		{
			EnemyScheme enemyScheme2 = (SpawnedBoss == null) ? enemyArchetypes[ManagerBase<EnemyManager>.Instance.killedBosses % enemyArchetypes.Count].BossAssistantScheme : SpawnedBoss.logic.currentScheme.Archetype.BossAssistantScheme;
			int playerLevel3 = PlayerLevel;
			return new EnemyCurrentScheme(enemyScheme2, enemyScheme2.Archetype, playerLevel3, -1);
		}
		default:
		{
			EnemyArchetype enemyArchetype = SelectArchetype();
			EnemyScheme enemyScheme = enemyArchetype.SimpleScheme;
			if (CanSpawnMiniboss(enemyArchetype))
			{
				enemyScheme = enemyArchetype.MiniBossScheme;
			}
			int zone = SelectZone(enemyScheme.Archetype);
			int playerLevel = PlayerLevel;
			return new EnemyCurrentScheme(enemyScheme, enemyScheme.Archetype, playerLevel, zone);
		}
		}
	}

	private EnemyArchetype SelectArchetype()
	{
		archetypesOccupancyRatio.Clear();
		foreach (EnemyArchetype enemyArchetype in enemyArchetypes)
		{
			int num = 0;
			foreach (EnemyMatrixElement item in quantityMatrix)
			{
				if (item.archetype == enemyArchetype)
				{
					num += item.value;
				}
			}
			int num2 = 0;
			foreach (EnemyMatrixElement item2 in priorityMatrix)
			{
				if (item2.archetype == enemyArchetype)
				{
					num2 += item2.value;
				}
			}
			float occupancyRatio = (float)num / (float)num2;
			if (num2 == 0)
			{
				occupancyRatio = float.MaxValue;
			}
			archetypesOccupancyRatio.Add(new ArchetypeOccupancyRatio(enemyArchetype, occupancyRatio));
		}
		archetypesOccupancyRatio.Sort((ArchetypeOccupancyRatio elem1, ArchetypeOccupancyRatio elem2) => elem1.occupancyRatio.CompareTo(elem2.occupancyRatio));
		return archetypesOccupancyRatio[0].archetype;
	}

    private bool CanSpawnMiniboss(EnemyArchetype archetype)
    {
        if (archetype == null) return false;

        // _HaveMinibossScheme
        var miniBossScheme = archetype.MiniBossScheme;
        if (miniBossScheme == null) return false;

        // _IsPlayerLevelSatisfactory
        if (miniBossScheme.MinimumLevel > this.PlayerLevel) return false;

        // _IsMinibossSpawned
        bool alreadySpawned = this.SpawnedEnemies.Any(x =>
            x != null &&
            x.logic?.currentScheme?.Scheme?.Type == EnemyScheme.SchemeType.MiniBoss &&
            x.logic.currentScheme.Archetype == archetype
        );
        if (alreadySpawned) return false;

        // _EnoughDeathsForSpawn
        var minibossData = ManagerBase<EnemyManager>.Instance.GetMinibossData(archetype);
        if (minibossData == null || minibossData.Archetype == null) return false;

        return minibossData.simpleSchemeDeaths >= minibossData.Archetype.killsForMinibossSpawn;
    }

    private EnemyLogic GetEnemyLogicFromPool()
    {
        // Lấy index 0 như decompile
        if (enemyLogicPool.Count > 0)
        {
            EnemyLogic logic = enemyLogicPool[0];
            enemyLogicPool.Remove(logic);
            if (logic?.controller != null)
                logic.controller.gameObject.SetActive(true);
            return logic;
        }

        // Hết pool -> tạo mới
        EnemyController ctrl = Instantiate(enemyLogicPrefab, Vector3.zero, Quaternion.identity)
                               .GetComponent<EnemyController>();
        return new EnemyLogic(ctrl, ctrl.CurrentScheme);
    }

    // Bản đúng theo decompile: truyền cả EnemyCurrentScheme để lấy đúng prefab
    private EnemyModel GetEnemyModelFromPool(EnemyCurrentScheme ecs)
    {
        EnemyArchetype archetype = ecs.Scheme.Archetype;

        // Tìm trong pool theo Archetype
        EnemyModel model = enemyModelPool.Find(m => m != null && m.Archetype == archetype);
        if (model != null)
        {
            enemyModelPool.Remove(model);
            model.gameObject.SetActive(true);
        }
        else
        {
            // Instantiate theo ecs.Archetype.modelPrefab dưới activeEnemiesTransform
            // (đây là điểm khác biệt quan trọng của decompile bạn gửi)
            model = Instantiate(ecs.Archetype.modelPrefab, activeEnemiesTransform)
                        .GetComponent<EnemyModel>();
            model.Archetype = archetype;
        }
        return model;
    }


    private void SpawnEnemy(EnemyCurrentScheme enemyCurrentScheme, bool isInitializeSpawn)
	{
        EnemyLogic enemyLogic = GetEnemyLogicFromPool();
        EnemyModel enemyModel = GetEnemyModelFromPool(enemyCurrentScheme);
        enemyLogic.controller.CurrentScheme = enemyCurrentScheme;
		enemyLogic.currentScheme = enemyCurrentScheme;
		enemyLogic.controller.EnemyModel = enemyModel;
		if (enemyCurrentScheme.Scheme.Type == EnemyScheme.SchemeType.Boss)
		{
			enemyLogic.controller.transform.parent = activeBossTransform;
		}
		else
		{
			enemyLogic.controller.transform.parent = activeEnemiesTransform;
		}
		enemyLogic.controller.gameObject.name = $"{enemyCurrentScheme.Scheme.Archetype.name} ({enemyCurrentScheme.Scheme.Type})";
		enemyModel.transform.parent = enemyLogic.controller.transform;
		enemyModel.transform.localPosition = Vector3.zero;
		enemyModel.transform.localRotation = Quaternion.identity;
		enemyModel.SetController(enemyLogic.controller);
		if (enemyCurrentScheme.Scheme.Type == EnemyScheme.SchemeType.Boss || enemyCurrentScheme.Scheme.Type == EnemyScheme.SchemeType.BossAssistant)
		{
			enemyLogic.controller.Spawn(bossSpawnPoint.transform.position, isInitializeSpawn, OnUnspawn);
		}
		else
		{
			if (enemyCurrentScheme.Scheme.Type == EnemyScheme.SchemeType.MiniBoss)
			{
				ManagerBase<EnemyManager>.Instance.GetMinibossData(enemyCurrentScheme.Archetype).simpleSchemeDeaths = 0;
			}
			EnemySpawnPoint enemySpawnPoint = enemySpawnPoints.FindAll((EnemySpawnPoint x) => x.CanSpawn()).Random();
			if (enemySpawnPoint == null)
			{
				enemySpawnPoint = enemySpawnPoints.Random();
			}
			enemyLogic.controller.Spawn(enemySpawnPoint.transform.position, isInitializeSpawn, OnUnspawn);
		}
		EnemyInstance item = new EnemyInstance(enemyLogic, enemyModel);
		if (enemyCurrentScheme.Scheme.Type == EnemyScheme.SchemeType.Boss)
		{
			spawnedBoss = item;
		}
		else if (enemyCurrentScheme.Scheme.Type == EnemyScheme.SchemeType.BossAssistant)
		{
			bossAssistantsSchemes.Add(item);
		}
		else
		{
			if (enemyCurrentScheme.Scheme.Type == EnemyScheme.SchemeType.Simple)
			{
				simpleSchemes.Add(item);
			}
			else
			{
				miniBossSchemes.Add(item);
			}
			quantityMatrix.Find((EnemyMatrixElement x) => x.archetype == enemyCurrentScheme.Scheme.Archetype && x.zoneId == enemyCurrentScheme.zone).value++;
		}
		if (enemyCurrentScheme.Scheme.Type != 0 && enemyCurrentScheme.Scheme.Type != EnemyScheme.SchemeType.BossAssistant)
		{
			SaveSchemes.Add(enemyCurrentScheme);
		}
		spawnedEnemies.Add(item);
		EnemySpawner.spawnEvent?.Invoke(enemyLogic.controller);
	}

    private void OnUnspawn(EnemyController enemyController)
    {
        if (enemyController == null) return;

        // Tương đương _OnUnspawn_b__0
        EnemyInstance enemyInstance = spawnedEnemies.Find(x => x.logic.controller == enemyController);
        if (enemyInstance == null) return;

        var schemeType = enemyController.CurrentScheme.Scheme.Type;

        // Phân loại xoá / cập nhật list
        if (schemeType == EnemyScheme.SchemeType.Boss)
        {
            spawnedBoss = null;
        }
        else if (schemeType == EnemyScheme.SchemeType.BossAssistant)
        {
            bossAssistantsSchemes.Remove(enemyInstance);
        }
        else
        {
            if (schemeType == EnemyScheme.SchemeType.Simple)
                simpleSchemes.Remove(enemyInstance);
            else
                miniBossSchemes.Remove(enemyInstance);

            // Tương đương _OnUnspawn_b__3
            var qElem = quantityMatrix.Find(x =>
                x.archetype == enemyInstance.logic.currentScheme.Scheme.Archetype &&
                x.zoneId == enemyInstance.logic.currentScheme.zone
            );
            if (qElem != null) qElem.value--;
        }

        // Remove khỏi SaveSchemes nếu KHÔNG phải Simple (decompile dùng != 0)
        if (enemyInstance.logic.currentScheme.Scheme.Type != EnemyScheme.SchemeType.Simple)
            SaveSchemes.Remove(enemyInstance.logic.currentScheme);

        // Bỏ khỏi spawnedEnemies
        spawnedEnemies.Remove(enemyInstance);

        // ===== Helpers push về pool (mapping _OnUnspawn_g__PushModelToPool|1 & |2) =====
        PushModelToPool(enemyInstance.model);
        PushLogicToPool(enemyInstance.logic);

        // Gọi event
        EnemySpawner.unspawnEvent?.Invoke(enemyInstance.logic.controller);
    }

    // --- Helpers tương đương decompile: set inactive, set parent pool, Add vào list ---

    private void PushModelToPool(EnemyModel enemyModel)
    {
        if (enemyModel == null) return;
        enemyModel.gameObject.SetActive(false);
        enemyModel.transform.parent = this.enemyModelPoolTransform;
        this.enemyModelPool.Add(enemyModel);
    }

    private void PushLogicToPool(EnemyLogic enemyLogic)
    {
        if (enemyLogic?.controller == null) return;
        enemyLogic.controller.gameObject.SetActive(false);
        enemyLogic.controller.transform.parent = this.enemyLogicPoolTransform;
        this.enemyLogicPool.Add(enemyLogic);
    }

    private int SelectZone(EnemyArchetype archetype)
	{
		zonesOccupancy.Clear();
		int i = 0;
		while (i < zonesCount)
		{
			int value = quantityMatrix.Find((EnemyMatrixElement matrixElem) => matrixElem.archetype == archetype && matrixElem.zoneId == i).value;
			int value2 = priorityMatrix.Find((EnemyMatrixElement matrixElem) => matrixElem.archetype == archetype && matrixElem.zoneId == i).value;
			float occupancyRatio = (float)value / (float)value2;
			if (value2 == 0)
			{
				occupancyRatio = float.MaxValue;
			}
			zonesOccupancy.Add(new ZoneOccupancyRatio
			{
				zoneId = i,
				occupancyRatio = occupancyRatio
			});
			int num = ++i;
		}
		zonesOccupancy.Sort((ZoneOccupancyRatio elem1, ZoneOccupancyRatio elem2) => elem1.occupancyRatio.CompareTo(elem2.occupancyRatio));
		return zonesOccupancy[0].zoneId;
	}

	private void Update()
	{
		if (!InitializeSpawnCompleted)
		{
			return;
		}
		if (simpleTimer <= 0f)
		{
			if ((float)SpawnedEnemies.Count((EnemyInstance x) => x.logic.currentScheme.Scheme.Type == EnemyScheme.SchemeType.Simple) < simpleMaximum.GetValue(PlayerLevel))
			{
				EnemyCurrentScheme enemyCurrentScheme = CreateScheme(EnemyScheme.SchemeType.Simple);
				if (enemyCurrentScheme != null)
				{
					simpleTimer = simpleFrequency.GetValue(PlayerLevel);
					SpawnEnemy(enemyCurrentScheme, isInitializeSpawn: false);
				}
			}
		}
		else
		{
			simpleTimer -= Time.deltaTime;
		}
	}

	public EnemyHabitatArea GetEnemyHabitatArea(EnemyController enemyController)
	{
		if (enemyController.CurrentScheme.Scheme.Type == EnemyScheme.SchemeType.Boss)
		{
			return null;
		}
		List<EnemyHabitatArea> list = EnemyHabitats.FindAll((EnemyHabitatArea habitat) => habitat.Archetypes.Contains(enemyController.CurrentScheme.Scheme.Archetype) && habitat.ZoneId == enemyController.CurrentScheme.zone && habitat.Inhabitants.Count == 0);
		if (list.Count > 0)
		{
			return list.Random();
		}
		ArchetypeAllies archetypeAllies = allyMatrix.Find((ArchetypeAllies x) => x.archetype == enemyController.CurrentScheme.Archetype);
		List<EnemyHabitatArea> list2 = EnemyHabitats.FindAll((EnemyHabitatArea habitat) => habitat.Inhabitants.Count > 0 && habitat.Archetypes.Contains(enemyController.CurrentScheme.Scheme.Archetype) && habitat.Inhabitants.Count < ManagerBase<EnemyManager>.Instance.MaxPopulationInHabitat && habitat.Inhabitants.All((EnemyController inhabitant) => archetypeAllies.allies.Contains(inhabitant.CurrentScheme.Archetype)) && habitat.ZoneId == enemyController.CurrentScheme.zone);
		if (list2.Count > 0)
		{
			list2.Sort((EnemyHabitatArea habitat1, EnemyHabitatArea habitat2) => habitat1.Inhabitants.Count.CompareTo(habitat2.Inhabitants.Count));
			int leastOccupiedHabitatOccupancy = list2[0].Inhabitants.Count;
			list2.RemoveAll((EnemyHabitatArea x) => x.Inhabitants.Count != leastOccupiedHabitatOccupancy);
			return list2.Random();
		}
		List<EnemyHabitatArea> list3 = EnemyHabitats.FindAll((EnemyHabitatArea habitat) => habitat.ZoneId == enemyController.CurrentScheme.zone && habitat.Inhabitants.Count < ManagerBase<EnemyManager>.Instance.MaxPopulationInHabitat);
		if (list3.Count > 0)
		{
			return list3.Random();
		}
		UnityEngine.Debug.LogError("Не найдена область обитания с ИД зоны " + enemyController.CurrentScheme.zone.ToString());
		return EnemyHabitats.Random();
	}

	public BossAssistantSpawnPoint GetFreeBossAssistantSpawnPoint()
	{
		return bossAssistantSpawnPoints.First((BossAssistantSpawnPoint x) => !x.IsBusy);
	}

	[CompilerGenerated]
	private void _003CInitializeSpawn_003Eg__TrySpawnBossAssistant_007C83_1()
	{
		int value = (int)Math.Truncate((decimal)(ManagerBase<EnemyManager>.Instance.killedBosses / enemyArchetypes.Count));
		value = Mathf.Clamp(value, 0, ManagerBase<EnemyManager>.Instance.MaxBossAssistantCount);
		for (int i = 0; i < value; i++)
		{
			EnemyCurrentScheme enemyCurrentScheme = CreateScheme(EnemyScheme.SchemeType.BossAssistant);
			SpawnEnemy(enemyCurrentScheme, isInitializeSpawn: true);
		}
	}
}
