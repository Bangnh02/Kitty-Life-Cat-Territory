using Avelog;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SatisfyFarmResidentButton : ActionButton, IInitializableUI
{
	private Button button;

	public void OnInitializeUI()
	{
		button = GetComponentInChildren<Button>(includeInactive: true);
		UpdateButtonState();
		PlayerSpawner.spawnPlayerEvent += OnSpawnPlayer;
		PlayerSpawner.spawnPlayerEvent += base.UpdateButtonState;
		PlayerFarmResident.updateNearFarmResidentsEvent += base.UpdateButtonState;
		FarmResident.changeNeedEvent += OnFarmResidentChangeNeed;
	}

	private void OnDestroy()
	{
		if (PlayerSpawner.PlayerInstance != null)
		{
			PlayerSpawner.PlayerInstance.PlayerPicker.changePickedItemEvent -= OnChangePickedItem;
		}
		PlayerSpawner.spawnPlayerEvent -= OnSpawnPlayer;
		PlayerSpawner.spawnPlayerEvent -= base.UpdateButtonState;
		PlayerFarmResident.updateNearFarmResidentsEvent -= base.UpdateButtonState;
		FarmResident.changeNeedEvent -= OnFarmResidentChangeNeed;
	}

	private void OnSpawnPlayer()
	{
		PlayerSpawner.spawnPlayerEvent -= OnSpawnPlayer;
		PlayerSpawner.PlayerInstance.PlayerPicker.changePickedItemEvent += OnChangePickedItem;
		UpdateButtonState();
	}

	private void OnChangePickedItem(bool havePickedItem)
	{
		UpdateButtonState();
	}

	private void OnFarmResidentChangeNeed(FarmResident farmResident)
	{
		UpdateButtonState();
	}

	public void SatisfyFarmResident()
	{
		Avelog.Input.FireSatisfyFarmResidentPressed();
	}

	public override bool WantToEnable()
	{
		if (PlayerSpawner.PlayerInstance != null)
		{
			return PlayerSpawner.PlayerInstance.PlayerFarmResident.CanSatisfyFarmResident();
		}
		return false;
	}
}
