using UnityEngine;

public class GameWindow : WindowSingleton<GameWindow>
{
	[SerializeField]
	private GameObject hotKeysPanelGO;

	[SerializeField]
	private GameObject mainPanelGO;

	private MenuButton menuButton;

	private bool showHotKeysPanel;

	protected override void OnInitialize()
	{
		menuButton = GetComponentInChildren<MenuButton>(includeInactive: true);
		if (!ManagerBase<SettingsManager>.Instance.hotKeysHintShowedOnce)
		{
			ManagerBase<SettingsManager>.Instance.hotKeysHintShowedOnce = true;
			//showHotKeysPanel = true;
		}
		SetupPanels();
		SceneController.changeActiveSceneEvent += OnChangeActiveScene;
		OnChangeActiveScene(SceneController.Instance.CurActiveScene);
	}

	private void OnDestroy()
	{
		SceneController.changeActiveSceneEvent -= OnChangeActiveScene;
	}

	private void OnChangeActiveScene(SceneController.SceneType newActiveScene)
	{
		if (newActiveScene == SceneController.SceneType.Game && (WindowSingleton<LoadingWindow>.Instance == null || !WindowSingleton<LoadingWindow>.Instance.IsOpened))
		{
			Open();
		}
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.F1))
		{
			showHotKeysPanel = !showHotKeysPanel;
			SetupPanels();
		}
		if (!Input.GetKeyDown(KeyCode.Escape))
		{
			return;
		}
		if (!showHotKeysPanel)
		{
			if (!GameBlockingPanel.Instance.gameObject.activeInHierarchy)
			{
				menuButton.OpenMainMenu();
			}
		}
		else
		{
			showHotKeysPanel = false;
			SetupPanels();
		}
	}

	private void SetupPanels()
	{
		hotKeysPanelGO.SetActive(showHotKeysPanel);
		mainPanelGO.SetActive(!showHotKeysPanel);
	}

	public void CloseHotKeysPanel()
	{
		showHotKeysPanel = false;
		SetupPanels();
	}
}
