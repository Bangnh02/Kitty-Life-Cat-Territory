using UnityEngine;
using UnityEngine.UI;

public class QuestCompletePanel : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private Text coinsText;

	[SerializeField]
	private Text experienceText;

	public void OnInitializeUI()
	{
		Quest.startEvent += OnQuestStart;
		Quest.completeEvent += OnQuestComplete;
	}

	private void OnDestroy()
	{
		Quest.startEvent -= OnQuestStart;
		Quest.completeEvent -= OnQuestComplete;
	}

	private void OnQuestComplete(Quest quest)
	{
		base.gameObject.SetActive(value: true);
		coinsText.text = "+" + quest.Reward.Coins.ToString();
		experienceText.text = "+" + Mathf.Round(quest.Reward.Experience + quest.Reward.Experience / 100f * ManagerBase<PlayerManager>.Instance.ExperienceBonusPerc).ToString();
	}

	private void OnQuestStart(Quest quest)
	{
		base.gameObject.SetActive(value: false);
	}
}
