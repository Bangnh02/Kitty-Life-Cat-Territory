using Avelog;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DG_Sphere))]
public class PathWaypoint : MonoBehaviour, Pathfinding.IPathNode
{
	private class NeighborDistance
	{
		public Pathfinding.IPathNode neighbor;

		public float distance;
	}

	private static List<PathWaypoint> instances;

	[SerializeField]
	private Color color = Color.white;

	[SerializeField]
	private Color selectedColor = Color.red;

	[SerializeField]
	private List<PathWaypoint> neighbors;

	private bool listCorrected;

	private List<NeighborDistance> neighborDistances = new List<NeighborDistance>();

	public static List<PathWaypoint> Instances
	{
		get
		{
			if (instances == null)
			{
				instances = new List<PathWaypoint>(UnityEngine.Object.FindObjectsOfType<PathWaypoint>());
			}
			return instances;
		}
	}

	public IEnumerable<Pathfinding.IPathNode> Neighbors
	{
		get
		{
			if (!listCorrected)
			{
				neighbors.RemoveAll((PathWaypoint x) => x == null);
				listCorrected = true;
			}
			return neighbors;
		}
	}

	public float GetDistance(Pathfinding.IPathNode otherPathNode)
	{
		NeighborDistance neighborDistance = neighborDistances.Find((NeighborDistance x) => x.neighbor == otherPathNode);
		if (neighborDistance == null)
		{
			float distance = Vector3.Distance(base.transform.position, (otherPathNode as PathWaypoint).transform.position);
			neighborDistance = new NeighborDistance
			{
				neighbor = otherPathNode,
				distance = distance
			};
			neighborDistances.Add(neighborDistance);
		}
		return neighborDistance.distance;
	}

	public float GetHeuristicDistanceEstimate(Pathfinding.IPathNode otherPathNode)
	{
		if (neighbors.Contains(otherPathNode as PathWaypoint))
		{
			return GetDistance(otherPathNode);
		}
		return Vector3.Distance(base.transform.position, (otherPathNode as PathWaypoint).transform.position);
	}
}
