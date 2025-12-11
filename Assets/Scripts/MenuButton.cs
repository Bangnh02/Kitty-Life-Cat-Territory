using Avelog;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
	public void OpenMainMenu()
	{
		if (SceneController.Instance.CurActiveScene == SceneController.SceneType.Game)
		{
			Avelog.Input.FireToMainMenuPressed();
		}
		else
		{
			WindowSingleton<MenuWindow>.Instance.Open();
		}
	}
}
