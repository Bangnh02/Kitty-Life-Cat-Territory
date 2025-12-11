using Avelog;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlantButton : XORButton, IInitializableUI
{
	private Button button;

	public void OnInitializeUI()
	{
		button = GetComponentInChildren<Button>(includeInactive: true);
		UpdateButtonState();
		UpdateButtonInteractable();
		PlayerSpawner.spawnPlayerEvent += OnSpawnPlayerEvent;
		PlayerPlanter.updateNearPotentialPlantEvent += OnUpdateNearPotentialPlant;
		PlayerManager.coinsChangeEvent += UpdateButtonInteractable;
	}

	private void OnDestroy()
	{
		PlayerSpawner.spawnPlayerEvent -= OnSpawnPlayerEvent;
		PlayerPlanter.updateNearPotentialPlantEvent -= OnUpdateNearPotentialPlant;
		PlayerManager.coinsChangeEvent -= UpdateButtonInteractable;
	}

	private void OnSpawnPlayerEvent()
	{
		UpdateButtonState();
		UpdateButtonInteractable();
	}

	private void UpdateButtonInteractable(int coinsInc = 0)
	{
		button.interactable = (PlayerSpawner.PlayerInstance != null && PlayerSpawner.PlayerInstance.PlayerPlanter.CanAffordPlant());
	}

	private void OnUpdateNearPotentialPlant()
	{
		UpdateButtonState();
		UpdateButtonInteractable();
	}

	public void Plant()
	{
		Avelog.Input.FirePlantPressed();
	}

	public override bool WantToEnable()
	{
		if (PlayerSpawner.PlayerInstance != null)
		{
			return PlayerSpawner.PlayerInstance.PlayerPlanter.HaveNearPotentialPlants;
		}
		return false;
	}
}
