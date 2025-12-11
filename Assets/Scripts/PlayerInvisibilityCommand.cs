public class PlayerInvisibilityCommand : CommandBase
{
	public override CommandId CommandId => CommandId.Invisibility;

	public override CommandId CancelMask => CommandId.Eat | CommandId.Drink;

	public override void Execute()
	{
		PlayerSpawner.PlayerInstance.PlayerCombat.SwitchInvisibility();
	}
}
