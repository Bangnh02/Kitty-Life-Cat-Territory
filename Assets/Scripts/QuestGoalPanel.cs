using System;
using UnityEngine;

public class QuestGoalPanel : MonoBehaviour, IInitializableUI
{
	public string questType;

	public void OnInitializeUI()
	{
		Quest.startEvent += OnQuestStart;
		Quest.completeEvent += OnQuestComplete;
		if (Singleton<QuestSpawner>.IsExist)
		{
			OnQuestSpawnerInitialize();
		}
		QuestSpawner.initializeEvent = (Action)Delegate.Combine(QuestSpawner.initializeEvent, new Action(OnQuestSpawnerInitialize));
	}

	private void OnDestroy()
	{
		Quest.startEvent -= OnQuestStart;
		Quest.completeEvent -= OnQuestComplete;
		QuestSpawner.initializeEvent = (Action)Delegate.Remove(QuestSpawner.initializeEvent, new Action(OnQuestSpawnerInitialize));
	}

	private void OnQuestSpawnerInitialize()
	{
		if (Singleton<QuestSpawner>.Instance.ActiveQuest != null)
		{
			OnQuestStart(Singleton<QuestSpawner>.Instance.ActiveQuest);
		}
	}

	private void OnQuestStart(Quest quest)
	{
		base.gameObject.SetActive(quest.GetType().ToString() == questType);
	}

	private void OnQuestComplete(Quest quest)
	{
		base.gameObject.SetActive(value: false);
	}
}
