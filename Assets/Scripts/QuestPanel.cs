using Avelog;
using Avelog.UI;
using I2.Loc;
using System;
using System.Collections;
using UnityEngine;

public class QuestPanel : MonoBehaviour, IInitializableUI
{
	public enum PanelState
	{
		Pushed,
		Pushing,
		Pulled,
		Pulling
	}

	[SerializeField]
	private float drivingTime;

	[Header("Ссылки")]
	[SerializeField]
	private GameObject hintButton;

	[SerializeField]
	private Bar bar;

	[SerializeField]
	private Localize questNameLoc;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Seconds)]
	private float playerMoveToPushDuration = 3f;

	private float playerMoveTimer;

	private float waitAfterQuestCompleateTimer;

	private RectTransform questPanelRectTransform;

	private Coroutine switchingCoroutine;

	private float openPanelXPos;

	private float closePanelXPos;

	private float curDrivingTime;

	private const string questCompleteTerm = "QuestComplete";

	private const string questNamesLocCategory = "QuestNames/";

	private PanelState _curPanelState;

	private PanelState prevPanelState = PanelState.Pushing;

	private bool HaveQuest => Singleton<QuestSpawner>.Instance.HaveActiveQuest;

	private float WaitAfterQuestCompleateTime => Singleton<QuestSpawner>.Instance.BetweenQuestsPauseDuration - 2f * drivingTime;

	public PanelState CurPanelState
	{
		get
		{
			return _curPanelState;
		}
		private set
		{
			prevPanelState = _curPanelState;
			_curPanelState = value;
		}
	}

	public static event Action switchPanelStateEvent;

	public void OnInitializeUI()
	{
		Quest.startEvent += OnStartQuest;
		Quest.updateProgressEvent += UpdateProgressUI;
		Quest.completeEvent += OnCompleteQuest;
		Quest.cancelEvent += OnCancelQuest;
		questPanelRectTransform = GetComponent<RectTransform>();
		if (SceneController.Instance.StartScene == SceneController.SceneType.Game)
		{
			UpdateProgressUI(Singleton<QuestSpawner>.Instance.ActiveQuest);
			if (Singleton<QuestSpawner>.Instance.ActiveQuest != null)
			{
				OnStartQuest(Singleton<QuestSpawner>.Instance.ActiveQuest);
			}
		}
		RectTransform component = base.transform.parent.GetComponentInChildren<MenuButton>(includeInactive: true).gameObject.GetComponent<RectTransform>();
		float num = Mathf.Abs(component.rect.width / 2f - Mathf.Abs(component.anchoredPosition.x));
		openPanelXPos = -1f * (questPanelRectTransform.rect.width / 2f + num);
		closePanelXPos = questPanelRectTransform.anchoredPosition.x;
	}

	private void OnDestroy()
	{
		Quest.startEvent -= OnStartQuest;
		Quest.updateProgressEvent -= UpdateProgressUI;
		Quest.completeEvent -= OnCompleteQuest;
		Quest.cancelEvent -= OnCancelQuest;
	}

	private void OnStartQuest(Quest quest)
	{
		questNameLoc.SetTerm("QuestNames/" + quest.name);
		hintButton.SetActive(value: true);
		bar.SetValueInstantly(quest.CurProgress / quest.MaxProgress);
		UpdateProgressUI(quest);
	}

	private void UpdateProgressUI(Quest quest)
	{
		if (quest != null)
		{
			bar.SetValue(quest.CurProgress / quest.MaxProgress);
		}
		else
		{
			bar.SetValueInstantly(0f);
		}
	}

	private void OnCompleteQuest(Quest quest)
	{
		questNameLoc.SetTerm("QuestNames/QuestComplete");
		hintButton.SetActive(value: false);
		if (CurPanelState == PanelState.Pushed)
		{
			SwitchPanel();
		}
	}

	private void OnCancelQuest(Quest quest)
	{
		hintButton.SetActive(value: false);
		if (CurPanelState == PanelState.Pulled || CurPanelState == PanelState.Pulling)
		{
			SwitchPanel();
		}
	}

	public void SwitchPanel()
	{
		bool flag = CurPanelState == PanelState.Pushed || CurPanelState == PanelState.Pushing;
		if (NeedSwitch(flag) && base.gameObject.activeInHierarchy)
		{
			CurPanelState = ((!flag) ? PanelState.Pushing : PanelState.Pulling);
			if (switchingCoroutine != null)
			{
				StopCoroutine(switchingCoroutine);
			}
			switchingCoroutine = StartCoroutine(Switching());
			QuestPanel.switchPanelStateEvent?.Invoke();
		}
	}

	private bool NeedSwitch(bool neededEnableState)
	{
		if (neededEnableState)
		{
			if (CurPanelState == PanelState.Pushed)
			{
				return true;
			}
		}
		else if (CurPanelState == PanelState.Pulled)
		{
			return true;
		}
		return false;
	}

	private IEnumerator Switching(bool needResetTime = true)
	{
		if (needResetTime)
		{
			curDrivingTime = 0f;
		}
		while (true)
		{
			curDrivingTime += Time.deltaTime;
			float t = curDrivingTime / drivingTime;
			float x = 0f;
			if (CurPanelState == PanelState.Pulling)
			{
				x = Mathf.Lerp(closePanelXPos, openPanelXPos, t);
			}
			else if (CurPanelState == PanelState.Pushing)
			{
				x = Mathf.Lerp(openPanelXPos, closePanelXPos, t);
			}
			Vector2 anchoredPosition = new Vector2(x, questPanelRectTransform.anchoredPosition.y);
			questPanelRectTransform.anchoredPosition = anchoredPosition;
			if (curDrivingTime >= drivingTime)
			{
				break;
			}
			yield return null;
		}
		if (CurPanelState == PanelState.Pulling)
		{
			OnQuestPanelPullEnd();
		}
		else if (CurPanelState == PanelState.Pushing)
		{
			OnQuestPanelPushEnd();
		}
	}

	public void OpenHintWindow()
	{
		Avelog.Input.FireQuestHintPressed();
	}

	private void OnDisable()
	{
		if (CurPanelState == PanelState.Pulling || CurPanelState == PanelState.Pushing)
		{
			StopCoroutine(switchingCoroutine);
		}
	}

	private void OnEnable()
	{
		if (CurPanelState == PanelState.Pulling || CurPanelState == PanelState.Pushing)
		{
			switchingCoroutine = StartCoroutine(Switching(needResetTime: false));
		}
	}

	private void OnQuestPanelPushEnd()
	{
		CurPanelState = PanelState.Pushed;
	}

	private void OnQuestPanelPullEnd()
	{
		CurPanelState = PanelState.Pulled;
	}

	private void Update()
	{
		if (Time.deltaTime == 0f)
		{
			return;
		}
		if (CurPanelState == PanelState.Pulled && Avelog.Input.VertAxis != 0f)
		{
			playerMoveTimer -= Time.deltaTime;
			if (playerMoveTimer <= 0f)
			{
				SwitchPanel();
			}
		}
		else
		{
			playerMoveTimer = playerMoveToPushDuration;
		}
		if (CurPanelState == PanelState.Pulled && UnityEngine.Input.GetKeyDown(KeyCode.Q))
		{
			SwitchPanel();
		}
		if (!HaveQuest && CurPanelState == PanelState.Pulled)
		{
			waitAfterQuestCompleateTimer -= Time.deltaTime;
			if (waitAfterQuestCompleateTimer <= 0f)
			{
				SwitchPanel();
			}
		}
		else if (waitAfterQuestCompleateTimer != WaitAfterQuestCompleateTime)
		{
			waitAfterQuestCompleateTimer = WaitAfterQuestCompleateTime;
		}
	}
}
