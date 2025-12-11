using UnityEngine;
using UnityEngine.UI;

public class UIScaleSetting : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private Slider slider;

	[SerializeField]
	private CanvasScaler canvasScaler;

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
		slider.value = ManagerBase<SettingsManager>.Instance.curUIScale;
		canvasScaler.referenceResolution = ManagerBase<SettingsManager>.Instance.UIScaleResolution;
	}

	public void OnSliderMove()
	{
		ManagerBase<SettingsManager>.Instance.curUIScale = slider.value;
		canvasScaler.referenceResolution = ManagerBase<SettingsManager>.Instance.UIScaleResolution;
	}
}
