using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSleepController : MonoBehaviour, IInitializablePlayerComponent
{
	private float manualSleepTimer;

	private bool manualSleepTimerStarted;

	private bool prevCanSleepState;

	private Coroutine sleepingCor;

	private Action sleepEndCallback;

	private float SleepTimer
	{
		get
		{
			return ManagerBase<PlayerManager>.Instance.sleepTimer;
		}
		set
		{
			ManagerBase<PlayerManager>.Instance.sleepTimer = value;
		}
	}

	private float SleepFrequency => ManagerBase<PlayerManager>.Instance.SleepFrequency;

	private bool IsSleeping
	{
		get
		{
			return ManagerBase<PlayerManager>.Instance.isSleeping;
		}
		set
		{
			ManagerBase<PlayerManager>.Instance.isSleeping = value;
		}
	}

	private float FallAsleepTime => ManagerBase<PlayerManager>.Instance.SleepBlockingTime;

	private float AwakingTime => ManagerBase<PlayerManager>.Instance.SleepUnblockingTime;

	private float TimeToManualSleepSec => ManagerBase<PlayerManager>.Instance.TimeToManualSleepSec;

	private PlayerCombat PlayerCombat => PlayerSpawner.PlayerInstance.PlayerCombat;

	private PlayerIdleController PlayerIdleController => PlayerSpawner.PlayerInstance.PlayerIdleController;

	private PlayerFamilyController PlayerFamilyController => PlayerSpawner.PlayerInstance.PlayerFamilyController;

	private PlayerMovement PlayerMovement => PlayerSpawner.PlayerInstance.PlayerMovement;

	public static event Action fallingAsleepEvent;

	public static event Action sleepStartEvent;

	public static event Action sleepEvent;

	public static event Action sleepEndEvent;

	public static event Action awakeEndEvent;

	public static event Action changeCanSleepStateEvent;

	public void Initialize()
	{
		PlayerMovement.flyingStartEvent += UpdateCanSleepState;
		PlayerMovement.flyingEndEvent += UpdateCanSleepState;
		UpdateCanSleepState();
		if (IsSleeping)
		{
			sleepingCor = StartCoroutine(Awaking());
		}
	}

	private void OnDestroy()
	{
		if (PlayerSpawner.IsPlayerSpawned)
		{
			PlayerMovement.flyingStartEvent -= UpdateCanSleepState;
			PlayerMovement.flyingEndEvent -= UpdateCanSleepState;
		}
	}

	private void UpdateCanSleepState()
	{
		bool flag = CanSleep();
		if (prevCanSleepState != flag)
		{
			prevCanSleepState = flag;
			if (flag && !manualSleepTimerStarted)
			{
				manualSleepTimer = TimeToManualSleepSec;
				manualSleepTimerStarted = true;
			}
			PlayerSleepController.changeCanSleepStateEvent?.Invoke();
		}
	}

	public bool CanSleep()
	{
		if (!IsSleeping && !PlayerSpawner.PlayerInstance.IsCommandExecuting() && sleepingCor == null && !PlayerCombat.InCombat && PlayerCombat.CurLifeState == ActorCombat.LifeState.Alive && SleepTimer <= 0f && !PlayerMovement.IsFalling)
		{
			return PlayerFamilyController.FamilyCanSleep();
		}
		return false;
	}

	public bool ReadyToAutoSleep()
	{
		if (manualSleepTimerStarted && manualSleepTimer <= 0f)
		{
			return CanSleep();
		}
		return false;
	}

	public void Sleep(Action endCallback)
	{
		if (!CanSleep())
		{
			endCallback?.Invoke();
			return;
		}
		sleepEndCallback = endCallback;
		sleepingCor = StartCoroutine(Sleeping());
	}

	private IEnumerator Sleeping()
	{
		IsSleeping = true;
		UpdateCanSleepState();
		PlayerSleepController.sleepStartEvent?.Invoke();
		if (PlayerCombat.IsInvisibilityActive())
		{
			PlayerCombat.SwitchInvisibility();
		}
		PlayerMovement.NavMeshObstacle.carving = true;
		PlayerFamilyController.Sleep();
		List<FamilyMemberController> familyOnStartSleep = new List<FamilyMemberController>(FamilyMemberController.Instances);
		bool enterSleepAnimation = false;
		Action endCallback = delegate
		{
			enterSleepAnimation = true;
		};
		PlayerIdleController.EnterSleepAnimation(endCallback);
		yield return new WaitUntil(() => familyOnStartSleep.TrueForAll((FamilyMemberController x) => x.IsSleeping));
		yield return new WaitUntil(() => enterSleepAnimation);
		PlayerMovement.NavMeshObstacle.carving = false;
		PlayerSleepController.fallingAsleepEvent?.Invoke();
		yield return new WaitForSecondsRealtime(FallAsleepTime);
		ManagerBase<SaveManager>.Instance.SaveToLocal();
		PlayerSleepController.sleepEvent?.Invoke();
		sleepingCor = null;
		sleepingCor = StartCoroutine(Awaking());
	}

	private IEnumerator Awaking()
	{
		SleepTimer = SleepFrequency;
		manualSleepTimerStarted = false;
		PlayerFamilyController.AwakeFamily();
		Action endCallback = delegate
		{
			IsSleeping = false;
		};
		PlayerIdleController.ExitSleepAnimation(endCallback);
		PlayerSleepController.sleepEndEvent?.Invoke();
		yield return new WaitUntil(() => !IsSleeping && FamilyMemberController.Instances.TrueForAll((FamilyMemberController x) => !x.IsSleeping));
		sleepingCor = null;
		sleepEndCallback?.Invoke();
		sleepEndCallback = null;
		PlayerSleepController.awakeEndEvent?.Invoke();
	}

	private void Update()
	{
		if (Time.deltaTime == 0f)
		{
			return;
		}
		if (!IsSleeping)
		{
			if (SleepTimer >= 0f)
			{
				SleepTimer -= Time.deltaTime;
			}
			else if (manualSleepTimerStarted && manualSleepTimer > 0f)
			{
				manualSleepTimer -= Time.deltaTime;
			}
		}
		UpdateCanSleepState();
	}
}
