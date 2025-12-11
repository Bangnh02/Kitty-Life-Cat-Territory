using UnityEngine;

public class ButtonsHelpF1 : MonoBehaviour, IInitializableUI
{
	private bool HotKeysHintState => ManagerBase<SettingsManager>.Instance.hotKeysHint;

	void IInitializableUI.OnInitializeUI()
	{
		HotKeysHintToggle.updateHintStateEvent += OnUpdateHintState;
		OnUpdateHintState();
	}

	private void OnUpdateHintState()
	{
		base.gameObject.SetActive(HotKeysHintState);
	}
}
