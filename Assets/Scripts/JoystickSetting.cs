using Avelog;
using System;
using UnityEngine;

public class JoystickSetting : MonoBehaviour
{
	private static JoystickSetting instance;

	private static bool isInited;

	private JoystickType JoystickType
	{
		get
		{
			return ManagerBase<SettingsManager>.Instance.curJoystickType;
		}
		set
		{
			ManagerBase<SettingsManager>.Instance.curJoystickType = value;
		}
	}

	public static JoystickSetting Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType<JoystickSetting>();
				instance.Init();
			}
			return instance;
		}
		private set
		{
			instance = value;
		}
	}

	public event Action joystickTypeChangeEvent;

	private void Start()
	{
		Init();
	}

	private void Init()
	{
		if (!isInited)
		{
			Avelog.Input.pressJoystickTypeChangeToggleEvent += OnPressJoystickTypeChangeToggle;
			SaveManager.LoadEndEvent += OnLoadEnd;
			OnPressJoystickTypeChangeToggle(JoystickType);
			isInited = true;
		}
	}

	private void OnDestroy()
	{
		Avelog.Input.pressJoystickTypeChangeToggleEvent -= OnPressJoystickTypeChangeToggle;
		SaveManager.LoadEndEvent -= OnLoadEnd;
	}

	private void OnPressJoystickTypeChangeToggle(JoystickType joystickType)
	{
		JoystickType = joystickType;
		this.joystickTypeChangeEvent?.Invoke();
	}

	private void OnLoadEnd()
	{
		OnPressJoystickTypeChangeToggle(JoystickType);
	}
}
