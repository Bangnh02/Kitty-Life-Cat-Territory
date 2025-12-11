using Avelog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerManager : ManagerBase<PlayerManager>
{
	[Serializable]
	public class SurfaceTag
	{
		public string tag;

		[Tooltip("Должен ли игрок выравниваться относительно поверхности с данным тэгом")]
		public bool align;

		[Tooltip("Максимальный угол на который будет произведено выравнивание относительно поверхности с данным тэгом")]
		[SerializeField]
		private float alignMaxAngle;

		[Tooltip("Должен ли игрок скользить по поверхности с данным тэгом")]
		public bool slide;

		[Tooltip("Минимальный угол между нормалью поверхности и направлением вверх, при котором игрок начинает скользить")]
		[SerializeField]
		private float slideAngleMinimum;

		[SerializeField]
		private float slideSpeed;

		[Tooltip("Должен ли игрок замедляться на поверхности с данным тэгом")]
		public bool slowing;

		[Tooltip("Сила замедления на поверхности с данным тэгом")]
		[Range(0f, 1f)]
		[SerializeField]
		private float slowingPart;

		public float AlignMaxAngle
		{
			get
			{
				if (!align)
				{
					return 0f;
				}
				return alignMaxAngle;
			}
		}

		public float SlideAngleMinimum
		{
			get
			{
				if (!slide)
				{
					return float.MaxValue;
				}
				return slideAngleMinimum;
			}
		}

		public float SlideSpeed
		{
			get
			{
				if (!slide)
				{
					return 0f;
				}
				return slideSpeed;
			}
		}

		public float SlowingPart
		{
			get
			{
				if (!slowing)
				{
					return 0f;
				}
				return slowingPart;
			}
		}
	}

	public delegate void CoinsUpdateHandler(int coinsInc = 0);

	public delegate void ExperienceUpdateHandler(float experience, int levelUps);

	[Header("Движение")]
	[ReadonlyInspector]
	public float curSpeed;

	[SerializeField]
	private float speedMinimum = 10f;

	[SerializeField]
	private float speedMedium = 20f;

	[SerializeField]
	private float speedMaximum = 30f;

	[SerializeField]
	private float speedBackward = -5f;

	[SerializeField]
	private float speedAcceleration = 10f;

	[SerializeField]
	private float movementLimit = 50f;

	[SerializeField]
	private float rotationSpeed = 100f;

	[SerializeField]
	private float eatDrinkRotationSpeed = 120f;

	[SerializeField]
	private float jumpPower = 70f;

	[SerializeField]
	private float jumpPowerLimit = 100f;

	[SerializeField]
	private float jumpSpeedBoost = 10f;

	[Postfix(PostfixAttribute.Id.Seconds)]
	[SerializeField]
	private float jumpFrequency = 0.3f;

	[SerializeField]
	private float gravity = 130f;

	[SerializeField]
	private List<SurfaceTag> surfaceTags;

	[Tooltip("Время смены анимации стандартного простоя на анимацию сидения")]
	[Postfix(PostfixAttribute.Id.Seconds)]
	[SerializeField]
	private float switchToSittingTime = 4f;

	[Tooltip("Время смены анимации стандартного простоя на анимацию лежания")]
	[Postfix(PostfixAttribute.Id.Seconds)]
	[SerializeField]
	private float switchToRestTime = 6f;

	[Tooltip("Интерполяция поворота граф. модели к повороту физ. модели")]
	[SerializeField]
	private float graphicRotLerp = 300f;

	[Tooltip("Интерполяция выравнивания (относительно земли) граф. модели к выравниванию физ. модели")]
	[SerializeField]
	private float graphicAlignLerp = 10f;

	[SerializeField]
	[Separator("Ограничение скорости при преследовании", SeparatorAttribute.Colors.WHITE, 20f, false)]
	private float speedLimitStartingDistance = 30f;

	[SerializeField]
	private float speedLimitFinalDistance = 5f;

	[SerializeField]
	[Separator("Помощь в навигации при преследовании", SeparatorAttribute.Colors.WHITE, 20f, false)]
	private bool useRaycastToCheckObstacles = true;

	[SerializeField]
	private float maxDistancePursuitAssistance = 50f;

	[SerializeField]
	private float minDistancePursuitAssistance = 10f;

	[SerializeField]
	private float maxDistAnglePursuitAssistance = 25f;

	[SerializeField]
	private float minDistAnglePursuitAssistance = 50f;

	[Header("Жизненные")]
	[Tooltip("Частота сна")]
	[SerializeField]
	[Postfix(PostfixAttribute.Id.Minutes)]
	private float sleepFrequency = 13f;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.PBS)]
	private FloatPBS health;

	private float prevHealthMaximum;

	[SerializeField]
	private float satiety = 100f;

	[SerializeField]
	private float thirst = 100f;

	[Postfix(PostfixAttribute.Id.Seconds)]
	[SerializeField]
	private float eatingDuration = 1f;

	[Postfix(PostfixAttribute.Id.Seconds)]
	[SerializeField]
	private float drinkingDuration = 1f;

	[Header("Жизненные (Отладка)")]
	[Save]
	public float healthCurrent = 100f;

	[Save]
	public float satietyCurrent = 100f;

	[Save]
	public float thirstCurrent = 100f;

	[Save]
	public float sleepTimer;

	[Save]
	public bool isSleeping;

	[Header("Регуляторы")]
	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	[Tooltip("Процент от максимального здоровья")]
	private float healthFall = 1f;

	[SerializeField]
	private float satietyFall = 1f;

	[SerializeField]
	private float thirstFall = 1f;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private float satietyHitFall = 0.5f;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private float thirstHitFall = 0.5f;

	[Postfix(PostfixAttribute.Id.Percents)]
	[SerializeField]
	private float healHealth = 1f;

	[Postfix(PostfixAttribute.Id.Percents)]
	[SerializeField]
	private float healMinSatiety = 10f;

	[Postfix(PostfixAttribute.Id.Percents)]
	[SerializeField]
	private float healSatietyFallMod = 10f;

	[Postfix(PostfixAttribute.Id.Percents)]
	[SerializeField]
	private float thirstSatietyFallMod = 10f;

	[Postfix(PostfixAttribute.Id.Percents)]
	[SerializeField]
	private float fightThirstSatietyFallMod = 10f;

	[Header("Боевые")]
	[Postfix(PostfixAttribute.Id.Seconds)]
	[SerializeField]
	private float hitFrequency = 1f;

	[Postfix(PostfixAttribute.Id.Percents)]
	[SerializeField]
	private float hitRegistrationAnimPerc = 30f;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.PBS)]
	private FloatPBS hitPower;

	[Postfix(PostfixAttribute.Id.Seconds)]
	[SerializeField]
	private float hitSlowTime = 0.5f;

	[SerializeField]
	private float hitSlowSpeed = 50f;

	[Tooltip("Поворот до врага пока не будет установлен данный (или меньше) угол до врага")]
	[SerializeField]
	private float minAngleTurnByAttack = 30f;

	[SerializeField]
	[Tooltip("Максимальное растояние до убегающего бота для наблюдения за ним")]
	private float maxDistanceToObserving = 40f;

	[SerializeField]
	[Tooltip("Количество врагов, у которых отображается поле зрения при нахождении игрока в невидимости")]
	private int enemyFieldOfViewCount = 2;

	[Tooltip("Сектор преследования игрока, пока враг в этом секторе, игрок бежит быстрее")]
	[SerializeField]
	private PlayerCombat.AggressionZoneAngles pursuitZoneAngles;

	[Tooltip("Сектор побега игрока, пока враг в этом секторе, игрок бежит быстрее")]
	[SerializeField]
	private PlayerCombat.AggressionZoneAngles escapeZoneAngles;

	[SerializeField]
	private PlayerCombat.InvisibilityAbility invisibility;

	[SerializeField]
	[Separator("Принцип интуитивно понятного выбора цели для атаки", SeparatorAttribute.Colors.WHITE, 20f, false)]
	private float maxAngleToAttack;

	[SerializeField]
	private float maxDistanceToAttack;

	[SerializeField]
	private float angleWeight;

	[SerializeField]
	private float distanceWeight;

	[Header("Прокачка")]
	[SerializeField]
	private int maxLevel = 300;

	[SerializeField]
	[Save]
	private int level;

	[Save]
	public float levelExperience;

	[SerializeField]
	private float levelUpExperience = 1000f;

	[SerializeField]
	private float thirstExperience = 10f;

	[SerializeField]
	private float plantExperience = 100f;

	[SerializeField]
	[Header("Экономика")]
	[Save]
	private CoinsData curCoins;

	[Save]
	public int coinsOnSpawnPoints;

	[Save]
	public long lastCoinSpawnTime;

	[SerializeField]
	private int coinValue = 1;

	[SerializeField]
	[Header("Остальное")]
	[Save]
	public FamilyManager.GenderType gender;

	[Save]
	[HideInInspector]
	public int deathsCount;

	[Tooltip("Время, которое даётся игроку для ручного включения сна, после того как сон становится доступным. По истечению этого времени, сон наступит автоматически при первой возможности")]
	[SerializeField]
	[Postfix(PostfixAttribute.Id.Seconds)]
	private float timeToManualSleepSec = 20f;

	[SerializeField]
	private float sleepBlockingTime = 0.18f;

	[SerializeField]
	private float sleepUnblockingTime = 1.5f;

	public float SpeedMinimum => speedMinimum + speedMinimum / 100f * MoveSpeedBonusPerc;

	public float OriginalSpeedMedium => speedMedium;

	public float SpeedMedium => speedMedium + speedMedium / 100f * MoveSpeedBonusPerc;

	public float OriginalSpeedMaximum => speedMaximum;

	public float SpeedMaximum => speedMaximum + speedMaximum / 100f * MoveSpeedBonusPerc;

	public float SpeedBackward => speedBackward + speedBackward / 100f * MoveSpeedBonusPerc;

	private float MoveSpeedBonusPerc
	{
		get
		{
			float num = (float)ManagerBase<VegetableManager>.Instance.vegetablesData.Count((VegetableManager.VegetableData x) => x.type == VegetableManager.VegetableType.Carrot && x.IsBonusActive) * ManagerBase<VegetableManager>.Instance.CarrotMoveSpeedBonus;
			float num2 = ManagerBase<FarmResidentManager>.Instance.SuperBonusesData.Find((FarmResidentManager.SuperBonusData x) => x.id == SuperBonus.Id.BootsWalkers).IsActive ? ManagerBase<FarmResidentManager>.Instance.BootsSpeedBonus : 0f;
			float num3 = 0f;
			if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Abyssinian)
			{
				num3 = ManagerBase<SkinManager>.Instance.AbyssinianMoveSpeedBonus;
			}
			else if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Egyptian)
			{
				num3 = ManagerBase<SkinManager>.Instance.EgyptianMoveSpeedBonus;
			}
			else if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Bengal)
			{
				num3 = ManagerBase<SkinManager>.Instance.BengalMoveSpeedBonus;
			}
			return num + num2 + num3;
		}
	}

	public float SpeedAcceleration => speedAcceleration + speedAcceleration / 100f * MoveSpeedBonusPerc;

	public float MovementLimit => movementLimit;

	public float RotationSpeed => rotationSpeed;

	public float EatDrinkRotationSpeed => eatDrinkRotationSpeed;

	public float JumpPower
	{
		get
		{
			float num = 0f;
			if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Egyptian)
			{
				num = ManagerBase<SkinManager>.Instance.EgyptianJumpPowerBonus;
			}
			return Mathf.Clamp(jumpPower + num + (ManagerBase<FarmResidentManager>.Instance.SuperBonusesData.Find((FarmResidentManager.SuperBonusData x) => x.id == SuperBonus.Id.BootsWalkers).IsActive ? ManagerBase<FarmResidentManager>.Instance.BootsJumpPowerBonus : 0f), 0f, jumpPowerLimit);
		}
	}

	public float JumpSpeedBoost => jumpSpeedBoost;

	public float JumpFrequency => jumpFrequency;

	public float Gravity => gravity;

	public List<SurfaceTag> SurfaceTags => surfaceTags;

	public float SwitchToSittingTime => switchToSittingTime;

	public float SwitchToRestTime => switchToRestTime;

	public float GraphicRotLerp => graphicRotLerp;

	public float GraphicAlignLerp => graphicAlignLerp;

	public float SpeedLimitStartingDistance => speedLimitStartingDistance;

	public float SpeedLimitFinalDistance => speedLimitFinalDistance;

	public bool UseRaycastToCheckObstacles => useRaycastToCheckObstacles;

	public float MaxDistancePursuitAssistance => maxDistancePursuitAssistance;

	public float MinDistancePursuitAssistance => minDistancePursuitAssistance;

	public float MaxDistAnglePursuitAssistance => maxDistAnglePursuitAssistance;

	public float MinDistAnglePursuitAssistance => minDistAnglePursuitAssistance;

	public float ObstacleValue
	{
		get
		{
			if (PlayerSpawner.PlayerInstance == null)
			{
				return 0f;
			}
			if (PlayerSpawner.PlayerInstance.PlayerMovement.NavMeshObstacle.shape != 0)
			{
				return Mathf.Max(PlayerSpawner.PlayerInstance.PlayerMovement.NavMeshObstacle.size.x, PlayerSpawner.PlayerInstance.PlayerMovement.NavMeshObstacle.size.z) / 2f;
			}
			return PlayerSpawner.PlayerInstance.PlayerMovement.NavMeshObstacle.radius;
		}
	}

	public float SleepFrequency => sleepFrequency * 60f;

	public float HealthMaximum
	{
		get
		{
			float value = health.GetValue(level);
			float y = 0f;
			if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Russian)
			{
				y = ManagerBase<SkinManager>.Instance.RussianMaxHealthBonus;
			}
			else if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Mainecoon)
			{
				y = ManagerBase<SkinManager>.Instance.MainecoonMaxHealthBonus;
			}
			else if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Burmilla)
			{
				y = ManagerBase<SkinManager>.Instance.BurmillaHealthByFamilyCountBonus + ManagerBase<SkinManager>.Instance.BurmillaHealthByFamilyCountBonus * (float)ManagerBase<FamilyManager>.Instance.family.Count;
			}
			else if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Alien)
			{
				y = ManagerBase<SkinManager>.Instance.AlienMaxHealthBonus;
			}
			float num = CalculationsHelpUtils.CalculateProp(value, y, 100f);
			float num2 = (float)ManagerBase<VegetableManager>.Instance.vegetablesData.Count((VegetableManager.VegetableData x) => x.type == VegetableManager.VegetableType.Pumpkin && x.IsBonusActive) * ManagerBase<VegetableManager>.Instance.PumpkinHealthBonus;
			return value + value / 100f * num2 + num;
		}
	}

	public float SatietyMaximum => satiety;

	public float ThirstMaximum => thirst;

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

	public float HealthFall => CalculationsHelpUtils.CalculateProp(HealthMaximum, healthFall, 100f);

	public float SatietyFall
	{
		get
		{
			float num = satietyFall;
			if (PlayerSpawner.IsPlayerSpawned && PlayerSpawner.PlayerInstance.PlayerCombat.InCombat)
			{
				num += satietyFall * fightThirstSatietyFallMod / 100f;
			}
			if (healthCurrent < HealthMaximum && CanHeal)
			{
				num += satietyFall * healSatietyFallMod / 100f;
			}
			if (thirstCurrent == 0f)
			{
				num += satietyFall * thirstSatietyFallMod / 100f;
			}
			float num2 = 0f;
			if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Abyssinian)
			{
				num2 = CalculationsHelpUtils.CalculateProp(num, ManagerBase<SkinManager>.Instance.AbyssinianThirstAndSatietyBonus, 100f);
			}
			return satietyFall - num2;
		}
	}

	public float ThirstFall
	{
		get
		{
			float num = thirstFall;
			if (PlayerSpawner.IsPlayerSpawned && PlayerSpawner.PlayerInstance.PlayerCombat.InCombat)
			{
				num += thirstFall * fightThirstSatietyFallMod / 100f;
			}
			float num2 = 0f;
			if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Abyssinian)
			{
				num2 = CalculationsHelpUtils.CalculateProp(num, ManagerBase<SkinManager>.Instance.AbyssinianThirstAndSatietyBonus, 100f);
			}
			float num3 = 0f;
			float y = (float)ManagerBase<VegetableManager>.Instance.vegetablesData.Count((VegetableManager.VegetableData x) => x.type == VegetableManager.VegetableType.Turnip && x.IsBonusActive) * ManagerBase<VegetableManager>.Instance.TurnipThirstBonus;
			num3 = CalculationsHelpUtils.CalculateProp(num, y, 100f);
			return num - num2 - num3;
		}
	}

	public float SatietyHitFallPerc => satietyHitFall;

	public float ThirstHitFallPerc => thirstHitFall;

	public float ThirstEffect => ThirstMaximum / (float)CountThirstObjs;

	public bool HaveThirst => ThirstMaximum - thirstCurrent >= ThirstEffect;

	public bool ThirstIsMaximum => thirstCurrent == ThirstMaximum;

	public int CountThirstObjs
	{
		get
		{
			if (!(PlayerThirstPanel.Instance != null))
			{
				return 1;
			}
			return PlayerThirstPanel.Instance.ThirstObjsCount;
		}
	}

	public int ThirstParts
	{
		get
		{
			float num = (ThirstMaximum - thirstCurrent) / ThirstEffect;
			if (num % 1f == 0f)
			{
				return Mathf.CeilToInt(num);
			}
			return Mathf.CeilToInt(num) - 1;
		}
	}

	public float HealHealth => healHealth;

	public bool CanHeal
	{
		get
		{
			if (satietyCurrent > satiety * healMinSatiety / 100f)
			{
				return healthCurrent < HealthMaximum;
			}
			return false;
		}
	}

	public float TimeToFullHeal => 100f / HealHealth;

	public float HitFrequency => hitFrequency;

	public float HitRegistrationAnimNormalizedTime => hitRegistrationAnimPerc / 100f;

	public float HitPower
	{
		get
		{
			float value = hitPower.GetValue(level);
			float num = CalculationsHelpUtils.CalculateProp(value, _003Cget_HitPower_003Eg__GetSkinHitPowerBonusPerc_007C156_0(), 100f);
			float num2 = Invisibility.IsActive ? CalculationsHelpUtils.CalculateProp(value, _003Cget_HitPower_003Eg__GetVegetableHitPowerBonusPerc_007C156_1(), 100f) : 0f;
			float num3 = Invisibility.IsActive ? Invisibility.InvisibilityAttackPowerMulty : 1f;
			if (Invisibility.IsActive)
			{
				return (value + num + num2) * num3;
			}
			return value + num;
		}
	}

	public float HitSlowTime => hitSlowTime;

	public float HitSlowSpeed => hitSlowSpeed;

	public float MinAngleTurnByAttack => minAngleTurnByAttack;

	public float MaxDistanceToObserving => maxDistanceToObserving;

	public int EnemyFieldOfViewCount => enemyFieldOfViewCount;

	public PlayerCombat.AggressionZoneAngles PursuitZoneAngles => pursuitZoneAngles;

	public PlayerCombat.AggressionZoneAngles EscapeZoneAngles => escapeZoneAngles;

	public PlayerCombat.InvisibilityAbility Invisibility => invisibility;

	public float MaxAngleToAttack => maxAngleToAttack;

	public float MaxDistanceToAttack => maxDistanceToAttack;

	public float AngleWeight => angleWeight;

	public float DistanceWeight => distanceWeight;

	public int Level
	{
		get
		{
			return level;
		}
		set
		{
			level = Mathf.Clamp(value, 0, maxLevel);
			PlayerManager.levelChangeEvent?.Invoke();
		}
	}

	public float LevelUpExperience => levelUpExperience;

	public float ThirstExperience => thirstExperience;

	public float PlantExperience => plantExperience;

	public float ExperienceBonusPerc => 0f;

	public int CurCoins
	{
		get
		{
			return curCoins.CoinsCount;
		}
		private set
		{
			curCoins.CoinsCount = value;
		}
	}

	[Save]
	public int AllCollectedCoins
	{
		get;
		private set;
	}

	public int CoinValue => coinValue;

	public float TimeToManualSleepSec => timeToManualSleepSec;

	public float SleepBlockingTime => sleepBlockingTime;

	public float SleepUnblockingTime => sleepUnblockingTime;

	public static event Action changeHealthMaximum;

	public static event CoinsUpdateHandler coinsChangeEvent;

	public static event CoinsUpdateHandler notRewardedChangeCoinsEvent;

	public static event ExperienceUpdateHandler experienceUpdateEvent;

	public static event Action levelChangeEvent;

	private void UpdateHealthMaximum()
	{
		if (HealthMaximum != prevHealthMaximum)
		{
			if (prevHealthMaximum == 0f)
			{
				prevHealthMaximum = HealthMaximum;
				return;
			}
			healthCurrent = healthCurrent / prevHealthMaximum * HealthMaximum;
			prevHealthMaximum = HealthMaximum;
			PlayerManager.changeHealthMaximum?.Invoke();
		}
	}

	public float GetDrinkTimeNeeded(int thirstParts)
	{
		return (float)thirstParts * DrinkingDuration;
	}

	protected override void OnInit()
	{
		CoinsData.coinsValidateFailedEvent += OnCoinsValidateFailed;
		VegetableBehaviour.plantEvent += OnVegetablePlant;
		SkinManager.currentSkinChangedEvent += OnCurrentSkinChanged;
		PlayerFamilyController.addFamilyMemberEvent += OnAddFamilyMember;
		if (!ManagerBase<SaveManager>.Instance.IsDataLoaded)
		{
			healthCurrent = HealthMaximum;
			satietyCurrent = SatietyMaximum;
			thirstCurrent = ThirstMaximum;
			sleepTimer = SleepFrequency;
		}
	}

	private void OnDestroy()
	{
		CoinsData.coinsValidateFailedEvent -= OnCoinsValidateFailed;
		VegetableBehaviour.plantEvent -= OnVegetablePlant;
		SkinManager.currentSkinChangedEvent -= OnCurrentSkinChanged;
		PlayerFamilyController.addFamilyMemberEvent -= OnAddFamilyMember;
	}

	private void OnVegetablePlant(VegetableBehaviour vegetable)
	{
		if (vegetable.VegetableData.vegetableParams.type == VegetableManager.VegetableType.Pumpkin)
		{
			UpdateHealthMaximum();
		}
	}

	private void OnCurrentSkinChanged()
	{
		UpdateHealthMaximum();
	}

	private void OnAddFamilyMember(FamilyManager.FamilyMemberData familyMember)
	{
		if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Burmilla)
		{
			UpdateHealthMaximum();
		}
	}

	private void OnCoinsValidateFailed()
	{
		PlayerManager.coinsChangeEvent?.Invoke();
	}

	public void ChangeCoins(int coinsInc, bool isRewardedCoins = false)
	{
		CurCoins = Mathf.Clamp(CurCoins + coinsInc, 0, int.MaxValue);
		if (coinsInc > 0)
		{
			AllCollectedCoins += coinsInc;
		}
		PlayerManager.coinsChangeEvent?.Invoke(coinsInc);
		if (!isRewardedCoins)
		{
			PlayerManager.notRewardedChangeCoinsEvent?.Invoke(coinsInc);
		}
	}

	public void AddExperience(float experience)
	{
		bool flag = Level == maxLevel;
		bool flag2 = levelExperience == LevelUpExperience;
		if (float.IsNaN(experience) || float.IsInfinity(experience) || (flag && flag2))
		{
			return;
		}
		if (experience < 0f)
		{
			UnityEngine.Debug.LogError("negative experience");
		}
		else
		{
			if (experience == 0f)
			{
				return;
			}
			experience += experience / 100f * ExperienceBonusPerc;
			experience = Mathf.Round(experience);
			float num = levelExperience;
			if (Level == maxLevel)
			{
				levelExperience = Mathf.Clamp(levelExperience + experience, 0f, LevelUpExperience);
			}
			else
			{
				levelExperience += experience;
			}
			if (num != levelExperience)
			{
				float experience2 = levelExperience - num;
				int num2 = 0;
				while (levelExperience >= LevelUpExperience)
				{
					levelExperience -= LevelUpExperience;
					int num3 = ++Level;
					num2++;
				}
				if (num2 > 0)
				{
					UpdateHealthMaximum();
				}
				PlayerManager.experienceUpdateEvent?.Invoke(experience2, num2);
			}
		}
	}

	public float CalculcatePBF(float value, float pbf)
	{
		return CalculationsHelpUtils.CalculcatePBF(value, pbf, level);
	}

	public void SetCheatMediumSpeed(float newMediumSpeed)
	{
		speedMedium = newMediumSpeed;
	}

	public float GetOriginalSpeedMedium()
	{
		return speedMedium;
	}

	[CompilerGenerated]
	private float _003Cget_HitPower_003Eg__GetSkinHitPowerBonusPerc_007C156_0()
	{
		float result = 0f;
		if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Siamese)
		{
			if (healthCurrent / HealthMaximum < ManagerBase<SkinManager>.Instance.SiameseHealthPart)
			{
				result = ManagerBase<SkinManager>.Instance.SiameseHitPowerBonus;
			}
		}
		else if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Bengal)
		{
			result = ManagerBase<SkinManager>.Instance.BengalHitPowerBonus;
		}
		else if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Alien)
		{
			result = ManagerBase<SkinManager>.Instance.AlienHitPowerBonus;
		}
		else if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Bombay && Invisibility.IsActive)
		{
			result = ManagerBase<SkinManager>.Instance.BombayStealthHitPowerBonus;
		}
		return result;
	}

	[CompilerGenerated]
	private static float _003Cget_HitPower_003Eg__GetVegetableHitPowerBonusPerc_007C156_1()
	{
		return (float)ManagerBase<VegetableManager>.Instance.vegetablesData.Count((VegetableManager.VegetableData x) => x.type == VegetableManager.VegetableType.Beet && x.IsBonusActive) * ManagerBase<VegetableManager>.Instance.BeetHitPowerBonus;
	}
}
