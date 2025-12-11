using Avelog;
using I2.Loc;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintsWindow : WindowSingleton<HintsWindow>
{
	[SerializeField]
	private GameObject hintLeftPanel;

	[SerializeField]
	private Image hintLeftImage;

	[SerializeField]
	private GameObject hintRightPanel;

	[SerializeField]
	private Image hintRightImage;

	[SerializeField]
	private GameObject hintCenterPanel;

	[SerializeField]
	private Image hintCenterImgae;

	[SerializeField]
	private Localize hintLoc;

	private const string hintsTermCategory = "Hints/";

	protected override void OnInitialize()
	{
		HelpHintsController.needShowHintEvent += OnNeedShowHint;
		Avelog.Input.questHintPressedEvent += OnQuestHintPressed;
	}

	private void OnDestroy()
	{
		HelpHintsController.needShowHintEvent -= OnNeedShowHint;
		Avelog.Input.questHintPressedEvent -= OnQuestHintPressed;
	}

	private void OnQuestHintPressed()
	{
		Quest activeQuest = Singleton<QuestSpawner>.Instance.ActiveQuest;
		hintLoc.SetTerm(activeQuest.DescriptionTerm);
		List<Sprite> questHints = ManagerBase<UIManager>.Instance.GetQuestHints(activeQuest);
		if (questHints.Count == 2)
		{
			hintCenterPanel.SetActive(value: false);
			hintLeftPanel.SetActive(value: true);
			hintLeftImage.sprite = questHints[0];
			hintRightPanel.SetActive(value: true);
			hintRightImage.sprite = questHints[1];
		}
		else if (questHints.Count == 1)
		{
			hintLeftPanel.SetActive(value: false);
			hintRightPanel.SetActive(value: false);
			hintCenterPanel.SetActive(value: true);
			hintCenterImgae.sprite = questHints[0];
		}
		Open();
	}

	private void OnNeedShowHint(HelpManager.Hint hint)
	{
		hintLoc.SetTerm("Hints/" + hint.ToString());
		List<Sprite> helpHintsSprites = ManagerBase<UIManager>.Instance.GetHelpHintsSprites(hint);
		if (helpHintsSprites.Count == 2)
		{
			hintCenterPanel.SetActive(value: false);
			hintLeftPanel.SetActive(value: true);
			hintLeftImage.sprite = helpHintsSprites[0];
			hintRightPanel.SetActive(value: true);
			hintRightImage.sprite = helpHintsSprites[1];
		}
		else if (helpHintsSprites.Count == 1)
		{
			hintLeftPanel.SetActive(value: false);
			hintRightPanel.SetActive(value: false);
			hintCenterPanel.SetActive(value: true);
			hintCenterImgae.sprite = helpHintsSprites[0];
		}
		ManagerBase<UIManager>.Instance.PlayHintOpenSound();
		Open();
	}

	public void CloseWindow()
	{
		if (SceneController.Instance.CurActiveScene == SceneController.SceneType.Game)
		{
			WindowSingleton<GameWindow>.Instance.Open();
		}
		else
		{
			WindowSingleton<MenuWindow>.Instance.Open();
		}
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		{
			CloseWindow();
		}
	}
}
