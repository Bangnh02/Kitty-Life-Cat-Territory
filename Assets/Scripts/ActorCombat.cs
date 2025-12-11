using Avelog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ActorCombat : MonoBehaviour
{
	[Serializable]
	public class AnimationInfo
	{
		public string name;

		public int layerIndex;
	}

	[Serializable]
	public class AttackTriggerInfo
	{
		public string trigger;

		[ReadonlyInspector]
		public bool isUsed;

		public AttackTriggerInfo(string trigger)
		{
			this.trigger = trigger;
			isUsed = false;
		}
	}

	public enum LifeState
	{
		Alive,
		Dying,
		Dead
	}

	public enum ChangeHealthState
	{
		AttackedByEnemy,
		LoseHealth,
		Healing,
		Respawn
	}

	public enum TakeDamageType
	{
		AttackedByEnemy,
		LoseHealth
	}

	public delegate void TakeDamageHandler(TakeDamageType takeDamageType, float damage, ActorCombat attacker);

	public delegate void ChangeStateHandler(LifeState state);

	public delegate void KillHandler(ActorCombat killer, ActorCombat target);

	public delegate void HitHandler(ActorCombat attacker, ActorCombat target);

	private float attackTimer;

	private float delayedHitPower;

	private const bool allowTakeDamageAnimationOnAttack = false;

	[Header("Имена тригеров аниматора")]
	[SerializeField]
	private List<AttackTriggerInfo> attackTriggers = new List<AttackTriggerInfo>
	{
		new AttackTriggerInfo("Attack")
	};

	[SerializeField]
	private List<string> takeDamageTriggers = new List<string>
	{
		"TakeDamage"
	};

	[SerializeField]
	private List<string> dieTriggers = new List<string>
	{
		"Die"
	};

	[SerializeField]
	private List<string> respawnTriggers = new List<string>
	{
		"Respawn"
	};

	[Header("Имена анимаций (Полные) для калбэков завершения")]
	[SerializeField]
	private List<string> attackAnimationNames = new List<string>
	{
		"Base Layer.Attack",
		"Base Layer.Attack2"
	};

	[SerializeField]
	private List<string> dieAnimationNames = new List<string>
	{
		"Base Layer.Die"
	};

	private List<int> _attackAnimationHashes;

	[Header("Отладка")]
	[SerializeField]
	private LifeState lifeState;

	private bool isInitialized;

	private Animator animatorOnStartDie;

	private Coroutine animationTimerCor;

	private Animator animatorOnStartAttack;

	private ActorCombat delayedHitTarget;

	private Action attackAnimationEndCallback;

	public virtual float MaxHealth => 100f;

	protected virtual float AttackPower => 1f;

	protected virtual float AttackFrequency => 1f;

	protected virtual List<AttackTriggerInfo> AttackTriggers => attackTriggers;

	private List<int> AttackAnimationHashes
	{
		get
		{
			if (_attackAnimationHashes == null)
			{
				_attackAnimationHashes = GetAnimationHashes(attackAnimationNames);
			}
			return _attackAnimationHashes;
		}
	}

	public virtual float CurHealth
	{
		get
		{
			return 100f;
		}
		protected set
		{
		}
	}

	public LifeState CurLifeState
	{
		get
		{
			return lifeState;
		}
		private set
		{
			lifeState = value;
		}
	}

	public bool IsAttackAnimationPlaying
	{
		get;
		private set;
	}

	public event TakeDamageHandler takeDamageEvent;

	public event Action healEvent;

	public event Action respawnEvent;

	public event ChangeStateHandler changeLifeStateEvent;

	public static event KillHandler killEvent;

	public event HitHandler hitEvent;

	public event Action attackEvent;

	protected void TriggerKillEnemyEvent(ActorCombat target)
	{
		ActorCombat.killEvent?.Invoke(this, target);
	}

	protected void Initialize()
	{
		if (!isInitialized)
		{
			isInitialized = true;
		}
	}

	protected abstract Animator GetAnimator();

	private List<int> GetAnimationHashes(List<string> animationNames)
	{
		return animationNames?.Select((string animationName) => Animator.StringToHash(animationName)).ToList();
	}

	private bool IsAnimationPlaying(List<int> animationHashes)
	{
		if (animationHashes == null || animationHashes.Count == 0)
		{
			return false;
		}
		for (int i = 0; i < GetAnimator().layerCount; i++)
		{
			AnimatorStateInfo animatorStateInfo = GetAnimator().GetCurrentAnimatorStateInfo(i);
			AnimatorStateInfo nextAnimatorStateInfo = GetAnimator().GetNextAnimatorStateInfo(i);
			if (animationHashes.Any((int x) => x == animatorStateInfo.fullPathHash))
			{
				return animatorStateInfo.normalizedTime <= 1f;
			}
			if (animationHashes.Any((int x) => x == nextAnimatorStateInfo.fullPathHash))
			{
				return nextAnimatorStateInfo.normalizedTime <= 1f;
			}
		}
		return false;
	}

	private bool IsAnimationPlaying(int tagHash)
	{
		for (int i = 0; i < GetAnimator().layerCount; i++)
		{
			AnimatorStateInfo currentAnimatorStateInfo = GetAnimator().GetCurrentAnimatorStateInfo(i);
			AnimatorStateInfo nextAnimatorStateInfo = GetAnimator().GetNextAnimatorStateInfo(i);
			if (tagHash == currentAnimatorStateInfo.tagHash)
			{
				return currentAnimatorStateInfo.normalizedTime <= 1f;
			}
			if (tagHash == nextAnimatorStateInfo.tagHash)
			{
				return nextAnimatorStateInfo.normalizedTime <= 1f;
			}
		}
		return false;
	}

	private void SetAnimatorTrigger(string trigger)
	{
		if (!(GetAnimator() == null) && !string.IsNullOrEmpty(trigger))
		{
			GetAnimator().SetTrigger(trigger);
		}
	}

	private void OnDestroy()
	{
		DieAnimController.endAnimationEvent -= OnDieEndAnimation;
		AttackAnimSpeedController.hitRegistrationEvent -= OnHitRegistration;
		if (delayedHitTarget != null)
		{
			delayedHitTarget.changeLifeStateEvent -= OnDelayedHitTargetChangeLifeState;
		}
	}

	private void ChangeState(LifeState newState)
	{
		if (newState != CurLifeState)
		{
			CurLifeState = newState;
			this.changeLifeStateEvent?.Invoke(CurLifeState);
		}
	}

	public virtual void TakeDamage(TakeDamageType takeDamageType, float damage, ActorCombat attacker)
	{
		if (damage == 0f || CurLifeState == LifeState.Dying || CurLifeState == LifeState.Dead)
		{
			return;
		}
		CurHealth = Mathf.Clamp(CurHealth - damage, 0f, MaxHealth);
		if (CurHealth > 0f)
		{
			switch (takeDamageType)
			{
			case TakeDamageType.AttackedByEnemy:
				if (!IsAnimationPlaying(GetAnimationHashes(attackAnimationNames)) && !IsAnimationPlaying(AnimationHashes.takeDamage.Value))
				{
					SetAnimatorTrigger(takeDamageTriggers.Random());
				}
				break;
			}
		}
		else
		{
			ChangeState(LifeState.Dying);
			animatorOnStartDie = GetAnimator();
			SetAnimatorTrigger(dieTriggers.Random());
			DieAnimController.endAnimationEvent += OnDieEndAnimation;
		}
		this.takeDamageEvent?.Invoke(takeDamageType, damage, attacker);
	}

	private void OnDieEndAnimation(Animator animator)
	{
		if (GetAnimator() == animator)
		{
			animatorOnStartDie = null;
			DieAnimController.endAnimationEvent -= OnDieEndAnimation;
			ChangeState(LifeState.Dead);
		}
	}

	public void Heal(float healPower)
	{
		if (CurLifeState != LifeState.Dying && CurLifeState != LifeState.Dead)
		{
			CurHealth = Mathf.Clamp(CurHealth + healPower, 0f, MaxHealth);
			this.healEvent?.Invoke();
		}
	}

	public void Respawn()
	{
		if (CurLifeState == LifeState.Dying || CurLifeState == LifeState.Dead)
		{
			ChangeState(LifeState.Alive);
			Heal(MaxHealth);
			SetAnimatorTrigger(respawnTriggers.Random());
			this.respawnEvent?.Invoke();
		}
	}

	protected virtual void Update()
	{
		if (attackTimer > 0f)
		{
			attackTimer = Mathf.Clamp(attackTimer - Time.deltaTime, 0f, 2.14748365E+09f);
		}
		if (IsAttackAnimationPlaying && GetAnimator() != animatorOnStartAttack)
		{
			OnAttackEndAnimation(GetAnimator());
		}
		if (animatorOnStartDie != null && GetAnimator() != animatorOnStartDie)
		{
			OnDieEndAnimation(animatorOnStartDie);
		}
	}

	public bool IsAttackTime()
	{
		if (attackTimer == 0f)
		{
			return !IsAttackAnimationPlaying;
		}
		return false;
	}

	public virtual void Attack(ActorCombat target, Action attackAnimationEndCallback, bool oneShotHitPower = false)
	{
		if (!IsAttackTime())
		{
			return;
		}
		animatorOnStartAttack = GetAnimator();
		AttackAnimSpeedController.startAnimationEvent += OnAttackStartAnimation;
		AttackAnimSpeedController.endAnimationEvent += OnAttackEndAnimation;
		this.attackAnimationEndCallback = attackAnimationEndCallback;
		SetAnimatorTrigger(GetPriorityAttackTrigger());
		this.attackEvent?.Invoke();
		if (target != null)
		{
			AttackAnimSpeedController.hitRegistrationEvent += OnHitRegistration;
			if (delayedHitTarget != null)
			{
				delayedHitTarget.changeLifeStateEvent -= OnDelayedHitTargetChangeLifeState;
			}
			delayedHitTarget = target;
			delayedHitTarget.changeLifeStateEvent += OnDelayedHitTargetChangeLifeState;
			if (oneShotHitPower)
			{
				delayedHitPower = float.MaxValue;
			}
			else
			{
				delayedHitPower = AttackPower;
			}
		}
		attackTimer = AttackFrequency;
	}

	private void OnAttackStartAnimation(Animator animator)
	{
		if (animator == GetAnimator())
		{
			IsAttackAnimationPlaying = true;
		}
	}

	private void OnHitRegistration(Animator animator)
	{
		if (!(animator == GetAnimator()))
		{
			return;
		}
		AttackAnimSpeedController.hitRegistrationEvent -= OnHitRegistration;
		if (delayedHitTarget != null)
		{
			delayedHitTarget.changeLifeStateEvent -= OnDelayedHitTargetChangeLifeState;
			LifeState curLifeState = delayedHitTarget.CurLifeState;
			delayedHitTarget.TakeDamage(TakeDamageType.AttackedByEnemy, delayedHitPower, this);
			this.hitEvent?.Invoke(this, delayedHitTarget);
			if (curLifeState == LifeState.Alive && delayedHitTarget.CurLifeState != 0)
			{
				ActorCombat.killEvent?.Invoke(this, delayedHitTarget);
			}
			delayedHitTarget = null;
		}
	}

	private void OnDelayedHitTargetChangeLifeState(LifeState state)
	{
		if (state == LifeState.Dying)
		{
			AttackAnimSpeedController.hitRegistrationEvent -= OnHitRegistration;
			if (delayedHitTarget != null)
			{
				delayedHitTarget.changeLifeStateEvent -= OnDelayedHitTargetChangeLifeState;
			}
			delayedHitTarget = null;
		}
	}

	private void OnAttackEndAnimation(Animator animator)
	{
		if (animator == GetAnimator())
		{
			animatorOnStartAttack = null;
			if (delayedHitTarget != null)
			{
				delayedHitTarget.changeLifeStateEvent -= OnDelayedHitTargetChangeLifeState;
			}
			AttackAnimSpeedController.hitRegistrationEvent -= OnHitRegistration;
			delayedHitTarget = null;
			AttackAnimSpeedController.startAnimationEvent -= OnAttackStartAnimation;
			AttackAnimSpeedController.endAnimationEvent -= OnAttackEndAnimation;
			attackAnimationEndCallback?.Invoke();
			attackAnimationEndCallback = null;
			IsAttackAnimationPlaying = false;
		}
	}

	protected string GetPriorityAttackTrigger()
	{
		if (AttackTriggers.Count == 1)
		{
			return AttackTriggers[0].trigger;
		}
		AttackTriggerInfo attackTriggerInfo = AttackTriggers.FindAll((AttackTriggerInfo x) => !x.isUsed).Random();
		attackTriggerInfo.isUsed = true;
		if (AttackTriggers.Count((AttackTriggerInfo x) => x.isUsed) == AttackTriggers.Count)
		{
			AttackTriggers.ForEach(delegate(AttackTriggerInfo x)
			{
				x.isUsed = false;
			});
		}
		return attackTriggerInfo.trigger;
	}

	private IEnumerator AnimationTimer(List<string> animationNames, Action animationEndCallback, float timeoutTime)
	{
		if (animationNames == null || animationNames.Count == 0)
		{
			animationEndCallback?.Invoke();
			yield break;
		}
		List<int> animationHashes = GetAnimationHashes(animationNames);
		float timeoutTimer = timeoutTime;
		while (!IsAnimationPlaying(animationHashes) && timeoutTimer > 0f && GetAnimator().enabled)
		{
			yield return null;
			timeoutTimer -= Time.deltaTime;
		}
		while (IsAnimationPlaying(animationHashes) && timeoutTimer > 0f && GetAnimator().enabled)
		{
			yield return null;
			timeoutTimer -= Time.deltaTime;
		}
		animationTimerCor = null;
		animationEndCallback?.Invoke();
	}
}
