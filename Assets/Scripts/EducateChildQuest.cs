using UnityEngine;

public class EducateChildQuest : Quest
{
	[Postfix(PostfixAttribute.Id.Minutes)]
	[SerializeField]
	private float questCooldownMin = 7.5f;

	[Postfix(PostfixAttribute.Id.Percents)]
	[SerializeField]
	private float neededChildProgressPerc = 15f;

	public override bool CanStart()
	{
		if (ManagerBase<FamilyManager>.Instance.HaveGrowingChild)
		{
			return ManagerBase<QuestManager>.Instance.educateChildQuestTimer <= 0f;
		}
		return false;
	}

	protected override float CalculateMaxProgress()
	{
		return neededChildProgressPerc;
	}

	protected override void OnStart()
	{
		FamilyManager.gotExperienceEvent += OnGotExperience;
	}

	protected override void OnEnd()
	{
		FamilyManager.gotExperienceEvent -= OnGotExperience;
		ManagerBase<QuestManager>.Instance.educateChildQuestTimer = questCooldownMin * 60f;
	}

	private void OnGotExperience(FamilyManager.FamilyMemberData familyMemberData, float experience, float experiencePercent)
	{
		if (!ManagerBase<FamilyManager>.Instance.HaveGrowingChild)
		{
			base.CurProgress = base.MaxProgress;
		}
		else if (familyMemberData == ManagerBase<FamilyManager>.Instance.GrowingChild)
		{
			base.CurProgress += experiencePercent;
		}
	}

	private void Update()
	{
		if (ManagerBase<QuestManager>.Instance.educateChildQuestTimer > 0f)
		{
			ManagerBase<QuestManager>.Instance.educateChildQuestTimer = Mathf.Clamp(ManagerBase<QuestManager>.Instance.educateChildQuestTimer - Time.deltaTime, 0f, float.MaxValue);
		}
	}
}
