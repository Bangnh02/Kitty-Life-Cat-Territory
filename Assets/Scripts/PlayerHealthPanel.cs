using Avelog.UI;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthPanel : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private Text healthText;

	[SerializeField]
	private Bar bar;

	[SerializeField]
	private Image paramStateImage;

	private float prevHealthMaximum;

	private const int notDefinedHealth = -1;

	private int prevHealth = -1;

	private PlayerCombat PlayerCombat => PlayerSpawner.PlayerInstance?.PlayerCombat;

	private PlayerEating PlayerEating => PlayerSpawner.PlayerInstance?.PlayerEating;

	public void OnInitializeUI()
	{
		prevHealthMaximum = ManagerBase<PlayerManager>.Instance.HealthMaximum;
		bar.SetValueInstantly(ManagerBase<PlayerManager>.Instance.healthCurrent / ManagerBase<PlayerManager>.Instance.HealthMaximum);
		if (PlayerSpawner.IsPlayerSpawned)
		{
			OnSpawnPlayer();
		}
		PlayerSpawner.spawnPlayerEvent += OnSpawnPlayer;
		VegetableBehaviour.grownEvent += OnVegetableGrown;
		PlayerManager.levelChangeEvent += OnLevelChange;
		PlayerManager.changeHealthMaximum += OnChangeHealthMaximum;
		PlayerSpawner.respawnPlayerEvent += OnRespawnPlayer;
		SaveManager.LoadEndEvent += UpdateHealthBar;
	}

	private void OnRespawnPlayer()
	{
		UpdateHealthBar();
	}

	private void OnDestroy()
	{
		PlayerSpawner.spawnPlayerEvent -= OnSpawnPlayer;
		VegetableBehaviour.grownEvent -= OnVegetableGrown;
		PlayerManager.levelChangeEvent -= OnLevelChange;
		PlayerManager.changeHealthMaximum -= OnChangeHealthMaximum;
		PlayerSpawner.respawnPlayerEvent -= OnRespawnPlayer;
		SaveManager.LoadEndEvent -= UpdateHealthBar;
		if (PlayerCombat != null)
		{
			PlayerCombat.healEvent -= OnPlayerHeal;
			PlayerCombat.takeDamageEvent -= OnPlayerTakeDamage;
			PlayerCombat.respawnEvent -= OnPlayerRespawn;
		}
	}

	private void OnLevelChange()
	{
		UpdateHealthBar();
	}

	private void OnSpawnPlayer()
	{
		PlayerCombat.healEvent += OnPlayerHeal;
		PlayerCombat.takeDamageEvent += OnPlayerTakeDamage;
		PlayerCombat.respawnEvent += OnPlayerRespawn;
		UpdateHealthBar();
	}

	private void OnChangeHealthMaximum()
	{
		UpdateHealthBar();
	}

	private void OnPlayerRespawn()
	{
		bar.SetValueInstantly(ManagerBase<PlayerManager>.Instance.healthCurrent / ManagerBase<PlayerManager>.Instance.HealthMaximum);
		UpdateHealthBar();
	}

	private void OnVegetableGrown(VegetableBehaviour vegetable)
	{
		if (vegetable.VegetableData.type == VegetableManager.VegetableType.Pumpkin)
		{
			UpdateHealthBar();
		}
	}

	private void OnPlayerHeal()
	{
		UpdateHealthBar();
	}

	private void OnPlayerTakeDamage(ActorCombat.TakeDamageType takeDamageType, float damage, ActorCombat attacker)
	{
		UpdateHealthBar();
	}

	private void UpdateHealthBar()
	{
		bar.SetValue(ManagerBase<PlayerManager>.Instance.healthCurrent / ManagerBase<PlayerManager>.Instance.HealthMaximum);
		int num = Mathf.CeilToInt(ManagerBase<PlayerManager>.Instance.healthCurrent);
		if (num != prevHealth)
		{
			prevHealth = num;
			healthText.text = num.ToString();
		}
		if (PlayerSpawner.IsPlayerSpawned)
		{
			if (PlayerEating.IsLosingHPDueSatiety && !paramStateImage.enabled)
			{
				paramStateImage.enabled = true;
			}
			else if (!PlayerEating.IsLosingHPDueSatiety && paramStateImage.enabled)
			{
				paramStateImage.enabled = false;
			}
		}
	}
}
