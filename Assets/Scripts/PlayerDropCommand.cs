public class PlayerDropCommand : CommandBase
{
	public override CommandId CommandId => CommandId.Drop;

	public override void Execute()
	{
		if (PlayerSpawner.PlayerInstance.PlayerFarmResident.CanSatisfyFarmResident())
		{
			PlayerSpawner.PlayerInstance.PlayerFarmResident.SatisfyFarmResident();
		}
		else if (!PlayerSpawner.PlayerInstance.PlayerMovement.IsFalling)
		{
			PlayerSpawner.PlayerInstance.PlayerPicker.Drop();
		}
	}
}
