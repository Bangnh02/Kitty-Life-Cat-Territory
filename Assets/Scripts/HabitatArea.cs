using System.Collections.Generic;
using UnityEngine;

public class HabitatArea : MonoBehaviour
{
	private List<PatrolZone> patrolZones = new List<PatrolZone>();

	private Vector3 habitatCenter = Vector3.zero;

	public List<PatrolZone> PatrolZones
	{
		get
		{
			if (patrolZones.Count != base.transform.childCount)
			{
				patrolZones.Clear();
				patrolZones.AddRange(GetComponentsInChildren<PatrolZone>());
			}
			return patrolZones;
		}
	}

	public Vector3 HabitatCenter
	{
		get
		{
			if (!(habitatCenter != Vector3.zero))
			{
				foreach (PatrolZone patrolZone in PatrolZones)
				{
					habitatCenter += patrolZone.transform.position;
				}
				habitatCenter /= (float)PatrolZones.Count;
			}
			return habitatCenter;
		}
	}
}
