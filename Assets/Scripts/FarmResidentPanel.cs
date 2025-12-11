using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class FarmResidentPanel : WorldRelativePanel
{
	[Header("FarmResidentPanel")]
	[SerializeField]
	private FarmResidentId farmResidentId;

	[SerializeField]
	private GameObject fullInfoPanel;

	[SerializeField]
	private GameObject iconInfoPanel;

	[SerializeField]
	private Text needProgressText;

	[SerializeField]
	private Image needIcon;

	[SerializeField]
	private Image farmResidentIcon;

	[SerializeField]
	private Localize farmResidentNameLoc;

	[SerializeField]
	private ProcessingSwitch processingSwitch;

	private FarmResident farmResident;

	private RectTransform iconRectTransform;

	private RectTransform fullRectTransform;

	private const string farmResidentCategoryName = "FarmResidentNames/";

	public FarmResidentId FarmResidentId => farmResidentId;

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
			if (farmResident == null)
			{
				return false;
			}
			return farmResident.CurState == FarmResident.State.MeetingPlayer;
		}
	}

	private bool NeedToEnablePanel
	{
		get
		{
			if (HaveTarget())
			{
				return farmResident.FarmResidentData.HaveNeed;
			}
			return false;
		}
	}

	public void Spawn(FarmResident farmResident)
	{
		FarmResident.changeStateEvent += OnFarmResidentChangeState;
		FarmResident.changeNeedEvent += OnFarmResidentChangeNeed;
		FarmResident.updateNeedProgressEvent += OnFarmResidentUpdateNeedProgress;
		PlayerCombat.changeInCombatStateEvent += OnChangeInCombatState;
		this.farmResident = farmResident;
		farmResidentId = this.farmResident.FarmResidentData.farmResidentId;
		farmResidentIcon.sprite = ManagerBase<UIManager>.Instance.GetFarmResidentSprite(farmResidentId);
		farmResidentNameLoc.SetTerm("FarmResidentNames/" + farmResidentId.ToString());
		if (farmResident.ForceShowPanel)
		{
			processingSwitch.maxProcessingDistance = float.MaxValue;
		}
		else if (!farmResident.FarmResidentData.HaveNeed)
		{
			processingSwitch.maxProcessingDistance = 0f;
		}
		else
		{
			processingSwitch.maxProcessingDistance = Singleton<FarmResidentSpawner>.Instance.InfoPanelProcessingDistance;
		}
		processingSwitch.CheckDistanceGO = this.farmResident.gameObject;
		processingSwitch.enabled = true;
		base.gameObject.SetActive(value: true);
		processingSwitch.MeasureDistance();
		processingSwitch.UpdateProcessingState(forceUpdate: true, instantEnabling: false, instantDisabling: true);
		UpdatePanelsState();
		UpdatePanelInfo();
	}

	private void Awake()
	{
		fullRectTransform = fullInfoPanel.GetComponent<RectTransform>();
		iconRectTransform = iconInfoPanel.GetComponent<RectTransform>();
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		FarmResident.changeStateEvent -= OnFarmResidentChangeState;
		FarmResident.changeNeedEvent -= OnFarmResidentChangeNeed;
		FarmResident.updateNeedProgressEvent -= OnFarmResidentUpdateNeedProgress;
		PlayerCombat.changeInCombatStateEvent -= OnChangeInCombatState;
	}

	private void OnDisable()
	{
		if (!base.gameObject.activeSelf)
		{
			farmResident = null;
			FarmResident.changeStateEvent -= OnFarmResidentChangeState;
			FarmResident.changeNeedEvent -= OnFarmResidentChangeNeed;
			FarmResident.updateNeedProgressEvent -= OnFarmResidentUpdateNeedProgress;
			PlayerCombat.changeInCombatStateEvent -= OnChangeInCombatState;
		}
	}

	private void OnFarmResidentChangeState(FarmResident farmResident)
	{
		if (!(farmResident != this.farmResident))
		{
			UpdatePanelsState();
		}
	}

	private void UpdatePanelsState()
	{
		if (!IsPlayerInCombat && IsMeetingPlayer && HaveTarget() && farmResident.FarmResidentData.HaveNeed)
		{
			fullInfoPanel.SetActive(value: true);
			iconInfoPanel.SetActive(value: false);
		}
		else
		{
			fullInfoPanel.SetActive(value: false);
			iconInfoPanel.SetActive(value: true);
		}
	}

	private void OnChangeInCombatState(bool inCombat)
	{
		UpdatePanelsState();
	}

	private void OnFarmResidentChangeNeed(FarmResident farmResident)
	{
		if (!(farmResident != this.farmResident))
		{
			UpdatePanelInfo();
		}
	}

	private void OnFarmResidentUpdateNeedProgress(FarmResident farmResident)
	{
		if (!(farmResident != this.farmResident))
		{
			UpdatePanelInfo();
		}
	}

	private new void OnEnable()
	{
		base.OnEnable();
		if (HaveTarget())
		{
			UpdatePanelInfo();
		}
	}

	private void UpdatePanelInfo()
	{
		UpdatePanelsState();
		needIcon.sprite = ManagerBase<UIManager>.Instance.GetFarmResidentNeedSprite(farmResident.FarmResidentData.curNeed);
		needProgressText.text = $"{farmResident.FarmResidentData.needProgressCurrent}/{farmResident.FarmResidentData.needProgressMaximum}";
	}

	protected override Vector3 GetTargetPosition()
	{
		return farmResident.transform.position;
	}

	protected override bool HaveTarget()
	{
		return farmResident != null;
	}

	protected override Vector3 GetWorldPositionOffset()
	{
		if (iconInfoPanel.activeSelf)
		{
			return farmResident.WorldIconOffset;
		}
		return farmResident.WorldFullOffset;
	}

	protected override RectTransform GetEnabledRectTransform()
	{
		if (iconInfoPanel.activeSelf)
		{
			return iconRectTransform;
		}
		return fullRectTransform;
	}
}
