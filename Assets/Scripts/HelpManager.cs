using System;
using UnityEngine;

public class HelpManager : ManagerBase<HelpManager>
{
	[Serializable]
	public enum Hint
	{
		HelpHintChild1,
		HelpHintChild3,
		HelpHintFamilyParamZero,
		HelpHintGarden,
		HelpHintParamZero,
		HelpHintResidents,
		HelpHintSpouse,
		HelpHintStealth,
		HelpHintWelcome,
		HelpHintSleep,
		HelpHintClew
	}

	[SerializeField]
	private float welcomeHintDelay = 1.5f;

	[SerializeField]
	private float stealthHintDelay = 1.5f;

	[SerializeField]
	private float gardenHintDelay = 1f;

	[SerializeField]
	private float residentsHintDelay = 1f;

	[SerializeField]
	private float sleepHintDelay = 1.5f;

	[SerializeField]
	private float clewHintDelay = 0.5f;

	[Header("Отладка")]
	[Save]
	public bool welcomeHintShowed;

	[Save]
	public bool stealthHintShowed;

	[Save]
	public bool childFirstStageHintShowed;

	[Save]
	public bool childThirdStageHintShowed;

	[Save]
	public bool sleepHintShowed;

	[Save]
	public bool familyParamZeroHintShowed;

	[Save]
	public bool paramZeroHintShowed;

	[Save]
	public bool gardenHintShowed;

	[Save]
	public bool farmResidentHintShowed;

	[Save]
	public bool clewHintShowed;

	[Save]
	public bool joystickArrowShowed;

	[Save]
	public bool questArrowShowed;

	[Save]
	public bool mapArrowShowed;

	public float WelcomeHintDelay => welcomeHintDelay;

	public float StealthHintDelay => stealthHintDelay;

	public float GardenHintDelay => gardenHintDelay;

	public float ResidentsHintDelay => residentsHintDelay;

	public float SleepHintDelay => sleepHintDelay;

	public float ClewHintDelay => clewHintDelay;

	public bool IsAllHintShowed
	{
		get
		{
			if (welcomeHintShowed && stealthHintShowed && childFirstStageHintShowed && childThirdStageHintShowed && sleepHintShowed && familyParamZeroHintShowed && paramZeroHintShowed && gardenHintShowed && farmResidentHintShowed)
			{
				return clewHintShowed;
			}
			return false;
		}
	}

	public bool IsAllArrowShowed
	{
		get
		{
			if (joystickArrowShowed && questArrowShowed)
			{
				return mapArrowShowed;
			}
			return false;
		}
	}

	protected override void OnInit()
	{
	}
}
