using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SwitchQuestPanelButton : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private float blinkTime = 1f;

	[Header("Ссылки")]
	[SerializeField]
	private QuestPanel questPanel;

	[SerializeField]
	private GameObject exclamationtObj;

	[SerializeField]
	private Image exclamationtBlinkImg;

	[SerializeField]
	private GameObject questionObj;

	private bool isBlinking;

	private bool isFullAlphaBlink;

	private float curBlinkTime;

	private Coroutine blinkingCoroutine;

	public void OnInitializeUI()
	{
		Quest.startEvent += OnQuestStart;
		QuestPanel.switchPanelStateEvent += OnSwitchPanelState;
		if (Singleton<QuestSpawner>.IsExist)
		{
			OnQuestSpawnerInitialize();
		}
		QuestSpawner.initializeEvent = (Action)Delegate.Combine(QuestSpawner.initializeEvent, new Action(OnQuestSpawnerInitialize));
	}

	private void OnDestroy()
	{
		Quest.startEvent -= OnQuestStart;
		QuestSpawner.initializeEvent = (Action)Delegate.Remove(QuestSpawner.initializeEvent, new Action(OnQuestSpawnerInitialize));
		QuestPanel.switchPanelStateEvent -= OnSwitchPanelState;
	}

	private void OnQuestStart(Quest quest)
	{
		base.gameObject.SetActive(value: true);
		exclamationtObj.SetActive(value: true);
		questionObj.SetActive(value: false);
		if (!isBlinking)
		{
			isBlinking = true;
			if (base.gameObject.activeInHierarchy)
			{
				blinkingCoroutine = StartCoroutine(Blinking());
			}
		}
	}

	private IEnumerator Blinking(bool needResetTime = true)
	{
		if (needResetTime)
		{
			curBlinkTime = 0f;
			isFullAlphaBlink = true;
		}
		while (true)
		{
			float t = curBlinkTime / blinkTime;
			float a = (!isFullAlphaBlink) ? Mathf.Lerp(1f, 0f, t) : Mathf.Lerp(0f, 1f, t);
			Color color = exclamationtBlinkImg.color;
			color.a = a;
			exclamationtBlinkImg.color = color;
			curBlinkTime += Time.deltaTime;
			if (curBlinkTime >= blinkTime)
			{
				curBlinkTime -= blinkTime;
				isFullAlphaBlink = !isFullAlphaBlink;
			}
			yield return null;
		}
	}

	private void OnSwitchPanelState()
	{
		if (isBlinking)
		{
			isBlinking = false;
			StopCoroutine(blinkingCoroutine);
		}
		if (questPanel.CurPanelState == QuestPanel.PanelState.Pulling)
		{
			questionObj.SetActive(value: false);
			base.gameObject.SetActive(value: false);
		}
		else if (questPanel.CurPanelState == QuestPanel.PanelState.Pushing)
		{
			questionObj.SetActive(Singleton<QuestSpawner>.Instance.HaveActiveQuest);
			base.gameObject.SetActive(Singleton<QuestSpawner>.Instance.HaveActiveQuest);
		}
		exclamationtObj.SetActive(value: false);
	}

	private void OnQuestSpawnerInitialize()
	{
		if (Singleton<QuestSpawner>.Instance.ActiveQuest != null)
		{
			OnQuestStart(Singleton<QuestSpawner>.Instance.ActiveQuest);
		}
	}

	public void SwitchPanel()
	{
		questPanel.SwitchPanel();
		if (isBlinking)
		{
			isBlinking = false;
			StopCoroutine(blinkingCoroutine);
		}
	}

	private void OnDisable()
	{
		if (isBlinking)
		{
			StopCoroutine(blinkingCoroutine);
		}
	}

	private void OnEnable()
	{
		if (Singleton<QuestSpawner>.IsExist && isBlinking)
		{
			blinkingCoroutine = StartCoroutine(Blinking(needResetTime: false));
		}
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Q))
		{
			SwitchPanel();
		}
	}
}
