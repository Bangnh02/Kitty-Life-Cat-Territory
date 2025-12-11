using Avelog;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class JoystickTypeToggle : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private Sprite onSprite;

	[SerializeField]
	private Sprite offSprite;

	private Button myButton;

	private JoystickType JoystickType => ManagerBase<SettingsManager>.Instance.curJoystickType;

	public void OnInitializeUI()
	{
		myButton = GetComponent<Button>();
		SaveManager.LoadEndEvent += UpdateButton;
		UpdateButton();
	}

	private void OnDestroy()
	{
		SaveManager.LoadEndEvent -= UpdateButton;
	}

	public void SwitchJoystickType()
	{
		JoystickType joystickType = (JoystickType != JoystickType.Mobile) ? JoystickType.Mobile : JoystickType.Fixed;
		Avelog.Input.FirePressJoystickTypeChangeToggleEvent(joystickType);
		UpdateButton();
	}

	private void UpdateButton()
	{
		if (JoystickType == JoystickType.Mobile)
		{
			myButton.image.sprite = onSprite;
		}
		else
		{
			myButton.image.sprite = offSprite;
		}
	}
}
