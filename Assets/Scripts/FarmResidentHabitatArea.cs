using Avelog;
using System.Collections.Generic;
using UnityEngine;

public class FarmResidentHabitatArea : HabitatArea
{
	public class MigratePath
	{
		public List<PathWaypoint> waypoints;

		public FarmResidentHabitatArea otherHabitatArea;
	}

	[SerializeField]
	private List<FarmResidentId> allowedFarmResidents;

	public const int notDefinedHabitatId = -1;

	private int _habitatId = -1;

	private FarmResident _inhabitant;

	private List<DigZone> digZones = new List<DigZone>();

	private const int notIdentifiedZoneId = -1;

	private int zoneId = -1;

	private PathWaypoint _habitatNearestWaypoint;

	public List<FarmResidentId> AllowedFarmResidents => allowedFarmResidents;

	public int HabitatId
	{
		get
		{
			if (_habitatId == -1)
			{
				_habitatId = base.transform.GetSiblingIndex();
			}
			return _habitatId;
		}
	}

	public FarmResident Inhabitant
	{
		get
		{
			return _inhabitant;
		}
		set
		{
			_inhabitant = value;
			if (_inhabitant != null)
			{
				ManagerBase<FarmResidentManager>.Instance.ZonesPriorities.Find((FarmResidentManager.ZonePriority x) => x.zoneId == ZoneId).useCount++;
			}
		}
	}

	public List<DigZone> DigZones
	{
		get
		{
			if (digZones.Count != base.transform.childCount)
			{
				digZones.Clear();
				digZones.AddRange(GetComponentsInChildren<DigZone>());
			}
			return digZones;
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

	public List<MigratePath> MigratePaths
	{
		get;
		private set;
	} = new List<MigratePath>();


	private PathWaypoint HabitatNearestWaypoint
	{
		get
		{
			if (_habitatNearestWaypoint == null)
			{
				_habitatNearestWaypoint = PathWaypoint.Instances[0];
				float num = float.MaxValue;
				foreach (PathWaypoint instance in PathWaypoint.Instances)
				{
					float sqrMagnitude = (instance.transform.position - base.HabitatCenter).sqrMagnitude;
					if (num > sqrMagnitude)
					{
						_habitatNearestWaypoint = instance;
						num = sqrMagnitude;
					}
				}
			}
			return _habitatNearestWaypoint;
		}
	}

	public MigratePath GetMigratePath(FarmResidentHabitatArea otherHabitatArea)
	{
		if (PathWaypoint.Instances.Count == 0)
		{
			return null;
		}
		MigratePath migratePath = MigratePaths.Find((MigratePath x) => x.otherHabitatArea == otherHabitatArea);
		if (migratePath == null)
		{
			PathWaypoint start = HabitatNearestWaypoint;
			float num = float.MaxValue;
			PathWaypoint goal = PathWaypoint.Instances[0];
			float num2 = float.MaxValue;
			foreach (PathWaypoint instance in PathWaypoint.Instances)
			{
				float sqrMagnitude = (instance.transform.position - base.HabitatCenter).sqrMagnitude;
				if (num > sqrMagnitude)
				{
					start = instance;
					num = sqrMagnitude;
				}
				float sqrMagnitude2 = (instance.transform.position - otherHabitatArea.HabitatCenter).sqrMagnitude;
				if (num2 > sqrMagnitude2)
				{
					goal = instance;
					num2 = sqrMagnitude2;
				}
			}
			List<PathWaypoint> waypoints = Pathfinding.FindPath(start, goal);
			migratePath = new MigratePath
			{
				otherHabitatArea = otherHabitatArea,
				waypoints = waypoints
			};
			MigratePaths.Add(migratePath);
			MigratePath migratePath2 = new MigratePath
			{
				otherHabitatArea = this
			};
			migratePath2.waypoints = new List<PathWaypoint>(migratePath.waypoints);
			migratePath2.waypoints.Reverse();
			otherHabitatArea.MigratePaths.Add(migratePath2);
		}
		return migratePath;
	}
}
