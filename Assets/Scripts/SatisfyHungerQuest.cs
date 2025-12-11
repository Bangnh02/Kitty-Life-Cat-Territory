using UnityEngine;

public class SatisfyHungerQuest : Quest
{
	[Postfix(PostfixAttribute.Id.Percents)]
	[SerializeField]
	private float maxSatietyPercToStart = 35f;

	[Postfix(PostfixAttribute.Id.Percents)]
	[SerializeField]
	private float neededSatietyPerc = 80f;

	public override bool CanStart()
	{
		return ManagerBase<PlayerManager>.Instance.satietyCurrent <= maxSatietyPercToStart;
	}

	protected override float CalculateMaxProgress()
	{
		return neededSatietyPerc;
	}

	protected override void OnStart()
	{
		PlayerEating.changeSatietyEvent += OnChangeSatiety;
		base.CurProgress = ManagerBase<PlayerManager>.Instance.satietyCurrent / ManagerBase<PlayerManager>.Instance.SatietyMaximum * 100f;
	}

	protected override void OnEnd()
	{
		PlayerEating.changeSatietyEvent -= OnChangeSatiety;
	}

	private void OnChangeSatiety()
	{
		base.CurProgress = Mathf.CeilToInt(ManagerBase<PlayerManager>.Instance.satietyCurrent / ManagerBase<PlayerManager>.Instance.SatietyMaximum * 100f);
	}
}
