using Avelog;

public class MakeChildWindow : WindowSingleton<MakeChildWindow>
{
	protected override void OnInitialize()
	{
	}

	public void MakeChild()
	{
		Input.FireMakeChildPressed("Child");
		WindowSingleton<GameWindow>.Instance.Open();
	}
}
