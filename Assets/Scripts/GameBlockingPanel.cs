using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameBlockingPanel : MonoBehaviour, IInitializableUI
{
	private static GameBlockingPanel instance;

	[SerializeField]
	private Image image;

	private Coroutine switchingCor;

	private bool sleepEndFromMenu;

	public static GameBlockingPanel Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType<GameBlockingPanel>();
			}
			return instance;
		}
	}

	public void OnInitializeUI()
	{
		if (instance == null)
		{
			instance = this;
		}
		image.enabled = false;
		OutOfBoundsHandler.startBlockingGameEvent += OnStartBlockingGame;
		OutOfBoundsHandler.startUnblockingGameEvent += OnStartUnblockingGame;
		PlayerSpawner.startBlockingGameEvent += OnStartBlockingGame;
		PlayerSpawner.startUnblockingGameEvent += OnStartUnblockingGame;
		PlayerSleepController.fallingAsleepEvent += OnFallingAsleep;
		PlayerSleepController.sleepEndEvent += OnSleepEnd;
		Window.windowOpenedEvent += OnWindowOpened;
		image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
	}

	private void OnDestroy()
	{
		OutOfBoundsHandler.startBlockingGameEvent -= OnStartBlockingGame;
		OutOfBoundsHandler.startUnblockingGameEvent -= OnStartUnblockingGame;
		PlayerSpawner.startBlockingGameEvent -= OnStartBlockingGame;
		PlayerSpawner.startUnblockingGameEvent -= OnStartUnblockingGame;
		PlayerSleepController.fallingAsleepEvent -= OnFallingAsleep;
		PlayerSleepController.sleepEndEvent -= OnSleepEnd;
		Window.windowOpenedEvent -= OnWindowOpened;
	}

	private void OnFallingAsleep()
	{
		OnStartBlockingGame(ManagerBase<PlayerManager>.Instance.SleepBlockingTime);
	}

	private void OnSleepEnd()
	{
		if (SceneController.Instance.CurActiveScene == SceneController.SceneType.Game)
		{
			sleepEndFromMenu = false;
			OnStartBlockingGame(0f);
			OnStartUnblockingGame(ManagerBase<PlayerManager>.Instance.SleepUnblockingTime);
		}
		else
		{
			sleepEndFromMenu = true;
		}
	}

	private void OnWindowOpened(Window prevWindow)
	{
		if (Window.CurWindow == WindowSingleton<GameWindow>.Instance && sleepEndFromMenu)
		{
			OnSleepEnd();
		}
	}

	private void OnStartBlockingGame(float duration)
	{
		image.enabled = true;
		base.gameObject.SetActive(value: true);
		if (switchingCor != null)
		{
			StopCoroutine(switchingCor);
		}
		switchingCor = StartCoroutine(SwitchingPanelTransparency(transparency: true, duration, null));
	}

	private void OnStartUnblockingGame(float duration)
	{
		base.gameObject.SetActive(value: true);
		if (switchingCor != null)
		{
			StopCoroutine(switchingCor);
		}
		switchingCor = StartCoroutine(SwitchingPanelTransparency(transparency: false, duration, delegate
		{
			image.enabled = false;
		}));
	}

	private IEnumerator SwitchingPanelTransparency(bool transparency, float duration, Action endCallback)
	{
		float startAlpha = transparency ? 0f : 1f;
		float endAlpha = transparency ? 1f : 0f;
		float curTime = Mathf.InverseLerp(startAlpha, endAlpha, image.color.a) * duration;
		WaitForSecondsRealtime wait = new WaitForSecondsRealtime(0f);
		while (true)
		{
			float t = (duration != 0f) ? (curTime / duration) : 1f;
			float a = Mathf.Lerp(startAlpha, endAlpha, t);
			image.color = new Color(image.color.r, image.color.g, image.color.b, a);
			if (curTime >= duration)
			{
				break;
			}
			yield return wait;
			curTime = Mathf.Clamp(curTime + Time.unscaledDeltaTime, 0f, duration);
		}
		switchingCor = null;
		if (!transparency)
		{
			base.gameObject.SetActive(value: false);
		}
		endCallback?.Invoke();
	}
}
