using Avelog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class PlayerEating : MonoBehaviour, IInitializablePlayerComponent
{
	public delegate void ChangeNearFoodHandler(bool haveNearFood);

	public delegate void EndEatingHandler(float satietyInc, float foodEffect);

	public delegate void EndDrinkingHandler(int quenchThirstCount);

	public delegate void EndEatingFoodHandler(Food food);

	private bool isEating;

	private List<Food> nearFoods = new List<Food>();

	private List<Food> availableForFamilyFood = new List<Food>();

	private Food foodToEat;

	private Coroutine feedingCor;

	private Action endEatingCallback;

	private List<Water> nearWaterObjs = new List<Water>();

	private bool isDrinking;

	private int quenchThirstCount;

	private Action endDrinkingCallback;

	private Animator Animator => PlayerSpawner.PlayerInstance.Model.Animator;

	private ActorPicker PlayerPicker => PlayerSpawner.PlayerInstance.PlayerPicker;

	private ItemUser ItemUser => PlayerSpawner.PlayerInstance.ItemUser;

	public bool IsLosingHPDueSatiety
	{
		get;
		private set;
	}

	public bool IsLosingSatietyDueThirst
	{
		get;
		private set;
	}

	private List<Food> EatableFreeNearGoodFood => EatableFreeNearFood.FindAll((Food x) => !x.IsBadFood);

	private List<Food> EatableFreeNearFood => EatableNearFoods.FindAll((Food x) => x.CurCountFoodUnits > x.CountOccupiedFoodUnits);

	private List<Food> EatableNearFoods => nearFoods.FindAll((Food x) => x.Eatable);

	private List<Food> AvailableForFamilyFood => availableForFamilyFood;

	public bool HaveEatableNearFood => EatableFreeNearFood.Count > 0;

	public bool HaveNearGoodFood => EatableFreeNearGoodFood.Count > 0;

	public bool HaveAvailableFamilyFood => AvailableForFamilyFood.Count > 0;

	public float NormalizedEatTime
	{
		get;
		private set;
	}

	private Transform MouthDrinkPos => PlayerSpawner.PlayerInstance.Model.MouthDrinkPos;

	public bool HaveNearWater => nearWaterObjs.Count > 0;

	public List<Water> NearWaterObjs => nearWaterObjs;

	public bool IsDrinking => isDrinking;

	public float NormalizedDrinkTime
	{
		get;
		private set;
	}

	public event ChangeNearFoodHandler updateNearFoodEvent;

	public event ChangeNearFoodHandler updateNearGoodFoodEvent;

	public static event Action updateFamilyAvailableFoodEvent;

	public event Action changeNearWaterEvent;

	public static event Action changeThirstEvent;

	public static event Action changeSatietyEvent;

	public event EndEatingHandler endEatingEvent;

	public event Action endDrinkIterationEvent;

	public event EndDrinkingHandler endDrinkingEvent;

	public event Action compleateDrinkEvent;

	public event EndEatingFoodHandler endEatingFoodEvent;

	public void Initialize()
	{
		Food.holdFoodEvent += OnHoldFood;
		Food.unholdFoodEvent += OnUnholdFood;
		Food.unspawnEvent += OnUnspawnFood;
		PlayerSpawner.PlayerInstance.PlayerCombat.hitEvent += OnPlayerHit;
		PlayerPicker.changePickedItemEvent += OnChangePickedItem;
		PlayerPicker.dropEvent += OnDrop;
	}

	private void OnDestroy()
	{
		Food.holdFoodEvent -= OnHoldFood;
		Food.unholdFoodEvent -= OnUnholdFood;
		Food.unspawnEvent -= OnUnspawnFood;
		if (PlayerSpawner.IsPlayerSpawned)
		{
			PlayerSpawner.PlayerInstance.PlayerCombat.hitEvent += OnPlayerHit;
			PlayerPicker.changePickedItemEvent -= OnChangePickedItem;
			PlayerPicker.dropEvent -= OnDrop;
		}
	}

	private void OnUnholdFood(Food food)
	{
		if (nearFoods.Contains(food))
		{
			if (food.CountOccupiedFoodUnits == 0 && food.CurCountFoodUnits == 0)
			{
				nearFoods.Remove(food);
			}
			this.updateNearFoodEvent?.Invoke(HaveEatableNearFood);
			this.updateNearGoodFoodEvent?.Invoke(HaveNearGoodFood);
		}
		UpdateFoodForFamily(food);
	}

	private void OnHoldFood(Food food)
	{
		if (nearFoods.Contains(food))
		{
			this.updateNearFoodEvent?.Invoke(HaveEatableNearFood);
			this.updateNearGoodFoodEvent?.Invoke(HaveNearGoodFood);
		}
		UpdateFoodForFamily(food);
	}

	private void OnUnspawnFood(Food food)
	{
		if (nearFoods.Contains(food))
		{
			nearFoods.Remove(food);
			this.updateNearFoodEvent?.Invoke(HaveEatableNearFood);
			this.updateNearGoodFoodEvent?.Invoke(HaveNearGoodFood);
		}
		if (availableForFamilyFood.Contains(food))
		{
			availableForFamilyFood.Remove(food);
			PlayerEating.updateFamilyAvailableFoodEvent?.Invoke();
		}
	}

	private void OnPlayerHit(ActorCombat attacker, ActorCombat target)
	{
		if (target != null)
		{
			ChangeSatiety((0f - ManagerBase<PlayerManager>.Instance.SatietyMaximum) / 100f * ManagerBase<PlayerManager>.Instance.SatietyHitFallPerc);
			ChangeThirst((0f - ManagerBase<PlayerManager>.Instance.ThirstMaximum) / 100f * ManagerBase<PlayerManager>.Instance.ThirstHitFallPerc);
		}
	}

	private void OnChangePickedItem(bool havePickedItem)
	{
		Food food = PlayerPicker.PickedItem as Food;
		if (havePickedItem)
		{
			UpdateFoodForFamily(food);
		}
	}

	private void OnDrop(Item droppedItem, bool isSatisfyFarmResident)
	{
		Food food = droppedItem as Food;
		UpdateFoodForFamily(food);
	}

	private void UpdateFoodForFamily(Food food)
	{
		if (food != null)
		{
			bool flag = IsFoodAvailableForFamily(food);
			if (flag && !availableForFamilyFood.Contains(food))
			{
				availableForFamilyFood.Add(food);
				PlayerEating.updateFamilyAvailableFoodEvent?.Invoke();
			}
			else if (!flag && availableForFamilyFood.Contains(food))
			{
				availableForFamilyFood.Remove(food);
				PlayerEating.updateFamilyAvailableFoodEvent?.Invoke();
			}
		}
	}

	private void Update()
	{
		if (PlayerSpawner.PlayerInstance.PlayerCombat.CurLifeState != 0)
		{
			return;
		}
		if (!PlayerSpawner.PlayerInstance.PlayerCombat.InCombat && ManagerBase<PlayerManager>.Instance.CanHeal)
		{
			float num = ManagerBase<PlayerManager>.Instance.HealthMaximum / 100f * ManagerBase<PlayerManager>.Instance.HealHealth;
			float num2 = ManagerBase<FarmResidentManager>.Instance.SuperBonusesData.Find((FarmResidentManager.SuperBonusData x) => x.id == SuperBonus.Id.Milk).IsActive ? ManagerBase<FarmResidentManager>.Instance.MilkHealthRecoveryBonus : 0f;
			num += num / 100f * num2;
			PlayerSpawner.PlayerInstance.PlayerCombat.Heal(num * Time.deltaTime);
		}
		if (ManagerBase<PlayerManager>.Instance.healthCurrent > ManagerBase<PlayerManager>.Instance.HealthMaximum)
		{
			float damage = ManagerBase<PlayerManager>.Instance.HealthMaximum - ManagerBase<PlayerManager>.Instance.healthCurrent;
			PlayerSpawner.PlayerInstance.PlayerCombat.TakeDamage(ActorCombat.TakeDamageType.LoseHealth, damage, null);
		}
		if (ManagerBase<PlayerManager>.Instance.satietyCurrent > 0f && !isEating && !ManagerBase<PlayerManager>.Instance.isSleeping)
		{
			ChangeSatiety((0f - ManagerBase<PlayerManager>.Instance.SatietyFall) * Time.deltaTime);
		}
		if (ManagerBase<PlayerManager>.Instance.satietyCurrent == 0f && !isEating && !ManagerBase<PlayerManager>.Instance.isSleeping)
		{
			PlayerSpawner.PlayerInstance.PlayerCombat.TakeDamage(ActorCombat.TakeDamageType.LoseHealth, ManagerBase<PlayerManager>.Instance.HealthFall * Time.deltaTime, null);
			if (!IsLosingHPDueSatiety)
			{
				IsLosingHPDueSatiety = true;
			}
		}
		else if (ManagerBase<PlayerManager>.Instance.satietyCurrent != 0f && IsLosingHPDueSatiety)
		{
			IsLosingHPDueSatiety = false;
		}
		if (ManagerBase<PlayerManager>.Instance.thirstCurrent > 0f && !isDrinking && !ManagerBase<PlayerManager>.Instance.isSleeping)
		{
			ChangeThirst((0f - ManagerBase<PlayerManager>.Instance.ThirstFall) * Time.deltaTime);
		}
		if (ManagerBase<PlayerManager>.Instance.thirstCurrent == 0f && !IsLosingSatietyDueThirst)
		{
			IsLosingSatietyDueThirst = true;
		}
		else if (ManagerBase<PlayerManager>.Instance.thirstCurrent != 0f && IsLosingSatietyDueThirst)
		{
			IsLosingSatietyDueThirst = false;
		}
	}

	public void Respawn()
	{
		if (ManagerBase<PlayerManager>.Instance.satietyCurrent < ManagerBase<PlayerManager>.Instance.SatietyMaximum / 2f)
		{
			ManagerBase<PlayerManager>.Instance.satietyCurrent = ManagerBase<PlayerManager>.Instance.SatietyMaximum / 2f;
		}
		ChangeThirst(ManagerBase<PlayerManager>.Instance.ThirstMaximum);
		IsLosingHPDueSatiety = false;
		IsLosingSatietyDueThirst = false;
		if (nearFoods.Count > 0 || availableForFamilyFood.Count > 0)
		{
			nearFoods.Clear();
			availableForFamilyFood.Clear();
			this.updateNearFoodEvent?.Invoke(HaveEatableNearFood);
			this.updateNearGoodFoodEvent?.Invoke(HaveNearGoodFood);
			PlayerEating.updateFamilyAvailableFoodEvent?.Invoke();
		}
		if (nearWaterObjs.Count > 0)
		{
			nearWaterObjs.Clear();
			this.changeNearWaterEvent?.Invoke();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((other.gameObject.layer & Layers.TriggerLayer) != 0 && other.gameObject.tag == "Food")
		{
			Food component = other.gameObject.GetComponent<Food>();
			if (!nearFoods.Contains(component))
			{
				nearFoods.Add(component);
				this.updateNearFoodEvent?.Invoke(HaveEatableNearFood);
				this.updateNearGoodFoodEvent?.Invoke(HaveNearGoodFood);
			}
			UpdateFoodForFamily(component);
		}
		if ((other.gameObject.layer & Layers.TriggerLayer) != 0 && other.gameObject.tag == "Water")
		{
			Water component2 = other.GetComponent<Water>();
			if (component2 != null && !nearWaterObjs.Contains(component2))
			{
				nearWaterObjs.Add(component2);
			}
			this.changeNearWaterEvent?.Invoke();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((other.gameObject.layer & Layers.TriggerLayer) != 0 && other.gameObject.tag == "Food")
		{
			Food component = other.gameObject.GetComponent<Food>();
			if (PlayerPicker.HavePickedItem && PlayerPicker.PickedItem == component)
			{
				return;
			}
			if (nearFoods.Contains(component))
			{
				nearFoods.Remove(component);
				this.updateNearFoodEvent?.Invoke(HaveEatableNearFood);
				this.updateNearGoodFoodEvent?.Invoke(HaveNearGoodFood);
			}
			UpdateFoodForFamily(component);
		}
		if ((other.gameObject.layer & Layers.TriggerLayer) != 0 && other.gameObject.tag == "Water")
		{
			Water component2 = other.GetComponent<Water>();
			if (component2 != null && nearWaterObjs.Contains(component2))
			{
				nearWaterObjs.Remove(component2);
			}
			this.changeNearWaterEvent?.Invoke();
		}
	}

	private bool IsFoodAvailableForFamily(Food food)
	{
		if (food == null)
		{
			return false;
		}
		if (!food.Eatable)
		{
			return false;
		}
		if (!nearFoods.Contains(food))
		{
			return false;
		}
		if (PlayerPicker.PickedItem == food)
		{
			return false;
		}
		if (food.CurCountFoodUnits <= food.CountOccupiedFoodUnits || food.CurCountFoodUnits <= 0)
		{
			return false;
		}
		if (food.IsBadFood)
		{
			return false;
		}
		if (!ManagerBase<FamilyManager>.Instance.HaveFamily || FamilyMemberController.Instances.Count == 0)
		{
			return false;
		}
		if (NavMeshUtils.SamplePositionIterative(food.transform.position, out NavMeshHit navMeshHit, 1f, 2f, 2, -1))
		{
			NavMeshPath navMeshPath = new NavMeshPath();
			NavMesh.CalculatePath(FamilyMemberController.Instances[0].NavAgent.transform.position, navMeshHit.position, -1, navMeshPath);
			if (navMeshPath.status != 0)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public Food GetNearestFreeFood(bool onlyGoodFood = false)
	{
		List<Food> list = new List<Food>();
		list = ((!onlyGoodFood) ? EatableFreeNearFood : EatableFreeNearGoodFood);
		if (list.Count == 0)
		{
			return null;
		}
		if (list.Count == 1)
		{
			return list[0];
		}
		Food result = null;
		float num = float.MaxValue;
		Vector3 item = PlayerPicker.GetDropPosAndRot().position;
		foreach (Food item2 in list)
		{
			float sqrMagnitude = (item - item2.transform.position).sqrMagnitude;
			if (sqrMagnitude < num && PlayerPicker.PickedItem != item2)
			{
				result = item2;
				num = sqrMagnitude;
			}
		}
		return result;
	}

	public void Eat(Action endEatingCallback)
	{
		if (EatableFreeNearFood.Count != 0)
		{
			this.endEatingCallback = endEatingCallback;
			foodToEat = GetNearestFreeFood();
			foodToEat.HoldFoodUnit(ItemUser);
			feedingCor = StartCoroutine(Eating());
		}
	}

	private IEnumerator Eating()
	{
		isEating = true;
		Food eatedFood = foodToEat;
		Vector3 vector = eatedFood.transform.position - base.transform.position;
		Vector3 endForward = Vector3.ProjectOnPlane(vector, Vector3.up);
		while (PlayerSpawner.PlayerInstance.PlayerMovement.Rotate(endForward, Time.deltaTime, ManagerBase<PlayerManager>.Instance.EatDrinkRotationSpeed))
		{
			yield return null;
		}
		Animator.SetTrigger("Eat");
		Animator.SetBool("isEating", value: true);
		yield return new WaitUntil(() => Animator.IsPlayingByName(AnimationHashes.eatEnter.Value));
		yield return new WaitUntil(() => Animator.IsPlayingByTag(AnimationHashes.eat.Value) ? (!Animator.IsPlayingByName(AnimationHashes.eatEnter.Value)) : true);
		NormalizedEatTime = 0f;
		while (NormalizedEatTime < 1f)
		{
			NormalizedEatTime = Mathf.Clamp(NormalizedEatTime + Time.deltaTime / ManagerBase<PlayerManager>.Instance.EatingDuration, 0f, 1f);
			yield return null;
		}
		Animator.SetBool("isEating", value: false);
		eatedFood.EatFoodUnit();
		if (!eatedFood.IsBadFood)
		{
			float satietyInc = ChangeSatiety(eatedFood.FoodEffect);
			this.endEatingEvent?.Invoke(satietyInc, eatedFood.FoodEffect);
		}
		else
		{
			float num = CalculationsHelpUtils.CalculateProp(ManagerBase<PlayerManager>.Instance.SatietyMaximum, 50f, 100f);
			float damage = CalculationsHelpUtils.CalculateProp(ManagerBase<PlayerManager>.Instance.HealthMaximum, 50f, 100f);
			ChangeSatiety(0f - num);
			PlayerSpawner.PlayerInstance.PlayerCombat.TakeDamage(ActorCombat.TakeDamageType.LoseHealth, damage, null);
			ManagerBase<PlayerManager>.Instance.AddExperience(eatedFood.FoodEffect);
		}
		this.endEatingFoodEvent?.Invoke(eatedFood);
		foodToEat = null;
		yield return StartCoroutine(CompletingEat());
	}

	public void CancelEat()
	{
		if (foodToEat != null)
		{
			foodToEat.UnholdFoodUnit();
		}
		foodToEat = null;
		StartCoroutine(CompletingEat(instant: true));
	}

	private IEnumerator CompletingEat(bool instant = false)
	{
		if (feedingCor != null && isEating)
		{
			StopCoroutine(feedingCor);
		}
		feedingCor = null;
		isEating = false;
		Animator.SetBool("isEating", value: false);
		NormalizedEatTime = 0f;
		Action action = endEatingCallback;
		endEatingCallback = null;
		action?.Invoke();
		yield break;
	}

	private float ChangeSatiety(float amount)
	{
		float satietyCurrent = ManagerBase<PlayerManager>.Instance.satietyCurrent;
		ManagerBase<PlayerManager>.Instance.satietyCurrent = Mathf.Clamp(ManagerBase<PlayerManager>.Instance.satietyCurrent + amount, 0f, ManagerBase<PlayerManager>.Instance.SatietyMaximum);
		satietyCurrent = ManagerBase<PlayerManager>.Instance.satietyCurrent - satietyCurrent;
		PlayerEating.changeSatietyEvent?.Invoke();
		return satietyCurrent;
	}

	public bool CanDrink()
	{
		if (HaveNearWater)
		{
			return ManagerBase<PlayerManager>.Instance.HaveThirst;
		}
		return false;
	}

	public void Drink(Action endDrinkingCallback)
	{
		if (CanDrink())
		{
			if (PlayerPicker.HavePickedItem)
			{
				PlayerPicker.PickedItem.MeshRenderer.enabled = false;
			}
			List<(Water, Vector3, float)> list = new List<(Water, Vector3, float)>();
			foreach (Water nearWaterObj in nearWaterObjs)
			{
				Vector3 vector = nearWaterObj.Collider.ClosestPoint(MouthDrinkPos.position);
				list.Add((nearWaterObj, vector, Vector3.ProjectOnPlane(MouthDrinkPos.position - vector, Vector3.up).sqrMagnitude));
			}
			(Water, Vector3, float) valueTuple;
			if (((IEnumerable<(Water, Vector3, float)>)list).Any((Func<(Water, Vector3, float), bool>)(((Water water, Vector3 closestPoint, float sqrDistance) x) => x.sqrDistance == 0f)))
			{
				list.RemoveAll((Predicate<(Water, Vector3, float)>)(((Water water, Vector3 closestPoint, float sqrDistance) x) => x.sqrDistance != 0f));
				valueTuple = list.Random();
			}
			else
			{
				list.Sort((Comparison<(Water, Vector3, float)>)(((Water water, Vector3 closestPoint, float sqrDistance) x, (Water water, Vector3 closestPoint, float sqrDistance) y) => x.sqrDistance.CompareTo(y.sqrDistance)));
				valueTuple = list[0];
			}
			bool haveDrinkPoints = valueTuple.Item1.HaveDrinkPoints;
			this.endDrinkingCallback = endDrinkingCallback;
			feedingCor = StartCoroutine(Drinking(valueTuple.Item1, haveDrinkPoints, valueTuple.Item2));
			Vector3 zero = Vector3.zero;
			zero = ((valueTuple.Item3 != 0f) ? Vector3.ProjectOnPlane(valueTuple.Item2 - MouthDrinkPos.position, Vector3.up) : base.transform.forward);
			PlayerSpawner.PlayerInstance.PlayerFamilyController.Drink(zero, valueTuple.Item1);
		}
		else
		{
			endDrinkingCallback?.Invoke();
		}
	}

	public void CancelDrink()
	{
		StartCoroutine(CompletingDrink(instant: true));
	}

	private IEnumerator Drinking(Water water, bool rotateToWaterCenter, Vector3 closestWaterPoint)
	{
		isDrinking = true;
		int startThirstParts = ManagerBase<PlayerManager>.Instance.ThirstParts;
		PlayerSpawner.PlayerInstance.PlayerMovement.NavMeshObstacle.carving = true;
		Vector3 forward = base.transform.forward;
		Vector3 endForward;
		if (rotateToWaterCenter)
		{
			endForward = Vector3.ProjectOnPlane(water.Collider.transform.position - base.transform.position, Vector3.up);
		}
		else
		{
			endForward = Vector3.ProjectOnPlane(closestWaterPoint, Vector3.up) - Vector3.ProjectOnPlane(MouthDrinkPos.position, Vector3.up);
			Vector3 point = MouthDrinkPos.position - base.transform.position;
			Quaternion rotation = Quaternion.FromToRotation(base.transform.forward, endForward);
			Vector3 vector = base.transform.position + rotation * point;
			if (!(water.Collider.ClosestPoint(vector) == vector))
			{
				endForward = Vector3.ProjectOnPlane(closestWaterPoint, Vector3.up) - Vector3.ProjectOnPlane(base.transform.position, Vector3.up);
			}
		}
		Vector3.Angle(base.transform.forward, endForward);
		while (PlayerSpawner.PlayerInstance.PlayerMovement.Rotate(endForward, Time.deltaTime, ManagerBase<PlayerManager>.Instance.EatDrinkRotationSpeed))
		{
			yield return null;
		}
		Animator.SetTrigger("Drink");
		Animator.SetBool("isDrinking", value: true);
		yield return new WaitUntil(() => Animator.IsPlayingByName(AnimationHashes.drinkEnter.Value));
		yield return new WaitUntil(() => Animator.IsPlayingByTag(AnimationHashes.drink.Value) ? (!Animator.IsPlayingByName(AnimationHashes.drinkEnter.Value)) : true);
		quenchThirstCount = 0;
		NormalizedDrinkTime = 0f;
		while (!ManagerBase<PlayerManager>.Instance.ThirstIsMaximum)
		{
			float NormalizedDrinkIterationTime = 0f;
			while (NormalizedDrinkIterationTime < 1f)
			{
				NormalizedDrinkIterationTime = Mathf.Clamp(NormalizedDrinkIterationTime + Time.deltaTime / ManagerBase<PlayerManager>.Instance.DrinkingDuration, 0f, 1f);
				NormalizedDrinkTime = Mathf.Clamp(NormalizedDrinkTime + Time.deltaTime / ManagerBase<PlayerManager>.Instance.GetDrinkTimeNeeded(startThirstParts), 0f, 1f);
				yield return null;
			}
			ChangeThirst(_003CDrinking_003Eg__GetThirstIncValue_007C114_2());
			this.endDrinkIterationEvent?.Invoke();
			quenchThirstCount++;
		}
		yield return StartCoroutine(CompletingDrink());
	}

	private IEnumerator CompletingDrink(bool instant = false)
	{
		if (isDrinking)
		{
			PlayerSpawner.PlayerInstance.PlayerMovement.NavMeshObstacle.carving = false;
			if (feedingCor != null)
			{
				StopCoroutine(feedingCor);
			}
			feedingCor = null;
			Animator.SetBool("isDrinking", value: false);
			isDrinking = false;
			NormalizedDrinkTime = 0f;
			this.endDrinkingEvent?.Invoke(quenchThirstCount);
			if (!instant)
			{
				this.compleateDrinkEvent?.Invoke();
			}
			endDrinkingCallback?.Invoke();
			endDrinkingCallback = null;
		}
		if (PlayerPicker.HavePickedItem)
		{
			PlayerPicker.PickedItem.MeshRenderer.enabled = true;
		}
		yield break;
	}

	public void ChangeThirst(float amount)
	{
		ManagerBase<PlayerManager>.Instance.thirstCurrent = Mathf.Clamp(ManagerBase<PlayerManager>.Instance.thirstCurrent + amount, 0f, ManagerBase<PlayerManager>.Instance.ThirstMaximum);
		PlayerEating.changeThirstEvent?.Invoke();
	}

	private float GetThirstIncValue()
	{
		float num = ManagerBase<PlayerManager>.Instance.thirstCurrent / ManagerBase<PlayerManager>.Instance.ThirstEffect;
		int num2 = (num != 1f) ? (Mathf.FloorToInt(num) + 2) : (Mathf.FloorToInt(num) + 1);
		return ManagerBase<PlayerManager>.Instance.ThirstEffect * (float)num2 - ManagerBase<PlayerManager>.Instance.thirstCurrent;
	}

	[CompilerGenerated]
	private static float _003CDrinking_003Eg__GetThirstIncValue_007C114_2()
	{
		float num = ManagerBase<PlayerManager>.Instance.thirstCurrent / ManagerBase<PlayerManager>.Instance.ThirstEffect;
		int num2 = (num % 1f != 0f) ? (Mathf.FloorToInt(num) + 2) : (Mathf.FloorToInt(num) + 1);
		return ManagerBase<PlayerManager>.Instance.ThirstEffect * (float)num2 - ManagerBase<PlayerManager>.Instance.thirstCurrent;
	}
}
