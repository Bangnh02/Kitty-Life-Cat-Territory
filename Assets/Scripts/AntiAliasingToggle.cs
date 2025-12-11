using Avelog;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AntiAliasingToggle : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private Sprite onSprite;

	[SerializeField]
	private Sprite offSprite;

	private Button myButton;

	public void OnInitializeUI()
	{
		myButton = GetComponent<Button>();
		SaveManager.LoadEndEvent += UpdateButton;
		UpdateButton();
		AntiAliasingSetting.changeAntiAliasingEvent += OnChangeAntiAliasing;
	}

	private void OnDestroy()
	{
		SaveManager.LoadEndEvent -= UpdateButton;
		AntiAliasingSetting.changeAntiAliasingEvent -= OnChangeAntiAliasing;
	}

	private void OnChangeAntiAliasing()
	{
		UpdateButton();
	}

	public void SwitchAntiAliasingState()
	{
		Avelog.Input.FirePressAntiAliasingToggleEvent(!ManagerBase<SettingsManager>.Instance.antiAliasing);
	}

	private void UpdateButton()
	{
		if (ManagerBase<SettingsManager>.Instance.antiAliasing)
		{
			myButton.image.sprite = onSprite;
		}
		else
		{
			myButton.image.sprite = offSprite;
		}
	}
}
