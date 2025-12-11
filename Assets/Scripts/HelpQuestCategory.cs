using System;
using System.Collections.Generic;
using System.Linq;

public class HelpQuestCategory : StandartQuestCategory
{
	[Serializable]
	public new class SaveData : StandartQuestCategory.SaveData
	{
		public string lastCompletedQuestName;
	}

	private HelpFarmResidentQuest lastCompletedQuest;

	private new SaveData MySaveData => base.MySaveData as SaveData;

	protected override QuestCategory.SaveData CreateSaveData()
	{
		return new SaveData
		{
			categoryName = base.gameObject.name,
			lastCompletedQuestName = null,
			сurPriority = base.Priority
		};
	}

	protected override void OnInit()
	{
		lastCompletedQuest = (categoryQuests.Find((Quest x) => base.name == MySaveData.lastCompletedQuestName) as HelpFarmResidentQuest);
		Quest.completeEvent += OnCompleteQuest;
	}

	private void OnDestroy()
	{
		Quest.completeEvent -= OnCompleteQuest;
	}

	private void OnCompleteQuest(Quest completedQuest)
	{
		if (categoryQuests.Contains(completedQuest))
		{
			lastCompletedQuest = (completedQuest as HelpFarmResidentQuest);
			MySaveData.lastCompletedQuestName = completedQuest.name;
		}
	}

	public override Quest SelectQuest()
	{
		List<HelpFarmResidentQuest> list = (from x in categoryQuests
			where x.CanStart() && x is HelpFarmResidentQuest
			select x)?.Select((Quest x) => x as HelpFarmResidentQuest).ToList();
		if (list == null && list.Count == 0)
		{
			return null;
		}
		if (list.Count == 1)
		{
			return list[0];
		}
		List<(FarmResidentId farmResidentId, float satisfactionPart)> farmResidentsSatisfaction = new List<(FarmResidentId, float)>();
		foreach (FarmResidentManager.FarmResidentData farmResidentsDatum in ManagerBase<FarmResidentManager>.Instance.FarmResidentsData)
		{
			float item = farmResidentsDatum.needProgressCurrent / farmResidentsDatum.needProgressMaximum;
			farmResidentsSatisfaction.Add((farmResidentsDatum.farmResidentId, item));
		}
		float minSatisfactionPart = farmResidentsSatisfaction.Min(((FarmResidentId farmResidentId, float satisfactionPart) x) => x.satisfactionPart);
		farmResidentsSatisfaction.RemoveAll(((FarmResidentId farmResidentId, float satisfactionPart) x) => x.satisfactionPart > minSatisfactionPart);
		if (farmResidentsSatisfaction.Count > 1)
		{
			if (list.Contains(lastCompletedQuest))
			{
				list.Remove(lastCompletedQuest);
			}
			list.RemoveAll((HelpFarmResidentQuest x) => farmResidentsSatisfaction.All(((FarmResidentId farmResidentId, float satisfactionPart) y) => y.farmResidentId != x.FarmResidentId));
			return list.Random();
		}
		return list.Find((HelpFarmResidentQuest x) => x.FarmResidentId == farmResidentsSatisfaction[0].farmResidentId);
	}
}
