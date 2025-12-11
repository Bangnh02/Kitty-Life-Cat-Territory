using UnityEngine;
using UnityEngine.UI;

public class MusicSettingPanel : MonoBehaviour, IInitializableUI
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
		slider.value = ManagerBase<SettingsManager>.Instance.musicVolume;
	}

	public void SetMusicVolume()
	{
		ManagerBase<SettingsManager>.Instance.musicVolume = slider.value;
	}
}
