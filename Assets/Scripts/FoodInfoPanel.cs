using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class FoodInfoPanel : WorldRelativePanel
{
	[Header("FoodInfoPanel")]
	[SerializeField]
	private Localize nameLoc;

	[SerializeField]
	private GameObject fullInfoPanel;

	[SerializeField]
	private GameObject iconInfoPanel;

	[SerializeField]
	private float distanceToFullInfo;

	[SerializeField]
	private Image iconImage;

	[SerializeField]
	private ProcessingSwitch processingSwitch;

	private RectTransform iconRectTransform;

	private RectTransform fullRectTransform;

	private const string foodNamesTermCategory = "FoodNames/";

	public Food Food
	{
		get;
		private set;
	}

	private PlayerBrain Player => PlayerSpawner.PlayerInstance;

	private bool IsThisFoodPicked
	{
		get
		{
			if (Player.PlayerPicker.HavePickedItem)
			{
				return Player.PlayerPicker.PickedItem == Food;
			}
			return false;
		}
	}

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

	private void Awake()
	{
		fullRectTransform = fullInfoPanel.GetComponent<RectTransform>();
		iconRectTransform = iconInfoPanel.GetComponent<RectTransform>();
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		Food.holdFoodEvent -= OnHoldFood;
		Food.unholdFoodEvent -= OnUnholdFood;
	}

	public void Spawn(Food food)
	{
		Food = food;
		Food.holdFoodEvent += OnHoldFood;
		Food.unholdFoodEvent += OnUnholdFood;
		processingSwitch.CheckDistanceGO = Food.gameObject;
		processingSwitch.maxProcessingDistance = Singleton<ItemSpawner>.Instance.InfoPanelProcessingDistance;
		base.gameObject.SetActive(value: true);
		nameLoc.SetTerm("FoodNames/" + Food.Name);
		if (food is EnemyFood)
		{
			iconImage.sprite = ManagerBase<UIManager>.Instance.GetArchetypeFoodSprite((food as EnemyFood).EnemyArchetype);
		}
		else
		{
			iconImage.sprite = ManagerBase<UIManager>.Instance.GetFoodSprite(food.Id);
		}
		if (Food.IsBadFood)
		{
			iconInfoPanel.SetActive(value: false);
		}
		processingSwitch.MeasureDistance();
		processingSwitch.UpdateProcessingState(forceUpdate: true, instantEnabling: false, instantDisabling: true);
		UpdatePanel();
		UpdatePanelsState();
	}

	private void OnDisable()
	{
		if (!base.gameObject.activeSelf)
		{
			Food = null;
			Food.holdFoodEvent -= OnHoldFood;
			Food.unholdFoodEvent -= OnUnholdFood;
		}
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		UpdatePanelsState();
	}

	private void OnHoldFood(Food food)
	{
		if (food == Food)
		{
			UpdatePanelInfo();
		}
	}

	private void OnUnholdFood(Food food)
	{
		if (food == Food)
		{
			UpdatePanelInfo();
		}
	}

	private void UpdatePanelsState()
	{
		if ((Player.transform.position - Food.transform.position).IsShorterOrEqual(distanceToFullInfo))
		{
			fullInfoPanel.SetActive(value: true);
			iconInfoPanel.SetActive(value: false);
			return;
		}
		if (IsPlayerInCombat || Food.IsBadFood)
		{
			iconInfoPanel.SetActive(value: false);
		}
		else
		{
			iconInfoPanel.SetActive(value: true);
		}
		fullInfoPanel.SetActive(value: false);
	}

	private void UpdatePanelInfo()
	{
		if ((Food.CurCountFoodUnits == 0 && Food.CountOccupiedFoodUnits == 0) || !Food.gameObject.activeSelf)
		{
			processingSwitch.Switch(isEnabled: false, forceUpdate: true, instantEnabling: true, instantDisabling: true);
		}
	}

	protected override bool HaveTarget()
	{
		return Food != null;
	}

	protected override Vector3 GetTargetPosition()
	{
		return Food.transform.position;
	}

	protected override Vector3 GetWorldPositionOffset()
	{
		if (IsThisFoodPicked)
		{
			return Singleton<ItemSpawner>.Instance.PickedItemPanelOffset;
		}
		return Food.WorldPanelOffset;
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
