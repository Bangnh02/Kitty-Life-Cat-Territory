using System.Collections.Generic;
using UnityEngine;

public class MenuCatSpawnPointsGroup : MonoBehaviour
{
	private List<MenuCatSpawnPoint> _spawnPoints;

	public int PointsCount => SpawnPoints.Count;

	public List<MenuCatSpawnPoint> SpawnPoints
	{
		get
		{
			if (_spawnPoints == null || _spawnPoints.Count == 0)
			{
				_spawnPoints = new List<MenuCatSpawnPoint>(GetComponentsInChildren<MenuCatSpawnPoint>());
				_spawnPoints.Sort((MenuCatSpawnPoint o1, MenuCatSpawnPoint o2) => o1.transform.GetSiblingIndex().CompareTo(o2.transform.GetSiblingIndex()));
			}
			return _spawnPoints;
		}
	}
}
