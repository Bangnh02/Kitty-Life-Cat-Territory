using System;
using UnityEngine;

public class HelpHintsController : MonoBehaviour
{
	public delegate void NeedShowHintHandler(HelpManager.Hint hint);

	private float timer;

	[NonSerialized]
	private bool isClewPicked;

	private HelpManager HelpManager => ManagerBase<HelpManager>.Instance;

	private PlayerBrain Player => PlayerSpawner.PlayerInstance;

	public static event NeedShowHintHandler needShowHintEvent;

	private void Start()
	{
		if (!HelpManager.IsAllHintShowed)
		{
			PlayerFamilyController.addFamilyMemberEvent += OnAddFamilyMember;
			FamilyManager.stageUpEvent += OnFamilyMemberStageUp;
			PlayerPlanter.updateNearPotentialPlantEvent += OnUpdateNearPotentialPlant;
			PlayerSleepController.changeCanSleepStateEvent += OnChangeCanSleepState;
			PlayerCombat.InvisibilityAbility.switchInvisibilityEvent += OnSwitchInvisibility;
			PlayerFarmResident.updateNearFarmResidentsEvent += OnUpdateNearFarmResidents;
			SceneController.changeActiveSceneEvent += OnChangeActiveScene;
			Clew.pickEvent += OnClewPick;
		}
		if (!HelpManager.welcomeHintShowed)
		{
			OnChangeActiveScene(SceneController.Instance.CurActiveScene);
		}
	}

	private void OnDestroy()
	{
		PlayerFamilyController.addFamilyMemberEvent -= OnAddFamilyMember;
		FamilyManager.stageUpEvent -= OnFamilyMemberStageUp;
		PlayerPlanter.updateNearPotentialPlantEvent -= OnUpdateNearPotentialPlant;
		PlayerSleepController.changeCanSleepStateEvent -= OnChangeCanSleepState;
		PlayerCombat.InvisibilityAbility.switchInvisibilityEvent -= OnSwitchInvisibility;
		PlayerFarmResident.updateNearFarmResidentsEvent -= OnUpdateNearFarmResidents;
		SceneController.changeActiveSceneEvent -= OnChangeActiveScene;
		Clew.pickEvent -= OnClewPick;
	}

	private void OnAddFamilyMember(FamilyManager.FamilyMemberData familyMember)
	{
		if (familyMember.role == FamilyManager.FamilyMemberRole.Spouse)
		{
			HelpHintsController.needShowHintEvent?.Invoke(HelpManager.Hint.HelpHintSpouse);
		}
		else if (familyMember.role == FamilyManager.FamilyMemberRole.FirstStageChild && !HelpManager.childFirstStageHintShowed)
		{
			HelpManager.childFirstStageHintShowed = true;
			HelpHintsController.needShowHintEvent?.Invoke(HelpManager.Hint.HelpHintChild1);
		}
	}

	private void OnFamilyMemberStageUp(FamilyManager.FamilyMemberData familyMemberData)
	{
		if (familyMemberData.role == FamilyManager.FamilyMemberRole.ThirdStageChild && !HelpManager.childThirdStageHintShowed)
		{
			HelpManager.childThirdStageHintShowed = true;
			HelpHintsController.needShowHintEvent?.Invoke(HelpManager.Hint.HelpHintChild3);
		}
	}

	private void OnUpdateNearPotentialPlant()
	{
		timer = HelpManager.GardenHintDelay;
	}

	private void OnChangeCanSleepState()
	{
		timer = HelpManager.SleepHintDelay;
	}

	private void OnSwitchInvisibility(bool state)
	{
		timer = HelpManager.StealthHintDelay;
	}

	private void OnUpdateNearFarmResidents()
	{
		timer = HelpManager.ResidentsHintDelay;
	}

	private void OnClewPick()
	{
		isClewPicked = true;
		timer = HelpManager.ClewHintDelay;
	}

	private void OnChangeActiveScene(SceneController.SceneType newActiveScene)
	{
		if (newActiveScene == SceneController.SceneType.Game)
		{
			timer = HelpManager.WelcomeHintDelay;
		}
	}

	private void Update()
	{
		if (SceneController.Instance.CurActiveScene != SceneController.SceneType.Game || HelpManager.IsAllHintShowed || GameBlockingPanel.Instance.gameObject.activeSelf)
		{
			return;
		}
		if (!HelpManager.welcomeHintShowed)
		{
			if (timer <= 0f)
			{
				HelpManager.welcomeHintShowed = true;
				HelpHintsController.needShowHintEvent?.Invoke(HelpManager.Hint.HelpHintWelcome);
			}
			timer -= Time.deltaTime;
		}
		if (!Player.PlayerCombat.InCombat && ManagerBase<FamilyManager>.Instance.HaveFamily && ManagerBase<FamilyManager>.Instance.familySatiety == 0f && !HelpManager.familyParamZeroHintShowed)
		{
			HelpManager.familyParamZeroHintShowed = true;
			HelpHintsController.needShowHintEvent?.Invoke(HelpManager.Hint.HelpHintFamilyParamZero);
		}
		if (Player.PlayerPlanter.HaveNearPotentialPlants && !HelpManager.gardenHintShowed)
		{
			if (timer <= 0f)
			{
				HelpManager.gardenHintShowed = true;
				HelpHintsController.needShowHintEvent?.Invoke(HelpManager.Hint.HelpHintGarden);
			}
			timer -= Time.deltaTime;
		}
		if (!Player.PlayerCombat.InCombat && (ManagerBase<PlayerManager>.Instance.satietyCurrent == 0f || ManagerBase<PlayerManager>.Instance.thirstCurrent == 0f) && !HelpManager.paramZeroHintShowed)
		{
			HelpManager.paramZeroHintShowed = true;
			HelpHintsController.needShowHintEvent?.Invoke(HelpManager.Hint.HelpHintParamZero);
		}
		if (Player.PlayerSleepController.CanSleep() && !HelpManager.sleepHintShowed)
		{
			if (timer <= 0f)
			{
				HelpManager.sleepHintShowed = true;
				HelpHintsController.needShowHintEvent?.Invoke(HelpManager.Hint.HelpHintSleep);
			}
			timer -= Time.deltaTime;
		}
		if (Player.PlayerCombat.IsInvisibilityActive() && !HelpManager.stealthHintShowed)
		{
			if (timer <= 0f)
			{
				HelpManager.stealthHintShowed = true;
				HelpHintsController.needShowHintEvent?.Invoke(HelpManager.Hint.HelpHintStealth);
			}
			timer -= Time.deltaTime;
		}
		if (Player.PlayerFarmResident.HaveNearFarmResident() && !HelpManager.farmResidentHintShowed)
		{
			if (timer <= 0f)
			{
				HelpManager.farmResidentHintShowed = true;
				HelpHintsController.needShowHintEvent?.Invoke(HelpManager.Hint.HelpHintResidents);
			}
			timer -= Time.deltaTime;
		}
		if (isClewPicked && !HelpManager.clewHintShowed)
		{
			if (timer <= 0f)
			{
				HelpManager.clewHintShowed = true;
				HelpHintsController.needShowHintEvent?.Invoke(HelpManager.Hint.HelpHintClew);
			}
			timer -= Time.deltaTime;
		}
	}
}
