using UnityEngine;

public class FamilyMemberSound : ActorSound
{
	[Header("Звук потребности котенка")]
	[SerializeField]
	private float minMeowDuration = 1.5f;

	[SerializeField]
	private float maxMeowDuration = 10f;

	[Range(0f, 100f)]
	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private int meowVolume = 100;

	[SerializeField]
	private AudioClip meowSound;

	[Header("Звуки атаки")]
	[SerializeField]
	private SoundEffectsVolume hitAttackSounds;

	private float timer;

	private FamilyMemberController _familyMember;

	public float MeowVolume => (float)meowVolume / 100f;

	private FamilyMemberController FamilyMember
	{
		get
		{
			if (_familyMember == null)
			{
				_familyMember = GetComponent<FamilyMemberController>();
			}
			return _familyMember;
		}
	}

	private bool IsNecessaryRole
	{
		get
		{
			if (FamilyMember.familyMemberData.role != 0)
			{
				return FamilyMember.familyMemberData.role == FamilyManager.FamilyMemberRole.SecondStageChild;
			}
			return true;
		}
	}

	private bool IsNecessaryState
	{
		get
		{
			if (FamilyMember.CurState != FamilyMemberController.State.Following)
			{
				return FamilyMember.CurState == FamilyMemberController.State.WalkingAround;
			}
			return true;
		}
	}

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		FamilyMember.Combat.hitEvent += OnHit;
		timer = Random.Range(minMeowDuration, maxMeowDuration);
	}

	private void OnDestroy()
	{
		FamilyMember.Combat.hitEvent -= OnHit;
	}

	private void OnHit(ActorCombat attacker, ActorCombat target)
	{
		PlayRandomSound(hitAttackSounds.soundEffects, hitAttackSounds.Volume * FamilyMember.familyMemberData.Params.HitPowerPerc / 100f);
	}

	private void Update()
	{
		if (IsNecessaryRole && !string.IsNullOrEmpty(FamilyMember.familyMemberData.curNeed) && IsNecessaryState)
		{
			if (timer == 0f)
			{
				PlaySound(meowSound, MeowVolume);
				timer = Random.Range(minMeowDuration, maxMeowDuration);
			}
			else
			{
				timer = Mathf.Clamp(timer - Time.deltaTime, 0f, maxMeowDuration);
			}
		}
	}
}
