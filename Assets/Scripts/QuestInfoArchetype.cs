using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class QuestInfoArchetype : QuestInfoItem
{
	private Image archetypeImage;

	protected override void Initialize()
	{
		archetypeImage = GetComponent<Image>();
	}

	protected override void OnQuestStart(Quest quest)
	{
		HuntingQuest huntingQuest = quest as HuntingQuest;
		archetypeImage.sprite = ManagerBase<UIManager>.Instance.GetArchetypeSprite(huntingQuest.ArchetypeName);
	}

	protected override void OnQuestUpdateProgress(Quest quest)
	{
	}
}
