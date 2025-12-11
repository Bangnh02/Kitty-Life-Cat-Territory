using UnityEngine;

public class DieAnimController : StateMachineBehaviour
{
	public delegate void AnimationStateHandler(Animator animator);

	public static event AnimationStateHandler endAnimationEvent;

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
	{
		if (animatorStateInfo.normalizedTime >= 1f)
		{
			DieAnimController.endAnimationEvent?.Invoke(animator);
		}
	}
}
