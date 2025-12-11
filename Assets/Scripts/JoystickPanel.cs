using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoystickPanel : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private JoystickScript fixedJoystick;

	[SerializeField]
	private JoystickScript mobileJoystick;

	[SerializeField]
	private RectTransform mobileJoystickRect;

	[SerializeField]
	private Image mobileJoystickBackground;

	[SerializeField]
	private Image mobileJoystickPointer;

	private bool haveStartPos;

	private Vector3 joystickStartPos = Vector3.zero;

	private JoystickType joystickType => ManagerBase<SettingsManager>.Instance.curJoystickType;

	public void OnInitializeUI()
	{
		SetupJoystick();
		JoystickSetting.Instance.joystickTypeChangeEvent += SetupJoystick;
		DisableJoystickCol();
	}

	private void SetupJoystick()
	{
		if (joystickType == JoystickType.Fixed)
		{
			base.gameObject.SetActive(value: false);
			mobileJoystick.gameObject.SetActive(value: false);
			fixedJoystick.gameObject.SetActive(value: true);
		}
		else
		{
			base.gameObject.SetActive(value: true);
			mobileJoystick.gameObject.SetActive(value: true);
			fixedJoystick.gameObject.SetActive(value: false);
		}
	}

	public void OnDrag(BaseEventData event_data)
	{
		mobileJoystick.OnDrag(event_data);
	}

	public void PointerDown(BaseEventData event_data)
	{
		EnableJoystickCol();
		if (!haveStartPos)
		{
			haveStartPos = true;
			joystickStartPos = mobileJoystickRect.position;
		}
		PointerEventData pointerEventData = event_data as PointerEventData;
		mobileJoystickRect.position = pointerEventData.position;
		mobileJoystick.PointerDown(event_data);
	}

	public void PointerUp(BaseEventData event_data)
	{
		mobileJoystick.PointerUp(event_data);
		mobileJoystickRect.position = joystickStartPos;
		DisableJoystickCol();
	}

	private void DisableJoystickCol()
	{
	}

	private void EnableJoystickCol()
	{
	}
}
