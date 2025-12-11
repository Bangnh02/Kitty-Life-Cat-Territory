using Avelog;
using System;
using System.Collections.Generic;
using System.Linq;

public class PlantingQuestCategory : StandartQuestCategory
{
	[Serializable]
	public new class SaveData : StandartQuestCategory.SaveData
	{
		public long completePlantQuestTime = long.MinValue;
	}

	public new SaveData MySaveData => base.MySaveData as SaveData;

	protected override QuestCategory.SaveData CreateSaveData()
	{
		return new SaveData
		{
			categoryName = base.gameObject.name,
			completePlantQuestTime = long.MinValue,
			сurPriority = base.Priority
		};
	}

	protected override void OnInit()
	{
		Quest.completeEvent += OnCompleteQuest;
	}

	private void OnDestroy()
	{
		Quest.completeEvent -= OnCompleteQuest;
	}

	private void OnCompleteQuest(Quest completedQuest)
	{
		if (completedQuest as PlantVegetableQuest != null)
		{
			MySaveData.completePlantQuestTime = TimeUtils.GetDeviceTime(TimeUtils.TimeType.System);
		}
	}

	public override Quest SelectQuest()
	{
		List<PlantVegetableQuest> list = (from x in categoryQuests
			where x.CanStart() && x is PlantVegetableQuest
			select x)?.Select((Quest x) => x as PlantVegetableQuest).ToList();
		if (list == null || list.Count == 0)
		{
			return null;
		}
		return list[0];
	}
}
