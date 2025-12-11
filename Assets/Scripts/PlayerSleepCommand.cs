public class PlayerSleepCommand : CommandBase
{
	public override CommandId CommandId => CommandId.Sleep;

	public override void Execute()
	{
		if (PlayerSpawner.PlayerInstance.PlayerSleepController.CanSleep())
		{
			base.IsExecuting = true;
			PlayerSpawner.PlayerInstance.PlayerSleepController.Sleep(CompleteExecution);
		}
	}

	public bool ReadyToAutoSleep()
	{
		return PlayerSpawner.PlayerInstance.PlayerSleepController.ReadyToAutoSleep();
	}
}
