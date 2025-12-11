using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : ManagerBase<EnemyManager>
{
	[Serializable]
	public class MinibossData
	{
		[NonSerialized]
		private EnemyArchetype archetype;

		public string archetypeName;

		public int simpleSchemeDeaths;

		public EnemyArchetype Archetype
		{
			get
			{
				if (archetype == null)
				{
					archetype = Singleton<EnemySpawner>.Instance.Archetypes.Find((EnemyArchetype x) => x.name == archetypeName);
				}
				return archetype;
			}
		}

		public MinibossData(EnemyArchetype archetype)
		{
			this.archetype = archetype;
			archetypeName = archetype.name;
			simpleSchemeDeaths = 0;
		}
	}

	[SerializeField]
	private float habitatPopulationCheckFrequency = 10f;

	[SerializeField]
	private int maxPopulationInHabitat = 3;

	[SerializeField]
	private int maxBossAssistantCount = 2;

	[Header("Отладка")]
	[Save]
	public List<EnemyCurrentScheme> saveSchemes = new List<EnemyCurrentScheme>();

	[Save]
	public int killedBosses;

	[Save]
	[SerializeField]
	private List<MinibossData> minibossesData = new List<MinibossData>();

	public float HabitatPopulationCheckFrequency => habitatPopulationCheckFrequency;

	public int MaxPopulationInHabitat => maxPopulationInHabitat;

	public int MaxBossAssistantCount => maxBossAssistantCount;

	public MinibossData GetMinibossData(EnemyArchetype enemyArchetype)
	{
		MinibossData minibossData = minibossesData.Find((MinibossData x) => x.Archetype == enemyArchetype);
		if (minibossData == null)
		{
			minibossData = new MinibossData(enemyArchetype);
			minibossesData.Add(minibossData);
		}
		return minibossData;
	}

	protected override void OnInit()
	{
	}
}
