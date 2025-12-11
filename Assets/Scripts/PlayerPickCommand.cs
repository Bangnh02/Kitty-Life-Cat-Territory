public class PlayerPickCommand : CommandBase
{
	public override CommandId CommandId => CommandId.Pick;

	public override CommandId CancelMask => CommandId.Eat | CommandId.Drink;

	public override void Execute()
	{
		PlayerSpawner.PlayerInstance.PlayerPicker.Pick();
	}
}
