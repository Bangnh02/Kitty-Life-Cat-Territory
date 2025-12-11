using Avelog;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFarmResident : MonoBehaviour, IInitializablePlayerComponent
{
	private bool isInited;

	private List<FarmResident> nearFarmResidents = new List<FarmResident>();

	private ActorPicker PlayerPicker => PlayerSpawner.PlayerInstance.PlayerPicker;

	public static event Action updateNearFarmResidentsEvent;

	public void Initialize()
	{
		if (!(Singleton<FarmResidentSpawner>.Instance == null))
		{
			if (Singleton<FarmResidentSpawner>.Instance.InitializeSpawnCompleted)
			{
				OnFarmResidentSpawnerInitializeSpawnCompleted();
			}
			else
			{
				Singleton<FarmResidentSpawner>.Instance.initializeSpawnCompletedEvent += OnFarmResidentSpawnerInitializeSpawnCompleted;
			}
			Avelog.Input.satisfyFarmResidentPressedEvent += OnSatisfyFarmResidentPressed;
		}
	}

	private void OnSatisfyFarmResidentPressed()
	{
		if (isInited)
		{
			SatisfyFarmResident();
		}
	}

	private void OnDestroy()
	{
		if (Singleton<FarmResidentSpawner>.Instance != null)
		{
			Singleton<FarmResidentSpawner>.Instance.initializeSpawnCompletedEvent -= OnFarmResidentSpawnerInitializeSpawnCompleted;
		}
		Avelog.Input.satisfyFarmResidentPressedEvent -= OnSatisfyFarmResidentPressed;
	}

	private void OnFarmResidentSpawnerInitializeSpawnCompleted()
	{
		isInited = true;
	}

	public bool CanSatisfyFarmResident()
	{
		return nearFarmResidents.Find((FarmResident x) => x.IsNeededFood(PlayerPicker.PickedItem as Food)) != null;
	}

	public void SatisfyFarmResident()
	{
		FarmResident farmResident = nearFarmResidents.Find((FarmResident x) => x.IsNeededFood(PlayerPicker.PickedItem as Food));
		if (farmResident != null)
		{
			Food food = PlayerPicker.PickedItem as Food;
			PlayerPicker.Drop(isSatisfyFarmResident: true);
			farmResident.SatisfyNeed(food);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("FarmResident"))
		{
			FarmResident componentInParent = other.GetComponentInParent<FarmResident>();
			if (componentInParent != null && !nearFarmResidents.Contains(componentInParent))
			{
				nearFarmResidents.Add(componentInParent);
				PlayerFarmResident.updateNearFarmResidentsEvent?.Invoke();
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("FarmResident"))
		{
			FarmResident componentInParent = other.GetComponentInParent<FarmResident>();
			if (componentInParent != null && nearFarmResidents.Contains(componentInParent))
			{
				nearFarmResidents.Remove(componentInParent);
				PlayerFarmResident.updateNearFarmResidentsEvent?.Invoke();
			}
		}
	}

	public bool HaveNearFarmResident()
	{
		return nearFarmResidents.Count > 0;
	}
}
