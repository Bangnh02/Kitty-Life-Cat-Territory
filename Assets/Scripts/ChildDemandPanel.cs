using Avelog;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChildDemandPanel : WorldRelativePanel
{
	private FamilyManager.FamilyMemberData familyMemberData;

	private FamilyMemberController familyMemberController;

	[Header("ChildDemandPanel")]
	[SerializeField]
	private GameObject eatDemandPanel;

	[SerializeField]
	private Image foodIcon;

	[SerializeField]
	private GameObject huntDemandPanel;

	[SerializeField]
	private Image enemyIcon;

	[SerializeField]
	private GameObject backgroundObj;

	[SerializeField]
	private float alphaInCombat = 0.5f;

	private RectTransform rectTransform;

	public void Spawn(FamilyManager.FamilyMemberData familyMember)
	{
		familyMemberData = familyMember;
		familyMemberController = PlayerSpawner.PlayerInstance.PlayerFamilyController.FamilyMembersControllers.Find((FamilyMemberController x) => x.familyMemberData == familyMemberData);
		PlayerCombat.changeInCombatStateEvent += OnChangeInCombatState;
		OnChangeInCombatState(PlayerSpawner.PlayerInstance.PlayerCombat.InCombat);
		FamilyMemberController.changeNeedEvent += OnChangeNeed;
		FamilyMemberController.satisfyNeedEvent += OnSatisfyNeed;
		FamilyManager.stageUpEvent += OnStageUp;
		UpdateNeedAndDemand();
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		PlayerCombat.changeInCombatStateEvent -= OnChangeInCombatState;
		FamilyMemberController.changeNeedEvent -= OnChangeNeed;
		FamilyMemberController.satisfyNeedEvent -= OnSatisfyNeed;
		FamilyManager.stageUpEvent -= OnStageUp;
	}

	private void OnDisable()
	{
		if (!base.gameObject.activeSelf)
		{
			familyMemberData = null;
			familyMemberController = null;
			PlayerCombat.changeInCombatStateEvent -= OnChangeInCombatState;
			FamilyMemberController.changeNeedEvent -= OnChangeNeed;
			FamilyMemberController.satisfyNeedEvent -= OnSatisfyNeed;
			FamilyManager.stageUpEvent -= OnStageUp;
		}
	}

	private void Awake()
	{
		rectTransform = base.gameObject.GetComponent<RectTransform>();
	}

	private void OnChangeInCombatState(bool inCombat)
	{
		alphaManualControl = inCombat;
		UpdateNeedAndDemand();
	}

	private void OnChangeNeed(FamilyMemberController familyMemberController)
	{
		if (this.familyMemberController == familyMemberController)
		{
			UpdateNeedAndDemand();
		}
	}

	private void OnSatisfyNeed(FamilyMemberController familyMemberController)
	{
		if (this.familyMemberController == familyMemberController)
		{
			UpdateNeedAndDemand();
		}
	}

	private void OnStageUp(FamilyManager.FamilyMemberData familyMemberData)
	{
		if (familyMemberData == familyMemberController.familyMemberData && familyMemberData.role == FamilyManager.FamilyMemberRole.ThirdStageChild)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void UpdateNeedAndDemand()
	{
		if (!string.IsNullOrEmpty(familyMemberData.curNeed))
		{
			bool num = FamilyMemberController.ItemIds.Any((string x) => x.ToString() == familyMemberData.curNeed);
			Sprite childNeedSprite = ManagerBase<UIManager>.Instance.GetChildNeedSprite(familyMemberData.curNeed);
			if (num)
			{
				eatDemandPanel.SetActive(value: true);
				huntDemandPanel.SetActive(value: false);
				foodIcon.sprite = childNeedSprite;
			}
			else
			{
				eatDemandPanel.SetActive(value: false);
				huntDemandPanel.SetActive(value: true);
				enemyIcon.sprite = childNeedSprite;
			}
			backgroundObj.SetActive(value: true);
		}
		else
		{
			eatDemandPanel.SetActive(value: false);
			huntDemandPanel.SetActive(value: false);
			backgroundObj.SetActive(value: false);
		}
	}

	protected override bool HaveTarget()
	{
		return familyMemberController != null;
	}

	protected override Vector3 GetTargetPosition()
	{
		return familyMemberController.transform.position;
	}

	protected override Vector3 GetWorldPositionOffset()
	{
		return familyMemberData.Params.WorldPanelOffset;
	}

	protected override RectTransform GetEnabledRectTransform()
	{
		return rectTransform;
	}

	public static (bool onProcessingDistance, float sqrDistance) GetProcessingData(FamilyMemberController familyMemberController)
	{
		if (familyMemberController.familyMemberData.role == FamilyManager.FamilyMemberRole.ThirdStageChild)
		{
			return (false, float.MaxValue);
		}
		float item = (CameraUtils.PlayerCamera != null) ? (CameraUtils.PlayerCamera.transform.position - familyMemberController.transform.position).sqrMagnitude : float.MaxValue;
		return (CameraUtils.PlayerCamera != null, item);
	}

	protected override void LateUpdate()
	{
		if (alphaManualControl)
		{
			if (IsOnBackside())
			{
				SetTargetAlpha(0f);
			}
			else
			{
				SetTargetAlpha(alphaInCombat);
			}
		}
		UpdatePanel();
	}
}
