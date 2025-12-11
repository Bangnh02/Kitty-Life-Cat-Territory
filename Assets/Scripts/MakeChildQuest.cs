using System.Linq;

public class MakeChildQuest : Quest
{
	public override bool CanStart()
	{
		if (PlayerSpawner.PlayerInstance != null && PlayerSpawner.PlayerInstance.PlayerFamilyController.CanMakeChild() && PlayerSpawner.PlayerInstance.PlayerFamilyController.EnoughCoinsForChild())
		{
			return ManagerBase<FamilyManager>.Instance.family.Count((FamilyManager.FamilyMemberData x) => x.role == FamilyManager.FamilyMemberRole.ThirdStageChild) == 0;
		}
		return false;
	}

	protected override float CalculateMaxProgress()
	{
		return 1f;
	}

	protected override void OnStart()
	{
		PlayerFamilyController.addFamilyMemberEvent += OnAddFamilyMember;
		PlayerManager.coinsChangeEvent += OnCoinsChange;
	}

	protected override void OnEnd()
	{
		PlayerFamilyController.addFamilyMemberEvent -= OnAddFamilyMember;
		PlayerManager.coinsChangeEvent -= OnCoinsChange;
	}

	private void OnCoinsChange(int coinsInc = 0)
	{
		if (!PlayerSpawner.PlayerInstance.PlayerFamilyController.EnoughCoinsForChild())
		{
			Singleton<QuestSpawner>.Instance.CancelQuest();
		}
	}

	private void OnAddFamilyMember(FamilyManager.FamilyMemberData familyMember)
	{
		if (familyMember.role != FamilyManager.FamilyMemberRole.Spouse)
		{
			float num = base.CurProgress += 1f;
		}
	}
}
