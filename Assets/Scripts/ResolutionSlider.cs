using Avelog;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionSlider : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private Localize textLoc;

	[SerializeField]
	private Text curResolutionText;

	public void OnInitializeUI()
	{
		SaveManager.LoadEndEvent += UpdateResolution;
		UpdateResolution();
	}

	private void OnDestroy()
	{
		SaveManager.LoadEndEvent -= UpdateResolution;
	}

	public void NextResolution()
	{
		int num = ManagerBase<SettingsManager>.Instance.curResolutionIndex + 1;
		if (num >= ResolutionSetting.Instance.resolutions.Count)
		{
			num = 0;
		}
		SetResolution(num);
	}

	public void PrevResolution()
	{
		int num = ManagerBase<SettingsManager>.Instance.curResolutionIndex - 1;
		if (num < 0)
		{
			num = ResolutionSetting.Instance.resolutions.Count - 1;
		}
		SetResolution(num);
	}

	private void SetResolution(int resolutionInd)
	{
		Avelog.Input.FireSetResolutionEvent(ResolutionSetting.Instance.resolutions[resolutionInd]);
		UpdateResolution();
	}

	private void UpdateResolution()
	{
		curResolutionText.text = $"{ManagerBase<SettingsManager>.Instance.curResolutionWidth}x{ManagerBase<SettingsManager>.Instance.curResolutionHeight}";
		textLoc.OnLocalize(Force: true);
	}
}
