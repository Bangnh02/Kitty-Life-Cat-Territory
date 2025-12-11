using UnityEngine;

public class CloudLoadButton : MonoBehaviour, IInitializableUI
{
	public void OnInitializeUI()
	{
	}

	public void LoadFromCloud()
	{
		if (Social.localUser.authenticated)
		{
			ManagerBase<SaveManager>.Instance.LoadFromCloud(null);
		}
	}
}
