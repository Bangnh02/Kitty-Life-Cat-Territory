using Avelog;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldRelativePanelPriorityController : MonoBehaviour, IInitializableUI
{
	private enum QuestType
	{
		None,
		Hunting,
		QuenchThirst,
		SatisfyHunger,
		GetSpouse,
		HelpFarmResident,
		EducateChild
	}

	[SerializeField]
	private float rotationSpeed = 100f;

	private List<WorldRelativePanel> priorityPanels = new List<WorldRelativePanel>();

	public static WorldRelativePanelPriorityController Instance
	{
		get;
		private set;
	}

	public Quaternion PriorityRingRotation
	{
		get;
		private set;
	} = new Quaternion(0f, 0f, 0f, 1f);


	private bool HaveActiveQuest => Singleton<QuestSpawner>.Instance.HaveActiveQuest;

	private List<WorldRelativePanel> AllWorldRelativePanels => WorldRelativePanel.Instances;

	private PlayerBrain Player => PlayerSpawner.PlayerInstance;

	private ActorPicker PlayerPicker => PlayerSpawner.PlayerInstance.PlayerPicker;

	public void OnInitializeUI()
	{
		Instance = this;
		Quest.startEvent += OnQuestStart;
		Quest.completeEvent += OnQuestComplete;
	}

	private void OnDestroy()
	{
		Quest.startEvent -= OnQuestStart;
		Quest.completeEvent -= OnQuestComplete;
	}

	private void Update()
	{
		if (HaveActiveQuest)
		{
			switch (GetQuestType())
			{
			case QuestType.Hunting:
				if ((bool)(Singleton<QuestSpawner>.Instance.ActiveQuest as HuntingQuest))
				{
					HuntingQuest huntingQuest = (HuntingQuest)Singleton<QuestSpawner>.Instance.ActiveQuest;
					GetActiveEnemyPanels(huntingQuest.ArchetypeName);
				}
				else
				{
					HuntingMiniBossQuest huntingMiniBossQuest = (HuntingMiniBossQuest)Singleton<QuestSpawner>.Instance.ActiveQuest;
					GetActiveMiniBossPanels();
				}
				break;
			case QuestType.QuenchThirst:
				GetActiveWaterPanels();
				break;
			case QuestType.SatisfyHunger:
				GetActiveFoodPanels();
				break;
			case QuestType.GetSpouse:
				GetPotentialSpouseData();
				break;
			case QuestType.HelpFarmResident:
			{
				HelpFarmResidentQuest helpFarmResidentQuest = (HelpFarmResidentQuest)Singleton<QuestSpawner>.Instance.ActiveQuest;
				FarmResident farmResident = Singleton<FarmResidentSpawner>.Instance.SpawnedFarmResidents.Find((FarmResident x) => x.FarmResidentData.farmResidentId == helpFarmResidentQuest.FarmResidentId);
				string needToFarmResident = farmResident.FarmResidentData.curNeed;
				if (PlayerPicker.HavePickedItem && ((Food)PlayerPicker.PickedItem).Name == needToFarmResident)
				{
					GetFarmResidentData(farmResident.FarmResidentData.farmResidentId);
					if (priorityPanels.Count == 0)
					{
						farmResident.ForceShowPanel = true;
					}
					break;
				}
				if (farmResident.ForceShowPanel)
				{
					farmResident.ForceShowPanel = false;
				}
				if (EnumUtils.ToList<ItemId>().Any((ItemId x) => x.ToString() == needToFarmResident))
				{
					GetActiveFoodPanels(needToFarmResident);
				}
				else if (Singleton<ItemSpawner>.Instance.EnemySpawnItems != null && Singleton<ItemSpawner>.Instance.EnemySpawnItems.spawnedObjects.Count((GameObject x) => x.GetComponent<EnemyFood>().Name == needToFarmResident) > 0)
				{
					GetActiveFoodPanels(needToFarmResident);
				}
				else
				{
					GetActiveEnemyPanels(needToFarmResident);
				}
				break;
			}
			case QuestType.EducateChild:
			{
				FamilyManager.FamilyMemberData growingChild = ManagerBase<FamilyManager>.Instance.GrowingChild;
				string needToChild = growingChild.curNeed;
				if (string.IsNullOrEmpty(needToChild))
				{
					DisableRings();
				}
				else if (EnumUtils.ToList<ItemId>().Any((ItemId x) => x.ToString() == needToChild))
				{
					GetActiveFoodPanels(needToChild);
				}
				else
				{
					GetActiveEnemyPanels(needToChild);
				}
				break;
			}
			default:
				if (priorityPanels.Count != 0)
				{
					priorityPanels.Clear();
				}
				break;
			}
			ActivateRings();
		}
		else
		{
			DisableRings();
		}
		PriorityRingRotation *= Quaternion.Euler(0f, 0f, rotationSpeed * Time.deltaTime);
	}

	private QuestType GetQuestType()
	{
		Quest activeQuest = Singleton<QuestSpawner>.Instance.ActiveQuest;
		if (activeQuest is HuntingQuest)
		{
			return QuestType.Hunting;
		}
		if (activeQuest is QuenchThirstQuest)
		{
			return QuestType.QuenchThirst;
		}
		if (activeQuest is SatisfyHungerQuest || activeQuest is FeedFamilyQuest)
		{
			return QuestType.SatisfyHunger;
		}
		if (activeQuest is GetSpouseQuest)
		{
			return QuestType.GetSpouse;
		}
		if (activeQuest is HelpFarmResidentQuest)
		{
			return QuestType.HelpFarmResident;
		}
		if (activeQuest is EducateChildQuest)
		{
			return QuestType.EducateChild;
		}
		return QuestType.None;
	}

	private void GetActiveEnemyPanels(string archetype)
	{
		priorityPanels.Clear();
		foreach (WorldRelativePanel allWorldRelativePanel in AllWorldRelativePanels)
		{
			if (allWorldRelativePanel.gameObject.activeSelf && allWorldRelativePanel is EnemyInfoPanel && ((EnemyInfoPanel)allWorldRelativePanel).EnemyController.CurrentScheme.Archetype.name == archetype)
			{
				priorityPanels.Add(allWorldRelativePanel);
			}
		}
	}

	private void GetActiveMiniBossPanels()
	{
		priorityPanels.Clear();
		foreach (WorldRelativePanel allWorldRelativePanel in AllWorldRelativePanels)
		{
			if (allWorldRelativePanel.gameObject.activeSelf && allWorldRelativePanel is EnemyInfoPanel && ((EnemyInfoPanel)allWorldRelativePanel).EnemyController.CurrentScheme.Scheme.Type == EnemyScheme.SchemeType.MiniBoss)
			{
				priorityPanels.Add(allWorldRelativePanel);
			}
		}
	}

	private void GetActiveWaterPanels()
	{
		priorityPanels.Clear();
		foreach (WorldRelativePanel allWorldRelativePanel in AllWorldRelativePanels)
		{
			if (allWorldRelativePanel.gameObject.activeSelf && allWorldRelativePanel is WaterInfoPanel)
			{
				priorityPanels.Add(allWorldRelativePanel);
			}
		}
	}

	private void GetActiveFoodPanels(string foodName = null)
	{
		priorityPanels.Clear();
		foreach (WorldRelativePanel allWorldRelativePanel in AllWorldRelativePanels)
		{
			if (allWorldRelativePanel.gameObject.activeSelf && allWorldRelativePanel is FoodInfoPanel && !((FoodInfoPanel)allWorldRelativePanel).Food.IsBadFood && (string.IsNullOrEmpty(foodName) || !(((FoodInfoPanel)allWorldRelativePanel).Food.Name != foodName)))
			{
				priorityPanels.Add(allWorldRelativePanel);
			}
		}
	}

	private void GetPotentialSpouseData()
	{
		priorityPanels.Clear();
		foreach (WorldRelativePanel allWorldRelativePanel in AllWorldRelativePanels)
		{
			if (allWorldRelativePanel.gameObject.activeSelf && allWorldRelativePanel is PotentialSpouseInfoPanel)
			{
				priorityPanels.Add(allWorldRelativePanel);
			}
		}
	}

	private void GetFarmResidentData(FarmResidentId farmResidentId)
	{
		priorityPanels.Clear();
		foreach (WorldRelativePanel allWorldRelativePanel in AllWorldRelativePanels)
		{
			if (allWorldRelativePanel.gameObject.activeSelf && allWorldRelativePanel is FarmResidentPanel && ((FarmResidentPanel)allWorldRelativePanel).FarmResidentId == farmResidentId)
			{
				priorityPanels.Add(allWorldRelativePanel);
			}
		}
	}

	private void ActivateRings()
	{
		priorityPanels.ForEach(delegate(WorldRelativePanel x)
		{
			x.IconPriorityRing?.gameObject.SetActive(value: true);
		});
		foreach (WorldRelativePanel allWorldRelativePanel in AllWorldRelativePanels)
		{
			if (allWorldRelativePanel.IconPriorityRing != null && allWorldRelativePanel.IconPriorityRing.gameObject.activeSelf && !priorityPanels.Contains(allWorldRelativePanel))
			{
				allWorldRelativePanel.IconPriorityRing.gameObject.SetActive(value: false);
			}
		}
	}

	private void DisableRings()
	{
		AllWorldRelativePanels.ForEach(delegate(WorldRelativePanel x)
		{
			x.IconPriorityRing?.gameObject.SetActive(value: false);
		});
	}

	private void OnQuestStart(Quest quest)
	{
		DisableRings();
	}

	private void OnQuestComplete(Quest quest)
	{
		if ((bool)(quest as HelpFarmResidentQuest))
		{
			Singleton<FarmResidentSpawner>.Instance.SpawnedFarmResidents.Find((FarmResident x) => x.FarmResidentData.farmResidentId == ((HelpFarmResidentQuest)quest).FarmResidentId).ForceShowPanel = false;
		}
	}
}
