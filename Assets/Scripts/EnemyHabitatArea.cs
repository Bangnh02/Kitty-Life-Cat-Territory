using System.Collections.Generic;
using UnityEngine;

public class EnemyHabitatArea : HabitatArea
{
	[SerializeField]
	private List<EnemyArchetypeReference> archetypeReferences = new List<EnemyArchetypeReference>();

	private List<EnemyArchetype> _archetypes;

	private const int notIdentifiedZoneId = -1;

	private int zoneId = -1;

	public List<EnemyArchetype> Archetypes
	{
		get
		{
			if (_archetypes == null && _archetypes == null)
			{
				_archetypes = new List<EnemyArchetype>();
				archetypeReferences.ForEach(delegate(EnemyArchetypeReference archetypeRef)
				{
					_archetypes.Add(archetypeRef.Value);
				});
			}
			return _archetypes;
		}
	}

	public int ZoneId
	{
		get
		{
			if (zoneId == -1)
			{
				zoneId = base.transform.parent.GetSiblingIndex();
			}
			return zoneId;
		}
	}

	public List<EnemyController> Inhabitants
	{
		get;
		set;
	} = new List<EnemyController>();

}
