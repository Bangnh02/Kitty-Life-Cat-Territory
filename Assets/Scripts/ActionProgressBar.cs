using Avelog.UI;
using UnityEngine;

public class ActionProgressBar : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private Color eatActionColor = Color.red;

	[SerializeField]
	private Color drinkActionColor = Color.blue;

	[Header("Ссылки")]
	[SerializeField]
	private Bar bar;

	public void OnInitializeUI()
	{
		PlayerSpawner.spawnPlayerEvent += OnSpawnPlayer;
		if (PlayerSpawner.IsPlayerSpawned)
		{
			OnSpawnPlayer();
		}
		bar.SetValueInstantly(0f);
		bar.gameObject.SetActive(value: false);
	}

	private void OnDestroy()
	{
		PlayerSpawner.spawnPlayerEvent -= OnSpawnPlayer;
		if (PlayerSpawner.PlayerInstance != null)
		{
			PlayerSpawner.PlayerInstance.startCommandEvent -= OnStartCommand;
			PlayerSpawner.PlayerInstance.endCommandEvent -= OnEndCommand;
		}
	}

	private void OnSpawnPlayer()
	{
		PlayerSpawner.PlayerInstance.startCommandEvent += OnStartCommand;
		PlayerSpawner.PlayerInstance.endCommandEvent += OnEndCommand;
	}

	private void OnStartCommand(CommandBase command)
	{
		if (command.IsExecuting)
		{
			bar.gameObject.SetActive(value: true);
			bar.SetValueInstantly(0f);
			bar.MainFillImage.color = ((command.CommandId == CommandId.Eat) ? eatActionColor : drinkActionColor);
		}
	}

	private void OnEndCommand(CommandBase command)
	{
		if (bar.gameObject.activeSelf)
		{
			bar.SetValueInstantly(0f);
			bar.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (PlayerSpawner.PlayerInstance != null && PlayerSpawner.PlayerInstance.CurCommand != null && PlayerSpawner.PlayerInstance.CurCommand.IsExecuting)
		{
			bar.SetValueInstantly(PlayerSpawner.PlayerInstance.CurCommand.GetExecutedTimePart());
		}
	}
}
