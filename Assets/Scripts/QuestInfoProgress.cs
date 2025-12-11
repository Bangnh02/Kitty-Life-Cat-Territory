using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class QuestInfoProgress : QuestInfoItem
{
	private Text progressText;

	private const int notInitializedProgress = -1;

	private int prevProgress = -1;

	protected override void Initialize()
	{
		progressText = GetComponent<Text>();
	}

	protected override void OnQuestStart(Quest quest)
	{
		prevProgress = -1;
		UpdateProgress(quest);
	}

	private void OnEnable()
	{
		if (base.IsInitialized)
		{
			UpdateProgress(Singleton<QuestSpawner>.Instance.ActiveQuest);
		}
	}

	protected override void OnQuestUpdateProgress(Quest quest)
	{
		UpdateProgress(quest);
	}

	private void UpdateProgress(Quest quest)
	{
		if (quest != null && prevProgress != (int)quest.CurProgress)
		{
			prevProgress = (int)quest.CurProgress;
			progressText.text = $"{(int)quest.CurProgress}/{(int)quest.MaxProgress}";
		}
	}
}
