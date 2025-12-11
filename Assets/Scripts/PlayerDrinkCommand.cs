public class PlayerDrinkCommand : CommandBase
{
	public override CommandId CommandId => CommandId.Drink;

	public override void Execute()
	{
		if (PlayerSpawner.PlayerInstance.PlayerEating.CanDrink())
		{
			PlayerSpawner.PlayerInstance.PlayerEating.Drink(CompleteExecution);
			base.IsExecuting = true;
		}
	}

	public override void Cancel()
	{
		base.Cancel();
		PlayerSpawner.PlayerInstance.PlayerEating.CancelDrink();
	}

	public override float GetExecutedTimePart()
	{
		return PlayerSpawner.PlayerInstance.PlayerEating.NormalizedDrinkTime;
	}
}
