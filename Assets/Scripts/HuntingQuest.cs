using System.Linq;
using UnityEngine;

public class HuntingQuest : Quest
{
	[SerializeField]
	private int playerLevelMinimum = 3;

	[SerializeField]
	private int familyMembersCountMinimum;

	[Tooltip("ограничивается количеством Врагов текущего Архетипа на Ферме делённым на Х3 (1.5)")]
	[SerializeField]
	private float enemiesClampDiv = 1.5f;

	[SerializeField]
	[Tooltip("Число для задания случайного необходимого прогресса (минимум)")]
	private int progressRandomVal1;

	[SerializeField]
	[Tooltip("Число для задания случайного необходимого прогресса (максимум)")]
	private int progressRandomVal2;

	[SerializeField]
	private string archetypeName;

	private EnemyArchetype _enemyArchetype;

	protected int ProgressRandomVal1 => progressRandomVal1;

	protected int ProgressRandomVal2 => progressRandomVal2;

	public string ArchetypeName => archetypeName;

	private EnemyArchetype EnemyArchetype
	{
		get
		{
			if (_enemyArchetype == null)
			{
				_enemyArchetype = Singleton<EnemySpawner>.Instance.Archetypes.Find((EnemyArchetype x) => x.name == archetypeName);
			}
			return _enemyArchetype;
		}
	}

	private int EnemiesCount
	{
		get
		{
			if (!(Singleton<EnemySpawner>.Instance != null))
			{
				return 0;
			}
			return Singleton<EnemySpawner>.Instance.SpawnedEnemies.Count((EnemySpawner.EnemyInstance x) => x.logic.currentScheme.Archetype == EnemyArchetype);
		}
	}

	public override bool CanStart()
	{
		if (EnemiesCount > progressRandomVal2 && ManagerBase<PlayerManager>.Instance.Level >= playerLevelMinimum)
		{
			return ManagerBase<FamilyManager>.Instance.family.Count >= familyMembersCountMinimum;
		}
		return false;
	}

	protected override float CalculateMaxProgress()
	{
		HuntingQuestCategory.HuntingQuestData huntingQuestData = (base.Category as HuntingQuestCategory).MySaveData.huntingQuestDatas.Find((HuntingQuestCategory.HuntingQuestData x) => x.archetypeName == archetypeName);
		int b = (huntingQuestData == null) ? 1 : (huntingQuestData.completedQuestsCount + 1);
		int a = (int)((float)EnemiesCount / enemiesClampDiv);
		int num = Mathf.Clamp(ProgressRandomVal2, 1, Mathf.Min(a, b));
		return Random.Range(Mathf.Clamp(ProgressRandomVal1, 1, num), num);
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
			if (componentInParent != null && componentInParent.CurrentScheme.Archetype == EnemyArchetype)
			{
				float num = base.CurProgress += 1f;
			}
		}
	}
}
