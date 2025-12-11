using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestSpawner : Singleton<QuestSpawner>
{
	[Postfix(PostfixAttribute.Id.Seconds)]
	[SerializeField]
	private float betweenQuestsPauseDuration = 3f;

	private float betweenQuestsPauseTimer;

	[Header("Ссылки")]
	[SerializeField]
	private List<Quest> startQuests;

	private QuestCategory importantQuestCategory;

	private List<QuestCategory> categories;

	private List<StandartQuestCategory> standartCategories = new List<StandartQuestCategory>();

	private List<Quest> _quests;

	[SerializeField]
	[ReadonlyInspector]
	private Quest _activeQuest;

	[SerializeField]
	[ReadonlyInspector]
	private Quest prevQuest;

	public static Action initializeEvent;

	public float BetweenQuestsPauseDuration => betweenQuestsPauseDuration;

	public List<Quest> Quests
	{
		get
		{
			if (_quests == null || _quests.Count == 0)
			{
				_quests = new List<Quest>(GetComponentsInChildren<Quest>());
			}
			return _quests;
		}
	}

	public Quest ActiveQuest
	{
		get
		{
			return _activeQuest;
		}
		set
		{
			_activeQuest = value;
		}
	}

	public bool HaveActiveQuest => ActiveQuest != null;

	private bool NeedForceRunSatisfyHungerQuest
	{
		get
		{
			if (ManagerBase<PlayerManager>.Instance.satietyCurrent == 0f)
			{
				return ManagerBase<PlayerManager>.Instance.healthCurrent / ManagerBase<PlayerManager>.Instance.HealthMaximum <= 0.8f;
			}
			return false;
		}
	}

	public bool InitializeSpawnCompleted
	{
		get;
		private set;
	}

	protected override void OnInit()
	{
		categories = new List<QuestCategory>(GetComponentsInChildren<QuestCategory>());
		importantQuestCategory = categories.Find((QuestCategory x) => x is ImportantQuestCategory);
		foreach (QuestCategory category in categories)
		{
			if (category is StandartQuestCategory)
			{
				standartCategories.Add(category as StandartQuestCategory);
			}
		}
		categories.ForEach(delegate(QuestCategory x)
		{
			x.Init();
		});
		startQuests.RemoveAll((Quest x) => !x.gameObject.activeSelf);
		PlayerSpawner.spawnPlayerEvent += TryInitializeSpawn;
		if (Singleton<EnemySpawner>.Instance != null)
		{
			Singleton<EnemySpawner>.Instance.initializeSpawnCompletedEvent += TryInitializeSpawn;
		}
		if (Singleton<FarmResidentSpawner>.Instance != null)
		{
			Singleton<FarmResidentSpawner>.Instance.initializeSpawnCompletedEvent += TryInitializeSpawn;
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
		Quest.completeEvent -= OnCompleteQuest;
	}

	private void TryInitializeSpawn()
	{
		if ((Singleton<EnemySpawner>.Instance == null || Singleton<EnemySpawner>.Instance.InitializeSpawnCompleted) && (Singleton<FarmResidentSpawner>.Instance == null || Singleton<FarmResidentSpawner>.Instance.InitializeSpawnCompleted) && PlayerSpawner.IsPlayerSpawned && !InitializeSpawnCompleted)
		{
			PlayerSpawner.spawnPlayerEvent -= TryInitializeSpawn;
			if (Singleton<EnemySpawner>.Instance != null)
			{
				Singleton<EnemySpawner>.Instance.initializeSpawnCompletedEvent -= TryInitializeSpawn;
			}
			if (Singleton<FarmResidentSpawner>.Instance != null)
			{
				Singleton<FarmResidentSpawner>.Instance.initializeSpawnCompletedEvent -= TryInitializeSpawn;
			}
			Quest.completeEvent += OnCompleteQuest;
			StartNewQuest();
			InitializeSpawnCompleted = true;
		}
	}

	private Quest SelectQuest()
	{
		Quest quest = importantQuestCategory.SelectQuest();
		if (quest != null)
		{
			return quest;
		}
		if (startQuests.Count != ManagerBase<QuestManager>.Instance.passedStartQuests.Count)
		{
			foreach (Quest startQuest in startQuests)
			{
				if (!ManagerBase<QuestManager>.Instance.passedStartQuests.Contains(startQuest.name) && startQuest.CanStart())
				{
					if (startQuest.Category is StandartQuestCategory)
					{
						int num = --(startQuest.Category as StandartQuestCategory).CurPriority;
					}
					return startQuest;
				}
			}
		}
		List<StandartQuestCategory> list = new List<StandartQuestCategory>(standartCategories);
		while (list.Count > 0)
		{
			if (standartCategories.All((StandartQuestCategory x) => x.CurPriority == 0))
			{
				standartCategories.ForEach(delegate(StandartQuestCategory x)
				{
					x.CurPriority = x.Priority;
				});
			}
			StandartQuestCategory standartQuestCategory = list.Random((StandartQuestCategory x) => x.CurPriority);
			int num = --standartQuestCategory.CurPriority;
			list.Remove(standartQuestCategory);
			Quest quest2 = standartQuestCategory.SelectQuest();
			if (quest2 != null)
			{
				return quest2;
			}
		}
		return null;
	}

	public void CancelQuest()
	{
		if (!(ActiveQuest == null))
		{
			prevQuest = ActiveQuest;
			if (startQuests.Contains(ActiveQuest) && !ManagerBase<QuestManager>.Instance.passedStartQuests.Contains(ActiveQuest.name))
			{
				ManagerBase<QuestManager>.Instance.passedStartQuests.Add(ActiveQuest.name);
			}
			ActiveQuest.CancelQuest();
			ActiveQuest = null;
			betweenQuestsPauseTimer = betweenQuestsPauseDuration;
		}
	}

	private void StartNewQuest()
	{
		Quest quest = SelectQuest();
		if (quest != null && quest.CanStart())
		{
			ActiveQuest = quest;
			quest.StartQuest();
			if (quest.IsInProgress)
			{
				ActiveQuest = quest;
			}
		}
		else
		{
			betweenQuestsPauseTimer = betweenQuestsPauseDuration;
		}
	}

	private void OnCompleteQuest(Quest quest)
	{
		if (startQuests.Contains(quest) && !ManagerBase<QuestManager>.Instance.passedStartQuests.Contains(quest.name))
		{
			ManagerBase<QuestManager>.Instance.passedStartQuests.Add(quest.name);
		}
		prevQuest = quest;
		ActiveQuest = null;
		betweenQuestsPauseTimer = betweenQuestsPauseDuration;
	}

	private void Update()
	{
		if (!PlayerSpawner.IsPlayerSpawned)
		{
			return;
		}
		if (NeedForceRunSatisfyHungerQuest && !(ActiveQuest as SatisfyHungerQuest))
		{
			CancelQuest();
			Quest quest = Quests.Find((Quest x) => x as SatisfyHungerQuest);
			if (quest.CanStart())
			{
				ActiveQuest = quest;
				quest.StartQuest();
				if (quest.IsInProgress)
				{
					ActiveQuest = quest;
				}
			}
		}
		if (ActiveQuest == null)
		{
			if (betweenQuestsPauseTimer > 0f)
			{
				betweenQuestsPauseTimer -= Time.deltaTime;
			}
			else
			{
				StartNewQuest();
			}
		}
	}
}
