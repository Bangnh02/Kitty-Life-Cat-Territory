using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExternalPanels : MonoBehaviour, IInitializableUI
{
	private class ExternalPanelTarget
	{
		public ExternalPanelsData panelsData;

		public ExternalPanel panel;

		public GameObject targetGO;

		public float sqrDistance;

		public bool isVisible;
	}

	private class EnemyTarget : ExternalPanelTarget
	{
		public EnemyModel enemyModel;
	}

	private class VegetableTarget : ExternalPanelTarget
	{
		public VegetableBehaviour vegetable;
	}

	private class FamilyMemberTarget : ExternalPanelTarget
	{
		public FamilyMemberController familyMemberController;
	}

	private class ExternalPanel
	{
		public ExternalPanelTarget target;

		public WorldRelativePanel worldRelativePanel;

		public ProcessingSwitch processingSwitch;
	}

	[Serializable]
	private abstract class ExternalPanelsData
	{
		[SerializeField]
		public GameObject prefab;

		private WorldRelativePanel prefabWorldRelativePanel;

		private ProcessingSwitch prefabProcessingSwitch;

		[HideInInspector]
		public List<ExternalPanelTarget> targets = new List<ExternalPanelTarget>();

		[HideInInspector]
		public List<ExternalPanel> activePanels = new List<ExternalPanel>();

		[HideInInspector]
		public List<ExternalPanel> pool = new List<ExternalPanel>();

		public WorldRelativePanel PrefabWorldRelativePanel
		{
			get
			{
				if (prefabWorldRelativePanel == null)
				{
					prefabWorldRelativePanel = prefab.GetComponent<WorldRelativePanel>();
				}
				return prefabWorldRelativePanel;
			}
		}

		public ProcessingSwitch PrefabProcessingSwitch
		{
			get
			{
				if (prefabProcessingSwitch == null)
				{
					prefabProcessingSwitch = prefab.GetComponent<ProcessingSwitch>();
				}
				return prefabProcessingSwitch;
			}
		}

		public abstract void SetupExternalPanel(ExternalPanel externalPanel, ExternalPanelTarget externalPanelTarget);

		public void UpdateTargetProcessingData(ExternalPanelTarget target)
		{
			(bool, float) processingData = GetProcessingData(target, this);
			target.sqrDistance = processingData.Item2;
			target.isVisible = processingData.Item1;
		}

		public virtual (bool isProcessing, float sqrDistance) GetProcessingData(ExternalPanelTarget externalPanelTarget, ExternalPanelsData externalPanelsData)
		{
			return externalPanelsData.PrefabProcessingSwitch.MeasureDistance(externalPanelTarget.targetGO.transform.position);
		}
	}

	[Serializable]
	private class FoodPanelsData : ExternalPanelsData
	{
		public override void SetupExternalPanel(ExternalPanel externalPanel, ExternalPanelTarget externalPanelTarget)
		{
			FoodInfoPanel obj = externalPanel.worldRelativePanel as FoodInfoPanel;
			Food componentInChildren = externalPanelTarget.targetGO.GetComponentInChildren<Food>(includeInactive: true);
			obj.Spawn(componentInChildren);
		}
	}

	[Serializable]
	private class WaterPanelsData : ExternalPanelsData
	{
		public override void SetupExternalPanel(ExternalPanel externalPanel, ExternalPanelTarget externalPanelTarget)
		{
			WaterInfoPanel obj = externalPanel.worldRelativePanel as WaterInfoPanel;
			Water componentInChildren = externalPanelTarget.targetGO.GetComponentInChildren<Water>(includeInactive: true);
			obj.Spawn(componentInChildren);
		}
	}

	[Serializable]
	private class FarmResidentPanelsData : ExternalPanelsData
	{
		public override void SetupExternalPanel(ExternalPanel externalPanel, ExternalPanelTarget externalPanelTarget)
		{
			FarmResidentPanel obj = externalPanel.worldRelativePanel as FarmResidentPanel;
			FarmResident componentInChildren = externalPanelTarget.targetGO.GetComponentInChildren<FarmResident>(includeInactive: true);
			obj.Spawn(componentInChildren);
		}

		public override (bool isProcessing, float sqrDistance) GetProcessingData(ExternalPanelTarget externalPanelTarget, ExternalPanelsData externalPanelsData)
		{
			FarmResident componentInChildren = externalPanelTarget.targetGO.GetComponentInChildren<FarmResident>(includeInactive: true);
			if (componentInChildren.ForceShowPanel)
			{
				if (externalPanelTarget.panel != null && externalPanelTarget.panel.processingSwitch.maxProcessingDistance != float.MaxValue)
				{
					externalPanelTarget.panel.processingSwitch.maxProcessingDistance = float.MaxValue;
				}
				(bool, float) processingData = base.GetProcessingData(externalPanelTarget, externalPanelsData);
				return (true, processingData.Item2);
			}
			if (!componentInChildren.FarmResidentData.HaveNeed)
			{
				if (externalPanelTarget.panel != null && externalPanelTarget.panel.processingSwitch.maxProcessingDistance != 0f)
				{
					externalPanelTarget.panel.processingSwitch.maxProcessingDistance = 0f;
				}
				return (false, 0f);
			}
			if (externalPanelTarget.panel != null && externalPanelTarget.panel.processingSwitch.maxProcessingDistance != Singleton<FarmResidentSpawner>.Instance.InfoPanelProcessingDistance)
			{
				externalPanelTarget.panel.processingSwitch.maxProcessingDistance = Singleton<FarmResidentSpawner>.Instance.InfoPanelProcessingDistance;
			}
			return base.GetProcessingData(externalPanelTarget, externalPanelsData);
		}
	}

	[Serializable]
	private class EnemyPanelsData : ExternalPanelsData
	{
		public override void SetupExternalPanel(ExternalPanel externalPanel, ExternalPanelTarget externalPanelTarget)
		{
			EnemyInfoPanel obj = externalPanel.worldRelativePanel as EnemyInfoPanel;
			EnemyModel enemyModel = (externalPanelTarget as EnemyTarget).enemyModel;
			obj.Spawn(enemyModel.EnemyController);
		}

		public override (bool isProcessing, float sqrDistance) GetProcessingData(ExternalPanelTarget externalPanelTarget, ExternalPanelsData externalPanelsData)
		{
			return EnemyInfoPanel.GetProcessingData((externalPanelTarget as EnemyTarget).enemyModel.EnemyController);
		}
	}

	[Serializable]
	private class FamilyMemberPanelsData : ExternalPanelsData
	{
		public override void SetupExternalPanel(ExternalPanel externalPanel, ExternalPanelTarget externalPanelTarget)
		{
			ChildDemandPanel obj = externalPanel.worldRelativePanel as ChildDemandPanel;
			FamilyMemberController componentInChildren = externalPanelTarget.targetGO.GetComponentInChildren<FamilyMemberController>(includeInactive: true);
			obj.Spawn(componentInChildren.familyMemberData);
		}

		public override (bool isProcessing, float sqrDistance) GetProcessingData(ExternalPanelTarget externalPanelTarget, ExternalPanelsData externalPanelsData)
		{
			return ChildDemandPanel.GetProcessingData((externalPanelTarget as FamilyMemberTarget).familyMemberController);
		}
	}

	[Serializable]
	private class VegetablePanelsData : ExternalPanelsData
	{
		public override void SetupExternalPanel(ExternalPanel externalPanel, ExternalPanelTarget externalPanelTarget)
		{
			VegetableInfoPanel obj = externalPanel.worldRelativePanel as VegetableInfoPanel;
			VegetableBehaviour componentInChildren = externalPanelTarget.targetGO.GetComponentInChildren<VegetableBehaviour>(includeInactive: true);
			obj.Spawn(componentInChildren);
		}

		public override (bool isProcessing, float sqrDistance) GetProcessingData(ExternalPanelTarget externalPanelTarget, ExternalPanelsData externalPanelsData)
		{
			return VegetableInfoPanel.GetProcessingData((externalPanelTarget as VegetableTarget).vegetable);
		}
	}

	[Serializable]
	private class PotentialSpousePanelsData : ExternalPanelsData
	{
		public override void SetupExternalPanel(ExternalPanel externalPanel, ExternalPanelTarget externalPanelTarget)
		{
			PotentialSpouseInfoPanel obj = externalPanel.worldRelativePanel as PotentialSpouseInfoPanel;
			PotentialSpouseController componentInChildren = externalPanelTarget.targetGO.GetComponentInChildren<PotentialSpouseController>(includeInactive: true);
			obj.Spawn(componentInChildren);
		}

		public override (bool isProcessing, float sqrDistance) GetProcessingData(ExternalPanelTarget externalPanelTarget, ExternalPanelsData externalPanelsData)
		{
			if (ManagerBase<FamilyManager>.Instance.HaveSpouse)
			{
				return (false, float.MaxValue);
			}
			return base.GetProcessingData(externalPanelTarget, externalPanelsData);
		}
	}

	private static ExternalPanels instance;

	[SerializeField]
	private float updatePanelsFrequency = 0.2f;

	private float updatePanelsTimer;

	[Header("Ссылки")]
	[SerializeField]
	private FoodPanelsData foodPanelsData;

	[SerializeField]
	private WaterPanelsData waterPanelsData;

	[SerializeField]
	private FarmResidentPanelsData farmResidentPanelsData;

	[SerializeField]
	private EnemyPanelsData enemyPanelsData;

	[SerializeField]
	private FamilyMemberPanelsData familyMemberPanelsData;

	[SerializeField]
	private VegetablePanelsData vegetablePanelsData;

	[SerializeField]
	private PotentialSpousePanelsData potentialSpousePanelsData;

	private List<ExternalPanel> allActivePanels = new List<ExternalPanel>();

	private List<ExternalPanelsData> allPanelsData = new List<ExternalPanelsData>();

	private List<ExternalPanelTarget> allTargets = new List<ExternalPanelTarget>();

	public static ExternalPanels Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType<ExternalPanels>();
			}
			return instance;
		}
	}

	public void OnInitializeUI()
	{
		instance = this;
		ProcessingSwitch.switchEvent += OnProcessingSwitch;
		allPanelsData.Add(foodPanelsData);
		allPanelsData.Add(waterPanelsData);
		allPanelsData.Add(farmResidentPanelsData);
		allPanelsData.Add(enemyPanelsData);
		allPanelsData.Add(familyMemberPanelsData);
		allPanelsData.Add(vegetablePanelsData);
		allPanelsData.Add(potentialSpousePanelsData);
		new List<Food>();
		UnityEngine.Object.FindObjectsOfType<Food>().ToList().ForEach(delegate(Food food)
		{
			OnFoodSpawn(food);
		});
		ItemSpawner.foodSpawnEvent += OnFoodSpawn;
		Food.unspawnEvent += OnFoodUnspawn;
		UnityEngine.Object.FindObjectsOfType<Water>().ToList().ForEach(delegate(Water water)
		{
			OnWaterSpawn(water);
		});
		Water.spawnEvent = (Water.SpawnHandler)Delegate.Combine(Water.spawnEvent, new Water.SpawnHandler(OnWaterSpawn));
		if (Singleton<FarmResidentSpawner>.Instance != null)
		{
			Singleton<FarmResidentSpawner>.Instance.SpawnedFarmResidents.ForEach(delegate(FarmResident farmResident)
			{
				OnFarmResidentSpawn(farmResident);
			});
		}
		FarmResidentSpawner.spawnEvent += OnFarmResidentSpawn;
		EnemyController.Instances.ForEach(delegate(EnemyController enemy)
		{
			OnEnemySpawn(enemy);
		});
		EnemySpawner.spawnEvent += OnEnemySpawn;
		EnemySpawner.unspawnEvent += OnEnemyUnspawn;
		FamilyMemberController.Instances.ForEach(delegate(FamilyMemberController controller)
		{
			OnFamilyMemberControllerSpawn(controller);
		});
		FamilyMemberController.spawnEvent += OnFamilyMemberControllerSpawn;
		VegetableBehaviour.Instances.ForEach(delegate(VegetableBehaviour vegetable)
		{
			OnVegetableSpawn(vegetable);
		});
		VegetableBehaviour.spawnEvent += OnVegetableSpawn;
		PotentialSpouseController.Instances.ForEach(delegate(PotentialSpouseController potentialSpouse)
		{
			OnPotentialSpouseSpawn(potentialSpouse);
		});
		SpouseSpawner.potentialSpouseSpawnEvent += OnPotentialSpouseSpawn;
	}

	private void OnDestroy()
	{
		ProcessingSwitch.switchEvent -= OnProcessingSwitch;
		ItemSpawner.foodSpawnEvent -= OnFoodSpawn;
		Food.unspawnEvent -= OnFoodUnspawn;
		Water.spawnEvent = (Water.SpawnHandler)Delegate.Remove(Water.spawnEvent, new Water.SpawnHandler(OnWaterSpawn));
		FarmResidentSpawner.spawnEvent -= OnFarmResidentSpawn;
		EnemySpawner.spawnEvent -= OnEnemySpawn;
		EnemySpawner.unspawnEvent -= OnEnemyUnspawn;
		FamilyMemberController.spawnEvent -= OnFamilyMemberControllerSpawn;
		VegetableBehaviour.spawnEvent -= OnVegetableSpawn;
		SpouseSpawner.potentialSpouseSpawnEvent -= OnPotentialSpouseSpawn;
	}

	private void OnProcessingSwitch(ProcessingSwitch processingSwitch)
	{
		if (processingSwitch.CurSwitchState == ProcessingSwitch.SwitchState.Disabled)
		{
			ExternalPanel externalPanel = allActivePanels.Find((ExternalPanel x) => x.processingSwitch == processingSwitch);
			if (externalPanel != null)
			{
				ExternalPanelsData externalPanelsData = allPanelsData.Find((ExternalPanelsData x) => x.activePanels.Contains(externalPanel));
				PushPanelToPool(externalPanel, externalPanelsData);
			}
		}
	}

	private void OnFoodSpawn(Food food)
	{
		if (food.Eatable)
		{
			OnSpawnTarget(food.gameObject, foodPanelsData);
		}
	}

	private void OnFoodUnspawn(Food food)
	{
		OnUnspawnTarget(food.gameObject, foodPanelsData);
	}

	private void OnWaterSpawn(Water water)
	{
		if (water.HaveDrinkPoints)
		{
			OnSpawnTarget(water.gameObject, waterPanelsData);
		}
	}

	private void OnFarmResidentSpawn(FarmResident farmResident)
	{
		OnSpawnTarget(farmResident.gameObject, farmResidentPanelsData);
	}

	private void OnEnemySpawn(EnemyController enemyController)
	{
		EnemyTarget item = new EnemyTarget
		{
			targetGO = enemyController.EnemyModel.gameObject,
			enemyModel = enemyController.EnemyModel,
			panelsData = enemyPanelsData
		};
		enemyPanelsData.targets.Add(item);
		allTargets.Add(item);
	}

	private void OnEnemyUnspawn(EnemyController enemyController)
	{
		OnUnspawnTarget(enemyController.EnemyModel.gameObject, enemyPanelsData);
	}

	private void OnFamilyMemberControllerSpawn(FamilyMemberController familyMemberController)
	{
		if (familyMemberController.familyMemberData.role != FamilyManager.FamilyMemberRole.Spouse && familyMemberController.familyMemberData.role != FamilyManager.FamilyMemberRole.ThirdStageChild)
		{
			FamilyMemberTarget item = new FamilyMemberTarget
			{
				targetGO = familyMemberController.gameObject,
				familyMemberController = familyMemberController,
				panelsData = familyMemberPanelsData
			};
			familyMemberPanelsData.targets.Add(item);
			allTargets.Add(item);
		}
	}

	private void OnVegetableSpawn(VegetableBehaviour vegetable)
	{
		VegetableTarget item = new VegetableTarget
		{
			targetGO = vegetable.gameObject,
			vegetable = vegetable,
			panelsData = vegetablePanelsData
		};
		vegetablePanelsData.targets.Add(item);
		allTargets.Add(item);
	}

	private void OnPotentialSpouseSpawn(PotentialSpouseController potentialSpouse)
	{
		OnSpawnTarget(potentialSpouse.gameObject, potentialSpousePanelsData);
	}

	private void OnSpawnTarget(GameObject target, ExternalPanelsData externalPanelsData)
	{
		ExternalPanelTarget item = new ExternalPanelTarget
		{
			targetGO = target,
			panelsData = externalPanelsData
		};
		externalPanelsData.targets.Add(item);
		allTargets.Add(item);
	}

	private void OnUnspawnTarget(GameObject target, ExternalPanelsData externalPanelsData)
	{
		ExternalPanelTarget externalPanelTarget = externalPanelsData.targets.Find((ExternalPanelTarget x) => x.targetGO == target);
		if (externalPanelTarget != null)
		{
			externalPanelsData.targets.Remove(externalPanelTarget);
			allTargets.Remove(externalPanelTarget);
			if (externalPanelTarget.panel != null)
			{
				PushPanelToPool(externalPanelTarget.panel, externalPanelsData);
			}
		}
	}

	private void Update()
	{
		updatePanelsTimer -= Time.deltaTime;
		if (updatePanelsTimer <= 0f)
		{
			updatePanelsTimer = updatePanelsFrequency;
			for (int i = 0; i < allTargets.Count; i++)
			{
				allTargets[i].panelsData.UpdateTargetProcessingData(allTargets[i]);
				if (allTargets[i].isVisible && allTargets[i].panel == null)
				{
					allTargets[i].panel = GetPanelFromPool(allTargets[i].panelsData, allTargets[i]);
				}
			}
		}
		allActivePanels.Sort(delegate(ExternalPanel x, ExternalPanel y)
		{
			if (PlayerSpawner.IsPlayerSpawned && PlayerSpawner.PlayerInstance.PlayerCombat.InCombat)
			{
				EnemyTarget enemyTarget = x.target as EnemyTarget;
				EnemyTarget enemyTarget2 = y.target as EnemyTarget;
				if (enemyTarget != null && enemyTarget2 != null)
				{
					if (enemyTarget.enemyModel.EnemyController.MyCombat == PlayerSpawner.PlayerInstance.PlayerCombat.RecommendedTargetForAttack)
					{
						return -1;
					}
					if (enemyTarget2.enemyModel.EnemyController.MyCombat == PlayerSpawner.PlayerInstance.PlayerCombat.RecommendedTargetForAttack)
					{
						return 1;
					}
				}
				else
				{
					if (enemyTarget != null)
					{
						return -1;
					}
					if (enemyTarget2 != null)
					{
						return 1;
					}
				}
			}
			return x.target.sqrDistance.CompareTo(y.target.sqrDistance);
		});
		for (int j = 0; j < allActivePanels.Count; j++)
		{
			allActivePanels[j].worldRelativePanel.transform.SetSiblingIndex(allActivePanels.Count - 1 - j);
		}
	}

	private ExternalPanel GetPanelFromPool(ExternalPanelsData externalPanelsData, ExternalPanelTarget externalPanelTarget)
	{
		ExternalPanel externalPanel = null;
		if (externalPanelsData.pool.Count > 0)
		{
			externalPanel = externalPanelsData.pool[0];
			externalPanelsData.pool.Remove(externalPanel);
		}
		else
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(externalPanelsData.prefab, base.transform);
			externalPanel = new ExternalPanel
			{
				target = externalPanelTarget,
				processingSwitch = gameObject.GetComponent<ProcessingSwitch>(),
				worldRelativePanel = gameObject.GetComponent<WorldRelativePanel>()
			};
		}
		externalPanel.target = externalPanelTarget;
		externalPanelTarget.panel = externalPanel;
		externalPanelsData.SetupExternalPanel(externalPanel, externalPanelTarget);
		externalPanel.worldRelativePanel.gameObject.SetActive(value: true);
		externalPanelsData.activePanels.Add(externalPanel);
		allActivePanels.Add(externalPanel);
		return externalPanel;
	}

	private void PushPanelToPool(ExternalPanel externalPanel, ExternalPanelsData externalPanelsData)
	{
		ExternalPanelTarget target = externalPanel.target;
		target.panel.worldRelativePanel.gameObject.SetActive(value: false);
		target.panel.worldRelativePanel.transform.SetAsFirstSibling();
		externalPanelsData.pool.Add(target.panel);
		externalPanelsData.activePanels.Remove(target.panel);
		allActivePanels.Remove(target.panel);
		target.panel.target = null;
		target.panel = null;
	}
}
