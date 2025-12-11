using Avelog;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class FamilyEatButton : XORButton, IInitializableUI
{
	private PlayerEating PlayerEating => PlayerSpawner.PlayerInstance?.PlayerEating;

	private ActorPicker PlayerPicker => PlayerSpawner.PlayerInstance?.PlayerPicker;

	private PlayerCombat PlayerCombat => PlayerSpawner.PlayerInstance?.PlayerCombat;

	private void Start()
	{
		base.gameObject.SetActive(value: false);
		if (PlayerSpawner.IsPlayerSpawned)
		{
			OnSpawnPlayer();
		}
		PlayerSpawner.spawnPlayerEvent += OnSpawnPlayer;
	}

	public void OnInitializeUI()
	{
		UpdateButtonState();
		if (PlayerSpawner.IsPlayerSpawned)
		{
			OnSpawnPlayer();
		}
		PlayerSpawner.spawnPlayerEvent += OnSpawnPlayer;
	}

	private void OnSpawnPlayer()
	{
		PlayerPicker.changePickedItemEvent += OnChangePickedItem;
		PlayerFamilyController.addFamilyMemberEvent += OnAddFamilyMember;
		PlayerEating.updateFamilyAvailableFoodEvent += base.UpdateButtonState;
		UpdateButtonState();
	}

	private void OnDestroy()
	{
		PlayerSpawner.spawnPlayerEvent -= OnSpawnPlayer;
		PlayerFamilyController.addFamilyMemberEvent -= OnAddFamilyMember;
		PlayerEating.updateFamilyAvailableFoodEvent -= base.UpdateButtonState;
		if (PlayerSpawner.IsPlayerSpawned)
		{
			PlayerPicker.changePickedItemEvent -= OnChangePickedItem;
		}
	}

	private void OnChangeNearFood(bool haveNearFood)
	{
		UpdateButtonState();
	}

	private void OnChangePickedItem(bool havePickedItem)
	{
		UpdateButtonState();
	}

	private void OnAddFamilyMember(FamilyManager.FamilyMemberData familyMember)
	{
		UpdateButtonState();
	}

	public void FamilyEat()
	{
		Avelog.Input.FireFamilyEatPressed();
	}

	public override bool WantToEnable()
	{
		if (PlayerSpawner.PlayerInstance != null && PlayerSpawner.PlayerInstance.PlayerFamilyController.CanEat() && !PlayerCombat.InCombat)
		{
			if (PlayerCombat.IsInvisibilityActive())
			{
				return !PlayerCombat.HaveEnemyInAttackBox;
			}
			return true;
		}
		return false;
	}
}
