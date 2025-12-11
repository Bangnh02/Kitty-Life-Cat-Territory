using Avelog;

public class GenderWindow : WindowSingleton<GenderWindow>
{
	protected override void OnInitialize()
	{
		SceneController.changeActiveSceneEvent += OnChangeActiveScene;
		OnChangeActiveScene(SceneController.Instance.CurActiveScene);
	}

	private void OnDestroy()
	{
		SceneController.changeActiveSceneEvent -= OnChangeActiveScene;
	}

	private void OnChangeActiveScene(SceneController.SceneType newActiveScene)
	{
		if (newActiveScene == SceneController.SceneType.Menu && Window.CurWindow == null && !ManagerBase<GameConfigManager>.Instance.isGenderChoosed)
		{
			Open();
		}
	}

	public void ChooseGender(bool isMaleGender)
	{
		ManagerBase<PlayerManager>.Instance.gender = ((!isMaleGender) ? FamilyManager.GenderType.Female : FamilyManager.GenderType.Male);
		ManagerBase<GameConfigManager>.Instance.isGenderChoosed = true;
		Input.FireChangeGenderPressed();
		WindowSingleton<MenuWindow>.Instance.Open();
	}
}
