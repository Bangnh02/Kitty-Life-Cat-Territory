public class PlayerJumpCommand : CommandBase
{
	public override CommandId CommandId => CommandId.Jump;

	public override CommandId CancelMask => CommandId.Eat | CommandId.Drink;

	public override void Execute()
	{
		PlayerSpawner.PlayerInstance.PlayerMovement.Jump();
	}
}
