using Avelog;
using UnityEngine;
using UnityEngine.UI;

public class EatButton : MonoBehaviour
{
	private Button button;

	private PlayerEating PlayerEating => PlayerSpawner.PlayerInstance?.PlayerEating;

	private void Start()
	{
		button = GetComponent<Button>();
		button.interactable = false;
		if (PlayerSpawner.IsPlayerSpawned)
		{
			OnSpawnPlayer();
		}
		PlayerSpawner.spawnPlayerEvent += OnSpawnPlayer;
	}

	private void OnSpawnPlayer()
	{
		PlayerEating.updateNearFoodEvent += OnChangeNearFood;
		PlayerEating.changeNearWaterEvent += UpdateActiveState;
		PlayerEating.changeThirstEvent += UpdateActiveState;
		PlayerSpawner.PlayerInstance.startCommandEvent += OnPlayerStartCommand;
		UpdateActiveState();
		OnChangeNearFood(PlayerEating.HaveEatableNearFood);
	}

	private void OnDestroy()
	{
		PlayerSpawner.spawnPlayerEvent -= OnSpawnPlayer;
		if (PlayerEating != null)
		{
			PlayerEating.updateNearFoodEvent -= OnChangeNearFood;
			PlayerEating.changeNearWaterEvent -= UpdateActiveState;
		}
		PlayerEating.changeThirstEvent -= UpdateActiveState;
		if (PlayerSpawner.PlayerInstance != null)
		{
			PlayerSpawner.PlayerInstance.startCommandEvent -= OnPlayerStartCommand;
		}
	}

	private void OnChangeNearFood(bool haveNearFood)
	{
		if (haveNearFood)
		{
			button.interactable = true;
		}
		else
		{
			button.interactable = false;
		}
	}

	private void OnPlayerStartCommand(CommandBase command)
	{
		UpdateActiveState();
	}

	private void UpdateActiveState()
	{
		if (PlayerEating.CanDrink())
		{
			base.gameObject.SetActive(value: false);
		}
		else
		{
			base.gameObject.SetActive(value: true);
		}
	}

	public void Eat()
	{
		Avelog.Input.FireEatPressed();
	}
}
