using Avelog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerCombat : ActorCombat, IInitializablePlayerComponent
{
	public class EnemyForAttackInfo
	{
		public EnemyController enemyController;

		public ActorCombat enemyCombat;

		public float hpPart;

		public float priority;

		public EnemyForAttackInfo(EnemyController enemyController)
		{
			this.enemyController = enemyController;
			enemyCombat = enemyController.MyCombat;
			hpPart = enemyController.MyCombat.CurHealth / enemyController.MyCombat.MaxHealth;
			Transform transform = PlayerSpawner.PlayerInstance.transform;
			float num = Vector3.Angle(enemyController.ModelTransform.position - transform.position, transform.forward) / ManagerBase<PlayerManager>.Instance.MaxAngleToAttack;
			float num2 = (enemyController.ModelTransform.position - transform.position).sqrMagnitude / (ManagerBase<PlayerManager>.Instance.MaxDistanceToAttack * ManagerBase<PlayerManager>.Instance.MaxDistanceToAttack);
			priority = num * ManagerBase<PlayerManager>.Instance.AngleWeight + num2 * ManagerBase<PlayerManager>.Instance.DistanceWeight;
		}
	}

	[Serializable]
	public class AggressionZoneAngles
	{
		public float smallerAngle;

		public float largerAngle;
	}

	[Serializable]
	private class AggressionTarget
	{
		[Flags]
		public enum Zone
		{
			None = 0x0,
			Pursuit = 0x1,
			Escape = 0x2,
			AttackZoneMask = 0x1
		}

		public EnemyController enemyController;

		public EnemyFieldOfView enemyFieldOfView;

		public Zone zone;

		public float zoneInfluencePart;

		public bool inCombat;

		public ActorCombat ActorCombat => enemyController.MyCombat;

		public AggressionTarget(EnemyController enemyController)
		{
			this.enemyController = enemyController;
			enemyFieldOfView = enemyController.GetComponentInChildren<EnemyFieldOfView>(includeInactive: true);
			zone = Zone.None;
			zoneInfluencePart = 0f;
			inCombat = false;
		}
	}

	public delegate void ChangeInCombatStateHandler(bool inCombat);

	[Serializable]
	public abstract class Ability
	{
		protected Animator animator;

		public bool IsActive
		{
			get;
			private set;
		}

		public void Use(Animator animator)
		{
			this.animator = animator;
			IsActive = true;
			OnUse();
		}

		protected abstract void OnUse();

		public void Interrupt()
		{
			IsActive = false;
			OnInterrupt();
		}

		protected abstract void OnInterrupt();
	}

	[Serializable]
	public class InvisibilityAbility : Ability
	{
		public delegate void SwitchInvisibilityHandler(bool state);

		[SerializeField]
		private float invisibilityAttackPowerMulty = 3f;

		public float InvisibilityAttackPowerMulty => invisibilityAttackPowerMulty;

		public static event SwitchInvisibilityHandler switchInvisibilityEvent;

		protected override void OnUse()
		{
			animator.SetBool("invisibility", value: true);
			PlayerSpawner.PlayerInstance.PlayerFamilyController.Invisibility();
			InvisibilityAbility.switchInvisibilityEvent?.Invoke(state: true);
			PlayerSpawner.PlayerInstance.PlayerCombat.UpdateTargetFOW();
		}

		protected override void OnInterrupt()
		{
			animator.SetBool("invisibility", value: false);
			PlayerSpawner.PlayerInstance.PlayerFamilyController.CancelInvisibility();
			InvisibilityAbility.switchInvisibilityEvent?.Invoke(state: false);
			PlayerSpawner.PlayerInstance.PlayerCombat.UpdateTargetFOW();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass96_0
	{
		public EnemyController enemyController;

		internal bool _003COnEnemyControllerChangeState_003Eb__0(AggressionTarget x)
		{
			return x.enemyController == enemyController;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass97_0
	{
		public EnemyController enemyController;

		internal bool _003CRemoveAggressionTarget_003Eb__0(AggressionTarget x)
		{
			return x.enemyController == enemyController;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass98_0
	{
		public ActorCombat target;

		internal bool _003CAggression_OnAttack_003Eb__0(AggressionTarget x)
		{
			return x.ActorCombat == target;
		}
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Func<AggressionTarget, bool> _003C_003E9__99_0;

		public static Comparison<(AggressionTarget aggressionTarget, float sqrDistance)> _003C_003E9__101_2;

		public static Action<AggressionTarget> _003C_003E9__101_0;

		public static Predicate<Collider> _003C_003E9__102_1;

		public static Comparison<EnemyForAttackInfo> _003C_003E9__102_4;

		public static Comparison<EnemyForAttackInfo> _003C_003E9__102_6;

		public static Predicate<EnemyController> _003C_003E9__103_0;

		public static Predicate<EnemyController> _003C_003E9__103_1;

		public static Comparison<(EnemyController enemy, float distance)> _003C_003E9__103_2;

		public static Predicate<AggressionTarget> _003C_003E9__104_0;

		public static Func<AggressionTarget, float> _003C_003E9__104_1;

		internal bool _003CUpdateCombatState_003Eb__99_0(AggressionTarget x)
		{
			return x.inCombat;
		}

		internal int _003CUpdateTargetFOW_003Eb__101_2((AggressionTarget aggressionTarget, float sqrDistance) x, (AggressionTarget aggressionTarget, float sqrDistance) y)
		{
			return x.sqrDistance.CompareTo(y.sqrDistance);
		}

		internal void _003CUpdateTargetFOW_003Eb__101_0(AggressionTarget x)
		{
			x.enemyFieldOfView.Switch(value: false);
		}

		internal bool _003CFindTargetForAttack_003Eb__102_1(Collider x)
		{
			return x.GetComponentInChildren<EnemyModel>().EnemyController.MyCombat.CurLifeState != LifeState.Alive;
		}

		internal int _003CFindTargetForAttack_003Eb__102_4(EnemyForAttackInfo o1, EnemyForAttackInfo o2)
		{
			return o1.hpPart.CompareTo(o2.hpPart);
		}

		internal int _003CFindTargetForAttack_003Eb__102_6(EnemyForAttackInfo o1, EnemyForAttackInfo o2)
		{
			return o1.priority.CompareTo(o2.priority);
		}

		internal bool _003CFindTargetForPursuit_003Eb__103_0(EnemyController enemy)
		{
			return enemy.CurState == EnemyController.State.Escape;
		}

		internal bool _003CFindTargetForPursuit_003Eb__103_1(EnemyController x)
		{
			return !x.EnemyModel.Renderer.isVisible;
		}

		internal int _003CFindTargetForPursuit_003Eb__103_2((EnemyController enemy, float distance) o1, (EnemyController enemy, float distance) o2)
		{
			return o1.distance.CompareTo(o2.distance);
		}

		internal bool _003CCalcLargerZoneInfluencePart_003Eb__104_0(AggressionTarget x)
		{
			return x.zone != AggressionTarget.Zone.None;
		}

		internal float _003CCalcLargerZoneInfluencePart_003Eb__104_1(AggressionTarget x)
		{
			return x.zoneInfluencePart;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass101_0
	{
		public PlayerCombat _003C_003E4__this;

		public List<(AggressionTarget aggressionTarget, float sqrDistance)> targetDistances;

		internal void _003CUpdateTargetFOW_003Eb__1(AggressionTarget x)
		{
			targetDistances.Add((x, (x.enemyController.ModelTransform.position - _003C_003E4__this.transform.position).sqrMagnitude));
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass102_0
	{
		public float minHpPart;

		internal bool _003CFindTargetForAttack_003Eb__5(EnemyForAttackInfo x)
		{
			return x.hpPart == minHpPart;
		}
	}

	[Header("PlayerCombat")]
	[SerializeField]
	private List<AttackTriggerInfo> walkAttackTriggers = new List<AttackTriggerInfo>
	{
		new AttackTriggerInfo("WalkingAttack")
	};

	[SerializeField]
	private List<AttackTriggerInfo> runAttackTriggers = new List<AttackTriggerInfo>
	{
		new AttackTriggerInfo("RunningAttack")
	};

	[SerializeField]
	private List<AttackTriggerInfo> creepAttackTriggers = new List<AttackTriggerInfo>
	{
		new AttackTriggerInfo("CreepingAttack")
	};

	[SerializeField]
	private string walkingStateName = "Walking";

	[SerializeField]
	private string runningStateName = "Running";

	[SerializeField]
	private string flyingStateName = "Flying";

	[SerializeField]
	private GameObject attackBox;

	[SerializeField]
	private GameObject stealthAttackBox;

	[Header("Агрессия")]
	[SerializeField]
	private bool drawZonesGizmo = true;

	[Header("Отладка")]
	[SerializeField]
	private List<AggressionTarget> potentialAggressionTargets = new List<AggressionTarget>();

	[SerializeField]
	private List<EnemyForAttackInfo> enemyForAttackInfos = new List<EnemyForAttackInfo>();

	[SerializeField]
	private List<EnemyForAttackInfo> enemyForAttackInBoxInfos = new List<EnemyForAttackInfo>();

	private float updateAggressionTargetsTimer;

	[SerializeField]
	private Color pursuitZoneColor;

	[SerializeField]
	private Color escapeZoneColor;

	private Mesh pursuitZoneMaxInfluenceMesh;

	private Mesh pursuitZoneMinInfluenceMesh;

	private Mesh escapeZoneMaxInfluenceMesh;

	private Mesh escapeZoneMinInfluenceMesh;

	protected override List<AttackTriggerInfo> AttackTriggers
	{
		get
		{
			if (ManagerBase<PlayerManager>.Instance.curSpeed == 0f && !IsInvisibilityActive())
			{
				return base.AttackTriggers;
			}
			AnimatorStateInfo currentAnimatorStateInfo = GetAnimator().GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName(walkingStateName) || (currentAnimatorStateInfo.IsName(flyingStateName) && GetAnimator().GetFloat("MovementSpeed") > 0f))
			{
				return walkAttackTriggers;
			}
			if (currentAnimatorStateInfo.IsName(runningStateName))
			{
				return runAttackTriggers;
			}
			if (IsInvisibilityActive())
			{
				return creepAttackTriggers;
			}
			return base.AttackTriggers;
		}
	}

	private GameObject CurAttackBox
	{
		get
		{
			if (!Invisibility.IsActive)
			{
				return attackBox;
			}
			return stealthAttackBox;
		}
	}

	private PlayerMovement PlayerMovement => PlayerSpawner.PlayerInstance.PlayerMovement;

	protected override float AttackPower
	{
		get
		{
			if (!Invisibility.IsActive)
			{
				return ManagerBase<PlayerManager>.Instance.HitPower * ManagerBase<PlayerManager>.Instance.HitFrequency;
			}
			return ManagerBase<PlayerManager>.Instance.HitPower;
		}
	}

	public override float MaxHealth => ManagerBase<PlayerManager>.Instance.HealthMaximum;

	public override float CurHealth
	{
		get
		{
			return ManagerBase<PlayerManager>.Instance.healthCurrent;
		}
		protected set
		{
			base.CurHealth = value;
			ManagerBase<PlayerManager>.Instance.healthCurrent = value;
		}
	}

	protected override float AttackFrequency => ManagerBase<PlayerManager>.Instance.HitFrequency;

	public bool DrawZonesGizmo => drawZonesGizmo;

	public bool HavePursuitedEnemy => PursuitedEnemy != null;

	public EnemyController PursuitedEnemy
	{
		get;
		private set;
	}

	public bool HaveEnemyInAttackBox
	{
		get;
		private set;
	}

	public AggressionZoneAngles PursuitZoneAngles => ManagerBase<PlayerManager>.Instance.PursuitZoneAngles;

	public AggressionZoneAngles EscapeZoneAngles => ManagerBase<PlayerManager>.Instance.EscapeZoneAngles;

	public List<EnemyForAttackInfo> EnemyForAttackInBoxInfos => enemyForAttackInBoxInfos;

	public float LargerZoneInfluencePart
	{
		get;
		private set;
	}

	public ActorCombat LastAttackedTarget
	{
		get;
		private set;
	}

	public ActorCombat RecommendedTargetForAttack
	{
		get;
		private set;
	}

	public ActorCombat TargetForAttackInBox
	{
		get;
		private set;
	}

	public bool AttackTargetChanged
	{
		get;
		private set;
	}

	public bool InCombat
	{
		get;
		private set;
	}

	public Color PursuitZoneColor => pursuitZoneColor;

	public Color EscapeZoneColor => escapeZoneColor;

	public bool IsHearable => !Invisibility.IsActive;

	private InvisibilityAbility Invisibility => ManagerBase<PlayerManager>.Instance.Invisibility;

	public event Action changeHaveEnemyInAttackBoxEvent;

	public static event Action deathEvent;

	public static event ChangeInCombatStateHandler changeInCombatStateEvent;

	public new void Initialize()
	{
		base.Initialize();
		base.changeLifeStateEvent += OnChangeLifeState;
		SpecialAbility_OnInit();
		Aggression_OnInit();
	}

	private void OnChangeLifeState(LifeState state)
	{
		if (state == LifeState.Dead)
		{
			ManagerBase<PlayerManager>.Instance.deathsCount++;
			PlayerCombat.deathEvent?.Invoke();
		}
	}

	private void OnDestroy()
	{
		SpecialAbility_OnDestroy();
		Aggression_OnDestroy();
	}

	protected override Animator GetAnimator()
	{
		return PlayerSpawner.PlayerInstance.Model.Animator;
	}

	public void AttackBox(Action animationEndCallback)
	{
		if (IsAttackTime())
		{
			if (IsInvisibilityActive())
			{
				PlayerMovement.ResetVelocity();
			}
			Attack(TargetForAttackInBox, animationEndCallback);
		}
	}

	public override void Attack(ActorCombat target, Action attackAnimationEndCallback, bool oneShotHitPower = false)
	{
		if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Pixibob && target != null && target.CurHealth / target.MaxHealth < ManagerBase<SkinManager>.Instance.PixibobEnemyHealthPerc)
		{
			oneShotHitPower = true;
		}
		base.Attack(target, attackAnimationEndCallback, oneShotHitPower);
		Aggression_OnAttack(target);
		SpecialAbility_OnAttack();
	}

	public void Kill(ActorCombat target)
	{
		target.TakeDamage(TakeDamageType.AttackedByEnemy, target.MaxHealth, this);
		TriggerKillEnemyEvent(target);
	}

	public override void TakeDamage(TakeDamageType takeDamageType, float damage, ActorCombat attacker)
	{
		if (ManagerBase<PlayerManager>.Instance.isSleeping && CurHealth - damage <= 0f)
		{
			damage = CurHealth - 1f;
		}
		base.TakeDamage(takeDamageType, damage, attacker);
		SpecialAbility_OnTakeDamage(takeDamageType);
	}

	protected override void Update()
	{
		base.Update();
		AggressionUpdate();
		SpecialAbility_Update();
	}

	private void Aggression_OnInit()
	{
		if (Singleton<EnemySpawner>.Instance != null)
		{
			foreach (EnemySpawner.EnemyInstance spawnedEnemy in Singleton<EnemySpawner>.Instance.SpawnedEnemies)
			{
				OnEnemySpawn(spawnedEnemy.logic.controller);
			}
		}
		EnemySpawner.spawnEvent += OnEnemySpawn;
		EnemySpawner.unspawnEvent += OnEnemyUnspawn;
		EnemyController.changeStateEvent += OnEnemyControllerChangeState;
	}

	private void Aggression_OnDestroy()
	{
		EnemySpawner.spawnEvent -= OnEnemySpawn;
		EnemySpawner.unspawnEvent -= OnEnemyUnspawn;
		EnemyController.changeStateEvent -= OnEnemyControllerChangeState;
	}

	private void OnEnemySpawn(EnemyController enemyController)
	{
		enemyController.GetComponentInChildren<ActorCombat>();
		potentialAggressionTargets.Add(new AggressionTarget(enemyController));
	}

	private void OnEnemyUnspawn(EnemyController enemyController)
	{
		RemoveAggressionTarget(enemyController);
	}

	private void OnEnemyControllerChangeState(EnemyController enemyController, EnemyController.State newState)
	{
		AggressionTarget aggressionTarget = potentialAggressionTargets.Find((AggressionTarget x) => x.enemyController == enemyController);
		if (aggressionTarget != null)
		{
			aggressionTarget.inCombat = (base.CurLifeState == LifeState.Alive && aggressionTarget.ActorCombat.CurLifeState == LifeState.Alive && (aggressionTarget.enemyController.CurState == EnemyController.State.Pursuit || aggressionTarget.enemyController.CurState == EnemyController.State.Attack || aggressionTarget.enemyController.CurState == EnemyController.State.Escape));
		}
		UpdateCombatState();
	}

	private void RemoveAggressionTarget(EnemyController enemyController)
	{
		AggressionTarget aggressionTarget = potentialAggressionTargets.Find((AggressionTarget x) => x.enemyController == enemyController);
		if (aggressionTarget != null)
		{
			potentialAggressionTargets.Remove(aggressionTarget);
			aggressionTarget.enemyFieldOfView.Switch(value: false);
			UpdateCombatState();
		}
	}

	private void Aggression_OnAttack(ActorCombat target)
	{
		if (target == null)
		{
			LastAttackedTarget = null;
			return;
		}
		AttackTargetChanged = (HaveEnemyInAttackBox && TargetForAttackInBox != PursuitedEnemy?.MyCombat);
		AggressionTarget aggressionTarget = potentialAggressionTargets.Find((AggressionTarget x) => x.ActorCombat == target);
		aggressionTarget.inCombat = (aggressionTarget.ActorCombat.CurLifeState == LifeState.Alive);
		if (aggressionTarget != null && aggressionTarget.inCombat)
		{
			LastAttackedTarget = target;
		}
		else
		{
			LastAttackedTarget = null;
		}
		UpdateCombatState();
	}

	private void UpdateCombatState()
	{
		bool inCombat = InCombat;
		InCombat = potentialAggressionTargets.Any((AggressionTarget x) => x.inCombat);
		PlayerSpawner.PlayerInstance.Model.Animator.SetBool("InCombat", InCombat);
		CalcLargerZoneInfluencePart();
		if (inCombat != InCombat)
		{
			PlayerCombat.changeInCombatStateEvent?.Invoke(InCombat);
		}
	}

	private void AggressionUpdate()
	{
		FindTargetForPursuit();
		FindTargetForAttack();
		if (updateAggressionTargetsTimer > 0f)
		{
			updateAggressionTargetsTimer -= Time.deltaTime;
			return;
		}
		updateAggressionTargetsTimer = 0.05f;
		foreach (AggressionTarget potentialAggressionTarget in potentialAggressionTargets)
		{
			potentialAggressionTarget.zone = AggressionTarget.Zone.None;
			potentialAggressionTarget.zoneInfluencePart = 0f;
			if (potentialAggressionTarget.ActorCombat.CurLifeState == LifeState.Alive)
			{
				Vector3 vector = potentialAggressionTarget.ActorCombat.transform.position - base.transform.position;
				new Vector3(vector.x, 0f, vector.z);
				if (potentialAggressionTarget.enemyController.CurState == EnemyController.State.Pursuit || potentialAggressionTarget.enemyController.CurState == EnemyController.State.Attack)
				{
					(bool, float) valueTuple = _003CAggressionUpdate_003Eg__CheckAggressionZone_007C100_0(potentialAggressionTarget, -base.transform.forward, EscapeZoneAngles);
					if (valueTuple.Item1)
					{
						potentialAggressionTarget.zone |= AggressionTarget.Zone.Escape;
						potentialAggressionTarget.zoneInfluencePart = valueTuple.Item2;
					}
				}
				if (potentialAggressionTarget.zone != AggressionTarget.Zone.Pursuit && potentialAggressionTarget.enemyController.CurState == EnemyController.State.Escape)
				{
					(bool, float) valueTuple2 = _003CAggressionUpdate_003Eg__CheckAggressionZone_007C100_0(potentialAggressionTarget, base.transform.forward, PursuitZoneAngles);
					if (valueTuple2.Item1)
					{
						potentialAggressionTarget.zone |= AggressionTarget.Zone.Pursuit;
						potentialAggressionTarget.zoneInfluencePart = valueTuple2.Item2;
					}
				}
			}
		}
		CalcLargerZoneInfluencePart();
		UpdateTargetFOW();
	}

	private void UpdateTargetFOW()
	{
		if (IsInvisibilityActive())
		{
			List<(AggressionTarget aggressionTarget, float sqrDistance)> targetDistances = new List<(AggressionTarget, float)>();
			potentialAggressionTargets.ForEach(delegate(AggressionTarget x)
			{
				targetDistances.Add((x, (x.enemyController.ModelTransform.position - base.transform.position).sqrMagnitude));
			});
			targetDistances.Sort(((AggressionTarget aggressionTarget, float sqrDistance) x, (AggressionTarget aggressionTarget, float sqrDistance) y) => x.sqrDistance.CompareTo(y.sqrDistance));
			for (int i = 0; i < targetDistances.Count; i++)
			{
				targetDistances[i].aggressionTarget.enemyFieldOfView.Switch(i < ManagerBase<PlayerManager>.Instance.EnemyFieldOfViewCount);
			}
		}
		else
		{
			potentialAggressionTargets.ForEach(delegate(AggressionTarget x)
			{
				x.enemyFieldOfView.Switch(value: false);
			});
		}
	}

	private void FindTargetForAttack()
	{
		List<Collider> list = new List<Collider>(Physics.OverlapBox(CurAttackBox.transform.position, CurAttackBox.transform.localScale / 2f, CurAttackBox.transform.rotation, 1 << Layers.TriggerLayer));
		list.RemoveAll((Collider x) => (!(x.tag != "AI")) ? (x.gameObject.name == base.gameObject.name) : true);
		list.RemoveAll((Collider x) => x.GetComponentInChildren<EnemyModel>().EnemyController.MyCombat.CurLifeState != LifeState.Alive);
		if (HaveEnemyInAttackBox != list.Count > 0)
		{
			HaveEnemyInAttackBox = (list.Count > 0);
			this.changeHaveEnemyInAttackBoxEvent?.Invoke();
		}
		enemyForAttackInfos.Clear();
		enemyForAttackInBoxInfos.Clear();
		list.ForEach(delegate(Collider enemyCol)
		{
			enemyForAttackInfos.Add(new EnemyForAttackInfo(enemyCol.GetComponentInChildren<EnemyModel>().EnemyController));
		});
		enemyForAttackInBoxInfos.AddRange(enemyForAttackInfos);
		TargetForAttackInBox = _003CFindTargetForAttack_003Eg__GetBestAttackTarget_007C102_3();
		if (HavePursuitedEnemy)
		{
			if (!AttackTargetChanged)
			{
				RecommendedTargetForAttack = PursuitedEnemy.MyCombat;
				return;
			}
			AttackTargetChanged = (LastAttackedTarget != null && LastAttackedTarget.CurLifeState != LifeState.Dead);
			if (AttackTargetChanged)
			{
				RecommendedTargetForAttack = LastAttackedTarget;
			}
			else
			{
				RecommendedTargetForAttack = PursuitedEnemy.MyCombat;
			}
		}
		else
		{
			if (AttackTargetChanged)
			{
				AttackTargetChanged = false;
			}
			enemyForAttackInfos.Clear();
			foreach (AggressionTarget potentialAggressionTarget in potentialAggressionTargets)
			{
				if (potentialAggressionTarget.ActorCombat.CurLifeState == LifeState.Alive && potentialAggressionTarget.enemyController.EnemyModel.Renderer.isVisible)
				{
					enemyForAttackInfos.Add(new EnemyForAttackInfo(potentialAggressionTarget.enemyController));
				}
			}
			RecommendedTargetForAttack = _003CFindTargetForAttack_003Eg__GetBestAttackTarget_007C102_3();
		}
	}

	private void FindTargetForPursuit()
	{
		List<EnemyController> list = EnemyController.Instances.FindAll((EnemyController enemy) => enemy.CurState == EnemyController.State.Escape);
		list.RemoveAll((EnemyController x) => !x.EnemyModel.Renderer.isVisible);
		if (list.Count == 0)
		{
			PursuitedEnemy = null;
			return;
		}
		if (list.Count == 1)
		{
			PursuitedEnemy = list[0];
			return;
		}
		List<(EnemyController, float)> list2 = new List<(EnemyController, float)>();
		foreach (EnemyController item in list)
		{
			list2.Add((item, (item.ModelTransform.position - base.transform.position).sqrMagnitude));
		}
		list2.Sort((Comparison<(EnemyController, float)>)(((EnemyController enemy, float distance) o1, (EnemyController enemy, float distance) o2) => o1.distance.CompareTo(o2.distance)));
		PursuitedEnemy = list2[0].Item1;
	}

	private void CalcLargerZoneInfluencePart()
	{
		List<AggressionTarget> list = potentialAggressionTargets.FindAll((AggressionTarget x) => x.zone != AggressionTarget.Zone.None);
		if (list.Count == 0)
		{
			LargerZoneInfluencePart = 0f;
		}
		else
		{
			LargerZoneInfluencePart = list.Max((AggressionTarget x) => x.zoneInfluencePart);
		}
	}

	private void OnDrawGizmos()
	{
		if (drawZonesGizmo)
		{
			Gizmos.color = pursuitZoneColor;
			Gizmos.DrawMesh(GizmosUtils.GetTriangleMesh(ref pursuitZoneMaxInfluenceMesh, PursuitZoneAngles.smallerAngle, Vector3.forward), base.transform.position + Vector3.up * 0.01f, base.transform.rotation, Vector3.one * 10f);
			Gizmos.DrawMesh(GizmosUtils.GetTriangleMesh(ref pursuitZoneMinInfluenceMesh, PursuitZoneAngles.largerAngle, Vector3.forward), base.transform.position, base.transform.rotation, Vector3.one * 10f);
			Gizmos.color = escapeZoneColor;
			Gizmos.DrawMesh(GizmosUtils.GetTriangleMesh(ref escapeZoneMaxInfluenceMesh, EscapeZoneAngles.smallerAngle, -Vector3.forward), base.transform.position + Vector3.up * 0.01f, base.transform.rotation, Vector3.one * 10f);
			Gizmos.DrawMesh(GizmosUtils.GetTriangleMesh(ref escapeZoneMinInfluenceMesh, EscapeZoneAngles.largerAngle, -Vector3.forward), base.transform.position, base.transform.rotation, Vector3.one * 10f);
		}
	}

	private void SpecialAbility_OnInit()
	{
		PlayerSpawner.PlayerInstance.PlayerMovement.jumpEvent += SpecialAbility_OnJump;
		PlayerSpawner.PlayerInstance.beforeStartCommandEvent += OnBeforeStartCommand;
		changeInCombatStateEvent += OnChangeInCombatState;
	}

	private void SpecialAbility_OnDestroy()
	{
		PlayerSpawner.PlayerInstance.PlayerMovement.jumpEvent -= SpecialAbility_OnJump;
		PlayerSpawner.PlayerInstance.beforeStartCommandEvent -= OnBeforeStartCommand;
		changeInCombatStateEvent -= OnChangeInCombatState;
	}

	private void SpecialAbility_OnJump()
	{
		if (Invisibility.IsActive)
		{
			Invisibility.Interrupt();
		}
	}

	private void OnBeforeStartCommand(CommandBase command)
	{
		if (IsInvisibilityActive() && command.CommandId != CommandId.Invisibility && command.CommandId != CommandId.Attack && command.CommandId != CommandId.Pick && command.CommandId != CommandId.Drop && command.CommandId != CommandId.Move)
		{
			Invisibility.Interrupt();
		}
	}

	private void OnChangeInCombatState(bool inCombat)
	{
		if (inCombat && Invisibility.IsActive)
		{
			Invisibility.Interrupt();
		}
	}

	private void SpecialAbility_Update()
	{
	}

	private void SpecialAbility_OnAttack()
	{
		if (Invisibility.IsActive)
		{
			Invisibility.Interrupt();
		}
	}

	private void SpecialAbility_OnTakeDamage(TakeDamageType takeDamageType)
	{
		if (takeDamageType == TakeDamageType.AttackedByEnemy && Invisibility.IsActive)
		{
			Invisibility.Interrupt();
		}
	}

	public bool IsInvisibilityActive()
	{
		return Invisibility.IsActive;
	}

	public bool CanUseInvisibility()
	{
		if (base.CurLifeState == LifeState.Alive && !InCombat)
		{
			return !Invisibility.IsActive;
		}
		return false;
	}

	public void SwitchInvisibility()
	{
		if (Invisibility.IsActive)
		{
			Invisibility.Interrupt();
		}
		else if (CanUseInvisibility())
		{
			Invisibility.Use(GetAnimator());
		}
	}

	[CompilerGenerated]
	private (bool inAggressionZone, float zoneInfluencePart) _003CAggressionUpdate_003Eg__CheckAggressionZone_007C100_0(AggressionTarget aggressionTarget, Vector3 zoneDirection, AggressionZoneAngles aggressionZoneAngles)
	{
		if (aggressionTarget == null)
		{
			return (false, 0f);
		}
		Vector3 to = new Vector3(zoneDirection.x, 0f, zoneDirection.z);
		Vector3 vector = aggressionTarget.ActorCombat.transform.position - base.transform.position;
		float num = Vector3.Angle(new Vector3(vector.x, 0f, vector.z), to);
		if (!(num <= aggressionZoneAngles.largerAngle / 2f))
		{
			return (false, 0f);
		}
		float item = 1f - (Mathf.Clamp(num, aggressionZoneAngles.smallerAngle / 2f, aggressionZoneAngles.largerAngle / 2f) - aggressionZoneAngles.smallerAngle / 2f) / (aggressionZoneAngles.largerAngle / 2f - aggressionZoneAngles.smallerAngle / 2f);
		return (true, item);
	}

	[CompilerGenerated]
	private ActorCombat _003CFindTargetForAttack_003Eg__GetBestAttackTarget_007C102_3()
	{
		if (enemyForAttackInfos.Count == 0)
		{
			return null;
		}
		enemyForAttackInfos.Sort((EnemyForAttackInfo o1, EnemyForAttackInfo o2) => o1.hpPart.CompareTo(o2.hpPart));
		float minHpPart = enemyForAttackInfos[0].hpPart;
		List<EnemyForAttackInfo> list = enemyForAttackInfos.FindAll((EnemyForAttackInfo x) => x.hpPart == minHpPart);
		if (list.Count == 1)
		{
			return list[0].enemyCombat;
		}
		list.Sort((EnemyForAttackInfo o1, EnemyForAttackInfo o2) => o1.priority.CompareTo(o2.priority));
		return list[0].enemyCombat;
	}
}
