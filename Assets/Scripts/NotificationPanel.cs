using I2.Loc;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationPanel : MonoBehaviour
{
	public enum Type
	{
		Experience,
		Coin,
		Quest,
		Attacked,
		Noticed,
		Clew
	}

	public enum State
	{
		None = 0,
		Enabling = 1,
		View = 2,
		Disabling = 4,
		Processing = 7
	}

	[SerializeField]
	private float enablingTime = 0.5f;

	[SerializeField]
	private float viewTime = 4f;

	[SerializeField]
	private float disablingTime = 2f;

	[Header("Ссылки")]
	[SerializeField]
	[Header("Панель с иконкой")]
	private GameObject withIconPanel;

	[SerializeField]
	private Text withIconInfoText;

	[SerializeField]
	private Image withIconTypeImage;

	[Header("Панель с рамкой и фоном")]
	[SerializeField]
	private GameObject framedPanel;

	[SerializeField]
	private Localize framedTextLoc;

	[Header("Панель без рамки с полупрозрачным фоном")]
	[SerializeField]
	private GameObject notFramedPanel;

	[SerializeField]
	private Localize notFramedTextLoc;

	[Header("Общие")]
	[SerializeField]
	private CanvasGroup canvasGroup;

	private Coroutine myCoroutine;

	private Vector3 startPosition;

	private float timer;

	private const string notificationsTermCategory = "Notifications/";

	private const string questNotificationTerm = "QuestNotification";

	private const string attackedNotificationTerm = "AttackedNotification";

	private const string noticedNotificationTerm = "NoticedNotification";

	public static List<NotificationPanel> Instances
	{
		get;
		private set;
	} = new List<NotificationPanel>();


	public State CurState
	{
		get;
		private set;
	}

	public Type CurType
	{
		get;
		private set;
	}

	public RectTransform RectTransform
	{
		get;
		private set;
	}

	public static event Action endProcessingEvent;

	public void Init()
	{
		Instances.Add(this);
		RectTransform = GetComponent<RectTransform>();
		startPosition = RectTransform.localPosition;
	}

	public void SetupAndSpawn(int incValue, Type type)
	{
		withIconPanel.SetActive(value: true);
		framedPanel.SetActive(value: false);
		notFramedPanel.SetActive(value: false);
		CurType = type;
		withIconInfoText.text = ((incValue > 0) ? ("+" + incValue.ToString()) : incValue.ToString());
		Sprite sprite;
		switch (type)
		{
		case Type.Coin:
			sprite = ManagerBase<UIManager>.Instance.SmallCoinSprite;
			break;
		case Type.Experience:
			sprite = ManagerBase<UIManager>.Instance.ExperienceIcon;
			break;
		case Type.Clew:
			sprite = ManagerBase<UIManager>.Instance.ClewIcon;
			break;
		default:
			sprite = ManagerBase<UIManager>.Instance.SmallCoinSprite;
			break;
		}
		withIconTypeImage.sprite = sprite;
		StartEnabling();
	}

	public void SetupAndSpawnFramedPanel(string text, Type type)
	{
		withIconPanel.SetActive(value: false);
		framedPanel.SetActive(value: true);
		notFramedPanel.SetActive(value: false);
		CurType = type;
		if (type == Type.Quest)
		{
			framedTextLoc.SetTerm("Notifications/QuestNotification");
		}
		StartEnabling();
	}

	public void SetupAndSpawnNotFramedPanel(string text, Type type)
	{
		withIconPanel.SetActive(value: false);
		framedPanel.SetActive(value: false);
		notFramedPanel.SetActive(value: true);
		CurType = type;
		switch (type)
		{
		case Type.Attacked:
			notFramedTextLoc.SetTerm("Notifications/AttackedNotification");
			break;
		case Type.Noticed:
			notFramedTextLoc.SetTerm("Notifications/NoticedNotification");
			break;
		}
		StartEnabling();
	}

	private void StartEnabling()
	{
		RectTransform.localPosition = startPosition;
		CurState = State.Enabling;
		canvasGroup.alpha = 0f;
		if (base.gameObject.activeInHierarchy)
		{
			myCoroutine = StartCoroutine(Enabling());
		}
	}

	private IEnumerator Enabling(bool needResetTimer = true)
	{
		while (Time.deltaTime == 0f)
		{
			yield return null;
		}
		if (needResetTimer)
		{
			timer = enablingTime;
		}
		while (true)
		{
			timer = Mathf.Clamp(timer - Time.deltaTime, 0f, enablingTime);
			float num = timer / enablingTime;
			canvasGroup.alpha = Mathf.Lerp(1f, 0f, num);
			if (num == 0f)
			{
				break;
			}
			yield return null;
		}
		myCoroutine = StartCoroutine(View());
	}

	private IEnumerator View(bool needResetTimer = true)
	{
		CurState = State.View;
		while (Time.deltaTime == 0f)
		{
			yield return null;
		}
		if (needResetTimer)
		{
			timer = viewTime;
		}
		while (true)
		{
			timer = Mathf.Clamp(timer - Time.deltaTime, 0f, viewTime);
			if (timer == 0f)
			{
				break;
			}
			yield return null;
		}
		myCoroutine = StartCoroutine(Disabling());
	}

	private IEnumerator Disabling(bool needResetTimer = true)
	{
		CurState = State.Disabling;
		while (Time.deltaTime == 0f)
		{
			yield return null;
		}
		if (needResetTimer)
		{
			timer = disablingTime;
		}
		while (true)
		{
			timer = Mathf.Clamp(timer - Time.deltaTime, 0f, disablingTime);
			float num = timer / disablingTime;
			canvasGroup.alpha = Mathf.Lerp(0f, 1f, num);
			if (num == 0f)
			{
				break;
			}
			yield return null;
		}
		CurState = State.None;
		NotificationPanel.endProcessingEvent?.Invoke();
	}

	public void ForeEndProcessing()
	{
		canvasGroup.alpha = 0f;
		CurState = State.None;
		if (myCoroutine != null)
		{
			StopCoroutine(myCoroutine);
			NotificationPanel.endProcessingEvent?.Invoke();
		}
	}

	private void OnEnable()
	{
		if (CurState == State.Enabling)
		{
			myCoroutine = StartCoroutine(Enabling(needResetTimer: false));
		}
		else if (CurState == State.View)
		{
			myCoroutine = StartCoroutine(View(needResetTimer: false));
		}
		else if (CurState == State.Disabling)
		{
			myCoroutine = StartCoroutine(Disabling(needResetTimer: false));
		}
	}

	private void OnDisable()
	{
		if (myCoroutine != null)
		{
			StopCoroutine(myCoroutine);
		}
	}
}
