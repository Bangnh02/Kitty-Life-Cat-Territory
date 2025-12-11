using UnityEngine;

public class FeedFamilyQuest : Quest
{
	[Postfix(PostfixAttribute.Id.Percents)]
	[SerializeField]
	private float maxSatietyPercToStart = 35f;

	[Postfix(PostfixAttribute.Id.Percents)]
	[SerializeField]
	private float neededSatietyPerc = 80f;

	public override bool CanStart()
	{
		return ManagerBase<FamilyManager>.Instance.SatietyPart * 100f <= maxSatietyPercToStart;
	}

	protected override float CalculateMaxProgress()
	{
		return neededSatietyPerc;
	}

	protected override void OnStart()
	{
		PlayerFamilyController.familyChangeSatietyEvent += OnFamilyChangeSatiety;
		base.CurProgress = ManagerBase<FamilyManager>.Instance.SatietyPart * 100f;
	}

	protected override void OnEnd()
	{
		PlayerFamilyController.familyChangeSatietyEvent -= OnFamilyChangeSatiety;
	}

	private void OnFamilyChangeSatiety()
	{
		base.CurProgress = ManagerBase<FamilyManager>.Instance.SatietyPart * 100f;
	}
}
