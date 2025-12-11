using Avelog;

public class PauseWindow : WindowSingleton<PauseWindow>
{
	protected override void OnInitialize()
	{
	}

	public void Back()
	{
		WindowSingleton<GameWindow>.Instance.Open();
	}

	public void ToMainMenu()
	{
		Input.FireToMainMenuPressed();
	}
}
