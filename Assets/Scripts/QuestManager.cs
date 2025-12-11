using System.Collections.Generic;
using UnityEngine;

public class QuestManager : ManagerBase<QuestManager>
{
	[Header("Отладка")]
	[Save]
	public List<QuestCategory.SaveData> questCategoriesData = new List<QuestCategory.SaveData>();

	[Save]
	public List<string> passedStartQuests = new List<string>();

	[Save]
	public float educateChildQuestTimer;

	protected override void OnInit()
	{
	}
}
