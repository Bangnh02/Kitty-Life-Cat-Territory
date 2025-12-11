using UnityEngine;
using UnityEngine.UI;

public class SoundEffectSettingPanel : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private Slider slider;

	public void OnInitializeUI()
	{
		SaveManager.LoadEndEvent += OnLoadEnd;
		if (ManagerBase<SaveManager>.Instance.IsDataLoaded)
		{
			OnLoadEnd();
		}
	}

	private void OnDestroy()
	{
		SaveManager.LoadEndEvent -= OnLoadEnd;
	}

	private void OnLoadEnd()
	{
		slider.value = ManagerBase<SettingsManager>.Instance.effectsVolume;
	}

	public void SetEffectVolume()
	{
		ManagerBase<SettingsManager>.Instance.effectsVolume = slider.value;
	}
}
