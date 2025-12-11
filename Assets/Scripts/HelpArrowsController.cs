using UnityEngine;

public class HelpArrowsController : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private GameObject joystickArrow;

	[SerializeField]
	private float timeToCompleteJoystickHint = 3.5f;

	[SerializeField]
	private GameObject questArrow;

	[SerializeField]
	private GameObject mapArrow;

	private float moveTime;

	public void OnInitializeUI()
	{
		if (ManagerBase<HelpManager>.Instance.IsAllArrowShowed)
		{
			joystickArrow.SetActive(value: false);
			questArrow.SetActive(value: false);
			mapArrow.SetActive(value: false);
			return;
		}
		moveTime = 0f;
		QuestPanel.switchPanelStateEvent += OnQuestPanelSwitchState;
		Window.windowOpenedEvent += OnWindowOpened;
		SaveManager.LoadEndEvent += OnLoadEnd;
		if (!ManagerBase<HelpManager>.Instance.joystickArrowShowed)
		{
			joystickArrow.SetActive(value: true);
		}
		else if (!ManagerBase<HelpManager>.Instance.questArrowShowed)
		{
			questArrow.SetActive(value: true);
		}
		else if (!ManagerBase<HelpManager>.Instance.mapArrowShowed)
		{
			mapArrow.SetActive(value: true);
		}
	}

	private void OnDestroy()
	{
		QuestPanel.switchPanelStateEvent -= OnQuestPanelSwitchState;
		Window.windowOpenedEvent -= OnWindowOpened;
		SaveManager.LoadEndEvent += OnLoadEnd;
	}

	private void Update()
	{
		if (ManagerBase<HelpManager>.Instance.IsAllArrowShowed || ManagerBase<HelpManager>.Instance.joystickArrowShowed)
		{
			return;
		}
		if (ManagerBase<PlayerManager>.Instance.curSpeed > 0f)
		{
			if (joystickArrow.activeSelf)
			{
				joystickArrow.SetActive(value: false);
			}
			if (moveTime >= timeToCompleteJoystickHint)
			{
				ManagerBase<HelpManager>.Instance.joystickArrowShowed = true;
				if (!ManagerBase<HelpManager>.Instance.questArrowShowed)
				{
					questArrow.SetActive(value: true);
				}
				else if (!ManagerBase<HelpManager>.Instance.mapArrowShowed)
				{
					mapArrow.SetActive(value: true);
				}
			}
			moveTime += Time.deltaTime;
		}
		else if (!joystickArrow.activeSelf)
		{
			joystickArrow.SetActive(value: true);
		}
	}

	private void OnQuestPanelSwitchState()
	{
		questArrow.SetActive(value: false);
		ManagerBase<HelpManager>.Instance.questArrowShowed = true;
		if (!ManagerBase<HelpManager>.Instance.mapArrowShowed && ManagerBase<HelpManager>.Instance.joystickArrowShowed)
		{
			mapArrow.SetActive(value: true);
		}
		QuestPanel.switchPanelStateEvent -= OnQuestPanelSwitchState;
	}

	private void OnWindowOpened(Window prevWindow)
	{
		if (Window.CurWindow == WindowSingleton<MapWindow>.Instance)
		{
			mapArrow.SetActive(value: false);
			ManagerBase<HelpManager>.Instance.mapArrowShowed = true;
			Window.windowOpenedEvent -= OnWindowOpened;
		}
	}

	private void OnLoadEnd()
	{
		if (ManagerBase<HelpManager>.Instance.joystickArrowShowed)
		{
			joystickArrow.SetActive(value: false);
			questArrow.SetActive(value: true);
		}
		if (ManagerBase<HelpManager>.Instance.questArrowShowed)
		{
			questArrow.SetActive(value: false);
			mapArrow.SetActive(value: true);
			QuestPanel.switchPanelStateEvent -= OnQuestPanelSwitchState;
		}
		if (ManagerBase<HelpManager>.Instance.mapArrowShowed)
		{
			mapArrow.SetActive(value: false);
			Window.windowOpenedEvent -= OnWindowOpened;
		}
	}
}
