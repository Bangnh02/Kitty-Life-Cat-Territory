public class CombatStateIcon : XORButton, IInitializableUI
{
	public void OnInitializeUI()
	{
		PlayerCombat.changeInCombatStateEvent += OnChangeInCombatState;
	}

	private void OnDestroy()
	{
		PlayerCombat.changeInCombatStateEvent -= OnChangeInCombatState;
	}

	private void OnChangeInCombatState(bool inCombat)
	{
		UpdateButtonState();
	}

	public override bool WantToEnable()
	{
		if (PlayerSpawner.PlayerInstance != null)
		{
			return PlayerSpawner.PlayerInstance.PlayerCombat.InCombat;
		}
		return false;
	}
}
