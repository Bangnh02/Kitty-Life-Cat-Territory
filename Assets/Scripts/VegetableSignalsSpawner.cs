using System.Collections.Generic;
using UnityEngine;

public class VegetableSignalsSpawner : Singleton<VegetableSignalsSpawner>
{
	[SerializeField]
	private GameObject vegetableSignalPrefab;

	[SerializeField]
	private Transform vegetableSignalParent;

	private List<VegetableSignalObject> vegetableSignals = new List<VegetableSignalObject>();

	protected override void OnInit()
	{
		VegetableBehaviour.Instances.ForEach(delegate(VegetableBehaviour x)
		{
			if (x.IsNextToPlant)
			{
				AssignFreeSignal(x);
			}
		});
		VegetableBehaviour.spawnEvent += OnVegetableSpawn;
		VegetableBehaviour.plantEvent += OnVegetablePlant;
	}

	private void OnDestroy()
	{
		VegetableBehaviour.spawnEvent -= OnVegetableSpawn;
		VegetableBehaviour.plantEvent -= OnVegetablePlant;
	}

	private void OnVegetableSpawn(VegetableBehaviour vegetable)
	{
		if (vegetable.IsNextToPlant)
		{
			AssignFreeSignal(vegetable);
		}
	}

	private void OnVegetablePlant(VegetableBehaviour vegetable)
	{
		vegetableSignals.Find((VegetableSignalObject x) => x.CurBehaviour == vegetable).Unspawn();
		vegetable.SetSignalModel(null);
		VegetableBehaviour vegetable2 = VegetableBehaviour.Instances.Find((VegetableBehaviour x) => x.IsNextToPlant && x.SignalModel == null);
		AssignFreeSignal(vegetable2);
	}

	private void AssignFreeSignal(VegetableBehaviour vegetable)
	{
		if (!(vegetable == null))
		{
			VegetableSignalObject vegetableSignalObject = vegetableSignals.Find((VegetableSignalObject x) => x.IsFree);
			if (vegetableSignalObject == null)
			{
				vegetableSignalObject = Object.Instantiate(vegetableSignalPrefab, vegetableSignalParent).GetComponent<VegetableSignalObject>();
				vegetableSignals.Add(vegetableSignalObject);
			}
			vegetable.SetSignalModel(vegetableSignalObject.gameObject);
			vegetableSignalObject.Spawn(vegetable);
		}
	}
}
