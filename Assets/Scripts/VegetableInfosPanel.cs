using UnityEngine;

public class VegetableInfosPanel : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private GameObject infoPanelPrefab;

	public void OnInitializeUI()
	{
		VegetableBehaviour.Instances.ForEach(delegate(VegetableBehaviour x)
		{
			OnVegetableSpawn(x);
		});
		VegetableBehaviour.spawnEvent += OnVegetableSpawn;
	}

	private void OnDestroy()
	{
		VegetableBehaviour.spawnEvent -= OnVegetableSpawn;
	}

	private void OnVegetableSpawn(VegetableBehaviour spawnedVegetable)
	{
		Object.Instantiate(infoPanelPrefab, base.transform).GetComponent<VegetableInfoPanel>().Spawn(spawnedVegetable);
	}
}
