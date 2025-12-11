using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class DropdownElem : MonoBehaviour
{
	[SerializeField]
	private Text textUI;

	[SerializeField]
	private Localize localize;

	[SerializeField]
	private LocalizationParamsManager localizationParamsManager;

	[SerializeField]
	private GameObject activeFrameGO;

	[SerializeField]
	[ReadonlyInspector]
	private DropdownWindow.DropdownOption option;

	public void Setup(DropdownWindow.DropdownOption option, bool selected)
	{
		this.option = option;
		localize.enabled = option.UseLocalization;
		localizationParamsManager.enabled = option.UseLocalization;
		if (option.UseLocalization)
		{
			localize.SetTerm(option.Term);
			option.ApplyLocalizeParams(localizationParamsManager);
		}
		else
		{
			textUI.text = option.ToString();
		}
		SwitchSelectState(selected);
	}

	public void SwitchSelectState(bool selected)
	{
		activeFrameGO.SetActive(selected);
	}

	public bool IsSelected(DropdownWindow.DropdownOption selectedOption)
	{
		return option == selectedOption;
	}

	public void OnClick()
	{
		WindowSingleton<DropdownWindow>.Instance.OnElemClick(option);
	}
}
