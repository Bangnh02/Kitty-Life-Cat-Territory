using Avelog;
using UnityEngine;
using UnityEngine.UI;

public class PickDropButton : XORButton, IInitializableUI
{
	[SerializeField]
	private Button button;

	[SerializeField]
	private Sprite pickSprite;

	[SerializeField]
	private Sprite dropSprite;

	private ActorPicker PlayerPicker => PlayerSpawner.PlayerInstance.PlayerPicker;

	public void OnInitializeUI()
	{
		button.image.sprite = pickSprite;
		button.interactable = false;
		if (PlayerSpawner.IsPlayerSpawned)
		{
			OnSpawnPlayer();
		}
		PlayerSpawner.spawnPlayerEvent += OnSpawnPlayer;
	}

	private void OnSpawnPlayer()
	{
		PlayerPicker.updatePickableItemsEvent += OnUpdatePickableItems;
		PlayerPicker.changePickedItemEvent += OnChangePickedItem;
		OnUpdatePickableItems(PlayerPicker.HavePickableItems);
	}

	private void OnChangePickedItem(bool havePickedItem)
	{
		if (havePickedItem)
		{
			button.image.sprite = dropSprite;
		}
		else
		{
			button.image.sprite = pickSprite;
		}
	}

	private void OnUpdatePickableItems(bool haveNearPickableItem)
	{
		if (haveNearPickableItem || PlayerPicker.HavePickedItem)
		{
			button.interactable = true;
		}
		else
		{
			button.interactable = false;
		}
	}

	public void PickOrDrop()
	{
		if (PlayerPicker.HavePickedItem)
		{
			Avelog.Input.FireDropPressed();
		}
		else
		{
			Avelog.Input.FirePickPressed();
		}
	}

	public override bool WantToEnable()
	{
		return true;
	}
}
