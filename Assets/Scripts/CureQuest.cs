using System;
using UnityEngine;

public class CureQuest : Quest
{
	[SerializeField]
	private float maxHealthToStart = 20f;

	public override bool CanStart()
	{
		return ManagerBase<PlayerManager>.Instance.healthCurrent / ManagerBase<PlayerManager>.Instance.HealthMaximum * 100f < maxHealthToStart;
	}

	protected override void OnStart()
	{
		PlayerSpawner.PlayerInstance.PlayerCombat.healEvent += OnPlayerHeal;
		PlayerSpawner.PlayerInstance.PlayerCombat.takeDamageEvent += OnTakeDamage;
	}

	private void OnPlayerHeal()
	{
		UpdateProgress();
	}

	private void OnTakeDamage(ActorCombat.TakeDamageType takeDamageType, float damage, ActorCombat attacker)
	{
		UpdateProgress();
	}

	private void UpdateProgress()
	{
		base.CurProgress = ManagerBase<PlayerManager>.Instance.healthCurrent / ManagerBase<PlayerManager>.Instance.HealthMaximum * 100f;
	}

	protected override void OnEnd()
	{
		PlayerSpawner.PlayerInstance.PlayerCombat.healEvent += OnPlayerHeal;
		PlayerSpawner.PlayerInstance.PlayerCombat.takeDamageEvent += OnTakeDamage;
	}

	protected override float CalculateMaxProgress()
	{
		throw new NotImplementedException();
	}
}
