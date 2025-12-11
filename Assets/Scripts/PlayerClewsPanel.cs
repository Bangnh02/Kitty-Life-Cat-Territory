public class PlayerClewsPanel : CollectablePanel<PlayerClewsPanel>
{
	public override void OnInitializeUI()
	{
		Clew.pickEvent += OnClewPick;
		NotificationPanel.endProcessingEvent += OnNotificationPanelEndProcessing;
		SaveManager.LoadEndEvent += OnLoadEnd;
		base.OnInitializeUI();
	}

	private void OnDestroy()
	{
		Clew.pickEvent -= OnClewPick;
		NotificationPanel.endProcessingEvent -= OnNotificationPanelEndProcessing;
		SaveManager.LoadEndEvent -= OnLoadEnd;
	}

	private void OnClewPick()
	{
		countText.text = $"{ManagerBase<ClewManager>.Instance.ClewsCollected}/{ManagerBase<ClewManager>.Instance.ClewsCount}";
		StartProcessingPanel();
	}

	private void OnLoadEnd()
	{
		countText.text = $"{ManagerBase<ClewManager>.Instance.ClewsCollected}/{ManagerBase<ClewManager>.Instance.ClewsCount}";
	}

	private void OnNotificationPanelEndProcessing()
	{
		bool num = NotificationPanel.Instances.Find((NotificationPanel x) => x.CurType == NotificationPanel.Type.Clew && (x.CurState & NotificationPanel.State.Processing) != NotificationPanel.State.None) != null;
		bool flag = canvasGroup.alpha == 1f && base.СurState == PanelState.Enabled;
		if (!num && flag && base.gameObject.activeInHierarchy)
		{
			myCoroutine = StartCoroutine(ChangeAlphaCoroutine());
		}
	}
}
