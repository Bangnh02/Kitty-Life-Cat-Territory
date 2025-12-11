using Avelog.UI;
using UnityEngine;

public class PlayerExperiencePanel : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private Bar experienceBar;

	private int notProcessedLevelUps;

	public void OnInitializeUI()
	{
		float valueInstantly = ManagerBase<PlayerManager>.Instance.levelExperience / ManagerBase<PlayerManager>.Instance.LevelUpExperience;
		experienceBar.SetValueInstantly(valueInstantly);
		UpdateExperienceUI();
		PlayerManager.experienceUpdateEvent += OnExperienceUpdate;
		PlayerSpawner.spawnPlayerEvent += OnSpawnPlayer;
		if (PlayerSpawner.IsPlayerSpawned)
		{
			OnSpawnPlayer();
		}
		SaveManager.LoadEndEvent += OnLoadEnd;
	}

	private void OnDestroy()
	{
		PlayerManager.experienceUpdateEvent -= OnExperienceUpdate;
		PlayerSpawner.spawnPlayerEvent -= OnSpawnPlayer;
		if (PlayerSpawner.PlayerInstance != null)
		{
			PlayerSpawner.PlayerInstance.startCommandEvent -= OnStartCommand;
		}
		SaveManager.LoadEndEvent -= OnLoadEnd;
	}

	private void Update()
	{
		if (PlayerSpawner.PlayerInstance != null && PlayerSpawner.PlayerInstance.CurCommand != null && PlayerSpawner.PlayerInstance.CurCommand.IsExecuting)
		{
			experienceBar.MainFillImage.enabled = (PlayerSpawner.PlayerInstance.CurCommand.GetExecutedTimePart() == 0f || PlayerSpawner.PlayerInstance.CurCommand.GetExecutedTimePart() >= 1f);
		}
		else
		{
			experienceBar.MainFillImage.enabled = true;
		}
	}

	private void OnSpawnPlayer()
	{
		PlayerSpawner.PlayerInstance.startCommandEvent += OnStartCommand;
	}

	private void OnStartCommand(CommandBase command)
	{
	}

	private void OnLoadEnd()
	{
		if (notProcessedLevelUps == 0)
		{
			UpdateExperienceUI();
		}
	}

	private void OnExperienceUpdate(float expInc, int levelUps)
	{
		notProcessedLevelUps += levelUps;
		if (notProcessedLevelUps <= levelUps)
		{
			UpdateExperienceUI();
		}
	}

	private void UpdateExperienceUI()
	{
		float valuePart = ManagerBase<PlayerManager>.Instance.levelExperience / ManagerBase<PlayerManager>.Instance.LevelUpExperience;
		if (notProcessedLevelUps > 0)
		{
			experienceBar.SetValue(1f, OnEndProcessLevelUp);
		}
		else
		{
			experienceBar.SetValue(valuePart);
		}
	}

	private void OnEndProcessLevelUp()
	{
		notProcessedLevelUps--;
		experienceBar.SetValueInstantly(0f);
		UpdateExperienceUI();
	}
}
