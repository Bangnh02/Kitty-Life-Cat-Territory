public class FindClewQuest : Quest
{
	public override bool CanStart()
	{
		return ManagerBase<ClewManager>.Instance.ClewsCollected < ManagerBase<ClewManager>.Instance.ClewsCount;
	}

	protected override float CalculateMaxProgress()
	{
		return 1f;
	}

	protected override void OnStart()
	{
		Clew.pickEvent += OnClewPick;
	}

	protected override void OnEnd()
	{
		Clew.pickEvent -= OnClewPick;
	}

	private void OnClewPick()
	{
		float num = base.CurProgress += 1f;
	}
}
