using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class QuestInfoChildCost : QuestInfoItem
{
	private Text childCostText;

	protected override void Initialize()
	{
		childCostText = GetComponent<Text>();
	}

	protected override void OnQuestStart(Quest quest)
	{
		childCostText.text = ManagerBase<FamilyManager>.Instance.ChildCost.ToString();
	}

	protected override void OnQuestUpdateProgress(Quest quest)
	{
	}
}
