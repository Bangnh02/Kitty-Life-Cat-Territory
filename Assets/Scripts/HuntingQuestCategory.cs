using System;
using System.Collections.Generic;

public class HuntingQuestCategory : StandartQuestCategory
{
	[Serializable]
	public class HuntingQuestData
	{
		public string archetypeName;

		public int completedQuestsCount;
	}

	[Serializable]
	public new class SaveData : StandartQuestCategory.SaveData
	{
		public string lastCompletedQuestName;

		public List<HuntingQuestData> huntingQuestDatas = new List<HuntingQuestData>();
	}

	private Quest lastCompletedQuest;

	public new SaveData MySaveData => base.MySaveData as SaveData;

	protected override QuestCategory.SaveData CreateSaveData()
	{
		return new SaveData
		{
			categoryName = base.gameObject.name,
			lastCompletedQuestName = null,
			сurPriority = base.Priority,
			huntingQuestDatas = new List<HuntingQuestData>()
		};
	}

	protected override void OnInit()
	{
		lastCompletedQuest = categoryQuests.Find((Quest x) => base.name == MySaveData.lastCompletedQuestName);
		Quest.completeEvent += OnCompleteQuest;
	}

	private void OnDestroy()
	{
		Quest.completeEvent -= OnCompleteQuest;
	}

	private void OnCompleteQuest(Quest completedQuest)
	{
		if (!categoryQuests.Contains(completedQuest))
		{
			return;
		}
		lastCompletedQuest = completedQuest;
		MySaveData.lastCompletedQuestName = completedQuest.name;
		if (lastCompletedQuest is HuntingQuest)
		{
			HuntingQuest huntingQuest = lastCompletedQuest as HuntingQuest;
			HuntingQuestData huntingQuestData = MySaveData.huntingQuestDatas.Find((HuntingQuestData x) => x.archetypeName == huntingQuest.ArchetypeName);
			if (huntingQuestData == null)
			{
				huntingQuestData = new HuntingQuestData
				{
					archetypeName = huntingQuest.ArchetypeName,
					completedQuestsCount = 0
				};
				MySaveData.huntingQuestDatas.Add(huntingQuestData);
			}
			huntingQuestData.completedQuestsCount++;
		}
	}

	public override Quest SelectQuest()
	{
		List<Quest> list = categoryQuests.FindAll((Quest x) => x.CanStart());
		if (list.Contains(lastCompletedQuest))
		{
			list.Remove(lastCompletedQuest);
		}
		return list.Random();
	}
}
