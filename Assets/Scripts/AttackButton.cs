using Avelog;

public class AttackButton : XORButton, IInitializableUI
{
	private PlayerCombat playerCombat;

	private bool attackPressed;

	public override int Priority
	{
		get
		{
			if (playerCombat != null && playerCombat.InCombat)
			{
				return 0;
			}
			return base.Priority;
		}
	}

	public void OnInitializeUI()
	{
		PlayerSpawner.spawnPlayerEvent += OnSpawnPlayer;
		UpdateButtonState();
	}

	private void OnDestroy()
	{
		PlayerSpawner.spawnPlayerEvent -= OnSpawnPlayer;
		PlayerCombat.changeInCombatStateEvent -= OnChangeInCombatState;
	}

	private void OnSpawnPlayer()
	{
		playerCombat = PlayerSpawner.PlayerInstance.PlayerCombat;
		PlayerCombat.changeInCombatStateEvent += OnChangeInCombatState;
		OnChangeInCombatState(playerCombat.InCombat);
	}

	private void OnChangeInCombatState(bool inCombat)
	{
		UpdateButtonState();
	}

	public override bool WantToEnable()
	{
		return true;
	}

	private void OnDisable()
	{
		attackPressed = false;
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause && attackPressed)
		{
			attackPressed = false;
		}
	}

	public void OnPointerDown()
	{
		attackPressed = true;
	}

	public void OnPointerUp()
	{
		attackPressed = false;
	}

	public void Attack()
	{
		Input.FireAttackPressed();
	}

	private void Update()
	{
		if (attackPressed)
		{
			Attack();
		}
	}
}
