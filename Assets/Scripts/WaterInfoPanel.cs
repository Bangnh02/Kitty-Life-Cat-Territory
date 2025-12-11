using UnityEngine;

public class WaterInfoPanel : WorldRelativePanel
{
	private Water water;

	[Header("WaterInfoPanel")]
	[SerializeField]
	private ProcessingSwitch processingSwitch;

	[SerializeField]
	private GameObject iconInfoPanel;

	private RectTransform rectTransform;

	public GameObject WaterObject
	{
		get;
		private set;
	}

	private PlayerEating PlayerEating => PlayerSpawner.PlayerInstance?.PlayerEating;

	private bool IsPlayerInCombat
	{
		get
		{
			if (PlayerSpawner.PlayerInstance == null)
			{
				return false;
			}
			return PlayerSpawner.PlayerInstance.PlayerCombat.InCombat;
		}
	}

	public void Spawn(Water water)
	{
		this.water = water;
		WaterObject = water.gameObject;
		PlayerEating.changeNearWaterEvent += UpdateActiveStatePanel;
		PlayerCombat.changeInCombatStateEvent += OnChangeInCombatState;
		PlayerEating.changeThirstEvent += UpdateActiveStatePanel;
		processingSwitch.CheckDistanceGO = WaterObject;
		base.gameObject.SetActive(value: true);
		rectTransform = base.gameObject.GetComponent<RectTransform>();
		UpdateActiveStatePanel();
	}

	private void OnDisable()
	{
		if (!base.gameObject.activeSelf)
		{
			WaterObject = null;
			water = null;
			PlayerEating.changeNearWaterEvent -= UpdateActiveStatePanel;
			PlayerCombat.changeInCombatStateEvent -= OnChangeInCombatState;
			PlayerEating.changeThirstEvent -= UpdateActiveStatePanel;
		}
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		PlayerEating.changeNearWaterEvent -= UpdateActiveStatePanel;
		PlayerCombat.changeInCombatStateEvent -= OnChangeInCombatState;
		PlayerEating.changeThirstEvent -= UpdateActiveStatePanel;
	}

	private void OnChangeInCombatState(bool inCombat)
	{
		UpdateActiveStatePanel();
	}

	private void UpdateActiveStatePanel()
	{
		bool flag = PlayerEating.NearWaterObjs.Exists((Water waterCollider) => waterCollider.gameObject == WaterObject);
		bool haveThirst = ManagerBase<PlayerManager>.Instance.HaveThirst;
		processingSwitch.MeasureDistance();
		if ((IsPlayerInCombat | flag) || !haveThirst || !processingSwitch.OnProcessingDistance)
		{
			iconInfoPanel.SetActive(value: false);
		}
		else
		{
			iconInfoPanel.SetActive(value: true);
		}
	}

	protected override Vector3 GetTargetPosition()
	{
		return WaterObject.transform.position;
	}

	protected override bool HaveTarget()
	{
		return WaterObject != null;
	}

	protected override Vector3 GetWorldPositionOffset()
	{
		return Vector3.zero;
	}

	protected override RectTransform GetEnabledRectTransform()
	{
		return rectTransform;
	}
}
