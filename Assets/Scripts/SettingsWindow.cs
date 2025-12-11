using UnityEngine;

public class SettingsWindow : WindowSingleton<SettingsWindow>
{
	protected override void OnInitialize()
	{
	}

	public void Back()
	{
		WindowSingleton<MenuWindow>.Instance.Open();
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		{
			Back();
		}
	}
}
