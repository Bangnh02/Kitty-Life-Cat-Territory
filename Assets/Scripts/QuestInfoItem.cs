using System;
using UnityEngine;

public abstract class QuestInfoItem : MonoBehaviour, IInitializableUI
{
	private QuestGoalPanel _questGoalPanel;

	protected bool IsInitialized
	{
		get;
		private set;
	}

	protected QuestGoalPanel QuestGoalPanel
	{
		get
		{
			Transform transform = base.transform;
			while (_questGoalPanel == null)
			{
				transform = transform.parent;
				_questGoalPanel = transform.GetComponentInChildren<QuestGoalPanel>(includeInactive: true);
			}
			return _questGoalPanel;
		}
	}

	public void OnInitializeUI()
	{
		Initialize();
		IsInitialized = true;
		Quest.startEvent += QuestInfoItem_OnQuestStart;
		Quest.updateProgressEvent += QuestInfoItem_OnQuestUpdateProgress;
		if (Singleton<QuestSpawner>.IsExist)
		{
			OnQuestSpawnerInitialize();
		}
		QuestSpawner.initializeEvent = (Action)Delegate.Combine(QuestSpawner.initializeEvent, new Action(OnQuestSpawnerInitialize));
	}

	private void OnDestroy()
	{
		Quest.startEvent -= QuestInfoItem_OnQuestStart;
		Quest.updateProgressEvent -= QuestInfoItem_OnQuestUpdateProgress;
		QuestSpawner.initializeEvent = (Action)Delegate.Remove(QuestSpawner.initializeEvent, new Action(OnQuestSpawnerInitialize));
	}

	private void OnQuestSpawnerInitialize()
	{
		if (Singleton<QuestSpawner>.Instance.ActiveQuest != null && IsTargetQuest(Singleton<QuestSpawner>.Instance.ActiveQuest))
		{
			OnQuestStart(Singleton<QuestSpawner>.Instance.ActiveQuest);
		}
	}

	private void QuestInfoItem_OnQuestStart(Quest quest)
	{
		if (quest != null && IsTargetQuest(quest))
		{
			OnQuestStart(quest);
			OnQuestUpdateProgress(quest);
		}
	}

	private void QuestInfoItem_OnQuestUpdateProgress(Quest quest)
	{
		if (IsTargetQuest(quest))
		{
			OnQuestUpdateProgress(quest);
		}
	}

	private bool IsTargetQuest(Quest quest)
	{
		return quest.GetType().ToString() == QuestGoalPanel.questType;
	}

	protected abstract void Initialize();

	protected abstract void OnQuestStart(Quest quest);

	protected abstract void OnQuestUpdateProgress(Quest quest);
}
