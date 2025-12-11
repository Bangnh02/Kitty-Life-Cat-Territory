using System.Linq;
using UnityEngine;

public class ActionPriorityController : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private PriorityRing priorityRing;

	[SerializeField]
	private Transform mainActionParent;

	[SerializeField]
	private Transform eatingActionParent;

	[SerializeField]
	private Transform itemActionParent;

	[SerializeField]
	private float minPlayerSatiety = 80f;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private float minFamilyAttackPower = 80f;

	private PlayerBrain Player => PlayerSpawner.PlayerInstance;

	private bool NeedSatisfyFamilySatiety
	{
		get
		{
			if (Player.PlayerEating.HaveAvailableFamilyFood && ManagerBase<FamilyManager>.Instance.HaveFamily && (ManagerBase<FamilyManager>.Instance.AttackPowerPercentFromSatiety < minFamilyAttackPower || Singleton<QuestSpawner>.Instance.ActiveQuest is FeedFamilyQuest) && ManagerBase<PlayerManager>.Instance.satietyCurrent != 0f && !Player.PlayerPicker.HavePickedItem)
			{
				return !(Singleton<QuestSpawner>.Instance.ActiveQuest is SatisfyHungerQuest);
			}
			return false;
		}
	}

	private bool NeedSatisfyPlayerSatiety
	{
		get
		{
			if (Player.PlayerEating.HaveNearGoodFood && ManagerBase<PlayerManager>.Instance.satietyCurrent <= minPlayerSatiety && !IsNeedToFarmResident)
			{
				return !Player.PlayerEating.IsDrinking;
			}
			return false;
		}
	}

	private bool IsNearQuestItem
	{
		get
		{
			if (!Player.PlayerEating.HaveNearGoodFood)
			{
				return false;
			}
			Quest curQuest = Singleton<QuestSpawner>.Instance.ActiveQuest;
			if (curQuest == null || !(curQuest is HelpFarmResidentQuest))
			{
				return false;
			}
			string curNeed = Singleton<FarmResidentSpawner>.Instance.SpawnedFarmResidents.Find((FarmResident x) => x.FarmResidentData.farmResidentId == ((HelpFarmResidentQuest)curQuest).FarmResidentId).FarmResidentData.curNeed;
			if (Player.PlayerEating.GetNearestFreeFood().Name != curNeed)
			{
				return false;
			}
			return true;
		}
	}

	private bool IsPickedQuestItem
	{
		get
		{
			if (!Player.PlayerPicker.HavePickedItem)
			{
				return false;
			}
			Quest curQuest = Singleton<QuestSpawner>.Instance.ActiveQuest;
			if (!(curQuest as HelpFarmResidentQuest))
			{
				return false;
			}
			string curNeed = Singleton<FarmResidentSpawner>.Instance.SpawnedFarmResidents.Find((FarmResident x) => x.FarmResidentData.farmResidentId == ((HelpFarmResidentQuest)curQuest).FarmResidentId).FarmResidentData.curNeed;
			string name = ((Food)Player.PlayerPicker.PickedItem).Name;
			if (curNeed != name)
			{
				return false;
			}
			return true;
		}
	}

	private bool NeedSatisfyThirst
	{
		get
		{
			if (Player.PlayerEating.CanDrink())
			{
				return !Player.PlayerEating.IsDrinking;
			}
			return false;
		}
	}

	private bool IsNeedToFarmResident
	{
		get
		{
			if (!Player.PlayerPicker.HavePickedItem)
			{
				return false;
			}
			string pickedItemName = ((Food)Player.PlayerPicker.PickedItem).Name;
			return Singleton<FarmResidentSpawner>.Instance.SpawnedFarmResidents.Any((FarmResident x) => x.FarmResidentData.HaveNeed && x.FarmResidentData.curNeed.Equals(pickedItemName));
		}
	}

	public void OnInitializeUI()
	{
		if (!PlayerSpawner.IsPlayerSpawned)
		{
			PlayerSpawner.spawnPlayerEvent += OnSpawnPlayer;
		}
		else
		{
			OnSpawnPlayer();
		}
	}

	private void OnSpawnPlayer()
	{
		Player.PlayerCombat.changeHaveEnemyInAttackBoxEvent += UpdateRings;
		Player.PlayerEating.updateNearGoodFoodEvent += OnUpdateNearGoodFood;
		Player.PlayerEating.changeNearWaterEvent += OnChangeNearWater;
		PlayerEating.updateFamilyAvailableFoodEvent += OnUpdateFamilyAvailableFood;
		PlayerSleepController.changeCanSleepStateEvent += UpdateRings;
		PlayerPlanter.updateNearPotentialPlantEvent += UpdateRings;
		PlayerFarmResident.updateNearFarmResidentsEvent += UpdateRings;
		FarmResident.changeNeedEvent += OnFarmResidentСhangeNeedEvent;
		PlayerSpawner.PlayerInstance.startCommandEvent += OnPlayerStartCommand;
		PlayerSleepController.awakeEndEvent += UpdateRings;
		if (!ManagerBase<FamilyManager>.Instance.HaveSpouse)
		{
			PotentialSpouseController.switchPlayerNearEvent += OnPotentialSpouseSwitchPlayerNear;
		}
		UpdateRings();
	}

	private void OnDestroy()
	{
		if (Player != null)
		{
			Player.PlayerCombat.changeHaveEnemyInAttackBoxEvent -= UpdateRings;
			Player.PlayerEating.updateNearGoodFoodEvent -= OnUpdateNearGoodFood;
			Player.PlayerEating.changeNearWaterEvent -= OnChangeNearWater;
			PlayerSpawner.PlayerInstance.startCommandEvent -= OnPlayerStartCommand;
		}
		PlayerEating.updateFamilyAvailableFoodEvent -= OnUpdateFamilyAvailableFood;
		PlayerSleepController.changeCanSleepStateEvent -= UpdateRings;
		PlayerSpawner.spawnPlayerEvent -= OnSpawnPlayer;
		PlayerPlanter.updateNearPotentialPlantEvent -= UpdateRings;
		PlayerFarmResident.updateNearFarmResidentsEvent -= UpdateRings;
		FarmResident.changeNeedEvent -= OnFarmResidentСhangeNeedEvent;
		PotentialSpouseController.switchPlayerNearEvent -= OnPotentialSpouseSwitchPlayerNear;
		PlayerSleepController.awakeEndEvent -= UpdateRings;
	}

	private void UpdateRings()
	{
		if (Player.PlayerCombat.HaveEnemyInAttackBox)
		{
			EnableMainActionRing();
		}
		else if (Player.PlayerSleepController.CanSleep())
		{
			EnableMainActionRing();
		}
		else if (Player.PlayerFarmResident.CanSatisfyFarmResident())
		{
			EnableItemActionRing();
		}
		else if (IsPickedQuestItem)
		{
			DisableActionRing();
		}
		else if (IsNearQuestItem)
		{
			EnableItemActionRing();
		}
		else if (NeedSatisfyThirst)
		{
			EnableEatingActionRing();
		}
		else if (NeedSatisfyFamilySatiety)
		{
			EnableMainActionRing();
		}
		else if (NeedSatisfyPlayerSatiety)
		{
			EnableEatingActionRing();
		}
		else if (Player.PlayerPlanter.CanPlant())
		{
			EnableMainActionRing();
		}
		else if (Player.PlayerFamilyController.CanMakeSpouse())
		{
			EnableMainActionRing();
		}
		else
		{
			DisableActionRing();
		}
	}

	private void OnUpdateNearGoodFood(bool haveNearFood)
	{
		if (haveNearFood)
		{
			PlayerEating.changeSatietyEvent += UpdateRings;
			PlayerFamilyController.familyChangeSatietyEvent += UpdateRings;
		}
		else
		{
			PlayerEating.changeSatietyEvent -= UpdateRings;
			PlayerFamilyController.familyChangeSatietyEvent -= UpdateRings;
		}
		UpdateRings();
	}

	private void OnChangeNearWater()
	{
		if (Player.PlayerEating.HaveNearWater)
		{
			PlayerEating.changeThirstEvent += UpdateRings;
		}
		else
		{
			PlayerEating.changeThirstEvent -= UpdateRings;
		}
		UpdateRings();
	}

	private void OnUpdateFamilyAvailableFood()
	{
		UpdateRings();
	}

	private void OnPotentialSpouseSwitchPlayerNear(bool isPlayerNear)
	{
		if (isPlayerNear)
		{
			PlayerFamilyController.addFamilyMemberEvent += OnAddFamilyMember;
		}
		else
		{
			PlayerFamilyController.addFamilyMemberEvent -= OnAddFamilyMember;
		}
		UpdateRings();
	}

	private void OnAddFamilyMember(FamilyManager.FamilyMemberData familyMember)
	{
		PlayerFamilyController.addFamilyMemberEvent -= OnAddFamilyMember;
		UpdateRings();
	}

	private void OnFarmResidentСhangeNeedEvent(FarmResident farmResident)
	{
		UpdateRings();
	}

	private void OnPlayerStartCommand(CommandBase command)
	{
		UpdateRings();
	}

	private void EnableMainActionRing()
	{
		priorityRing.RectTransform.SetParent(mainActionParent);
		SetRingRectIndent(priorityRing.BigButtonRectIndent);
		priorityRing.gameObject.SetActive(value: true);
	}

	private void EnableEatingActionRing()
	{
		priorityRing.RectTransform.SetParent(eatingActionParent);
		SetRingRectIndent(priorityRing.SmallButtonRectIndent);
		priorityRing.gameObject.SetActive(value: true);
	}

	private void EnableItemActionRing()
	{
		priorityRing.RectTransform.SetParent(itemActionParent);
		SetRingRectIndent(priorityRing.SmallButtonRectIndent);
		priorityRing.gameObject.SetActive(value: true);
	}

	private void SetRingRectIndent(float rectIndent)
	{
		priorityRing.RectTransform.offsetMin = new Vector2(0f - rectIndent, 0f - rectIndent);
		priorityRing.RectTransform.offsetMax = new Vector2(rectIndent, rectIndent);
	}

	private void DisableActionRing()
	{
		priorityRing.RectTransform.SetParent(base.transform);
		priorityRing.gameObject.SetActive(value: false);
	}
}
