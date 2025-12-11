using Avelog;
using I2.Loc;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuWindow : WindowSingleton<MenuWindow>
{
	[SerializeField]
	private GameObject hintPanel;

	[SerializeField]
	private Localize hintLoc;

	[SerializeField]
	private Text versionText;

	private const int minToSec = 60;

	private const string hintsTermCategory = "Hints/";

	private const string hintSpouseTerm = "SmallHint1";

	private const string hintChildTerm = "SmallHint2";

	private const string hintChildCountTerm = "SmallHint3";

	protected override void OnInitialize()
	{
		versionText.text = ManagerBase<GameConfigManager>.Instance.GameVersionNumber;
		SceneController.changeActiveSceneEvent += OnChangeActiveScene;
		SaveManager.LoadEndEvent += OnLoadEnd;
		OnChangeActiveScene(SceneController.Instance.CurActiveScene);
	}

	private void OnDestroy()
	{
		SceneController.changeActiveSceneEvent -= OnChangeActiveScene;
		SaveManager.LoadEndEvent -= OnLoadEnd;
	}

	private void OnChangeActiveScene(SceneController.SceneType newActiveScene)
	{
		if (newActiveScene == SceneController.SceneType.Menu && ManagerBase<GameConfigManager>.Instance.isGenderChoosed)
		{
			Open();
		}
	}

	private void OnEnable()
	{
		TryShowHintPanel();
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) && SceneController.Instance.IsGameSceneLoaded)
		{
			StartGame();
		}
	}

	private void OnLoadEnd()
	{
		TryShowHintPanel();
	}

	public void StartGame()
	{
		if (ManagerBase<GameConfigManager>.Instance.isFirstPlay)
		{
			ManagerBase<GameConfigManager>.Instance.isFirstPlay = false;
		}
		Avelog.Input.FireStartGamePressed();
	}

	public void OpenSettingsWindow()
	{
		WindowSingleton<SettingsWindow>.Instance.Open();
	}

	public void OpenSkinsWindow()
	{
		if (!ManagerBase<GameConfigManager>.Instance.isVisitedSkinsWindow)
		{
			ManagerBase<GameConfigManager>.Instance.isVisitedSkinsWindow = true;
		}
		WindowSingleton<SkinsWindow>.Instance.Open();
		ManagerBase<UIManager>.Instance.PlaySkinsButtonSound();
	}

	private void TryShowHintPanel()
	{
		int level = ManagerBase<PlayerManager>.Instance.Level;
		int num = ManagerBase<FamilyManager>.Instance.family.Count((FamilyManager.FamilyMemberData x) => x.role == FamilyManager.FamilyMemberRole.ThirdStageChild);
		if (level < ManagerBase<FamilyManager>.Instance.SpouseLevelNeed)
		{
			hintPanel.SetActive(value: true);
			hintLoc.SetTerm("Hints/SmallHint1");
		}
		else if (level >= ManagerBase<FamilyManager>.Instance.SpouseLevelNeed && level < ManagerBase<FamilyManager>.Instance.ChildLevelNeed)
		{
			hintPanel.SetActive(value: true);
			hintLoc.SetTerm("Hints/SmallHint2");
		}
		else if (level >= ManagerBase<FamilyManager>.Instance.ChildLevelNeed && num < ManagerBase<FamilyManager>.Instance.MaxChilds)
		{
			hintPanel.SetActive(value: true);
			hintLoc.SetTerm("Hints/SmallHint3");
		}
		else
		{
			hintPanel.SetActive(value: false);
		}
	}
}
