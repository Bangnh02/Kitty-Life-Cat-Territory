using System;
using UnityEngine;

[Serializable]
public class EnemyArchetypeReference
{
	[SerializeField]
	private EnemyArchetype enemyArchetypePrefabRef;

	private EnemyArchetype enemyArchetypeRuntimeRef;

	public EnemyArchetype Value
	{
		get
		{
			if (enemyArchetypeRuntimeRef == null || enemyArchetypePrefabRef.gameObject.name != enemyArchetypeRuntimeRef.gameObject.name)
			{
				enemyArchetypeRuntimeRef = Singleton<EnemySpawner>.Instance.Archetypes.Find((EnemyArchetype x) => x.gameObject.name == enemyArchetypePrefabRef.gameObject.name);
			}
			return enemyArchetypeRuntimeRef;
		}
	}
}
