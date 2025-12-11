using I2.Loc;
using UnityEngine;

public class LoadingWindow : WindowSingleton<LoadingWindow>
{
	[SerializeField]
	private Localize loadingTitle;

	[SerializeField]
	private string gameLoadingTerm = "Generals/LoadingText";

	private Window prevWindow;

	protected override void OnInitialize()
	{
		SceneController.changeActiveSceneEvent += OnChangeActiveScene;
		SceneController.sceneLoadingEvent += OnSceneLoading;
	}

	private void OnDestroy()
	{
		SceneController.changeActiveSceneEvent -= OnChangeActiveScene;
		SceneController.sceneLoadingEvent -= OnSceneLoading;
	}

	private void OnChangeActiveScene(SceneController.SceneType newActiveScene)
	{
		CompleteLoading();
	}

	private void OnSceneLoading(SceneController.SceneType sceneType)
	{
		if (sceneType == SceneController.SceneType.Game)
		{
			loadingTitle.SetTerm(gameLoadingTerm);
			Open();
		}
	}

	public void CompleteLoading()
	{
		if (base.IsOpened)
		{
			WindowSingleton<GameWindow>.Instance.Open();
		}
	}

	private void Update()
	{
		if (base.IsOpened && !SceneController.Instance.IsSceneLoading && !ManagerBase<SaveManager>.Instance.IsSaving && !ManagerBase<SaveManager>.Instance.IsLoading)
		{
			if (prevWindow != null)
			{
				prevWindow.Open();
			}
			else
			{
				WindowSingleton<MenuWindow>.Instance.Open();
			}
		}
	}
}
