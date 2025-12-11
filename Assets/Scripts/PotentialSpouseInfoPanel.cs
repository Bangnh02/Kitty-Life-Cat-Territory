using UnityEngine;

public class PotentialSpouseInfoPanel : WorldRelativePanel
{
	private PotentialSpouseController potentialSpouse;

	[Header("PotentialSpouseInfoPanel")]
	[SerializeField]
	private GameObject iconPanel;

	[SerializeField]
	private ProcessingSwitch processingSwitch;

	private RectTransform rectTransform;

	public PotentialSpouseController PotentialSpouse => potentialSpouse;

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

	private bool IsMeetingPlayer
	{
		get
		{
			if (Singleton<SpouseSpawner>.Instance.PotentialSpouse == null)
			{
				return false;
			}
			return Singleton<SpouseSpawner>.Instance.PotentialSpouse.IsMeetingPlayer;
		}
	}

	public void Spawn(PotentialSpouseController potentialSpouse)
	{
		this.potentialSpouse = potentialSpouse;
		PotentialSpouseController.unspawnEvent += OnUnspawn;
		PlayerCombat.changeInCombatStateEvent += OnChangeInCombatState;
		processingSwitch.CheckDistanceGO = this.potentialSpouse.gameObject;
		processingSwitch.maxProcessingDistance = Singleton<SpouseSpawner>.Instance.InfoPanelProcessingDistance;
		processingSwitch.UpdateProcessingState();
		base.gameObject.SetActive(value: true);
		rectTransform = base.gameObject.GetComponent<RectTransform>();
		UpdatePanel();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		PotentialSpouseController.unspawnEvent -= OnUnspawn;
		PlayerCombat.changeInCombatStateEvent -= OnChangeInCombatState;
	}

	private void OnDisable()
	{
		if (!base.gameObject.activeSelf)
		{
			potentialSpouse = null;
			PotentialSpouseController.unspawnEvent -= OnUnspawn;
			PlayerCombat.changeInCombatStateEvent -= OnChangeInCombatState;
		}
	}

	private void OnUnspawn()
	{
		base.gameObject.SetActive(value: false);
		PotentialSpouseController.unspawnEvent -= OnUnspawn;
		PlayerCombat.changeInCombatStateEvent -= OnChangeInCombatState;
	}

	private void OnChangeInCombatState(bool inCombat)
	{
		if (inCombat)
		{
			iconPanel.SetActive(value: false);
			return;
		}
		iconPanel.SetActive(value: true);
		UpdatePanel();
	}

	protected override Vector3 GetTargetPosition()
	{
		return potentialSpouse.transform.position;
	}

	protected override bool HaveTarget()
	{
		return potentialSpouse != null;
	}

	protected override Vector3 GetWorldPositionOffset()
	{
		return potentialSpouse.WorldPanelOffset;
	}

	protected override RectTransform GetEnabledRectTransform()
	{
		return rectTransform;
	}
}
