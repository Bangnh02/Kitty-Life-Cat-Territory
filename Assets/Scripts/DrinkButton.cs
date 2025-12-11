using Avelog;
using UnityEngine;
using UnityEngine.UI;

public class DrinkButton : MonoBehaviour, IInitializableUI
{
	private Button button;

	private PlayerEating PlayerEating => PlayerSpawner.PlayerInstance.PlayerEating;

	public void OnInitializeUI()
	{
		base.gameObject.SetActive(value: false);
		button = GetComponent<Button>();
		if (PlayerSpawner.IsPlayerSpawned)
		{
			OnSpawnPlayer();
		}
		PlayerSpawner.spawnPlayerEvent += OnSpawnPlayer;
	}

	private void OnDestroy()
	{
		PlayerEating.changeNearWaterEvent -= UpdateActiveState;
		PlayerSpawner.spawnPlayerEvent -= OnSpawnPlayer;
		PlayerEating.changeThirstEvent -= UpdateActiveState;
		PlayerSpawner.PlayerInstance.startCommandEvent -= OnPlayerStartCommand;
	}

	private void OnSpawnPlayer()
	{
		PlayerEating.changeNearWaterEvent += UpdateActiveState;
		PlayerEating.changeThirstEvent += UpdateActiveState;
		PlayerSpawner.PlayerInstance.startCommandEvent += OnPlayerStartCommand;
		UpdateActiveState();
	}

	private void OnPlayerStartCommand(CommandBase command)
	{
		UpdateActiveState();
	}

	private void UpdateActiveState()
	{
		if (PlayerEating.CanDrink())
		{
			if (PlayerEating.IsDrinking)
			{
				button.interactable = false;
			}
			else
			{
				button.interactable = true;
			}
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
		}
		else if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void Drink()
	{
		Avelog.Input.FireDrinkPressed();
	}
}
