using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class QuestInfoVegetable : QuestInfoItem
{
	private Image vegetableImage;

	protected override void Initialize()
	{
		vegetableImage = GetComponent<Image>();
	}

	protected override void OnQuestStart(Quest quest)
	{
		vegetableImage.sprite = ManagerBase<UIManager>.Instance.ShovelIcon;
	}

	protected override void OnQuestUpdateProgress(Quest quest)
	{
	}
}
