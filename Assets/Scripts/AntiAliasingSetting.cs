using Avelog;
using System;
using UnityEngine;

public class AntiAliasingSetting : MonoBehaviour
{
	private static AntiAliasingSetting instance;

	public static AntiAliasingSetting Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType<AntiAliasingSetting>();
				instance.Init();
			}
			return instance;
		}
		private set
		{
			instance = value;
		}
	}

	public static bool IsInited
	{
		get;
		private set;
	}

	public bool AntiAliasing
	{
		get
		{
			return ManagerBase<SettingsManager>.Instance.antiAliasing;
		}
		private set
		{
			ManagerBase<SettingsManager>.Instance.antiAliasing = value;
		}
	}

	public static event Action changeAntiAliasingEvent;

	public static event Action changeAAFromSettingsEvent;

	private void Start()
	{
		Init();
	}

	private void Init()
	{
		if (!IsInited)
		{
			Avelog.Input.pressAntiAliasingToggleEvent += OnPressAntiAliasingToggle;
			SaveManager.LoadEndEvent += UpdateAntiAliasing;
			SetAntiAliasing(AntiAliasing);
			IsInited = true;
		}
	}

	private void OnDestroy()
	{
		Avelog.Input.pressAntiAliasingToggleEvent += OnPressAntiAliasingToggle;
		SaveManager.LoadEndEvent -= UpdateAntiAliasing;
	}

	private void OnPressAntiAliasingToggle(bool antiAliasing)
	{
		ManagerBase<SettingsManager>.Instance.settingsAntiAliasing = antiAliasing;
		SetAntiAliasing(antiAliasing);
		AntiAliasingSetting.changeAAFromSettingsEvent?.Invoke();
	}

	public void SetAntiAliasing(bool antiAliasing)
	{
		AntiAliasing = antiAliasing;
		QualitySettings.antiAliasing = (antiAliasing ? 4 : 0);
		AntiAliasingSetting.changeAntiAliasingEvent?.Invoke();
	}

	private void UpdateAntiAliasing()
	{
		SetAntiAliasing(AntiAliasing);
	}
}
