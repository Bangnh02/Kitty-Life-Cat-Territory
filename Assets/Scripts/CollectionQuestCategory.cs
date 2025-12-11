using System;

public class CollectionQuestCategory : StandartQuestCategory
{
	[Serializable]
	public new class SaveData : StandartQuestCategory.SaveData
	{
		public int completedQuestsCount;
	}

	private Quest collectCoinQuest;

	public new SaveData MySaveData => base.MySaveData as SaveData;

	protected override QuestCategory.SaveData CreateSaveData()
	{
		return new SaveData
		{
			categoryName = base.gameObject.name,
			completedQuestsCount = 0,
			сurPriority = base.Priority
		};
	}

	protected override void OnInit()
	{
		collectCoinQuest = categoryQuests.Find((Quest x) => x is CollectCoinQuest);
		Quest.completeEvent += OnCompleteQuest;
	}

	private void OnDestroy()
	{
		Quest.completeEvent -= OnCompleteQuest;
	}

	private void OnCompleteQuest(Quest quest)
	{
		if (quest is CollectCoinQuest)
		{
			MySaveData.completedQuestsCount++;
		}
	}

	public override Quest SelectQuest()
	{
		return collectCoinQuest;
	}
}
