using Avelog;
using System;
using System.Collections;
using UnityEngine;

public class PlayerIdleController : MonoBehaviour, IInitializablePlayerComponent
{
	public enum IdleAnimationId
	{
		Standart,
		Sitting,
		Rest,
		Sleep
	}

	private float idleTime;

	private IdleAnimationId nextIdleType;

	private bool manualControl;

	private Action endCallback;

	private Coroutine waitForCor;

	private Animator Animator => PlayerSpawner.PlayerInstance.Model.Animator;

	private PlayerCombat PlayerCombat => PlayerSpawner.PlayerInstance.PlayerCombat;

	private PlayerMovement PlayerMovement => PlayerSpawner.PlayerInstance.PlayerMovement;

	public IdleAnimationId CurIdleType
	{
		get;
		private set;
	}

	public void Initialize()
	{
		Animator.SetInteger("IdleType", (int)CurIdleType);
	}

	private void Update()
	{
		if (Time.deltaTime == 0f || manualControl)
		{
			return;
		}
		if (Animator.IsPlayingByName(AnimationHashes.idle.Value) && !PlayerCombat.InCombat && !PlayerMovement.IsSlidingAlongGround)
		{
			idleTime += Time.deltaTime;
			if (CurIdleType == IdleAnimationId.Standart && nextIdleType == IdleAnimationId.Standart)
			{
				nextIdleType = ((UnityEngine.Random.Range(0f, 1f) > 0.3f) ? IdleAnimationId.Sitting : IdleAnimationId.Rest);
			}
			if (CurIdleType != nextIdleType && ((nextIdleType == IdleAnimationId.Sitting && idleTime >= ManagerBase<PlayerManager>.Instance.SwitchToSittingTime) || (nextIdleType == IdleAnimationId.Rest && idleTime >= ManagerBase<PlayerManager>.Instance.SwitchToRestTime)))
			{
				CurIdleType = nextIdleType;
				Animator.SetInteger("IdleType", (int)CurIdleType);
			}
		}
		if (PlayerCombat.InCombat || !Animator.IsPlayingByTag(AnimationHashes.idle.Value) || Animator.GetFloat("MovementSpeed") != 0f || PlayerMovement.IsSlidingAlongGround)
		{
			ResetIdle();
		}
	}

	private void ResetIdle()
	{
		CurIdleType = IdleAnimationId.Standart;
		nextIdleType = IdleAnimationId.Standart;
		idleTime = 0f;
		Animator.SetInteger("IdleType", (int)CurIdleType);
	}

	public void EnterSleepAnimation(Action endCallback)
	{
		if (waitForCor != null)
		{
			StopCoroutine(waitForCor);
			endCallback?.Invoke();
		}
		this.endCallback = endCallback;
		manualControl = true;
		waitForCor = StartCoroutine(WaitForEnterSleep(endCallback));
	}

	private IEnumerator WaitForEnterSleep(Action endCallback)
	{
		this.endCallback = endCallback;
		if (Animator.GetInteger("IdleType") != 2)
		{
			CurIdleType = IdleAnimationId.Standart;
			Animator.SetInteger("IdleType", 0);
			yield return new WaitWhile(() => !Animator.IsPlayingByName(AnimationHashes.idle.Value));
		}
		CurIdleType = IdleAnimationId.Rest;
		Animator.SetInteger("IdleType", 2);
		Animator.SetBool("isSleeping", value: true);
		yield return new WaitWhile(() => !Animator.IsPlayingByName(AnimationHashes.sleep.Value));
		CurIdleType = IdleAnimationId.Sleep;
		waitForCor = null;
		endCallback?.Invoke();
	}

	public void ExitSleepAnimation(Action endCallback)
	{
		if (waitForCor != null)
		{
			StopCoroutine(waitForCor);
			endCallback?.Invoke();
		}
		this.endCallback = endCallback;
		waitForCor = StartCoroutine(WaitForExitSleep(endCallback));
	}

	private IEnumerator WaitForExitSleep(Action endCallback)
	{
		CurIdleType = IdleAnimationId.Standart;
		Animator.SetInteger("IdleType", 0);
		Animator.SetBool("isSleeping", value: false);
		yield return new WaitWhile(() => !Animator.IsPlayingByName(AnimationHashes.idle.Value));
		waitForCor = null;
		manualControl = false;
		ResetIdle();
		endCallback?.Invoke();
	}
}
