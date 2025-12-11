using System.Collections.Generic;
using UnityEngine;

public class EnemyInfosPanel : MonoBehaviour, IInitializableUI
{
	private List<EnemyInfoPanel> spawnedInfoPanels = new List<EnemyInfoPanel>();

	[SerializeField]
	private GameObject infoPanelPrefab;

	public void OnInitializeUI()
	{
		foreach (EnemyController instance in EnemyController.Instances)
		{
			OnSpawn(instance);
		}
		EnemyController.spawnEvent += OnSpawn;
	}

	private void OnDestroy()
	{
		EnemyController.spawnEvent -= OnSpawn;
	}

	private void OnSpawn(EnemyController enemyController)
	{
		EnemyInfoPanel enemyInfoPanel = spawnedInfoPanels.Find((EnemyInfoPanel x) => x.EnemyController == enemyController);
		if (enemyInfoPanel == null)
		{
			enemyInfoPanel = UnityEngine.Object.Instantiate(infoPanelPrefab, base.transform).GetComponent<EnemyInfoPanel>();
			spawnedInfoPanels.Add(enemyInfoPanel);
		}
		enemyInfoPanel.Spawn(enemyController);
	}
}
