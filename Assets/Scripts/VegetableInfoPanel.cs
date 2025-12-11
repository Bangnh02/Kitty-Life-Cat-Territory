using Avelog;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class VegetableInfoPanel : WorldRelativePanel
{
	[Header("VegetableInfoPanel. Ссылки")]
	[SerializeField]
	private GameObject pricePanel;

	[SerializeField]
	private Localize nameLoc;

	[SerializeField]
	private Text priceText;

	[SerializeField]
	private Localize bonusLoc;

	[SerializeField]
	private LocalizationParamsManager bonusParamsLoc;

	[SerializeField]
	private ProcessingSwitch processingSwitch;

	private VegetableBehaviour vegetable;

	private VegetableManager.VegetableData vegetableData;

	private RectTransform rectTransform;

	private const string vegetableCategoryName = "VegetableNames/";

	private const string bonusLocalParam = "bonus";

	private const string turnipBonusTerm = "GameWindow/TurnipBonus";

	private const string moveSpeedBonusTerm = "GameWindow/MoveSpeedBonus";

	private const string healthBonusTerm = "GameWindow/HealthBonus";

	private const string beetBonusTerm = "GameWindow/BeetBonus";

	protected override Vector3 GetTargetPosition()
	{
		return vegetable.transform.position;
	}

	protected override bool HaveTarget()
	{
		return vegetable != null;
	}

	private new void OnEnable()
	{
		base.OnEnable();
		if (HaveTarget())
		{
			UpdateProcessingSwitchState();
			UpdatePanelTexts();
		}
	}

	public void Spawn(VegetableBehaviour vegetable)
	{
		this.vegetable = vegetable;
		vegetableData = vegetable.VegetableData;
		VegetableBehaviour.grownEvent += OnVegetableGrown;
		VegetableBehaviour.plantEvent += OnPlant;
		vegetable.updateStateEvent += UpdatePanelTexts;
		vegetable.updateStateEvent += UpdateProcessingSwitchState;
		vegetable.switchPlayerInTriggerEvent += UpdateProcessingSwitchState;
		base.gameObject.SetActive(value: true);
		processingSwitch.enabled = true;
		processingSwitch.manualControl = true;
		processingSwitch.Switch(isEnabled: true, forceUpdate: true);
		rectTransform = GetComponent<RectTransform>();
		UpdatePanelTexts();
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		VegetableBehaviour.grownEvent -= OnVegetableGrown;
		VegetableBehaviour.plantEvent -= OnPlant;
		if (vegetable != null)
		{
			vegetable.updateStateEvent -= UpdatePanelTexts;
			vegetable.updateStateEvent -= UpdateProcessingSwitchState;
			vegetable.switchPlayerInTriggerEvent -= UpdateProcessingSwitchState;
		}
	}

	private void OnDisable()
	{
		if (!base.gameObject.activeSelf)
		{
			VegetableBehaviour.grownEvent -= OnVegetableGrown;
			VegetableBehaviour.plantEvent -= OnPlant;
			if (vegetable != null)
			{
				vegetable.updateStateEvent -= UpdatePanelTexts;
				vegetable.updateStateEvent -= UpdateProcessingSwitchState;
				vegetable.switchPlayerInTriggerEvent -= UpdateProcessingSwitchState;
				vegetable = null;
			}
		}
	}

	private void OnVegetableGrown(VegetableBehaviour vegetable)
	{
		if (vegetable == this.vegetable)
		{
			UpdatePanelTexts();
		}
	}

	private void UpdateProcessingSwitchState()
	{
		if (vegetable.IsPlayerInTrigger)
		{
			base.CurAlpha = 1f;
		}
		processingSwitch.Switch(vegetable.IsPlayerInTrigger);
	}

	private void UpdatePanelTexts()
	{
		nameLoc.SetTerm("VegetableNames/" + vegetableData.type.ToString());
		if (vegetableData.isPlanted)
		{
			pricePanel.SetActive(value: false);
		}
		else
		{
			pricePanel.SetActive(value: true);
			priceText.text = vegetableData.vegetableParams.cost.ToString();
			if (ManagerBase<PlayerManager>.Instance.CurCoins >= vegetableData.vegetableParams.cost)
			{
				priceText.color = ManagerBase<UIManager>.Instance.EnoughCoinsTextColor;
			}
			else
			{
				priceText.color = ManagerBase<UIManager>.Instance.NotEnoughCoinsTextColor;
			}
		}
		if (vegetableData.type == VegetableManager.VegetableType.Beet)
		{
			bonusLoc.SetTerm("GameWindow/BeetBonus");
			bonusParamsLoc.SetParameterValue("bonus", ManagerBase<VegetableManager>.Instance.BeetHitPowerBonus.ToString());
		}
		else if (vegetableData.type == VegetableManager.VegetableType.Carrot)
		{
			bonusLoc.SetTerm("GameWindow/MoveSpeedBonus");
			bonusParamsLoc.SetParameterValue("bonus", ManagerBase<VegetableManager>.Instance.CarrotMoveSpeedBonus.ToString());
		}
		else if (vegetableData.type == VegetableManager.VegetableType.Pumpkin)
		{
			bonusLoc.SetTerm("GameWindow/HealthBonus");
			bonusParamsLoc.SetParameterValue("bonus", ManagerBase<VegetableManager>.Instance.PumpkinHealthBonus.ToString());
		}
		else if (vegetableData.type == VegetableManager.VegetableType.Turnip)
		{
			bonusLoc.SetTerm("GameWindow/TurnipBonus");
			bonusParamsLoc.SetParameterValue("bonus", ManagerBase<VegetableManager>.Instance.TurnipThirstBonus.ToString());
		}
	}

	private void OnPlant(VegetableBehaviour vegetable)
	{
		if (vegetable == this.vegetable)
		{
			UpdatePanelTexts();
		}
	}

	protected override Vector3 GetWorldPositionOffset()
	{
		return vegetable.WorldPanelOffset;
	}

	protected override RectTransform GetEnabledRectTransform()
	{
		return rectTransform;
	}

	public static (bool onProcessingDistance, float sqrDistance) GetProcessingData(VegetableBehaviour vegetable)
	{
		float item = (CameraUtils.PlayerCamera != null) ? (CameraUtils.PlayerCamera.transform.position - vegetable.transform.position).sqrMagnitude : float.MaxValue;
		return ((bool)CameraUtils.PlayerCamera && vegetable.IsPlayerInTrigger, item);
	}
}
