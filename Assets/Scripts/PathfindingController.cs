using Avelog;
using System.Linq;
using UnityEngine;

public class PathfindingController : MonoBehaviour
{
	[SerializeField]
	private PathWaypoint start;

	[SerializeField]
	private PathWaypoint goal;

	private void Start()
	{
		foreach (PathWaypoint item in from x in Pathfinding.FindPath(start, goal)
			select (x))
		{
			UnityEngine.Debug.Log(item.gameObject.name);
		}
	}

	private void Update()
	{
	}
}
