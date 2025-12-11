using System.Collections.Generic;
using UnityEngine;

public class ClewSpawner : Singleton<ClewSpawner>
{
	[SerializeField]
	private float clewStartPickDistance = 4f;

	[SerializeField]
	private Transform spawnedObjsParent;

	public float ClewStartPickDistance => clewStartPickDistance;

	public List<Clew> SpawnedClews
	{
		get;
		private set;
	} = new List<Clew>();


	protected override void OnInit()
	{
		foreach (ClewSpawnPoint item in new List<ClewSpawnPoint>(Object.FindObjectsOfType<ClewSpawnPoint>()))
		{
			if (!item.ClewData.isPicked)
			{
				GameObject gameObject = Object.Instantiate(item.ClewPrefab, item.transform.position, item.transform.rotation, spawnedObjsParent);
				gameObject.transform.localScale = item.transform.localScale;
				Clew clew = gameObject.GetComponent<Clew>();
				SpawnedClews.Add(clew);
				clew.Spawn(delegate
				{
					OnUnspuwn(clew);
				}, item);
			}
		}
	}

	private void OnUnspuwn(Clew clew)
	{
		SpawnedClews.Remove(clew);
	}
}
