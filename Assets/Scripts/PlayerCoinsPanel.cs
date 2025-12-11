public class PlayerCoinsPanel : CollectablePanel<PlayerCoinsPanel>
{
	private PlayerPlanter PlayerPlanter => PlayerSpawner.PlayerInstance.PlayerPlanter;

	private bool IsCLewPanelProcessing => CollectablePanel<PlayerClewsPanel>.Instance.СurState != CollectablePanel<PlayerClewsPanel>.PanelState.Disabled;

	public override void OnInitializeUI()
	{
		PlayerManager.coinsChangeEvent += UpdatePanel;
		NotificationPanel.endProcessingEvent += OnNotificationPanelEndProcessing;
		LevelUpPanel.startProcessingEvent += OnExtraPanelStartProcessing;
		SuperBonusPanel.startProcessingEvent += OnExtraPanelStartProcessing;
		PlayerPlanter.updateNearPotentialPlantEvent += OnUpdateNearPotentialPlant;
		CollectablePanel<PlayerClewsPanel>.Instance.startProcessingEvent += OnClewPanelStartProcessing;
		SaveManager.LoadEndEvent += OnLoadEnd;
		base.OnInitializeUI();
	}

	private void OnDestroy()
	{
		PlayerManager.coinsChangeEvent -= UpdatePanel;
		NotificationPanel.endProcessingEvent -= OnNotificationPanelEndProcessing;
		LevelUpPanel.startProcessingEvent -= OnExtraPanelStartProcessing;
		SuperBonusPanel.startProcessingEvent -= OnExtraPanelStartProcessing;
		PlayerPlanter.updateNearPotentialPlantEvent -= OnUpdateNearPotentialPlant;
		CollectablePanel<PlayerClewsPanel>.Instance.startProcessingEvent -= OnClewPanelStartProcessing;
		SaveManager.LoadEndEvent -= OnLoadEnd;
	}

	private void UpdatePanel(int coinsInc = 0)
	{
		countText.text = ManagerBase<PlayerManager>.Instance.CurCoins.ToString();
		if (base.gameObject.activeInHierarchy && LevelUpPanel.Instance.CurState == ExtraPanel.State.Disabled && SuperBonusPanel.Instance.CurState == ExtraPanel.State.Disabled && !IsCLewPanelProcessing && base.СurState != PanelState.Enabled && base.СurState != 0)
		{
			StartProcessingPanel();
		}
	}

	private void OnUpdateNearPotentialPlant()
	{
		countText.text = ManagerBase<PlayerManager>.Instance.CurCoins.ToString();
		if (PlayerPlanter.HaveNearPotentialPlants)
		{
			if (base.СurState != PanelState.Enabled && base.СurState != 0)
			{
				canvasGroup.alpha = 1f;
				myCoroutine = StartCoroutine(ChangeScaleCoroutine());
			}
		}
		else if (!(NotificationPanel.Instances.Find((NotificationPanel x) => x.CurType == NotificationPanel.Type.Coin && (x.CurState & NotificationPanel.State.Processing) != NotificationPanel.State.None) != null))
		{
			myCoroutine = StartCoroutine(ChangeAlphaCoroutine());
		}
	}

	private void OnNotificationPanelEndProcessing()
	{
		bool num = NotificationPanel.Instances.Find((NotificationPanel x) => x.CurType == NotificationPanel.Type.Coin && (x.CurState & NotificationPanel.State.Processing) != NotificationPanel.State.None) != null;
		bool flag = canvasGroup.alpha == 1f && base.СurState == PanelState.Enabled;
		if (!num && flag && !PlayerPlanter.HaveNearPotentialPlants && base.gameObject.activeInHierarchy)
		{
			myCoroutine = StartCoroutine(ChangeAlphaCoroutine());
		}
	}

	private void OnExtraPanelStartProcessing()
	{
		ForceDisable();
	}

	private void OnClewPanelStartProcessing()
	{
		ForceDisable();
	}

	private void ForceDisable()
	{
		if (canvasGroup.alpha != 0f)
		{
			base.СurState = PanelState.Disabled;
			canvasGroup.alpha = 0f;
			if (myCoroutine != null)
			{
				StopCoroutine(myCoroutine);
			}
		}
	}

	private void OnLoadEnd()
	{
		countText.text = ManagerBase<PlayerManager>.Instance.CurCoins.ToString();
	}
}
