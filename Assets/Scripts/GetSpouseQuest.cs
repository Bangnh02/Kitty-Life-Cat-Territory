public class GetSpouseQuest : Quest
{
	public override bool CanStart()
	{
		if (!ManagerBase<FamilyManager>.Instance.HaveSpouse)
		{
			return ManagerBase<PlayerManager>.Instance.Level >= ManagerBase<FamilyManager>.Instance.SpouseLevelNeed;
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
	}

	protected override void OnEnd()
	{
		PlayerFamilyController.addFamilyMemberEvent -= OnAddFamilyMember;
	}

	private void OnAddFamilyMember(FamilyManager.FamilyMemberData familyMember)
	{
		if (familyMember.role == FamilyManager.FamilyMemberRole.Spouse)
		{
			float num = base.CurProgress += 1f;
		}
	}
}
