using UnityEngine;

public class PlayerExperienceController : MonoBehaviour, IInitializablePlayerComponent
{
	private PlayerCombat PlayerCombat => PlayerSpawner.PlayerInstance.PlayerCombat;

	private PlayerEating PlayerEating => PlayerSpawner.PlayerInstance.PlayerEating;

	private PlayerFamilyController PlayerFamilyController => PlayerSpawner.PlayerInstance.PlayerFamilyController;

	public void Initialize()
	{
		ActorCombat.killEvent += OnKill;
		PlayerEating.endEatingEvent += OnEndEating;
		PlayerEating.endDrinkingEvent += OnEndDrinking;
		PlayerFamilyController.familyMemberEndEatEvent += OnFamilyMemberEndEat;
		FamilyMemberController.endDrinkEvent += OnFamilyMemberControllerEndDrink;
		VegetableBehaviour.plantEvent += OnPlant;
		Clew.pickEvent += OnClewPick;
		Quest.completeEvent += OnCompleteQuest;
	}

	private void OnDestroy()
	{
		ActorCombat.killEvent -= OnKill;
		PlayerEating.endEatingEvent -= OnEndEating;
		PlayerEating.endDrinkingEvent -= OnEndDrinking;
		PlayerFamilyController.familyMemberEndEatEvent -= OnFamilyMemberEndEat;
		FamilyMemberController.endDrinkEvent -= OnFamilyMemberControllerEndDrink;
		Quest.completeEvent -= OnCompleteQuest;
		VegetableBehaviour.plantEvent -= OnPlant;
		Clew.pickEvent -= OnClewPick;
	}

	private void OnKill(ActorCombat killer, ActorCombat target)
	{
		if (killer is PlayerCombat || killer is FamilyMemberCombat)
		{
			EnemyController componentInParent = target.gameObject.GetComponentInParent<EnemyController>();
			ManagerBase<PlayerManager>.Instance.AddExperience(componentInParent.GetExperienceForKill());
		}
	}

	private void OnEndEating(float satietyInc, float foodEffect)
	{
		ManagerBase<PlayerManager>.Instance.AddExperience(satietyInc);
	}

	private void OnEndDrinking(int quenchThirstCount)
	{
		ManagerBase<PlayerManager>.Instance.AddExperience(ManagerBase<PlayerManager>.Instance.ThirstExperience * (float)quenchThirstCount);
	}

	private void OnFamilyMemberEndEat(float satietyInc, float foodEffect)
	{
		ManagerBase<PlayerManager>.Instance.AddExperience(satietyInc);
	}

	private void OnFamilyMemberControllerEndDrink(FamilyMemberController familyMemberController, int quenchThirstCount)
	{
		ManagerBase<PlayerManager>.Instance.AddExperience(ManagerBase<FamilyManager>.Instance.PlayerThirstExperience * (float)quenchThirstCount);
	}

	private void OnPlant(VegetableBehaviour vegetable)
	{
		ManagerBase<PlayerManager>.Instance.AddExperience(ManagerBase<PlayerManager>.Instance.PlantExperience);
	}

	private void OnFamilyMemberEndDrinkIteration()
	{
	}

	private void OnCompleteQuest(Quest completedQuest)
	{
		ManagerBase<PlayerManager>.Instance.AddExperience(completedQuest.Reward.Experience);
	}

	private void OnClewPick()
	{
		ManagerBase<PlayerManager>.Instance.AddExperience(ManagerBase<ClewManager>.Instance.ExpiriencePerPick);
	}
}
