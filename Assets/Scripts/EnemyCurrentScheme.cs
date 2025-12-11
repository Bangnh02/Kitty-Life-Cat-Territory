using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class EnemyCurrentScheme
{
	[NonSerialized]
	private EnemyScheme scheme;

	[SerializeField]
	[ReadonlyInspector]
	private string schemeName;

	[NonSerialized]
	private EnemyArchetype archetype;

	[SerializeField]
	[ReadonlyInspector]
	private string archetypeName;

	public int level;

	public int zone = -1;

	public const int notInitializedZone = -1;

	[NonSerialized]
	public Action correctZoneEvent;

	[NonSerialized]
	public Action correctLevelEvent;

	public EnemyScheme Scheme
	{
		get
		{
			if (scheme == null)
			{
				List<EnemyScheme> list = Archetype.GetComponentsInChildren<EnemyScheme>().ToList();
				scheme = list.Find((EnemyScheme x) => x.name == schemeName);
			}
			return scheme;
		}
		set
		{
			scheme = value;
		}
	}

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

	public bool IsValid
	{
		get
		{
			if (Archetype != null)
			{
				return Scheme != null;
			}
			return false;
		}
	}

	public EnemyCurrentScheme(EnemyScheme scheme, EnemyArchetype archetype, int level, int zone)
	{
		this.scheme = scheme;
		this.archetype = archetype;
		this.level = level;
		this.zone = zone;
		schemeName = scheme.name;
		archetypeName = archetype.name;
	}

	public void CorrectZone(int zone)
	{
		if (zone != this.zone)
		{
			this.zone = zone;
			correctZoneEvent?.Invoke();
		}
	}

	public void CorrectLevel(int level)
	{
		if (level != this.level)
		{
			this.level = level;
			correctLevelEvent?.Invoke();
		}
	}
}
