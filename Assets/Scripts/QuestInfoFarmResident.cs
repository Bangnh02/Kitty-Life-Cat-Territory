using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class QuestInfoFarmResident : QuestInfoItem
{
	private Image farmResidentNeedImage;

	protected override void Initialize()
	{
		farmResidentNeedImage = GetComponent<Image>();
	}

	protected override void OnQuestStart(Quest quest)
	{
		HelpFarmResidentQuest helpFarmResidentQuest = quest as HelpFarmResidentQuest;
		ManagerBase<FarmResidentManager>.Instance.FarmResidentsData.Find((FarmResidentManager.FarmResidentData x) => x.farmResidentId == helpFarmResidentQuest.FarmResidentId);
		farmResidentNeedImage.sprite = ManagerBase<UIManager>.Instance.GetFarmResidentSprite(helpFarmResidentQuest.FarmResidentId);
	}

	protected override void OnQuestUpdateProgress(Quest quest)
	{
	}
}
