using UnityEngine;

public class AttackAnimSpeedController : StateMachineBehaviour
{
	public enum AnimatorType
	{
		Player,
		FamilyMember,
		Enemy
	}

	public delegate void AnimationStateHandler(Animator animator);

	[SerializeField]
	private AnimatorType animatorType;

	private const float initValue = 0f;

	private const float startAnimationSpeedMultiplier = 1f;

	private const float maxAnimationSpeedMuiltiplier = 100f;

	private float hitFrequency;

	private bool hitRegistrated;

	private float hitRegistrationNormalizedTime;

	private const string attackSpeedMultiplierName = "AttackSpeed";

	public static event AnimationStateHandler startAnimationEvent;

	public static event AnimationStateHandler hitRegistrationEvent;

	public static event AnimationStateHandler endAnimationEvent;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		AttackAnimSpeedController.startAnimationEvent?.Invoke(animator);
		if (hitFrequency == 0f || hitRegistrationNormalizedTime == 0f)
		{
			if (animatorType == AnimatorType.Player)
			{
				hitFrequency = ManagerBase<PlayerManager>.Instance.HitFrequency;
				hitRegistrationNormalizedTime = ManagerBase<PlayerManager>.Instance.HitRegistrationAnimNormalizedTime;
			}
			else if (animatorType == AnimatorType.FamilyMember)
			{
				hitFrequency = ManagerBase<FamilyManager>.Instance.HitFrequency;
				hitRegistrationNormalizedTime = ManagerBase<FamilyManager>.Instance.HitRegistrationAnimNormalizedTime;
			}
			else if (animatorType == AnimatorType.Enemy)
			{
				EnemyArchetype archetype = animator.GetComponent<EnemyModel>().Archetype;
				hitFrequency = archetype.hitFrequency;
				hitRegistrationNormalizedTime = archetype.HitRegistrationAnimNormalizedTime;
			}
		}
		if (stateInfo.length >= hitFrequency)
		{
			if (hitFrequency == 0f)
			{
				animator.SetFloat("AttackSpeed", 100f);
			}
			else
			{
				animator.SetFloat("AttackSpeed", stateInfo.length / hitFrequency);
			}
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		hitRegistrated = false;
		animator.SetFloat("AttackSpeed", 1f);
		AttackAnimSpeedController.endAnimationEvent?.Invoke(animator);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (stateInfo.normalizedTime >= hitRegistrationNormalizedTime && !hitRegistrated)
		{
			AttackAnimSpeedController.hitRegistrationEvent?.Invoke(animator);
			hitRegistrated = true;
		}
	}
}
