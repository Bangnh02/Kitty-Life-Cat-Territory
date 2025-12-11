using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class QuestInfoNeed : QuestInfoItem
{
	private Image farmResidentImage;

	protected override void Initialize()
	{
		farmResidentImage = GetComponent<Image>();
	}

	protected override void OnQuestStart(Quest quest)
	{
		HelpFarmResidentQuest helpFarmResidentQuest = quest as HelpFarmResidentQuest;
		FarmResidentManager.FarmResidentData farmResidentData = ManagerBase<FarmResidentManager>.Instance.FarmResidentsData.Find((FarmResidentManager.FarmResidentData x) => x.farmResidentId == helpFarmResidentQuest.FarmResidentId);
		farmResidentImage.sprite = ManagerBase<UIManager>.Instance.GetFarmResidentNeedSprite(farmResidentData.curNeed);
	}

	protected override void OnQuestUpdateProgress(Quest quest)
	{
	}
}
