using Avelog;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FamilyManager : ManagerBase<FamilyManager>
{
	public enum FamilyMemberRole
	{
		FirstStageChild,
		SecondStageChild,
		ThirdStageChild,
		Spouse
	}

	public enum GenderType
	{
		Male,
		Female
	}

	[Serializable]
	public class FamilyMemberData
	{
		public string name = "";

		public FamilyMemberRole role;

		public GenderType genderType = GenderType.Female;

		public float experience;

		public float thirst;

		public string curNeed;

		[NonSerialized]
		private FamilyMemberParams _familyMemberParams;

		public FamilyMemberParams Params
		{
			get
			{
				if (_familyMemberParams == null || _familyMemberParams.Type != role)
				{
					_familyMemberParams = ManagerBase<FamilyManager>.Instance.FamilyMembersParams.Find((FamilyMemberParams x) => x.Type == role);
				}
				return _familyMemberParams;
			}
		}

		public float CurScalePart
		{
			get
			{
				if (role != FamilyMemberRole.Spouse && role != FamilyMemberRole.ThirdStageChild)
				{
					FamilyMemberRole nextStage = (role == FamilyMemberRole.FirstStageChild) ? FamilyMemberRole.SecondStageChild : FamilyMemberRole.ThirdStageChild;
					FamilyMemberParams familyMemberParams = ManagerBase<FamilyManager>.Instance.FamilyMembersParams.Find((FamilyMemberParams x) => x.Type == nextStage);
					FamilyMemberParams familyMemberParams2 = ManagerBase<FamilyManager>.Instance.FamilyMembersParams.Find((FamilyMemberParams x) => x.Type == role);
					float t = Mathf.Clamp(experience / familyMemberParams2.LevelUpExperience, 0f, 1f);
					return Mathf.Lerp(Params.ModelScalePerc, familyMemberParams.ModelScalePerc, t) / 100f;
				}
				return Params.ModelScalePerc / 100f;
			}
		}

		public float HitPowerPerc
		{
			get
			{
				if (role != FamilyMemberRole.Spouse && role != FamilyMemberRole.ThirdStageChild)
				{
					FamilyMemberRole nextStage = (role == FamilyMemberRole.FirstStageChild) ? FamilyMemberRole.SecondStageChild : FamilyMemberRole.ThirdStageChild;
					FamilyMemberParams familyMemberParams = ManagerBase<FamilyManager>.Instance.FamilyMembersParams.Find((FamilyMemberParams x) => x.Type == nextStage);
					FamilyMemberParams familyMemberParams2 = ManagerBase<FamilyManager>.Instance.FamilyMembersParams.Find((FamilyMemberParams x) => x.Type == role);
					float t = Mathf.Clamp(experience / familyMemberParams2.LevelUpExperience, 0f, 1f);
					return Mathf.Lerp(Params.HitPowerPerc, familyMemberParams.HitPowerPerc, t) / 100f;
				}
				return Params.HitPowerPerc / 100f;
			}
		}

		public float CurHitPower => HitPowerPerc * ManagerBase<PlayerManager>.Instance.HitPower * ManagerBase<FamilyManager>.Instance.AttackPowerPercentFromSatiety / 100f;

		public bool IsMaxStage()
		{
			if (role != FamilyMemberRole.Spouse)
			{
				return role == FamilyMemberRole.ThirdStageChild;
			}
			return true;
		}
	}

	[Serializable]
	public class FamilyMemberParams
	{
		[SerializeField]
		private FamilyMemberRole type;

		[Postfix(PostfixAttribute.Id.Percents)]
		[SerializeField]
		private float hitPower;

		[Postfix(PostfixAttribute.Id.Percents)]
		[SerializeField]
		private float satietyMaximum;

		[Postfix(PostfixAttribute.Id.Percents)]
		[SerializeField]
		private float modelScale = 100f;

		[SerializeField]
		private float modelYOffset;

		[Postfix(PostfixAttribute.Id.Percents)]
		[SerializeField]
		private float levelUpExperiencePerc = 100f;

		public List<string> needs = new List<string>();

		[SerializeField]
		private Vector3 worldPanelOffset = Vector3.zero;

		[Header("Параметры прогулки")]
		[SerializeField]
		private float speedLow = 5f;

		[SerializeField]
		private float speedMedium = 10f;

		[Postfix(PostfixAttribute.Id.Percents)]
		[SerializeField]
		private float standartIdleChance = 30f;

		[SerializeField]
		[Postfix(PostfixAttribute.Id.Seconds)]
		private float standartIdleMinimum = 5f;

		[SerializeField]
		[Postfix(PostfixAttribute.Id.Seconds)]
		private float standartIdleMaximum = 10f;

		[Postfix(PostfixAttribute.Id.Percents)]
		[SerializeField]
		private float sittingIdleChance = 30f;

		[Postfix(PostfixAttribute.Id.Seconds)]
		[SerializeField]
		private float sittingIdleMinimum = 10f;

		[Postfix(PostfixAttribute.Id.Seconds)]
		[SerializeField]
		private float sittingIdleMaximum = 30f;

		[Postfix(PostfixAttribute.Id.Percents)]
		[SerializeField]
		private float restIdleChance = 30f;

		[Postfix(PostfixAttribute.Id.Seconds)]
		[SerializeField]
		private float restIdleMinimum = 15f;

		[Postfix(PostfixAttribute.Id.Seconds)]
		[SerializeField]
		private float restIdleMaximum = 15f;

		public FamilyMemberRole Type => type;

		public float HitPowerPerc => hitPower;

		public float SatietyMaximumPerc => satietyMaximum;

		public float ModelScalePerc => modelScale;

		public float ModelYOffset => modelYOffset;

		public float LevelUpExperience => ManagerBase<FamilyManager>.Instance.LevelUpExperience / 100f * levelUpExperiencePerc;

		public Vector3 WorldPanelOffset => worldPanelOffset;

		public float SpeedLow => speedLow;

		public float SpeedMedium => speedMedium;

		public float StandartIdleChance => standartIdleChance;

		public float StandartIdleMinimum => standartIdleMinimum;

		public float StandartIdleMaximum => standartIdleMaximum;

		public float SittingIdleChance => sittingIdleChance;

		public float SittingIdleMinimum => sittingIdleMinimum;

		public float SittingIdleMaximum => sittingIdleMaximum;

		public float RestIdleChance => restIdleChance;

		public float RestIdleMinimum => restIdleMinimum;

		public float RestIdleMaximum => restIdleMaximum;
	}

	public delegate void FamilyMemberEventHandler(FamilyMemberData familyMemberData);

	public delegate void GotExperienceHandler(FamilyMemberData familyMemberData, float experience, float experiencePercent);

	[SerializeField]
	private int spouseLevelNeed = 10;

	[SerializeField]
	private int childLevelNeed = 20;

	[SerializeField]
	private int maxChilds = 3;

	[SerializeField]
	private int childCost = 100;

	[SerializeField]
	private int childCostInc = 50;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Seconds)]
	private float demandCooldown = 10f;

	[SerializeField]
	private float demandExpMulty;

	[SerializeField]
	[Range(0f, 1f)]
	private float huntDemandChance = 0.5f;

	[SerializeField]
	[Range(0f, 1f)]
	private float eatDemandChance = 0.5f;

	[SerializeField]
	private List<FamilyMemberParams> familyMembersParams = new List<FamilyMemberParams>();

	[Header("Сопровождение + прогулка")]
	[SerializeField]
	private float acceleration = 40f;

	[SerializeField]
	private float rotationSpeed = 200f;

	[Postfix(PostfixAttribute.Id.Percents)]
	[SerializeField]
	private float corneringSlowing = 90f;

	[SerializeField]
	private float addSpeedMaximum = 10f;

	[SerializeField]
	private float followDistanceMaximum = 10f;

	[SerializeField]
	private float walkDistanceMaximum = 10f;

	[Postfix(PostfixAttribute.Id.Seconds)]
	[SerializeField]
	private float jumpTime = 0.5f;

	[SerializeField]
	private float walkSpaceMaximum = 10f;

	[SerializeField]
	private float followAvgSpeed;

	[Postfix(PostfixAttribute.Id.Seconds)]
	[SerializeField]
	private float followTimer;

	[Postfix(PostfixAttribute.Id.Seconds)]
	[SerializeField]
	private float followPauseTime;

	[Header("Атака")]
	[Postfix(PostfixAttribute.Id.Seconds)]
	[SerializeField]
	private float counterAttackTimer = 4f;

	[Postfix(PostfixAttribute.Id.Seconds)]
	[SerializeField]
	private float hitFrequency = 1f;

	[Postfix(PostfixAttribute.Id.Percents)]
	[SerializeField]
	private float hitRegistrationAnimPerc = 30f;

	[SerializeField]
	private float pursuitRadius = 15f;

	[SerializeField]
	private float attackRange = 3f;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private int attackPowerFallStep = 20;

	[Tooltip("Время лерпа позиции преследования (от позиции врага до позиции за игроком)")]
	[SerializeField]
	private float pursuitDistanceMaximum = 5f;

	[SerializeField]
	private float pursuitDecceleration = 5f;

	[Header("Прием пищи")]
	[Postfix(PostfixAttribute.Id.Seconds)]
	[SerializeField]
	private float eatingDuration = 1f;

	[SerializeField]
	private float eatingDistance = 2f;

	[SerializeField]
	private float childEatingExperience = 10f;

	[Postfix(PostfixAttribute.Id.Seconds)]
	[SerializeField]
	private float drinkingDuration = 1f;

	private float drinkDistanceFromPlayer = 3f;

	[SerializeField]
	private float childThirstExperience = 5f;

	[SerializeField]
	private float playerThirstExperience = 10f;

	[Header("Отладка")]
	[SerializeField]
	[Save]
	public float familySatiety;

	[Save]
	public List<FamilyMemberData> family = new List<FamilyMemberData>();

	public int SpouseLevelNeed => spouseLevelNeed;

	public int ChildLevelNeed => childLevelNeed;

	public int MaxChilds => maxChilds;

	public int ChildCost => childCost + childCostInc * Childs.Count();

	public float LevelUpExperience => ManagerBase<PlayerManager>.Instance.LevelUpExperience;

	public float DemandCooldown => demandCooldown;

	public float DemandExpMulty => demandExpMulty;

	public float HuntDemandChance => huntDemandChance;

	public float EatDemandChance => eatDemandChance;

	public List<FamilyMemberParams> FamilyMembersParams => familyMembersParams;

	public float Acceleration => acceleration;

	public float RotationSpeed => rotationSpeed;

	public float CorneringSlowing => corneringSlowing;

	public float AddSpeedMaximum => addSpeedMaximum;

	public float FollowDistanceMaximum => followDistanceMaximum;

	public float WalkDistanceMaximum => walkDistanceMaximum;

	public float JumpTime => jumpTime;

	public float WalkSpaceMaximum => walkSpaceMaximum;

	public float FollowAvgSpeed => followAvgSpeed;

	public float FollowTimer => followTimer;

	public float FollowPauseTime => followPauseTime;

	public float CounterAttackTimer => counterAttackTimer;

	public float HitFrequency => hitFrequency;

	public float HitRegistrationAnimNormalizedTime => hitRegistrationAnimPerc / 100f;

	public float PursuitRadius => pursuitRadius;

	public float AttackRange => attackRange;

	public float PursuitDistanceMaximum => pursuitDistanceMaximum;

	public float PursuitDecceleration => pursuitDecceleration;

	public float EatingDuration
	{
		get
		{
			if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Pixibob)
			{
				float num = 100f / eatingDuration;
				float num2 = CalculationsHelpUtils.CalculateProp(num, ManagerBase<SkinManager>.Instance.PixibobEatingSpeedBonus, 100f);
				num += num2;
				return 100f / num;
			}
			return eatingDuration;
		}
	}

	public float EatingDistance => eatingDistance;

	public float СhildEatingExperience => childEatingExperience;

	public float DrinkingDuration
	{
		get
		{
			if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Egyptian)
			{
				float num = 100f / drinkingDuration;
				float num2 = CalculationsHelpUtils.CalculateProp(num, ManagerBase<SkinManager>.Instance.EgyptianDrinkSpeedBonus, 100f);
				num += num2;
				return 100f / num;
			}
			return drinkingDuration;
		}
	}

	public float DrinkDistanceFromPlayer => drinkDistanceFromPlayer;

	public float ChildThirstExperience => childThirstExperience;

	public float PlayerThirstExperience => playerThirstExperience;

	public float ThirstMaximum => ManagerBase<PlayerManager>.Instance.ThirstMaximum;

	public float SatietyFall => ManagerBase<PlayerManager>.Instance.SatietyFall;

	public float ThirstFall => ManagerBase<PlayerManager>.Instance.ThirstFall;

	public float FamilySatietyMaximum
	{
		get
		{
			float num = 0f;
			foreach (FamilyMemberData item in family)
			{
				num += item.Params.SatietyMaximumPerc / 100f * ManagerBase<PlayerManager>.Instance.SatietyMaximum;
			}
			return num;
		}
	}

	public float SatietyPart => familySatiety / FamilySatietyMaximum;

	public float AttackPowerPercentFromSatiety => Mathf.CeilToInt(SatietyPart * 100f / (float)attackPowerFallStep) * attackPowerFallStep;

	public bool HaveSpouse => family.Any((FamilyMemberData x) => x.role == FamilyMemberRole.Spouse);

	public FamilyMemberData GrowingChild => family.Find((FamilyMemberData x) => (x.role != 0) ? (x.role == FamilyMemberRole.SecondStageChild) : true);

	public FamilyMemberRole GrowingChildNextStage
	{
		get
		{
			FamilyMemberData growingChild = GrowingChild;
			if (growingChild != null)
			{
				if (growingChild.role == FamilyMemberRole.FirstStageChild)
				{
					return FamilyMemberRole.SecondStageChild;
				}
				FamilyMemberRole role = growingChild.role;
				return FamilyMemberRole.ThirdStageChild;
			}
			return FamilyMemberRole.FirstStageChild;
		}
	}

	public bool HaveGrowingChild => GrowingChild != null;

	public bool HaveFamily => family.Count > 0;

	public IEnumerable<FamilyMemberData> Childs => from x in family
		where x.role != FamilyMemberRole.Spouse
		select x;

	public bool HaveChilds
	{
		get
		{
			if (Childs != null)
			{
				return Childs.Count() > 0;
			}
			return false;
		}
	}

	public static event FamilyMemberEventHandler stageUpEvent;

	public static event GotExperienceHandler gotExperienceEvent;

	protected override void OnInit()
	{
	}

	public void AddExperience(float experience, FamilyMemberData familyMemberData)
	{
		if (familyMemberData.IsMaxStage())
		{
			return;
		}
		float num = 0f;
		float num2 = experience;
		do
		{
			float num3 = familyMemberData.Params.LevelUpExperience - familyMemberData.experience;
			if (num2 >= num3)
			{
				num += num3 / familyMemberData.Params.LevelUpExperience;
				familyMemberData.experience = 0f;
				num2 -= num3;
				if (familyMemberData.role == FamilyMemberRole.FirstStageChild)
				{
					familyMemberData.role = FamilyMemberRole.SecondStageChild;
				}
				else if (familyMemberData.role == FamilyMemberRole.SecondStageChild)
				{
					familyMemberData.role = FamilyMemberRole.ThirdStageChild;
				}
				FamilyManager.stageUpEvent?.Invoke(familyMemberData);
			}
			else
			{
				num += num2 / familyMemberData.Params.LevelUpExperience;
				familyMemberData.experience += num2;
				num2 = 0f;
			}
		}
		while (num2 > 0f && !familyMemberData.IsMaxStage());
		num *= 100f;
		FamilyManager.gotExperienceEvent?.Invoke(familyMemberData, experience, num);
	}
}
