using UnityEngine;

public class MapButton : MonoBehaviour
{
	public void OpenMap()
	{
		ManagerBase<UIManager>.Instance.PlayMapButtonSound();
		WindowSingleton<MapWindow>.Instance.Open();
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.M))
		{
			OpenMap();
		}
	}
}
