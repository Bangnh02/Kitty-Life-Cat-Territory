using UnityEngine;

public class PlayerClewPicker : MonoBehaviour, IInitializablePlayerComponent
{
	public void Initialize()
	{
	}

	private void Update()
	{
		if (Singleton<ClewSpawner>.Instance == null)
		{
			return;
		}
		for (int i = 0; i < Singleton<ClewSpawner>.Instance.SpawnedClews.Count; i++)
		{
			Clew clew = Singleton<ClewSpawner>.Instance.SpawnedClews[i];
			if (clew.OnProcessingDistance && !clew.IsPicking && (clew.Position - PlayerSpawner.PlayerInstance.PlayerCenter.position).IsShorterOrEqual(Singleton<ClewSpawner>.Instance.ClewStartPickDistance))
			{
				clew.Pick();
			}
		}
	}
}
