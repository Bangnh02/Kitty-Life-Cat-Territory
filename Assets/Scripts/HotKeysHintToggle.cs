using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HotKeysHintToggle : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private Sprite onSprite;

	[SerializeField]
	private Sprite offSprite;

	private Button myButton;

	private static HotKeysHintToggle _instance;

	public static HotKeysHintToggle Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = WindowSingleton<GameWindow>.Instance.GetComponentInChildren<HotKeysHintToggle>();
			}
			return _instance;
		}
	}

	public bool HotKeysHintState
	{
		get
		{
			return ManagerBase<SettingsManager>.Instance.hotKeysHint;
		}
		private set
		{
			ManagerBase<SettingsManager>.Instance.hotKeysHint = value;
		}
	}

	public static event Action updateHintStateEvent;

	public void OnInitializeUI()
	{
		myButton = GetComponent<Button>();
		SaveManager.LoadEndEvent += OnLoadEnd;
		UpdateButton();
	}

	private void OnDestroy()
	{
		SaveManager.LoadEndEvent -= OnLoadEnd;
	}

	private void OnLoadEnd()
	{
		UpdateButton();
		HotKeysHintToggle.updateHintStateEvent?.Invoke();
	}

	public void Switch()
	{
		HotKeysHintState = !HotKeysHintState;
		UpdateButton();
		HotKeysHintToggle.updateHintStateEvent?.Invoke();
	}

	private void UpdateButton()
	{
		if (HotKeysHintState)
		{
			myButton.image.sprite = onSprite;
		}
		else
		{
			myButton.image.sprite = offSprite;
		}
	}
}
