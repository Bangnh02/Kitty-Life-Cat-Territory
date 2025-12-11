using Avelog;

public class SleepButton : XORButton, IInitializableUI
{
	public void OnInitializeUI()
	{
		PlayerSleepController.changeCanSleepStateEvent += base.UpdateButtonState;
		UpdateButtonState();
	}

	private void OnDestroy()
	{
		PlayerSleepController.changeCanSleepStateEvent -= base.UpdateButtonState;
	}

	public void Sleep()
	{
		Input.SleepButtonPressed();
	}

	public override bool WantToEnable()
	{
		if (PlayerSpawner.PlayerInstance != null)
		{
			return PlayerSpawner.PlayerInstance.PlayerSleepController.CanSleep();
		}
		return false;
	}
}
