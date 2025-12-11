using UnityEngine;

public class QuenchThirstQuest : Quest
{
	[Postfix(PostfixAttribute.Id.Percents)]
	[SerializeField]
	private float maxThirstPercToStart = 35f;

	[Postfix(PostfixAttribute.Id.Percents)]
	[SerializeField]
	private float neededThirstPerc = 80f;

	public override bool CanStart()
	{
		return ManagerBase<PlayerManager>.Instance.thirstCurrent <= maxThirstPercToStart;
	}

	protected override float CalculateMaxProgress()
	{
		return neededThirstPerc;
	}

	protected override void OnStart()
	{
		PlayerEating.changeThirstEvent += OnChangeThirst;
		PlayerSpawner.beforeRespawnPlayerEvent += OnRespawnPlayer;
		base.CurProgress = ManagerBase<PlayerManager>.Instance.thirstCurrent / ManagerBase<PlayerManager>.Instance.ThirstMaximum * 100f;
	}

	protected override void OnEnd()
	{
		PlayerEating.changeThirstEvent -= OnChangeThirst;
		PlayerSpawner.beforeRespawnPlayerEvent -= OnRespawnPlayer;
	}

	private void OnChangeThirst()
	{
		base.CurProgress = ManagerBase<PlayerManager>.Instance.thirstCurrent / ManagerBase<PlayerManager>.Instance.ThirstMaximum * 100f;
	}

	private void OnRespawnPlayer()
	{
		Singleton<QuestSpawner>.Instance.CancelQuest();
	}
}
