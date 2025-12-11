using System.Collections.Generic;
using UnityEngine;

public class NotificationsPanel : MonoBehaviour, IInitializableUI
{
	private List<NotificationPanel> spawnedPanels = new List<NotificationPanel>();

	[SerializeField]
	private GameObject notificationPanelPrefab;

	[SerializeField]
	private float minOffset;

	[SerializeField]
	private float forceEndProcessingPos;

	public void OnInitializeUI()
	{
		PlayerManager.coinsChangeEvent += OnCoinsChange;
		PlayerManager.experienceUpdateEvent += OnExperienceUpdate;
		Clew.pickEvent += OnClewPick;
		Quest.startEvent += OnQuestStart;
		EnemyController.selfTransitionToAnyAttackStateEvent += OnEnemySelfTransitionToAnyAttackState;
	}

	private void OnDestroy()
	{
		PlayerManager.coinsChangeEvent -= OnCoinsChange;
		PlayerManager.experienceUpdateEvent -= OnExperienceUpdate;
		Clew.pickEvent -= OnClewPick;
		Quest.startEvent -= OnQuestStart;
		EnemyController.selfTransitionToAnyAttackStateEvent += OnEnemySelfTransitionToAnyAttackState;
	}

	private void OnQuestStart(Quest quest)
	{
		SetupAndSpawnPanel(string.Empty, withFrame: true, NotificationPanel.Type.Quest);
	}

	private void OnClewPick()
	{
		SetupAndSpawnPanel(1, NotificationPanel.Type.Clew);
	}

	private void OnEnemySelfTransitionToAnyAttackState(bool isInAttack)
	{
		if (!PlayerSpawner.PlayerInstance.PlayerCombat.InCombat)
		{
			if (isInAttack)
			{
				SetupAndSpawnPanel(string.Empty, withFrame: false, NotificationPanel.Type.Attacked);
			}
			else
			{
				SetupAndSpawnPanel(string.Empty, withFrame: false, NotificationPanel.Type.Noticed);
			}
		}
	}

	private void OnExperienceUpdate(float expInc, int levelUps)
	{
		SetupAndSpawnPanel((int)expInc, NotificationPanel.Type.Experience);
	}

	private void OnCoinsChange(int coinsInc = 0)
	{
		SetupAndSpawnPanel(coinsInc, NotificationPanel.Type.Coin);
	}

	private void SetupAndSpawnPanel(string text, bool withFrame, NotificationPanel.Type type)
	{
		NotificationPanel freeNotificationPanel = GetFreeNotificationPanel();
		if (withFrame)
		{
			freeNotificationPanel.SetupAndSpawnFramedPanel(text, type);
		}
		else
		{
			freeNotificationPanel.SetupAndSpawnNotFramedPanel(text, type);
		}
		freeNotificationPanel.RectTransform.SetAsLastSibling();
		RecalculatePanelsPositions();
	}

	private void SetupAndSpawnPanel(int incValue, NotificationPanel.Type type)
	{
		if (incValue != 0)
		{
			NotificationPanel freeNotificationPanel = GetFreeNotificationPanel();
			freeNotificationPanel.SetupAndSpawn(incValue, type);
			freeNotificationPanel.RectTransform.SetAsLastSibling();
			RecalculatePanelsPositions();
		}
	}

	private NotificationPanel GetFreeNotificationPanel()
	{
		NotificationPanel notificationPanel = spawnedPanels.Find((NotificationPanel x) => x.CurState == NotificationPanel.State.None);
		if (notificationPanel == null)
		{
			notificationPanel = UnityEngine.Object.Instantiate(notificationPanelPrefab, base.transform).GetComponent<NotificationPanel>();
			notificationPanel.Init();
			spawnedPanels.Add(notificationPanel);
		}
		return notificationPanel;
	}

	private void RecalculatePanelsPositions()
	{
		List<NotificationPanel> list = spawnedPanels.FindAll((NotificationPanel x) => (x.CurState & NotificationPanel.State.Processing) != NotificationPanel.State.None);
		if (list.Count <= 1)
		{
			return;
		}
		list.Sort((NotificationPanel o1, NotificationPanel o2) => o2.RectTransform.GetSiblingIndex().CompareTo(o1.RectTransform.GetSiblingIndex()));
		for (int i = 0; i < list.Count - 1; i++)
		{
			NotificationPanel notificationPanel = list[i];
			NotificationPanel notificationPanel2 = list[i + 1];
			float num = notificationPanel.RectTransform.localPosition.y + notificationPanel.RectTransform.rect.height / 2f + minOffset;
			float num2 = notificationPanel2.RectTransform.localPosition.y - notificationPanel2.RectTransform.rect.height / 2f;
			if (num > num2)
			{
				Vector3 vector = new Vector3(0f, num - num2, 0f);
				notificationPanel2.RectTransform.localPosition += vector;
				if (notificationPanel2.RectTransform.localPosition.y >= forceEndProcessingPos)
				{
					notificationPanel2.ForeEndProcessing();
				}
			}
		}
	}
}
