using UnityEngine;

public class SaveLoadPanel : MonoBehaviour, IInitializableUI
{
	public void OnInitializeUI()
	{
		SaveManager.LoadStartEvent += OnLoadStart;
		SaveManager.LoadEndEvent += OnLoadEnd;
		SaveManager.SaveStartEvent += OnSaveStart;
		SaveManager.SaveEndEvent += OnSaveEnd;
	}

	private void OnDestroy()
	{
		SaveManager.LoadStartEvent -= OnLoadStart;
		SaveManager.LoadEndEvent -= OnLoadEnd;
		SaveManager.SaveStartEvent -= OnSaveStart;
		SaveManager.SaveEndEvent -= OnSaveEnd;
	}

	private void OnSaveEnd()
	{
		UnityEngine.Debug.Log("OnSaveEnd");
	}

	private void OnSaveStart()
	{
		UnityEngine.Debug.Log("OnSaveStart");
	}

	private void OnLoadEnd()
	{
		UnityEngine.Debug.Log("OnLoadEnd");
	}

	private void OnLoadStart()
	{
		UnityEngine.Debug.Log("OnLoadStart");
	}

	private void UpdateWindowState()
	{
	}
}
