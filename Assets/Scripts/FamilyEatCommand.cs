public class FamilyEatCommand : CommandBase
{
	public override CommandId CommandId => CommandId.FamilyEat;

	public override bool CanForceRun => true;

	public override void Execute()
	{
		PlayerSpawner.PlayerInstance.PlayerFamilyController.Eat(null);
	}
}
