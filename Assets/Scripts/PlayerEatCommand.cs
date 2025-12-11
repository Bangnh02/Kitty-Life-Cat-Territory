public class PlayerEatCommand : CommandBase
{
	public override CommandId CommandId => CommandId.Eat;

	public override void Execute()
	{
		if (PlayerSpawner.PlayerInstance.PlayerEating.HaveEatableNearFood)
		{
			ActorPicker playerPicker = PlayerSpawner.PlayerInstance.PlayerPicker;
			PlayerEating playerEating = PlayerSpawner.PlayerInstance.PlayerEating;
			if (playerPicker.HavePickedItem)
			{
				playerPicker.Drop();
			}
			base.IsExecuting = true;
			playerEating.Eat(CompleteExecution);
		}
	}

	public override void Cancel()
	{
		base.Cancel();
		PlayerSpawner.PlayerInstance.PlayerEating.CancelEat();
	}

	public override float GetExecutedTimePart()
	{
		return PlayerSpawner.PlayerInstance.PlayerEating.NormalizedEatTime;
	}
}
