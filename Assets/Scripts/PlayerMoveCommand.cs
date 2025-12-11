using Avelog;

public class PlayerMoveCommand : CommandBase
{
	public override CommandId CommandId => CommandId.Move;

	public override void Execute()
	{
		PlayerSpawner.PlayerInstance.PlayerMovement.Rotate(Input.HorAxis);
		PlayerSpawner.PlayerInstance.PlayerMovement.CalculateMove(Input.VertAxis);
	}
}
