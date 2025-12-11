using System.Linq;
using UnityEngine;

public class HuntingMiniBossQuest : Quest
{
	[SerializeField]
	private int playerLevelMinimum = 3;

	[SerializeField]
	private int familyMembersCountMinimum;

	public override bool CanStart()
	{
		if (ManagerBase<PlayerManager>.Instance.Level >= playerLevelMinimum && Singleton<EnemySpawner>.Instance != null && Singleton<EnemySpawner>.Instance.SpawnedEnemies.Any((EnemySpawner.EnemyInstance x) => x.logic.currentScheme.Scheme.Type == EnemyScheme.SchemeType.MiniBoss))
		{
			return ManagerBase<FamilyManager>.Instance.family.Count >= familyMembersCountMinimum;
		}
		return false;
	}

	protected override float CalculateMaxProgress()
	{
		return 1f;
	}

	protected override void OnStart()
	{
		ActorCombat.killEvent += OnKill;
	}

	protected override void OnEnd()
	{
		ActorCombat.killEvent -= OnKill;
	}

	private void OnKill(ActorCombat killer, ActorCombat target)
	{
		if (killer is PlayerCombat || killer is FamilyMemberCombat)
		{
			EnemyController componentInParent = target.GetComponentInParent<EnemyController>();
			if (componentInParent != null && componentInParent.CurrentScheme.Scheme.Type == EnemyScheme.SchemeType.MiniBoss)
			{
				base.CurProgress = base.MaxProgress;
			}
		}
	}
}
