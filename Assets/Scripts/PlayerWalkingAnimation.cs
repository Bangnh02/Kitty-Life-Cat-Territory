using UnityEngine;

public class PlayerWalkingAnimation : StateMachineBehaviour
{
	public static float animationSpeed;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
	{
		animationSpeed = animatorStateInfo.speed;
	}
}
