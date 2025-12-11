using Avelog;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerPlanter : MonoBehaviour, IInitializablePlayerComponent
{
	private List<VegetableBehaviour> nearPotentialPlants = new List<VegetableBehaviour>();

	public bool HaveNearPotentialPlants => nearPotentialPlants.Count((VegetableBehaviour x) => !x.VegetableData.isPlanted && x.IsNextToPlant) > 0;

	public static event Action updateNearPotentialPlantEvent;

	public void Initialize()
	{
		Avelog.Input.plantPressedEvent += OnPlantPressed;
	}

	private void OnDestroy()
	{
		Avelog.Input.plantPressedEvent -= OnPlantPressed;
	}

	private void OnPlantPressed()
	{
		Plant();
	}

	public bool CanPlant()
	{
		if (HaveNearPotentialPlants)
		{
			return CanAffordPlant();
		}
		return false;
	}

	public bool CanAffordPlant()
	{
		return nearPotentialPlants.Any((VegetableBehaviour x) => x.CanPlant());
	}

	public void Plant()
	{
		if (CanPlant())
		{
			List<VegetableBehaviour> list = nearPotentialPlants.FindAll((VegetableBehaviour x) => x.CanPlant());
			List<(VegetableBehaviour potentialPlant, float sqrDistance)> plantDistances = new List<(VegetableBehaviour, float)>();
			list.ForEach(delegate(VegetableBehaviour x)
			{
				plantDistances.Add((x, (PlayerSpawner.PlayerInstance.transform.position - x.transform.position).sqrMagnitude));
			});
			plantDistances.Sort(((VegetableBehaviour potentialPlant, float sqrDistance) x, (VegetableBehaviour potentialPlant, float sqrDistance) y) => x.sqrDistance.CompareTo(y.sqrDistance));
			VegetableBehaviour item = plantDistances[0].potentialPlant;
			item.Plant();
			ManagerBase<PlayerManager>.Instance.ChangeCoins(-item.PlantCost);
			nearPotentialPlants.Remove(item);
			PlayerPlanter.updateNearPotentialPlantEvent?.Invoke();
		}
	}

	private void Update()
	{
		foreach (VegetableBehaviour instance in VegetableBehaviour.Instances)
		{
			if (instance.IsNextToPlant && !instance.VegetableData.isPlanted)
			{
				if ((base.transform.position - instance.transform.position).IsShorterOrEqual(instance.DistanceToUpdateState) && !nearPotentialPlants.Contains(instance))
				{
					nearPotentialPlants.Add(instance);
					PlayerPlanter.updateNearPotentialPlantEvent?.Invoke();
				}
				else if ((base.transform.position - instance.transform.position).IsLonger(instance.DistanceToUpdateState) && nearPotentialPlants.Contains(instance))
				{
					nearPotentialPlants.Remove(instance);
					PlayerPlanter.updateNearPotentialPlantEvent?.Invoke();
				}
			}
		}
	}
}
