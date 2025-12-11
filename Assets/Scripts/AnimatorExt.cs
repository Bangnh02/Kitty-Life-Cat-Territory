using UnityEngine;

public static class AnimatorExt
{
	public static bool IsPlayingByTag(this Animator animator, int animationTagHash, int animationLayerIndex = 0)
	{
		return animator.GetCurrentAnimatorStateInfo(animationLayerIndex).tagHash == animationTagHash;
	}

	public static bool IsPlayingByName(this Animator animator, int animationHash, int animationLayerIndex = 0)
	{
		return animator.GetCurrentAnimatorStateInfo(animationLayerIndex).shortNameHash == animationHash;
	}

	public static bool IsPlayingNextByTag(this Animator animator, int animationTagHash, int animationLayerIndex = 0)
	{
		return animator.GetNextAnimatorStateInfo(animationLayerIndex).tagHash == animationTagHash;
	}

	public static bool IsPlayingNextByName(this Animator animator, int animationHash, int animationLayerIndex = 0)
	{
		return animator.GetNextAnimatorStateInfo(animationLayerIndex).shortNameHash == animationHash;
	}
}
