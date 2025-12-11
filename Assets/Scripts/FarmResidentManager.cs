using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FarmResidentManager : ManagerBase<FarmResidentManager>
{
	[Serializable]
	public class FarmResidentData
	{
		public FarmResidentId farmResidentId;

		public int helpProgressCurrent;

		public int curHabitatId = -1;

		public int curZoneId = -1;

		public string curNeed;

		public bool startNeedUsed;

		public int needProgressCurrent;

		public int needProgressMaximum;

		public bool HaveNeed => !string.IsNullOrEmpty(curNeed);
	}

	[Serializable]
	public class FarmResidentParams
	{
		public FarmResidentId farmResidentId;

		public List<string> needs = new List<string>();

		public GameObject prefab;

		public string startNeed;

		private FarmResidentData _farmResidentData;

		public FarmResidentData FarmResidentData
		{
			get
			{
				if (_farmResidentData == null)
				{
					_farmResidentData = ManagerBase<FarmResidentManager>.Instance.FarmResidentsData.Find((FarmResidentData x) => x.farmResidentId == farmResidentId);
				}
				return _farmResidentData;
			}
		}
	}

	[Serializable]
	public class SuperBonusData
	{
		public SuperBonus.Id id;

		public bool isPicking;

		[SerializeField]
		private float _timer;

		[SerializeField]
		private bool isPermanent;

		public float Timer
		{
			get
			{
				return _timer;
			}
			set
			{
				float timer = _timer;
				_timer = Mathf.Clamp(value, 0f, float.MaxValue);
				if (timer < value || (timer > 0f && _timer == 0f))
				{
					SuperBonusData.switchActiveStateEvent?.Invoke(id);
				}
				SuperBonusData.updateTimerEvent?.Invoke(id);
			}
		}

		public bool IsPermanent
		{
			get
			{
				return isPermanent;
			}
			set
			{
				isPermanent = value;
				SuperBonusData.switchActiveStateEvent?.Invoke(id);
			}
		}

		public bool IsActive
		{
			get
			{
				if (!IsPermanent)
				{
					return Timer > 0f;
				}
				return true;
			}
		}

		public static event SuperBonusHandler switchActiveStateEvent;

		public static event SuperBonusHandler updateTimerEvent;
	}

	[Serializable]
	public class BonusNeedCount
	{
		public int bonusCount;

		public int needCount;
	}

	[Serializable]
	public class ZonePriority
	{
		public int zoneId;

		public int useCount;
	}

	public delegate void SuperBonusHandler(SuperBonus.Id id);

	[Header("Параметры жителей")]
	[SerializeField]
	private int helpProgressMaximum = 10;

	[SerializeField]
	private float progressMaximumCoinsMulty = 2f;

	[SerializeField]
	private float pauseBetweenChangeHabitat = 5f;

	[SerializeField]
	private int coinsPerNeedUnit = 15;

	[SerializeField]
	private float experiencePerNeedUnit = 15f;

	[SerializeField]
	private List<BonusNeedCount> bonusNeedCounts = new List<BonusNeedCount>();

	[SerializeField]
	private List<FarmResidentParams> farmResidentsParams;

	[Header("Супер-бонусы")]
	[Postfix(PostfixAttribute.Id.Percents)]
	[SerializeField]
	private float bootsSpeedBonus = 15f;

	[SerializeField]
	private float bootsJumpPowerBonus = 10f;

	[Postfix(PostfixAttribute.Id.Percents)]
	[SerializeField]
	private float milkHealthRecoveryBonus = 25f;

	[SerializeField]
	private float superBonusTime = 30f;

	[HideInInspector]
	public float superBonusLifeTime;

	[HideInInspector]
	public float superBonusFirstLifeTime;

	[Header("Отладка")]
	[Save]
	[SerializeField]
	private List<FarmResidentData> _farmResidentsData;

	[Save]
	public List<string> lastNeeds = new List<string>();

	[Save]
	[SerializeField]
	private List<SuperBonusData> _superBonusesData;

	[Save]
	[SerializeField]
	private List<ZonePriority> zonesPriorities;

	public int HelpProgressMaximum => helpProgressMaximum;

	public float ProgressMaximumCoinsMulty => progressMaximumCoinsMulty;

	public float PauseBetweenChangeHabitat => pauseBetweenChangeHabitat;

	public int CoinsPerNeedUnit => coinsPerNeedUnit;

	public float ExperiencePerNeedUnit => experiencePerNeedUnit;

	public List<FarmResidentParams> FarmResidentsParams => farmResidentsParams;

	public float BootsSpeedBonus => bootsSpeedBonus;

	public float BootsJumpPowerBonus => bootsJumpPowerBonus;

	public float MilkHealthRecoveryBonus => milkHealthRecoveryBonus;

	public float SuperBonusTime => superBonusTime;

	public List<FarmResidentData> FarmResidentsData
	{
		get
		{
			if (_farmResidentsData == null || _farmResidentsData.Count == 0)
			{
				_farmResidentsData = new List<FarmResidentData>();
				_farmResidentsData.Add(new FarmResidentData
				{
					farmResidentId = FarmResidentId.Farmer,
					helpProgressCurrent = 0,
					curNeed = null,
					curHabitatId = -1,
					needProgressCurrent = 0,
					needProgressMaximum = 0
				});
				_farmResidentsData.Add(new FarmResidentData
				{
					farmResidentId = FarmResidentId.Goat,
					helpProgressCurrent = 0,
					curNeed = null,
					curHabitatId = -1,
					needProgressCurrent = 0,
					needProgressMaximum = 0
				});
				_farmResidentsData.Add(new FarmResidentData
				{
					farmResidentId = FarmResidentId.Pig,
					helpProgressCurrent = 0,
					curNeed = null,
					curHabitatId = -1,
					needProgressCurrent = 0,
					needProgressMaximum = 0
				});
			}
			return _farmResidentsData;
		}
	}

	public List<SuperBonusData> SuperBonusesData
	{
		get
		{
			if (_superBonusesData == null || _superBonusesData.Count == 0)
			{
				_superBonusesData = new List<SuperBonusData>();
				_superBonusesData.Add(new SuperBonusData
				{
					id = SuperBonus.Id.BootsWalkers,
					Timer = 0f,
					isPicking = false,
					IsPermanent = false
				});
				_superBonusesData.Add(new SuperBonusData
				{
					id = SuperBonus.Id.Acorn,
					Timer = 0f,
					isPicking = false,
					IsPermanent = false
				});
				_superBonusesData.Add(new SuperBonusData
				{
					id = SuperBonus.Id.Milk,
					Timer = 0f,
					isPicking = false,
					IsPermanent = false
				});
			}
			return _superBonusesData;
		}
	}

	public List<ZonePriority> ZonesPriorities
	{
		get
		{
			if (zonesPriorities == null || zonesPriorities.Count == 0)
			{
				zonesPriorities = new List<ZonePriority>();
				int num = UnityEngine.Object.FindObjectsOfType<FarmResidentHabitatArea>().Max((FarmResidentHabitatArea x) => x.ZoneId) + 1;
				for (int i = 0; i < num; i++)
				{
					zonesPriorities.Add(new ZonePriority
					{
						zoneId = i,
						useCount = 0
					});
				}
			}
			return zonesPriorities;
		}
	}

	public static event SuperBonusHandler upgradeSuperBonusEvent;

	protected override void OnInit()
	{
		foreach (SuperBonusData superBonusesDatum in SuperBonusesData)
		{
			if (superBonusesDatum.isPicking)
			{
				UpgradeSuperBonus(superBonusesDatum);
			}
		}
	}

	public void UpgradeSuperBonus(SuperBonusData superBonusData)
	{
		FarmResidentId farmResidentId = ConvertBonusToFarmResident(superBonusData.id);
		int helpProgressCurrent = FarmResidentsData.Find((FarmResidentData x) => x.farmResidentId == farmResidentId).helpProgressCurrent;
		int num = ManagerBase<FarmResidentManager>.Instance.HelpProgressMaximum;
		if (helpProgressCurrent >= num)
		{
			superBonusData.IsPermanent = true;
		}
		else
		{
			superBonusData.Timer = SuperBonusTime;
		}
		FarmResidentManager.upgradeSuperBonusEvent?.Invoke(superBonusData.id);
	}

	public FarmResidentId ConvertBonusToFarmResident(SuperBonus.Id id)
	{
		FarmResidentId result = FarmResidentId.None;
		switch (id)
		{
		case SuperBonus.Id.BootsWalkers:
			result = FarmResidentId.Farmer;
			break;
		case SuperBonus.Id.Acorn:
			result = FarmResidentId.Pig;
			break;
		case SuperBonus.Id.Milk:
			result = FarmResidentId.Goat;
			break;
		}
		return result;
	}

	public int GetNeedCount(FarmResidentId farmResidentId)
	{
		int helpProgressCurrent = FarmResidentsParams.Find((FarmResidentParams x) => x.farmResidentId == farmResidentId).FarmResidentData.helpProgressCurrent;
		int needCount = bonusNeedCounts[0].needCount;
		foreach (BonusNeedCount bonusNeedCount in bonusNeedCounts)
		{
			if (helpProgressCurrent < bonusNeedCount.bonusCount)
			{
				return needCount;
			}
			needCount = bonusNeedCount.needCount;
		}
		return needCount;
	}
}
