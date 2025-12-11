using Avelog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

public class FamilyMemberController : MonoBehaviour
{
    [Flags]
    public enum State
    {
        Disabled = 0,
        Following = 1,
        WalkingAround = 2,
        Attack = 4,
        Eating = 8,
        Drinking = 16,
        Sleeping = 32,
        Awaking = 64
    }

    public delegate void ChangeStateHandler(State newState);

	public delegate void SpawnHandler(FamilyMemberController familyMemberController);

	private abstract class CommandBase
	{
		private Action endCallback;

		public bool IsExecuting
		{
			get;
			private set;
		}

		public virtual bool CanExecute()
		{
			return true;
		}

		public virtual void Execute(Action endCallback)
		{
			if (!CanExecute())
			{
				endCallback?.Invoke();
				return;
			}
			this.endCallback = endCallback;
			IsExecuting = true;
			OnStartExecute();
		}

		public void Complete()
		{
			OnComplete();
			IsExecuting = false;
			endCallback?.Invoke();
		}

		protected abstract void OnComplete();

		protected abstract void OnStartExecute();
	}

	private class EatCommand : CommandBase
	{
		private FamilyMemberController familyMemberController;

		private Action endCallback;

		public Food Food
		{
			get;
			private set;
		}

		public EatCommand(FamilyMemberController familyMemberController, Food food, Action endCallback)
		{
			this.endCallback = endCallback;
			this.familyMemberController = familyMemberController;
			Food = food;
		}

		protected override void OnStartExecute()
		{
			familyMemberController.ChangeState(State.Eating, familyMemberController.Eating(Food, base.Complete));
		}

		protected override void OnComplete()
		{
			endCallback?.Invoke();
		}
	}

	private class DrinkCommand : CommandBase
	{
		private FamilyMemberController familyMemberController;

		private Vector3 familyDrinkPoint;

		private Water water;

		private DrinkType drinkType;

		public DrinkCommand(FamilyMemberController familyMemberController, Vector3 familyDrinkPoint, DrinkType drinkType, Water water)
		{
			this.familyMemberController = familyMemberController;
			this.familyDrinkPoint = familyDrinkPoint;
			this.water = water;
			this.drinkType = drinkType;
		}

		public override bool CanExecute()
		{
			return familyMemberController.HaveThirst();
		}

		protected override void OnStartExecute()
		{
			familyMemberController.ChangeState(State.Drinking, familyMemberController.Drinking(familyDrinkPoint, drinkType, water, base.Complete));
		}

		protected override void OnComplete()
		{
		}
	}

	private class InvisibilityEnterCommand : CommandBase
	{
		private FamilyMemberController familyMemberController;

		private Animator animator;

		public InvisibilityEnterCommand(FamilyMemberController familyMemberController, Animator animator)
		{
			this.familyMemberController = familyMemberController;
			this.animator = animator;
		}

		protected override void OnStartExecute()
		{
			familyMemberController.IsInvisibilityActive = true;
			animator.SetBool("invisibility", value: true);
			Complete();
		}

		protected override void OnComplete()
		{
		}
	}

	private class InvisibilityExitCommand : CommandBase
	{
		private FamilyMemberController familyMemberController;

		private Animator animator;

		public InvisibilityExitCommand(FamilyMemberController familyMemberController, Animator animator)
		{
			this.familyMemberController = familyMemberController;
			this.animator = animator;
		}

		protected override void OnStartExecute()
		{
			familyMemberController.IsInvisibilityActive = false;
			animator.SetBool("invisibility", value: false);
			Complete();
		}

		protected override void OnComplete()
		{
		}
	}

	private class SleepCommand : CommandBase
	{
		private FamilyMemberController familyMemberController;

		private Vector3 sleepPosition;

		public SleepCommand(FamilyMemberController familyMemberController, Vector3 sleepPosition)
		{
			this.familyMemberController = familyMemberController;
			this.sleepPosition = sleepPosition;
		}

		protected override void OnStartExecute()
		{
			familyMemberController.ChangeState(State.Sleeping, familyMemberController.Sleeping(sleepPosition, base.Complete));
		}

		protected override void OnComplete()
		{
		}
	}

	private class AwakeCommand : CommandBase
	{
		private FamilyMemberController familyMemberController;

		public AwakeCommand(FamilyMemberController familyMemberController)
		{
			this.familyMemberController = familyMemberController;
		}

		protected override void OnStartExecute()
		{
			familyMemberController.ChangeState(State.Awaking, familyMemberController.Awaking(base.Complete));
		}

		protected override void OnComplete()
		{
		}
	}

	private enum FeedingState
	{
		Move,
		Rotate,
		Feed,
		None
	}

	public enum DrinkType
	{
		FromNearestWaterPoint,
		FromDrinkPosition
	}

	public delegate void EndDrinkHandler(FamilyMemberController familyMemberController, int quenchThirstCount);

	public delegate void FamilyMemberEventHandler(FamilyMemberController familyMemberController);

	

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003C_003Ec__DisplayClass49_0
	{
		public FamilyManager.FamilyMemberData familyMemberData;
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Action<Food> _003C_003E9__63_0;

		public static Predicate<FamilyMemberController> _003C_003E9__65_0;

		public static Func<(float time, float speed), float> _003C_003E9__106_0;

		public static Func<ItemId, string> _003C_003E9__158_0;

		public static Func<CommandBase, bool> _003C_003E9__171_0;

		public static Func<CommandBase, EatCommand> _003C_003E9__171_1;

		public static Func<string, bool> _003C_003E9__173_1;

		public static Func<EnemySpawner.EnemyInstance, EnemyArchetype> _003C_003E9__173_3;

		internal void _003CResetController_003Eb__63_0(Food x)
		{
			x.UnholdFoodUnit();
		}

		internal bool _003CSpawn_003Eb__65_0(FamilyMemberController x)
		{
			return x.familyMemberData.role == FamilyManager.FamilyMemberRole.Spouse;
		}

		internal float _003CWalking_Update_003Eb__106_0((float time, float speed) x)
		{
			return x.speed;
		}

		internal string _003Cget_ItemIds_003Eb__158_0(ItemId x)
		{
			return x.ToString();
		}

		internal bool _003CHaveFoodInEatQueue_003Eb__171_0(CommandBase x)
		{
			return x is EatCommand;
		}

		internal EatCommand _003CHaveFoodInEatQueue_003Eb__171_1(CommandBase x)
		{
			return x as EatCommand;
		}

		internal bool _003CGenerateNeed_003Eb__173_1(string x)
		{
			return ItemIds.Contains(x);
		}

		internal EnemyArchetype _003CGenerateNeed_003Eb__173_3(EnemySpawner.EnemyInstance x)
		{
			return x.logic.currentScheme.Archetype;
		}
	}

	[CompilerGenerated]
	private sealed class _003CSpawn_003Ed__65 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int state;

		private object current;

		public FamilyMemberController _003C_003E4__this;

		public Action endCallback;

		private WaitForSeconds _003Cwait_003E5__2;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return current;
			}
		}

		[DebuggerHidden]
		public _003CSpawn_003Ed__65(int state)
		{
			this.state = state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = state;
			FamilyMemberController familyMemberController = _003C_003E4__this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				state = -1;
			}
			else
			{
				state = -1;
				familyMemberController.Model.gameObject.SetActive(value: false);
				_003Cwait_003E5__2 = new WaitForSeconds(1f);
			}
			Vector3 sourcePosition = (!familyMemberController.spawnInFolowingPos) ? ((familyMemberController.familyMemberData.role == FamilyManager.FamilyMemberRole.Spouse) ? Singleton<SpouseSpawner>.Instance.SpawnPoint : Instances.Find((FamilyMemberController x) => x.familyMemberData.role == FamilyManager.FamilyMemberRole.Spouse).transform.position) : familyMemberController.CurFollowPos;
			if (!PlayerSpawner.PlayerInstance.PlayerMovement.IsFalling && NavMeshUtils.SamplePositionIterative(sourcePosition, out NavMeshHit navMeshHit, 5f, 100f, 4, -1))
			{
				familyMemberController.spawnInFolowingPos = true;
				familyMemberController.navAgent.Warp(navMeshHit.position);
				familyMemberController.Model.gameObject.SetActive(value: true);
				if (!Instances.Contains(familyMemberController))
				{
					Instances.Add(familyMemberController);
				}
				FamilyMemberController.spawnEvent?.Invoke(familyMemberController);
				endCallback?.Invoke();
				return false;
			}
			current = _003Cwait_003E5__2;
			state = 1;
			return true;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	[CompilerGenerated]
	private sealed class _003CFollowing_003Ed__102 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int state;

		private object current;

		public FamilyMemberController _003C_003E4__this;

		private float _003CtimeInWalkSpace_003E5__2;

		private WaitForSeconds _003Cwait_003E5__3;

		private float _003CprevTime_003E5__4;

		private Vector3 _003CprevFollowPos_003E5__5;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return current;
			}
		}

		[DebuggerHidden]
		public _003CFollowing_003Ed__102(int state)
		{
			this.state = state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = state;
			FamilyMemberController familyMemberController = _003C_003E4__this;
			NavMeshHit navMeshHit;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				state = -1;
				if (familyMemberController.IsInWalkSpace())
				{
					_003CtimeInWalkSpace_003E5__2 += Time.time - _003CprevTime_003E5__4;
				}
				else
				{
					_003CtimeInWalkSpace_003E5__2 = 0f;
				}
				_003CprevTime_003E5__4 = Time.time;
			}
			else
			{
				state = -1;
				familyMemberController.UpdateAttackStoppingDistanceAndRange();
				familyMemberController.navAgent.acceleration = ManagerBase<FamilyManager>.Instance.Acceleration;
				Vector3 position = familyMemberController.PlayerGO.transform.position;
				_003CtimeInWalkSpace_003E5__2 = 0f;
				float seconds = 0f;
				_003Cwait_003E5__3 = new WaitForSeconds(seconds);
				_003CprevTime_003E5__4 = Time.time;
				navMeshHit = default(NavMeshHit);
				_003CprevFollowPos_003E5__5 = familyMemberController.CurFollowPos;
			}
			if (_003CtimeInWalkSpace_003E5__2 >= ManagerBase<FamilyManager>.Instance.FollowPauseTime)
			{
				familyMemberController.ChangeState(State.WalkingAround, familyMemberController.WalkingAround());
				return false;
			}
			if ((familyMemberController.CurFollowPos - familyMemberController.transform.position).sqrMagnitude < 90000f)
			{
				if (NavMeshUtils.SamplePositionIterative(familyMemberController.CurFollowPos, out navMeshHit, 2f, 100f, 4, -1))
				{
					familyMemberController.navAgent.SetDestination(navMeshHit.position);
					familyMemberController.navAgent.isStopped = false;
				}
			}
			else
			{
				NavMeshUtils.SamplePositionIterative(familyMemberController.PlayerMovement.transform.position - familyMemberController.PlayerMovement.transform.forward * 7f, out NavMeshHit navMeshHit2, 2f, 100f, 4, -1);
				NavMeshPath navMeshPath = new NavMeshPath();
				if (NavMesh.CalculatePath(navMeshHit2.position, familyMemberController.PlayerMovement.transform.position, -1, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
				{
					familyMemberController.navAgent.Warp(navMeshHit2.position);
				}
			}
			if (Time.deltaTime != 0f)
			{
				float f = (familyMemberController.CurFollowPos - _003CprevFollowPos_003E5__5).magnitude / Time.deltaTime;
				_003CprevFollowPos_003E5__5 = familyMemberController.CurFollowPos;
				bool flag = familyMemberController.PlayerMovement.CurMoveSpeed == 0f && Avelog.Input.VertAxis == 0f;
				if (NavMeshAgentUtils.HasPath(familyMemberController.navAgent))
				{
					if (!flag)
					{
						float num2 = familyMemberController.navAgent.remainingDistance;
						if (float.IsInfinity(familyMemberController.navAgent.remainingDistance))
						{
							num2 = 0f;
							for (int i = 0; i < familyMemberController.navAgent.path.corners.Length - 1; i++)
							{
								num2 += (familyMemberController.navAgent.path.corners[i + 1] - familyMemberController.navAgent.path.corners[i]).magnitude;
							}
						}
						float num3 = Mathf.Clamp(num2 / ManagerBase<FamilyManager>.Instance.FollowDistanceMaximum - 0.3f, -0.3f, 1f) * ManagerBase<FamilyManager>.Instance.AddSpeedMaximum;
						familyMemberController.navAgent.speed = Mathf.Clamp(Mathf.Abs(f) + num3, 0f, float.MaxValue);
					}
					else
					{
						familyMemberController.navAgent.speed = PlayerSpawner.PlayerInstance.PlayerMovement.MaxMoveSpeed;
					}
				}
				familyMemberController.navAgent.autoBraking = flag;
			}
			current = _003Cwait_003E5__3;
			state = 1;
			return true;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass103_0
	{
		public CatModel curModel;

		public FamilyMemberController _003C_003E4__this;

		internal bool _003CWalkingAround_003Eb__0()
		{
			if (!_003C_003E4__this.Animator.IsPlayingByName(AnimationHashes.idle.Value))
			{
				return curModel == _003C_003E4__this.Model;
			}
			return true;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass103_1
	{
		public CatModel curModel;

		public FamilyMemberController _003C_003E4__this;

		internal bool _003CWalkingAround_003Eb__1()
		{
			if (!_003C_003E4__this.Animator.IsPlayingByName(AnimationHashes.idle.Value))
			{
				return curModel == _003C_003E4__this.Model;
			}
			return true;
		}
	}

	[StructLayout(LayoutKind.Auto)]
	private struct MyRandom
	{
		public float randomNumber;

		public float curRandomMin;

		public float curRandomMax;
	}

	private sealed class _003CWalkingAround_003Ed__103 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int state;

		private object current;

		public FamilyMemberController thisController;

		private bool Cidle;

		private float CidleTimer;

		private Vector3 walkDestination;

		private float checkFollowingTimer;

        object IEnumerator<object>.Current
        {
            get
            {
                return this.current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return this.current;
            }
        }


		public _003CWalkingAround_003Ed__103(int state)
		{
			this.state = state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = state;
			FamilyMemberController locals2 = thisController;
			switch (num)
			{
			default:
				return false;
			case 0:
				state = -1;
				locals2.navAgent.autoBraking = false;
				new NavMeshPath();
				locals2.UpdateAttackStoppingDistanceAndRange();
				new List<float>();
				Cidle = true;
				CidleTimer = 0f;
				walkDestination = locals2.transform.position;
				checkFollowingTimer = 0f;
				goto IL_007d;
			case 1:
				state = -1;
				goto IL_00f8;
			case 2:
			{
				state = -1;
				NavMeshUtils.SamplePositionIterative(locals2.PlayerGO.transform.position, out NavMeshHit _, 5f, 100f, 4, -1);
				int num3 = 0;
				do
				{
					Vector3 b = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f)).normalized * UnityEngine.Random.Range(ManagerBase<PlayerManager>.Instance.ObstacleValue + locals2.navAgent.radius, locals2.CurWalkSpace);
					float num4 = b.y = Mathf.Sign(UnityEngine.Random.Range(-1f, 1f)) * UnityEngine.Random.Range(0f, locals2.CurWalkSpace);
					if (NavMeshUtils.SamplePositionIterative(locals2.PlayerGO.transform.position + b, out NavMeshHit navMeshHit3, 5f, 100f, 4, -1))
					{
						walkDestination = navMeshHit3.position;
						break;
					}
					num3++;
				}
				while (num3 <= 5);
				locals2.navAgent.acceleration = ManagerBase<FamilyManager>.Instance.Acceleration;
				locals2.SetDestination(walkDestination);
				locals2.mySpeedMeasurements.Clear();
				break;
			}
			case 3:
			{
				state = -1;
				int num2 = 0;
				float maxInclusive = locals2.FamilyMemberParams.StandartIdleChance + locals2.FamilyMemberParams.SittingIdleChance + locals2.FamilyMemberParams.RestIdleChance;
				MyRandom _003C_003Ec__DisplayClass103_ = default(MyRandom);
				_003C_003Ec__DisplayClass103_.randomNumber = UnityEngine.Random.Range(0f, maxInclusive);
				_003C_003Ec__DisplayClass103_.curRandomMin = 0f;
				_003C_003Ec__DisplayClass103_.curRandomMax = locals2.FamilyMemberParams.StandartIdleChance;
				if (_003CWalkingAround_003Eg__IsRandomInBounds_007C103_3(ref _003C_003Ec__DisplayClass103_))
				{
					num2 = 0;
				}
				else
				{
					_003C_003Ec__DisplayClass103_.curRandomMin = _003C_003Ec__DisplayClass103_.curRandomMax;
					_003C_003Ec__DisplayClass103_.curRandomMax += locals2.FamilyMemberParams.SittingIdleChance;
					num2 = (_003CWalkingAround_003Eg__IsRandomInBounds_007C103_3(ref _003C_003Ec__DisplayClass103_) ? 1 : 2);
				}
				locals2.Animator.SetInteger("IdleType", num2);
				switch (num2)
				{
				case 0:
					CidleTimer = Mathf.Lerp(locals2.FamilyMemberParams.StandartIdleMinimum, locals2.FamilyMemberParams.StandartIdleMaximum, UnityEngine.Random.Range(0f, 1f));
					break;
				case 1:
					CidleTimer = Mathf.Lerp(locals2.FamilyMemberParams.SittingIdleMinimum, locals2.FamilyMemberParams.SittingIdleMaximum, UnityEngine.Random.Range(0f, 1f));
					break;
				default:
					CidleTimer = Mathf.Lerp(locals2.FamilyMemberParams.RestIdleMinimum, locals2.FamilyMemberParams.RestIdleMaximum, UnityEngine.Random.Range(0f, 1f));
					break;
				}
				break;
			}
			case 4:
				{
					state = -1;
					goto IL_007d;
				}
				IL_007d:
				if (checkFollowingTimer <= 0f)
				{
					if (!locals2.IsInWalkSpace())
					{
						if (locals2.Animator.GetInteger("IdleType") != 0)
						{
							FamilyMemberController familyMemberController = locals2;
							locals2.Animator.SetInteger("IdleType", 0);
							CatModel curModel = locals2.Model;
							current = new WaitUntil(() => (!familyMemberController.Animator.IsPlayingByName(AnimationHashes.idle.Value)) ? (curModel == familyMemberController.Model) : true);
							state = 1;
							return true;
						}
						goto IL_00f8;
					}
					checkFollowingTimer = 1f;
				}
				else
				{
					checkFollowingTimer -= Time.deltaTime;
				}
				if (Cidle)
				{
					if (CidleTimer <= 0f)
					{
						FamilyMemberController familyMemberController2 = locals2;
						Cidle = false;
						locals2.Animator.SetInteger("IdleType", 0);
						CatModel curModel2 = locals2.Model;
						current = new WaitUntil(() => (!familyMemberController2.Animator.IsPlayingByName(AnimationHashes.idle.Value)) ? (curModel2 == familyMemberController2.Model) : true);
						state = 2;
						return true;
					}
					CidleTimer -= Time.deltaTime;
					break;
				}
				if (NavMeshAgentUtils.HasPath(locals2.navAgent))
				{
					if (locals2.IsStuck())
					{
						Vector3 vector = Vector3.Cross(locals2.PlayerGO.transform.position - locals2.transform.position, Vector3.up);
						float minInclusive = ManagerBase<PlayerManager>.Instance.ObstacleValue * 3f;
						float d = Mathf.Sign(UnityEngine.Random.Range(-1f, 1f));
						Vector3 sourcePosition = locals2.transform.position + vector.normalized * d * UnityEngine.Random.Range(minInclusive, locals2.CurWalkSpace);
						locals2.mySpeedMeasurements.Clear();
						NavMeshUtils.SamplePositionIterative(sourcePosition, out NavMeshHit navMeshHit, 5f, 100f, 4, -1);
						locals2.navAgent.SetDestination(navMeshHit.position);
					}
					break;
				}
				Cidle = true;
				locals2.ResetPath(resetVelocityInstantly: false);
				current = new WaitWhile(() => locals2.navAgent.velocity != Vector3.zero);
				state = 3;
				return true;
				IL_00f8:
				locals2.ChangeState(State.Following, locals2.Following());
				return false;
			}
			if (NavMeshAgentUtils.HasPath(locals2.navAgent))
			{
				float num5 = locals2.navAgent.remainingDistance;
				if (float.IsInfinity(locals2.navAgent.remainingDistance))
				{
					num5 = 0f;
					for (int i = 0; i < locals2.navAgent.path.corners.Length - 1; i++)
					{
						num5 += (locals2.navAgent.path.corners[i + 1] - locals2.navAgent.path.corners[i]).magnitude;
					}
				}
				locals2.navAgent.speed = Mathf.Clamp(locals2.FamilyMemberParams.SpeedLow + num5 / ManagerBase<FamilyManager>.Instance.WalkDistanceMaximum * (locals2.FamilyMemberParams.SpeedMedium - locals2.FamilyMemberParams.SpeedLow), 0f, locals2.FamilyMemberParams.SpeedMedium);
				locals2.navAgent.acceleration = ManagerBase<FamilyManager>.Instance.Acceleration;
			}
			current = null;
			state = 4;
			return true;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass117_0
	{
		public bool isAttackCompleted;

		internal void _003CAttack_003Eb__0()
		{
			isAttackCompleted = true;
		}
	}

	[CompilerGenerated]
	private sealed class _003CAttack_003Ed__117 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int state;

		private object current;

		public FamilyMemberController _003C_003E4__this;

		private _003C_003Ec__DisplayClass117_0 _003C_003E8__1;

		private ActorCombat _003CprevAttackTarget_003E5__2;

		private Vector3 _003CendForward_003E5__3;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return current;
			}
		}

		[DebuggerHidden]
		public _003CAttack_003Ed__117(int state)
		{
			this.state = state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = state;
			FamilyMemberController familyMemberController = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				state = -1;
				familyMemberController.UpdateAttackStoppingDistanceAndRange();
				familyMemberController.navAgent.speed = ManagerBase<PlayerManager>.Instance.SpeedMaximum;
				familyMemberController.navAgent.acceleration = ManagerBase<FamilyManager>.Instance.Acceleration;
				_003CprevAttackTarget_003E5__2 = familyMemberController.AttackTarget;
				goto IL_0065;
			case 1:
				state = -1;
				goto IL_01c7;
			case 2:
				{
					state = -1;
					goto IL_0065;
				}
				IL_0065:
				if (!(familyMemberController.AttackTarget != null))
				{
					familyMemberController.ChangeState(State.Following, familyMemberController.Following());
					return false;
				}
				if (_003CprevAttackTarget_003E5__2 != familyMemberController.AttackTarget)
				{
					_003CprevAttackTarget_003E5__2 = familyMemberController.AttackTarget;
					familyMemberController.UpdateAttackStoppingDistanceAndRange();
				}
				if ((familyMemberController.transform.position - familyMemberController.AttackTarget.transform.position).IsShorterOrEqual(familyMemberController.attackRange))
				{
					familyMemberController.ResetPath();
					_003CendForward_003E5__3 = Vector3.ProjectOnPlane(familyMemberController.AttackTarget.transform.position - familyMemberController.transform.position, Vector3.up);
					if (familyMemberController.actorCombat.IsAttackTime())
					{
						_003C_003E8__1 = new _003C_003Ec__DisplayClass117_0();
						_003C_003E8__1.isAttackCompleted = false;
						familyMemberController.actorCombat.Attack(familyMemberController.AttackTarget, delegate
						{
							_003C_003E8__1.isAttackCompleted = true;
						});
						goto IL_01c7;
					}
					goto IL_01db;
				}
				if (!NavMeshAgentUtils.HasPath(familyMemberController.navAgent) || !familyMemberController.navAgent.pathPending)
				{
					if (familyMemberController.AttackTargetController.CurState == EnemyController.State.Escape)
					{
						float num2 = (familyMemberController.navAgent.transform.position - familyMemberController.CurFollowPos).IsShorter(ManagerBase<FamilyManager>.Instance.PursuitDistanceMaximum) ? (-1f) : 1f;
						familyMemberController.navAgent.speed = Mathf.Clamp(familyMemberController.navAgent.speed + ManagerBase<FamilyManager>.Instance.PursuitDecceleration * Time.deltaTime * num2, 0f, Math.Max(ManagerBase<PlayerManager>.Instance.curSpeed, ManagerBase<PlayerManager>.Instance.SpeedMedium));
						NavMeshUtils.SamplePositionIterative(familyMemberController.CurFollowPos, out NavMeshHit navMeshHit, 2f, 100f, 4, -1);
						familyMemberController.SetDestination(navMeshHit.position);
					}
					else
					{
						familyMemberController.navAgent.speed = Mathf.Clamp(familyMemberController.navAgent.speed + ManagerBase<FamilyManager>.Instance.PursuitDecceleration * Time.deltaTime, 0f, Math.Max(ManagerBase<PlayerManager>.Instance.curSpeed, ManagerBase<PlayerManager>.Instance.SpeedMedium));
						NavMeshUtils.SamplePositionIterative(familyMemberController.AttackTarget.transform.position, out NavMeshHit navMeshHit2, 2f, 100f, 4, -1);
						familyMemberController.SetDestination(navMeshHit2.position);
					}
				}
				break;
				IL_01db:
				_003CendForward_003E5__3 = default(Vector3);
				break;
				IL_01c7:
				if (!_003C_003E8__1.isAttackCompleted)
				{
					float rotAngle = familyMemberController.navAgent.angularSpeed * Time.deltaTime;
					familyMemberController.transform.rotation *= QuaternionUtils.GetRotation(familyMemberController.transform.forward, _003CendForward_003E5__3, rotAngle);
					Vector3.Angle(familyMemberController.transform.forward, _003CendForward_003E5__3);
					current = null;
					state = 1;
					return true;
				}
				_003C_003E8__1 = null;
				goto IL_01db;
			}
			current = null;
			state = 2;
			return true;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass127_0
	{
		public FamilyMemberController _003C_003E4__this;

		public Food food;

		public CatModel curModel;

		public float GetEatingDistance(Vector3 foodPos)
		{
			float magnitude = (_003C_003E4__this.PlayerGO.transform.position - foodPos).magnitude;
			float num = Mathf.Clamp(ManagerBase<PlayerManager>.Instance.ObstacleValue - magnitude, 0f, ManagerBase<PlayerManager>.Instance.ObstacleValue);
			return Mathf.Clamp(ManagerBase<FamilyManager>.Instance.EatingDistance * Mathf.Lerp(_003C_003E4__this.FamilyMemberParams.ModelScalePerc / 100f, 1f, 0.3f), num + _003C_003E4__this.navAgent.radius, float.MaxValue);
		}

		public Vector3 _003CEating_003Eg__RotatedToFoodForward_007C3()
		{
			return Vector3.ProjectOnPlane(food.transform.position - _003C_003E4__this.transform.position, Vector3.up);
		}

		internal bool _003CEating_003Eb__1()
		{
			if (!_003C_003E4__this.Animator.IsPlayingByName(AnimationHashes.eatExit.Value))
			{
				return curModel == _003C_003E4__this.Model;
			}
			return false;
		}

		internal bool _003CEating_003Eb__2()
		{
			if (_003C_003E4__this.Animator.IsPlayingByName(AnimationHashes.eatExit.Value))
			{
				return curModel == _003C_003E4__this.Model;
			}
			return false;
		}
	}

	[CompilerGenerated]
	private sealed class _003CEating_003Ed__127 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int state;

		private object current;

		public FamilyMemberController _003C_003E4__this;

		public Food food;

		private _003C_003Ec__DisplayClass127_0 _003C_003E8__1;

		public Action endCallback;

		private NavMeshHit _003CfoodNavMeshHit_003E5__2;

		private float _003CeatingDistance_003E5__3;

		private float _003CupdateEatingDistanceTimer_003E5__4;

		private float _003CcurEatTime_003E5__5;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return current;
			}
		}

		[DebuggerHidden]
		public _003CEating_003Ed__127(int state)
		{
			this.state = state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = state;
			FamilyMemberController familyMemberController = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				state = -1;
				_003C_003E8__1 = new _003C_003Ec__DisplayClass127_0();
				_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
				_003C_003E8__1.food = food;
				_003C_003E8__1.curModel = familyMemberController.Model;
				familyMemberController.feedingState = FeedingState.None;
				NavMeshUtils.SamplePositionIterative(_003C_003E8__1.food.transform.position, out _003CfoodNavMeshHit_003E5__2, 5f, 100f, 4, -1);
				_003CeatingDistance_003E5__3 = _003C_003E8__1.GetEatingDistance(_003CfoodNavMeshHit_003E5__2.position);
				familyMemberController.navAgent.stoppingDistance = _003CeatingDistance_003E5__3;
				_003CupdateEatingDistanceTimer_003E5__4 = 0.2f;
				_003CcurEatTime_003E5__5 = 0f;
				goto IL_0307;
			case 1:
				state = -1;
				if (familyMemberController.feedingState == FeedingState.Feed)
				{
					_003CcurEatTime_003E5__5 += Time.deltaTime;
				}
				_003CupdateEatingDistanceTimer_003E5__4 -= Time.deltaTime;
				goto IL_0307;
			case 2:
				state = -1;
				current = new WaitWhile(() => _003C_003E8__1._003C_003E4__this.Animator.IsPlayingByName(AnimationHashes.eatExit.Value) && _003C_003E8__1.curModel == _003C_003E8__1._003C_003E4__this.Model);
				state = 3;
				return true;
			case 3:
				{
					state = -1;
					break;
				}
				IL_0307:
				if (_003CcurEatTime_003E5__5 < ManagerBase<FamilyManager>.Instance.EatingDuration)
				{
					if (_003CupdateEatingDistanceTimer_003E5__4 <= 0f)
					{
						_003CeatingDistance_003E5__3 = _003C_003E8__1.GetEatingDistance(_003CfoodNavMeshHit_003E5__2.position);
						familyMemberController.navAgent.stoppingDistance = _003CeatingDistance_003E5__3;
						_003CupdateEatingDistanceTimer_003E5__4 = 0.2f;
					}
					if (!(familyMemberController.transform.position - _003CfoodNavMeshHit_003E5__2.position).IsShorterOrEqual(_003CeatingDistance_003E5__3))
					{
						familyMemberController.feedingState = FeedingState.Move;
					}
					else if (Vector3.Angle(familyMemberController.transform.forward, _003C_003E8__1._003CEating_003Eg__RotatedToFoodForward_007C3()) != 0f)
					{
						familyMemberController.feedingState = FeedingState.Rotate;
					}
					else if (_003CcurEatTime_003E5__5 < ManagerBase<FamilyManager>.Instance.EatingDuration)
					{
						familyMemberController.feedingState = FeedingState.Feed;
					}
					if (familyMemberController.feedingState == FeedingState.Move && !familyMemberController.Animator.IsPlayingByName(AnimationHashes.eatExit.Value))
					{
						familyMemberController.navAgent.speed = ManagerBase<PlayerManager>.Instance.SpeedMaximum;
						familyMemberController.navAgent.acceleration = ManagerBase<FamilyManager>.Instance.Acceleration;
						if (!familyMemberController.navAgent.pathPending)
						{
							familyMemberController.SetDestination(_003CfoodNavMeshHit_003E5__2.position);
						}
					}
					else
					{
						familyMemberController.ResetPath();
					}
					if (familyMemberController.feedingState == FeedingState.Rotate)
					{
						float rotAngle = ManagerBase<PlayerManager>.Instance.EatDrinkRotationSpeed * Time.deltaTime;
						Quaternion rotation = QuaternionUtils.GetRotation(familyMemberController.transform.forward, _003C_003E8__1._003CEating_003Eg__RotatedToFoodForward_007C3(), rotAngle);
						familyMemberController.transform.rotation *= rotation;
						if (rotation == Quaternion.identity)
						{
							familyMemberController.feedingState = FeedingState.Feed;
						}
					}
					if (familyMemberController.feedingState == FeedingState.Feed)
					{
						if (!familyMemberController.Animator.GetBool("isEating"))
						{
							familyMemberController.Animator.SetTrigger("Eat");
							familyMemberController.Animator.SetBool("isEating", value: true);
						}
					}
					else
					{
						familyMemberController.Animator.SetBool("isEating", value: false);
					}
					current = null;
					state = 1;
					return true;
				}
				familyMemberController.feedingState = FeedingState.None;
				familyMemberController.Animator.SetBool("isEating", value: false);
				if (familyMemberController.IsNeededFood(_003C_003E8__1.food))
				{
					ManagerBase<FamilyManager>.Instance.AddExperience(ManagerBase<FamilyManager>.Instance.СhildEatingExperience * ManagerBase<FamilyManager>.Instance.DemandExpMulty, familyMemberController.familyMemberData);
					familyMemberController.SatisfyNeed();
				}
				else
				{
					ManagerBase<FamilyManager>.Instance.AddExperience(ManagerBase<FamilyManager>.Instance.СhildEatingExperience, familyMemberController.familyMemberData);
				}
				_003C_003E8__1.food.EatFoodUnit();
				familyMemberController.holdedFood.Remove(_003C_003E8__1.food);
				if (familyMemberController.Animator.IsPlayingByTag(AnimationHashes.eat.Value) || familyMemberController.Animator.IsPlayingNextByTag(AnimationHashes.eat.Value))
				{
					current = new WaitWhile(() => !_003C_003E8__1._003C_003E4__this.Animator.IsPlayingByName(AnimationHashes.eatExit.Value) && _003C_003E8__1.curModel == _003C_003E8__1._003C_003E4__this.Model);
					state = 2;
					return true;
				}
				break;
			}
			endCallback?.Invoke();
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass137_0
	{
		public FamilyMemberController _003C_003E4__this;

		public CatModel curModel;

		public float _003CDrinking_003Eg__GetDrinkingDistance_007C3(Vector3 drinkPos)
		{
			float magnitude = (_003C_003E4__this.PlayerGO.transform.position - drinkPos).magnitude;
			float obstacleValue = ManagerBase<PlayerManager>.Instance.ObstacleValue;
			obstacleValue += _003C_003E4__this.navAgent.radius;
			return Mathf.Clamp(obstacleValue - magnitude, 0f, obstacleValue);
		}

		internal bool _003CDrinking_003Eb__0()
		{
			if (!_003C_003E4__this.Animator.IsPlayingByName(AnimationHashes.drinkExit.Value))
			{
				return curModel == _003C_003E4__this.Model;
			}
			return false;
		}

		internal bool _003CDrinking_003Eb__1()
		{
			if (_003C_003E4__this.Animator.IsPlayingByName(AnimationHashes.drinkExit.Value))
			{
				return curModel == _003C_003E4__this.Model;
			}
			return false;
		}

		public bool _003CDrinking_003Eg__IsDrinkAnimationPlaying_007C2()
		{
			if (!_003C_003E4__this.Animator.IsPlayingByName(AnimationHashes.drinkExit.Value) && !_003C_003E4__this.Animator.IsPlayingByName(AnimationHashes.drinkEnter.Value))
			{
				return _003C_003E4__this.Animator.IsPlayingByName(AnimationHashes.drinkCycle.Value);
			}
			return true;
		}
	}

	[CompilerGenerated]
	private sealed class _003CDrinking_003Ed__137 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int state;

		private object current;

		public FamilyMemberController _003C_003E4__this;

		public DrinkType drinkType;

		public Vector3 familyDrinkPoint;

		public Water water;

		private _003C_003Ec__DisplayClass137_0 _003C_003E8__1;

		public Action endCallback;

		private float _003CupdateNavPosTimer_003E5__2;

		private float _003CcurDrinkTime_003E5__3;

		private bool _003CrotateToWaterCenter_003E5__4;

		private bool _003ConDrinkPos_003E5__5;

		private bool _003CmouthInWater_003E5__6;

		private bool _003ChaveWaterNeed_003E5__7;

		private NavMeshHit _003CwaterNavMeshHit_003E5__8;

		private int _003CquenchThirstCount_003E5__9;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return current;
			}
		}

		[DebuggerHidden]
		public _003CDrinking_003Ed__137(int state)
		{
			this.state = state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = state;
			FamilyMemberController familyMemberController = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				state = -1;
				_003C_003E8__1 = new _003C_003Ec__DisplayClass137_0();
				_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
				_003C_003E8__1.curModel = familyMemberController.Model;
				familyMemberController.mySpeedMeasurements.Clear();
				if (drinkType == DrinkType.FromDrinkPosition)
				{
					float d = familyMemberController.startNavAgentRadius - familyMemberController.navAgent.radius;
					familyDrinkPoint += (water.Collider.transform.position - familyDrinkPoint).normalized * d;
				}
				NavMesh.GetAreaFromName("Water");
				_003CupdateNavPosTimer_003E5__2 = 0f;
				_003CcurDrinkTime_003E5__3 = 0f;
				familyMemberController.feedingState = FeedingState.Move;
				_003CrotateToWaterCenter_003E5__4 = (drinkType == DrinkType.FromDrinkPosition);
				_003ConDrinkPos_003E5__5 = false;
				_003CmouthInWater_003E5__6 = false;
				_003ChaveWaterNeed_003E5__7 = (familyMemberController.familyMemberData.curNeed == "Water");
				NavMeshUtils.SamplePositionIterative(familyDrinkPoint, out _003CwaterNavMeshHit_003E5__8, 5f, 100f, 4, -1);
				_003CquenchThirstCount_003E5__9 = 0;
				goto IL_04a3;
			case 1:
				state = -1;
				if (familyMemberController.feedingState == FeedingState.Feed)
				{
					_003CcurDrinkTime_003E5__3 += Time.deltaTime;
				}
				_003CupdateNavPosTimer_003E5__2 -= Time.deltaTime;
				if (_003CcurDrinkTime_003E5__3 >= ManagerBase<FamilyManager>.Instance.DrinkingDuration)
				{
					_003CcurDrinkTime_003E5__3 = 0f;
					int num2 = ++_003CquenchThirstCount_003E5__9;
					familyMemberController.ChangeThirst(ManagerBase<PlayerManager>.Instance.ThirstEffect);
				}
				goto IL_04a3;
			case 2:
				state = -1;
				current = new WaitWhile(() => _003C_003E8__1._003C_003E4__this.Animator.IsPlayingByName(AnimationHashes.drinkExit.Value) && _003C_003E8__1.curModel == _003C_003E8__1._003C_003E4__this.Model);
				state = 3;
				return true;
			case 3:
				{
					state = -1;
					break;
				}
				IL_04a3:
				if (familyMemberController.HaveThirst())
				{
					if (_003CupdateNavPosTimer_003E5__2 <= 0f)
					{
						_003CupdateNavPosTimer_003E5__2 = 0.1f;
						NavMeshUtils.SamplePositionIterative(familyDrinkPoint, out _003CwaterNavMeshHit_003E5__8, 5f, 100f, 4, -1);
						Vector3 vector = water.Collider.ClosestPoint(familyMemberController.Model.MouthDrinkPos.position);
						_003CmouthInWater_003E5__6 = (vector.x == familyMemberController.Model.MouthDrinkPos.position.x && vector.z == familyMemberController.Model.MouthDrinkPos.position.z && vector.y >= familyMemberController.Model.MouthDrinkPos.position.y);
						_003ConDrinkPos_003E5__5 = (familyMemberController.transform.position - _003CwaterNavMeshHit_003E5__8.position).IsShorterOrEqual(familyMemberController.navAgent.stoppingDistance);
						familyMemberController.navAgent.stoppingDistance = _003C_003E8__1._003CDrinking_003Eg__GetDrinkingDistance_007C3(_003CwaterNavMeshHit_003E5__8.position);
					}
					if (_003CmouthInWater_003E5__6 && drinkType == DrinkType.FromNearestWaterPoint)
					{
						familyMemberController.feedingState = FeedingState.Feed;
					}
					else if (!_003ConDrinkPos_003E5__5)
					{
						familyMemberController.feedingState = FeedingState.Move;
					}
					else
					{
						familyMemberController.feedingState = FeedingState.Rotate;
					}
					if (familyMemberController.feedingState == FeedingState.Move && !_003C_003E8__1._003CDrinking_003Eg__IsDrinkAnimationPlaying_007C2())
					{
						familyMemberController.navAgent.speed = ManagerBase<PlayerManager>.Instance.SpeedMaximum;
						familyMemberController.navAgent.acceleration = ManagerBase<FamilyManager>.Instance.Acceleration;
						if (!familyMemberController.navAgent.pathPending)
						{
							familyMemberController.SetDestination(_003CwaterNavMeshHit_003E5__8.position);
						}
					}
					else
					{
						familyMemberController.ResetPath();
					}
					if (familyMemberController.feedingState != 0)
					{
						familyMemberController.mySpeedMeasurements.Clear();
					}
					if (familyMemberController.feedingState == FeedingState.Rotate)
					{
						float rotAngle = ManagerBase<PlayerManager>.Instance.EatDrinkRotationSpeed * Time.deltaTime;
						Vector3 zero = Vector3.zero;
						Quaternion quaternion = QuaternionUtils.GetRotation(endDir: (!_003CrotateToWaterCenter_003E5__4) ? Vector3.ProjectOnPlane(_003CwaterNavMeshHit_003E5__8.position - familyMemberController.transform.position, Vector3.up) : Vector3.ProjectOnPlane(water.Collider.transform.position - familyMemberController.transform.position, Vector3.up), startDir: familyMemberController.transform.forward, rotAngle: rotAngle);
						familyMemberController.transform.rotation *= quaternion;
						if (quaternion == Quaternion.identity)
						{
							familyMemberController.feedingState = FeedingState.Feed;
						}
					}
					if (familyMemberController.feedingState == FeedingState.Feed)
					{
						if (!familyMemberController.Animator.GetBool("isDrinking"))
						{
							familyMemberController.Animator.SetBool("isDrinking", value: true);
							familyMemberController.Animator.SetTrigger("Drink");
						}
					}
					else
					{
						familyMemberController.Animator.SetBool("isDrinking", value: false);
					}
					current = null;
					state = 1;
					return true;
				}
				if (_003ChaveWaterNeed_003E5__7)
				{
					ManagerBase<FamilyManager>.Instance.AddExperience(ManagerBase<FamilyManager>.Instance.ChildThirstExperience * ManagerBase<FamilyManager>.Instance.DemandExpMulty, familyMemberController.familyMemberData);
				}
				else
				{
					ManagerBase<FamilyManager>.Instance.AddExperience(ManagerBase<FamilyManager>.Instance.ChildThirstExperience, familyMemberController.familyMemberData);
				}
				if (_003ChaveWaterNeed_003E5__7)
				{
					familyMemberController.SatisfyNeed();
				}
				familyMemberController.feedingState = FeedingState.None;
				familyMemberController.Animator.SetBool("isDrinking", value: false);
				if (familyMemberController.Animator.IsPlayingByTag(AnimationHashes.drink.Value) || familyMemberController.Animator.IsPlayingNextByTag(AnimationHashes.drink.Value))
				{
					current = new WaitWhile(() => !_003C_003E8__1._003C_003E4__this.Animator.IsPlayingByName(AnimationHashes.drinkExit.Value) && _003C_003E8__1.curModel == _003C_003E8__1._003C_003E4__this.Model);
					state = 2;
					return true;
				}
				break;
			}
			FamilyMemberController.endDrinkEvent?.Invoke(familyMemberController, _003CquenchThirstCount_003E5__9);
			endCallback?.Invoke();
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

    public Vector3 sleepPosition;

    public CatModel curModel;

    float CalculateObstacleRadius(NavMeshAgent navMeshAgent)
    {
        return navMeshAgent.radius * Mathf.Max(navMeshAgent.transform.localScale.x, navMeshAgent.transform.localScale.z);
    }

	private float CalculateObstacleFromObject(float obstacleRadius, Vector3 obstaclePosition, float radius)
    {
        float magnitude = (obstaclePosition - sleepPosition).magnitude;
        return Mathf.Clamp(obstacleRadius + radius - magnitude, 0f, float.MaxValue);
    }

    private void RecalculateStoppingDistance()
    {
        float highestObstacle = 0f;
        float myObstacleRadius = CalculateObstacleRadius(NavAgent);
        Instances.ForEach(delegate (FamilyMemberController controller)
        {
            if (controller != this)
            {
                float obstacleRadius2 = _003CSleeping_003Eg__CalculateObstacleRadius_007C153_5(controller.NavAgent);
         

                float num2 = CalculateObstacleFromObject(obstacleRadius2, controller.NavAgent.transform.position, myObstacleRadius);
                if (highestObstacle < num2)
                {
                    highestObstacle = num2;
                }
            }
        });
        Singleton<FarmResidentSpawner>.Instance.SpawnedFarmResidents.ForEach(delegate (FarmResident farmResident)
        {
            float obstacleRadius = _003CSleeping_003Eg__CalculateObstacleRadius_007C153_5(farmResident.NavAgent);
            float num = CalculateObstacleFromObject(obstacleRadius, farmResident.NavAgent.transform.position, myObstacleRadius);
            if (highestObstacle < num)
            {
                highestObstacle = num;
            }
        });
        NavAgent.stoppingDistance = highestObstacle + 0.1f;
    }

    internal bool _003CSleeping_003Eb__1()
    {
        if (!this.Animator.IsPlayingByName(AnimationHashes.idle.Value))
        {
            return curModel == this.Model;
        }
        return false;
    }

    internal bool _003CSleeping_003Eb__2()
    {
        if (!this.Animator.IsPlayingByName(AnimationHashes.sleep.Value))
        {
            return curModel == this.Model;
        }
        return false;
    }


    [CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass153_0
	{
		public FamilyMemberController _003C_003E4__this;

		

		
	}



	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass155_0
	{
		public FamilyMemberController _003C_003E4__this;

		public CatModel curModel;

		internal bool _003CAwaking_003Eb__0()
		{
			if (!_003C_003E4__this.Animator.IsPlayingByName(AnimationHashes.idle.Value))
			{
				return curModel == _003C_003E4__this.Model;
			}
			return false;
		}
	}

	[CompilerGenerated]
	private sealed class _003CAwaking_003Ed__155 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int state;

		private object current;

		public FamilyMemberController _003C_003E4__this;

		private _003C_003Ec__DisplayClass155_0 _003C_003E8__1;

		public Action endCallback;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return current;
			}
		}

		[DebuggerHidden]
		public _003CAwaking_003Ed__155(int state)
		{
			this.state = state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = state;
			FamilyMemberController familyMemberController = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				state = -1;
				_003C_003E8__1 = new _003C_003Ec__DisplayClass155_0();
				_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
				current = new WaitForSeconds(UnityEngine.Random.Range(0f, 0.5f));
				state = 1;
				return true;
			case 1:
				state = -1;
				familyMemberController.Animator.SetInteger("IdleType", 0);
				familyMemberController.Animator.SetBool("isSleeping", value: false);
				_003C_003E8__1.curModel = familyMemberController.Model;
				current = new WaitWhile(() => !_003C_003E8__1._003C_003E4__this.Animator.IsPlayingByName(AnimationHashes.idle.Value) && _003C_003E8__1.curModel == _003C_003E8__1._003C_003E4__this.Model);
				state = 2;
				return true;
			case 2:
				state = -1;
				familyMemberController.IsSleeping = false;
				endCallback?.Invoke();
				familyMemberController.ChangeState(State.WalkingAround, familyMemberController.WalkingAround());
				return false;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass171_0
	{
		public Food food;

		internal bool _003CHaveFoodInEatQueue_003Eb__2(EatCommand x)
		{
			return x.Food.Name == food.Name;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass172_0
	{
		public string need;

		internal bool _003CIsNeedAvailableToSatisfy_003Eb__0(string x)
		{
			return x.ToString() == need;
		}

		internal bool _003CIsNeedAvailableToSatisfy_003Eb__1(EnemyArchetype x)
		{
			return x.name == need;
		}

		internal bool _003CIsNeedAvailableToSatisfy_003Eb__2(EnemySpawner.EnemyInstance x)
		{
			return x.logic.currentScheme.Archetype.name == need;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass173_0
	{
		public FamilyMemberController _003C_003E4__this;

		public List<string> availableNeeds;

		internal bool _003CGenerateNeed_003Eb__0(string need)
		{
			return _003C_003E4__this.IsNeedAvailableToSatisfy(need);
		}

		internal bool _003CGenerateNeed_003Eb__2(EnemySpawner.EnemyInstance x)
		{
			return availableNeeds.Contains(x.logic.currentScheme.Archetype.name);
		}
	}

	[CompilerGenerated]
	private sealed class _003CTimer_003Ed__175 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int state;

		private object current;

		public float time;

		public Action endCallback;

		private float _003CcurTime_003E5__2;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return current;
			}
		}

		[DebuggerHidden]
		public _003CTimer_003Ed__175(int state)
		{
			this.state = state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			switch (state)
			{
			default:
				return false;
			case 0:
				state = -1;
				_003CcurTime_003E5__2 = 0f;
				break;
			case 1:
				state = -1;
				_003CcurTime_003E5__2 += Time.deltaTime;
				break;
			}
			if (_003CcurTime_003E5__2 < time)
			{
				current = null;
				state = 1;
				return true;
			}
			endCallback?.Invoke();
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	[Header("Отладка")]
	[SerializeField]
	[ReadonlyInspector]
	private State curState;

	public FamilyManager.FamilyMemberData familyMemberData;

	[Header("Ссылки")]
	[SerializeField]
	private FamilyMemberCatSkinner skinner;

	[SerializeField]
	private NavMeshAgent navAgent;

	[SerializeField]
	private ActorCombat actorCombat;

	[SerializeField]
	private ItemUser itemUser;

	private Coroutine curStateCor;

	private Coroutine traverseNavLinkCor;

	private Vector3 prevPos;

	private float curSpeed;

	private Vector3 velocity;

	private float startNavAgentRadius;

	private float startNavAgentHeight;

	public bool spawnInFolowingPos;

	[NonSerialized]
	private bool isInited;

	private List<CommandBase> commands = new List<CommandBase>();

	private CommandBase curCommand;

	public PlayerFamilyController.FollowPos followPos;

	public PlayerFamilyController.FollowPos followInvisibilityPos;

	private float _curWalkSpace;

	private const float speedMeasurementFrequency = 0.05f;

	private float speedMeasurementTimer;

	private Queue<(float time, float speed)> playerSpeedMeasurements = new Queue<(float, float)>();

	private float playerAvgSpeed;

	private int maxMySpeedMeasurements = 10;

	private Queue<float> mySpeedMeasurements = new Queue<float>();

	private float myAvgSpeed;

	private Queue<Vector3> speedMeasurementPositions = new Queue<Vector3>();

	private Vector3 startSpeedMeasurementPos;

	[SerializeField]
	private bool walkSpaceGizmo = true;

	private float attackRange;

	private FeedingState feedingState = FeedingState.None;

	private List<Food> holdedFood = new List<Food>();

	public static FamilyMemberEventHandler changeCanSleepStateEvent;

	private bool prevCanSleep;

	private static List<string> _itemIds;

	private Coroutine needPauseCor;

	public const string WaterNeed = "Water";

	public State CurState => curState;

	private FamilyManager.FamilyMemberParams FamilyMemberParams => familyMemberData.Params;

	public NavMeshAgent NavAgent => navAgent;

	private Animator Animator => Model.Animator;

	public ActorCombat Combat => actorCombat;

	public CatModel Model => skinner.CurModel;

	private GameObject PlayerGO => PlayerSpawner.PlayerInstance?.gameObject;

	private PlayerMovement PlayerMovement => PlayerSpawner.PlayerInstance.PlayerMovement;

	private PlayerFamilyController PlayerFamilyController => PlayerSpawner.PlayerInstance.PlayerFamilyController;

	private Vector3 StartModelScale => Model.StartModelScale;

	public static List<FamilyMemberController> Instances
	{
		get;
		private set;
	} = new List<FamilyMemberController>();


	private Vector3 CurFollowPos => PlayerFamilyController.GetFollowPosition(this);

	private float CurWalkSpace
	{
		get
		{
			if (IsInvisibilityActive)
			{
				return 0f;
			}
			return _curWalkSpace;
		}
		set
		{
			_curWalkSpace = value;
		}
	}

	private ActorCombat AttackTarget => PlayerFamilyController.FamilyAttackTarget;

	private EnemyController AttackTargetController => PlayerFamilyController.FamilyAttackTargetController;

	private bool EnoughAttackPower => ManagerBase<FamilyManager>.Instance.AttackPowerPercentFromSatiety != 0f;

	public bool IsInvisibilityActive
	{
		get;
		private set;
	}

	public bool IsSleeping
	{
		get;
		private set;
	}

	public static List<string> ItemIds
	{
		get
		{
			if (_itemIds == null)
			{
				_itemIds = (from x in EnumUtils.ToList<ItemId>()
					select x.ToString()).ToList();
				_itemIds.Remove(ItemId.Enemy.ToString());
				_itemIds.Remove(ItemId.None.ToString());
			}
			return _itemIds;
		}
	}

	public event ChangeStateHandler changeStateEvent;

	public static event SpawnHandler spawnEvent;

	public static event FamilyMemberEventHandler commandExecutedEvent;

	public static event EndDrinkHandler endDrinkEvent;

	public static event FamilyMemberEventHandler changeNeedEvent;

	public static event FamilyMemberEventHandler satisfyNeedEvent;

	private void UpdateModel()
	{
		navAgent.angularSpeed = ManagerBase<FamilyManager>.Instance.RotationSpeed;
		float curScalePart = familyMemberData.CurScalePart;
		Model.transform.localScale = StartModelScale * curScalePart;
		navAgent.radius = startNavAgentRadius * curScalePart;
		navAgent.height = startNavAgentHeight * curScalePart;
		UpdateAttackStoppingDistanceAndRange();
	}

	private void UpdateAttackStoppingDistanceAndRange()
	{
		if (curState == State.WalkingAround)
		{
			navAgent.stoppingDistance = navAgent.radius + Mathf.Max(startNavAgentRadius, ManagerBase<PlayerManager>.Instance.ObstacleValue);
		}
		else if (curState == State.Following)
		{
			if (navAgent.autoBraking)
			{
				navAgent.stoppingDistance = navAgent.radius * 2f;
			}
			else
			{
				navAgent.stoppingDistance = 0f;
			}
		}
		else if (curState == State.Attack && !(AttackTarget == null))
		{
			NavMeshAgent component = AttackTarget.GetComponent<NavMeshAgent>();
			float num = navAgent.radius + component.radius;
			attackRange = ManagerBase<FamilyManager>.Instance.AttackRange * (FamilyMemberParams.ModelScalePerc / 100f);
			if (attackRange < num)
			{
				attackRange = num + 0.1f;
			}
			navAgent.stoppingDistance = (attackRange + navAgent.radius) / 2f;
			navAgent.stoppingDistance = Mathf.Clamp(navAgent.stoppingDistance, num, float.MaxValue);
		}
	}

	private void Update()
	{
		if (Time.deltaTime == 0f)
		{
			return;
		}
		Walking_Update();
		Attack_Update();
		Drinking_Update();
		float curMoveSpeed = (CurState != State.Eating && CurState != State.Drinking && CurState != State.Sleeping) ? curSpeed : navAgent.velocity.magnitude;
		if (CurState == State.Sleeping && !NavMeshAgentUtils.HasPath(NavAgent))
		{
			curMoveSpeed = 0f;
		}
		_003CUpdate_003Eg__CalculacteAnimationMovementSpeed_007C42_0(curMoveSpeed);
		prevPos = base.transform.position;
		if (navAgent.isOnOffMeshLink && traverseNavLinkCor == null)
		{
			traverseNavLinkCor = StartCoroutine(TraverseNavLinkParabola(navAgent, 2f, ManagerBase<FamilyManager>.Instance.JumpTime));
			if (navAgent.isOnNavMesh)
			{
				navAgent.CompleteOffMeshLink();
			}
		}
		else if (CurState != State.Drinking && CurState != State.Eating && CurState != State.Sleeping)
		{
			NavMeshAgentUtils.UpdateVelocity(navAgent, ref velocity, ref curSpeed, Time.deltaTime, ManagerBase<FamilyManager>.Instance.RotationSpeed, ManagerBase<FamilyManager>.Instance.CorneringSlowing, CurState != State.Following);
		}
		else
		{
			velocity = NavAgent.velocity;
			curSpeed = NavAgent.velocity.magnitude;
		}
	}

	private void ChangeState(State newState, IEnumerator routine)
	{
		Animator.SetInteger("IdleType", 0);
		navAgent.autoBraking = true;
		if (curStateCor != null)
		{
			StopCoroutine(curStateCor);
		}
		curState = newState;
		if (routine != null)
		{
			curStateCor = StartCoroutine(routine);
		}
		this.changeStateEvent?.Invoke(newState);
	}

	private IEnumerator TraverseNavLinkParabola(NavMeshAgent agent, float height, float duration)
	{
		OffMeshLinkData currentOffMeshLinkData = agent.currentOffMeshLinkData;
		Vector3 startPos = agent.transform.position;
		Vector3 endPos = currentOffMeshLinkData.endPos + Vector3.up * agent.baseOffset;
		Vector3 startForward = agent.transform.forward;
		Vector3 endForward = (new Vector3(endPos.x, agent.transform.position.y, endPos.z) - agent.transform.position).normalized;
		float normalizedTime = 0f;
		while (normalizedTime < 1f)
		{
			float d = height * 4f * (normalizedTime - normalizedTime * normalizedTime);
			Vector3 position = agent.transform.position;
			agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + d * Vector3.up;
			Vector3.Lerp(startForward, endForward, Mathf.Clamp(normalizedTime * 2f, 0f, 1f));
			agent.transform.LookAt(agent.transform.position + endForward);
			normalizedTime += Time.deltaTime / duration;
			yield return null;
		}
		traverseNavLinkCor = null;
		if (navAgent.isOnNavMesh)
		{
			navAgent.CompleteOffMeshLink();
		}
	}

	private void ResetAnimatorParams()
	{
		Animator.SetBool("isEating", value: false);
		Animator.SetBool("isFalling", value: false);
		Animator.SetBool("isDrinking", value: false);
		Animator.SetInteger("IdleType", 0);
	}

	private void ResetPath(bool resetVelocityInstantly = true)
	{
		navAgent.ResetPath();
		if (resetVelocityInstantly)
		{
			navAgent.velocity = Vector3.zero;
		}
		navAgent.isStopped = true;
	}

	private void SetDestination(Vector3 destination)
	{
		navAgent.isStopped = false;
		navAgent.SetDestination(destination);
	}

	private void SetPath(NavMeshPath newPath)
	{
		navAgent.isStopped = false;
		navAgent.SetPath(newPath);
	}

	private void OnStageUp(FamilyManager.FamilyMemberData familyMemberData)
	{
		_003C_003Ec__DisplayClass49_0 _003C_003Ec__DisplayClass49_ = default(_003C_003Ec__DisplayClass49_0);
		_003C_003Ec__DisplayClass49_.familyMemberData = familyMemberData;
		if (this.familyMemberData == _003C_003Ec__DisplayClass49_.familyMemberData && _003COnStageUp_003Eg__IsMaxStage_007C49_0(ref _003C_003Ec__DisplayClass49_))
		{
			SwitchInvisibility(PlayerSpawner.PlayerInstance.PlayerCombat.IsInvisibilityActive());
			RemoveNeed();
			if (needPauseCor != null)
			{
				StopCoroutine(needPauseCor);
				needPauseCor = null;
			}
		}
	}

	private void OnGotExperience(FamilyManager.FamilyMemberData familyMemberData, float experience, float experiencePercent)
	{
		if (this.familyMemberData == familyMemberData)
		{
			UpdateModel();
		}
	}

	private void Start()
	{
		Skin_Initialize();
		PlayerSpawner.PlayerInstance.PlayerCombat.changeLifeStateEvent += OnPlayerChangeLifeState;
		ActorCombat.killEvent += OnKill;
		FamilyManager.gotExperienceEvent += OnGotExperience;
		FamilyManager.stageUpEvent += OnStageUp;
		startNavAgentRadius = navAgent.radius;
		startNavAgentHeight = navAgent.height;
		UpdateModel();
		navAgent.autoTraverseOffMeshLink = false;
		Needs_Initialize();
		Following_Initialize();
		Sleep_Initialize();
		Drink_Initialize();
		isInited = true;
		StartCoroutine(Spawn(delegate
		{
			ChangeState(State.Following, Following());
			SwitchInvisibility(PlayerSpawner.PlayerInstance.PlayerCombat.IsInvisibilityActive());
		}));
	}

	private void OnDestroy()
	{
		Sleep_Destroy();
		Drink_Destroy();
		Needs_Destroy();
		if (PlayerSpawner.PlayerInstance != null)
		{
			PlayerSpawner.PlayerInstance.PlayerCombat.changeLifeStateEvent -= OnPlayerChangeLifeState;
		}
		ActorCombat.killEvent -= OnKill;
		FamilyManager.gotExperienceEvent -= OnGotExperience;
		FamilyManager.stageUpEvent -= OnStageUp;
	}

	private void OnKill(ActorCombat killer, ActorCombat target)
	{
		if (killer is PlayerCombat || killer is FamilyMemberCombat)
		{
			EnemyController componentInParent = target.gameObject.GetComponentInParent<EnemyController>();
			if (familyMemberData.curNeed == componentInParent.CurrentScheme.Archetype.name)
			{
				ManagerBase<FamilyManager>.Instance.AddExperience(componentInParent.GetExperienceForKill() * ManagerBase<FamilyManager>.Instance.DemandExpMulty, familyMemberData);
				SatisfyNeed();
			}
			else
			{
				ManagerBase<FamilyManager>.Instance.AddExperience(componentInParent.GetExperienceForKill(), familyMemberData);
			}
		}
	}

	private void OnPlayerChangeLifeState(ActorCombat.LifeState state)
	{
		if (state != 0)
		{
			ResetController();
		}
		if (state == ActorCombat.LifeState.Alive)
		{
			StartCoroutine(Spawn(delegate
			{
				ChangeState(State.Following, Following());
			}));
		}
	}

	private void ResetController()
	{
		ResetAnimatorParams();
		ClearCommands();
		ChangeState(State.Disabled, null);
		if (traverseNavLinkCor != null)
		{
			StopCoroutine(traverseNavLinkCor);
		}
		traverseNavLinkCor = null;
		holdedFood.ForEach(delegate(Food x)
		{
			x.UnholdFoodUnit();
		});
		if (IsInvisibilityActive)
		{
			new InvisibilityExitCommand(this, Animator).Execute(null);
		}
	}

	public void HandlePlayerOutOfBounds()
	{
		ResetController();
		StartCoroutine(Spawn(delegate
		{
			ChangeState(State.Following, Following());
		}));
	}

	private IEnumerator Spawn(Action endCallback)
	{
		Model.gameObject.SetActive(value: false);
		WaitForSeconds wait = new WaitForSeconds(1f);
		NavMeshHit navMeshHit;
		while (true)
		{
			Vector3 sourcePosition = (!spawnInFolowingPos) ? ((familyMemberData.role == FamilyManager.FamilyMemberRole.Spouse) ? Singleton<SpouseSpawner>.Instance.SpawnPoint : Instances.Find((FamilyMemberController x) => x.familyMemberData.role == FamilyManager.FamilyMemberRole.Spouse).transform.position) : CurFollowPos;
			if (!PlayerSpawner.PlayerInstance.PlayerMovement.IsFalling && NavMeshUtils.SamplePositionIterative(sourcePosition, out navMeshHit, 5f, 100f, 4, -1))
			{
				break;
			}
			yield return wait;
		}
		spawnInFolowingPos = true;
		navAgent.Warp(navMeshHit.position);
		Model.gameObject.SetActive(value: true);
		if (!Instances.Contains(this))
		{
			Instances.Add(this);
		}
		FamilyMemberController.spawnEvent?.Invoke(this);
		endCallback?.Invoke();
	}

	private void AddCommand(CommandBase command)
	{
		commands.Add(command);
		TryExecuteCommand();
	}

	private bool TryExecuteCommand()
	{
		if (commands.Count > 0 && curCommand == null)
		{
			while (commands.Count > 0 && !commands[0].CanExecute())
			{
				commands.Remove(commands[0]);
			}
			if (commands.Count == 0)
			{
				return false;
			}
			curCommand = commands[0];
			commands.Remove(commands[0]);
			curCommand.Execute(OnCompleteCommand);
			return true;
		}
		return false;
	}

	private void OnCompleteCommand()
	{
		curCommand = null;
		FamilyMemberController.commandExecutedEvent?.Invoke(this);
		if (!TryExecuteCommand())
		{
			if (AttackTarget != null && EnoughAttackPower)
			{
				ChangeState(State.Attack, Attack());
			}
			else if (CurState != State.Sleeping && CurState != State.Awaking)
			{
				ChangeState(State.Following, Following());
			}
		}
	}

	private void ClearCommands()
	{
		if (curCommand != null)
		{
			curCommand.Complete();
		}
		curCommand = null;
		commands.Clear();
	}

	private bool HaveCommands()
	{
		if (curCommand == null)
		{
			return commands.Count > 0;
		}
		return true;
	}

	private void Following_Initialize()
	{
	}

	private IEnumerator Following()
	{
		UpdateAttackStoppingDistanceAndRange();
		navAgent.acceleration = ManagerBase<FamilyManager>.Instance.Acceleration;
		Vector3 position = PlayerGO.transform.position;
		float timeInWalkSpace = 0f;
		float seconds = 0f;
		WaitForSeconds wait = new WaitForSeconds(seconds);
		float prevTime = Time.time;
		NavMeshHit navMeshHit = default(NavMeshHit);
		Vector3 prevFollowPos = CurFollowPos;
		while (!(timeInWalkSpace >= ManagerBase<FamilyManager>.Instance.FollowPauseTime))
		{
			if ((CurFollowPos - base.transform.position).sqrMagnitude < 90000f)
			{
				if (NavMeshUtils.SamplePositionIterative(CurFollowPos, out navMeshHit, 2f, 100f, 4, -1))
				{
					navAgent.SetDestination(navMeshHit.position);
					navAgent.isStopped = false;
				}
			}
			else
			{
				NavMeshUtils.SamplePositionIterative(PlayerMovement.transform.position - PlayerMovement.transform.forward * 7f, out NavMeshHit navMeshHit2, 2f, 100f, 4, -1);
				NavMeshPath navMeshPath = new NavMeshPath();
				if (NavMesh.CalculatePath(navMeshHit2.position, PlayerMovement.transform.position, -1, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
				{
					navAgent.Warp(navMeshHit2.position);
				}
			}
			if (Time.deltaTime != 0f)
			{
				float f = (CurFollowPos - prevFollowPos).magnitude / Time.deltaTime;
				prevFollowPos = CurFollowPos;
				bool flag = PlayerMovement.CurMoveSpeed == 0f && Avelog.Input.VertAxis == 0f;
				if (NavMeshAgentUtils.HasPath(navAgent))
				{
					if (!flag)
					{
						float num = navAgent.remainingDistance;
						if (float.IsInfinity(navAgent.remainingDistance))
						{
							num = 0f;
							for (int i = 0; i < navAgent.path.corners.Length - 1; i++)
							{
								num += (navAgent.path.corners[i + 1] - navAgent.path.corners[i]).magnitude;
							}
						}
						float num2 = Mathf.Clamp(num / ManagerBase<FamilyManager>.Instance.FollowDistanceMaximum - 0.3f, -0.3f, 1f) * ManagerBase<FamilyManager>.Instance.AddSpeedMaximum;
						navAgent.speed = Mathf.Clamp(Mathf.Abs(f) + num2, 0f, float.MaxValue);
					}
					else
					{
						navAgent.speed = PlayerSpawner.PlayerInstance.PlayerMovement.MaxMoveSpeed;
					}
				}
				navAgent.autoBraking = flag;
			}
			yield return wait;
			timeInWalkSpace = ((!IsInWalkSpace()) ? 0f : (timeInWalkSpace + (Time.time - prevTime)));
			prevTime = Time.time;
		}
		ChangeState(State.WalkingAround, WalkingAround());
	}

	private IEnumerator WalkingAround()
	{
		navAgent.autoBraking = false;
		new NavMeshPath();
		UpdateAttackStoppingDistanceAndRange();
		new List<float>();
		bool idle = true;
		float idleTimer = 0f;
		Vector3 walkDestination = base.transform.position;
		float checkFollowingTimer = 0f;
		MyRandom _003C_003Ec__DisplayClass103_ = default(MyRandom);
		while (true)
		{
			if (checkFollowingTimer <= 0f)
			{
				if (!IsInWalkSpace())
				{
					break;
				}
				checkFollowingTimer = 1f;
			}
			else
			{
				checkFollowingTimer -= Time.deltaTime;
			}
			if (idle)
			{
				if (idleTimer <= 0f)
				{
					idle = false;
					Animator.SetInteger("IdleType", 0);
					CatModel curModel = Model;
					yield return new WaitUntil(() => (!Animator.IsPlayingByName(AnimationHashes.idle.Value)) ? (curModel == Model) : true);
					NavMeshUtils.SamplePositionIterative(PlayerGO.transform.position, out NavMeshHit _, 5f, 100f, 4, -1);
					int num = 0;
					do
					{
						Vector3 b = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f)).normalized * UnityEngine.Random.Range(ManagerBase<PlayerManager>.Instance.ObstacleValue + navAgent.radius, CurWalkSpace);
						float num2 = b.y = Mathf.Sign(UnityEngine.Random.Range(-1f, 1f)) * UnityEngine.Random.Range(0f, CurWalkSpace);
						if (NavMeshUtils.SamplePositionIterative(PlayerGO.transform.position + b, out NavMeshHit navMeshHit2, 5f, 100f, 4, -1))
						{
							walkDestination = navMeshHit2.position;
							break;
						}
						num++;
					}
					while (num <= 5);
					navAgent.acceleration = ManagerBase<FamilyManager>.Instance.Acceleration;
					SetDestination(walkDestination);
					mySpeedMeasurements.Clear();
				}
				else
				{
					idleTimer -= Time.deltaTime;
				}
			}
			else if (NavMeshAgentUtils.HasPath(navAgent))
			{
				if (IsStuck())
				{
					Vector3 vector = Vector3.Cross(PlayerGO.transform.position - base.transform.position, Vector3.up);
					float minInclusive = ManagerBase<PlayerManager>.Instance.ObstacleValue * 3f;
					float d = Mathf.Sign(UnityEngine.Random.Range(-1f, 1f));
					Vector3 sourcePosition = base.transform.position + vector.normalized * d * UnityEngine.Random.Range(minInclusive, CurWalkSpace);
					mySpeedMeasurements.Clear();
					NavMeshUtils.SamplePositionIterative(sourcePosition, out NavMeshHit navMeshHit3, 5f, 100f, 4, -1);
					navAgent.SetDestination(navMeshHit3.position);
				}
			}
			else
			{
				idle = true;
				ResetPath(resetVelocityInstantly: false);
				yield return new WaitWhile(() => navAgent.velocity != Vector3.zero);
				float maxInclusive = FamilyMemberParams.StandartIdleChance + FamilyMemberParams.SittingIdleChance + FamilyMemberParams.RestIdleChance;
				_003C_003Ec__DisplayClass103_.randomNumber = UnityEngine.Random.Range(0f, maxInclusive);
				_003C_003Ec__DisplayClass103_.curRandomMin = 0f;
				_003C_003Ec__DisplayClass103_.curRandomMax = FamilyMemberParams.StandartIdleChance;
				int num3;
				if (_003CWalkingAround_003Eg__IsRandomInBounds_007C103_3(ref _003C_003Ec__DisplayClass103_))
				{
					num3 = 0;
				}
				else
				{
					_003C_003Ec__DisplayClass103_.curRandomMin = _003C_003Ec__DisplayClass103_.curRandomMax;
					_003C_003Ec__DisplayClass103_.curRandomMax += FamilyMemberParams.SittingIdleChance;
					num3 = (_003CWalkingAround_003Eg__IsRandomInBounds_007C103_3(ref _003C_003Ec__DisplayClass103_) ? 1 : 2);
				}
				Animator.SetInteger("IdleType", num3);
				switch (num3)
				{
				case 0:
					idleTimer = Mathf.Lerp(FamilyMemberParams.StandartIdleMinimum, FamilyMemberParams.StandartIdleMaximum, UnityEngine.Random.Range(0f, 1f));
					break;
				case 1:
					idleTimer = Mathf.Lerp(FamilyMemberParams.SittingIdleMinimum, FamilyMemberParams.SittingIdleMaximum, UnityEngine.Random.Range(0f, 1f));
					break;
				default:
					idleTimer = Mathf.Lerp(FamilyMemberParams.RestIdleMinimum, FamilyMemberParams.RestIdleMaximum, UnityEngine.Random.Range(0f, 1f));
					break;
				}
			}
			if (NavMeshAgentUtils.HasPath(navAgent))
			{
				float num4 = navAgent.remainingDistance;
				if (float.IsInfinity(navAgent.remainingDistance))
				{
					num4 = 0f;
					for (int i = 0; i < navAgent.path.corners.Length - 1; i++)
					{
						num4 += (navAgent.path.corners[i + 1] - navAgent.path.corners[i]).magnitude;
					}
				}
				navAgent.speed = Mathf.Clamp(FamilyMemberParams.SpeedLow + num4 / ManagerBase<FamilyManager>.Instance.WalkDistanceMaximum * (FamilyMemberParams.SpeedMedium - FamilyMemberParams.SpeedLow), 0f, FamilyMemberParams.SpeedMedium);
				navAgent.acceleration = ManagerBase<FamilyManager>.Instance.Acceleration;
			}
			yield return null;
		}
		if (Animator.GetInteger("IdleType") != 0)
		{
			Animator.SetInteger("IdleType", 0);
			CatModel curModel2 = Model;
			yield return new WaitUntil(() => (!Animator.IsPlayingByName(AnimationHashes.idle.Value)) ? (curModel2 == Model) : true);
		}
		ChangeState(State.Following, Following());
	}

	private bool IsStuck()
	{
		if (NavMeshAgentUtils.HasPath(navAgent) && mySpeedMeasurements.Count == maxMySpeedMeasurements)
		{
			if (!(myAvgSpeed <= 0.1f))
			{
				return (startSpeedMeasurementPos - navAgent.transform.position).IsShorter(0.1f);
			}
			return true;
		}
		return false;
	}

	private bool CanReachPlayerPosition(Vector3 sourcePos)
	{
		if (NavMeshUtils.SamplePositionIterative(PlayerSpawner.PlayerInstance.transform.position, out NavMeshHit navMeshHit, 5f, 100f, 4, -1))
		{
			NavMeshPath navMeshPath = new NavMeshPath();
			NavMesh.CalculatePath(sourcePos, navMeshHit.position, -1, navMeshPath);
			return navMeshPath.status == NavMeshPathStatus.PathComplete;
		}
		return false;
	}

	private void Walking_Update()
	{
		speedMeasurementTimer -= Time.deltaTime;
		if (!(speedMeasurementTimer <= 0f))
		{
			return;
		}
		speedMeasurementTimer = 0.05f;
		playerSpeedMeasurements.Enqueue((Time.time, PlayerMovement.CurMoveSpeed));
		while (Time.time - playerSpeedMeasurements.Peek().time > ManagerBase<FamilyManager>.Instance.FollowTimer)
		{
			playerSpeedMeasurements.Dequeue();
		}
		playerAvgSpeed = playerSpeedMeasurements.Average(((float time, float speed) x) => x.speed);
		if (!navAgent.isOnOffMeshLink)
		{
			mySpeedMeasurements.Enqueue(navAgent.velocity.magnitude);
			speedMeasurementPositions.Enqueue(navAgent.transform.position);
			while (mySpeedMeasurements.Count > maxMySpeedMeasurements)
			{
				mySpeedMeasurements.Dequeue();
			}
			while (speedMeasurementPositions.Count > maxMySpeedMeasurements)
			{
				startSpeedMeasurementPos = speedMeasurementPositions.Dequeue();
			}
			myAvgSpeed = mySpeedMeasurements.Average();
		}
		CurWalkSpace = Mathf.Clamp(ManagerBase<FamilyManager>.Instance.FollowAvgSpeed / Mathf.Abs(playerAvgSpeed) * ManagerBase<FamilyManager>.Instance.WalkSpaceMaximum, 0f, ManagerBase<FamilyManager>.Instance.WalkSpaceMaximum);
	}

	private bool IsInWalkSpace()
	{
		if (NavMeshAgentUtils.HasPath(navAgent))
		{
			return Vector3.ProjectOnPlane(PlayerGO.transform.position - navAgent.destination, Vector3.up).IsShorterOrEqual(CurWalkSpace);
		}
		return Vector3.ProjectOnPlane(PlayerGO.transform.position - base.transform.position, Vector3.up).IsShorterOrEqual(CurWalkSpace);
	}

	private void OnDrawGizmos()
	{
	}

	private void Attack_Update()
	{
		Animator.SetBool("InCombat", CurState == State.Attack);
		if (curCommand == null && commands.Count <= 0 && AttackTarget != null && curState != State.Attack && EnoughAttackPower)
		{
			ChangeState(State.Attack, Attack());
		}
	}

	private IEnumerator Attack()
	{
		UpdateAttackStoppingDistanceAndRange();
		navAgent.speed = ManagerBase<PlayerManager>.Instance.SpeedMaximum;
		navAgent.acceleration = ManagerBase<FamilyManager>.Instance.Acceleration;
		ActorCombat prevAttackTarget = AttackTarget;
		while (AttackTarget != null)
		{
			if (prevAttackTarget != AttackTarget)
			{
				prevAttackTarget = AttackTarget;
				UpdateAttackStoppingDistanceAndRange();
			}
			if ((base.transform.position - AttackTarget.transform.position).IsShorterOrEqual(attackRange))
			{
				ResetPath();
				Vector3 endForward = Vector3.ProjectOnPlane(AttackTarget.transform.position - base.transform.position, Vector3.up);
				if (actorCombat.IsAttackTime())
				{
					bool isAttackCompleted = false;
					actorCombat.Attack(AttackTarget, delegate
					{
						isAttackCompleted = true;
					});
					while (!isAttackCompleted)
					{
						float rotAngle = navAgent.angularSpeed * Time.deltaTime;
						base.transform.rotation *= QuaternionUtils.GetRotation(base.transform.forward, endForward, rotAngle);
						Vector3.Angle(base.transform.forward, endForward);
						yield return null;
					}
				}
			}
			else if (!NavMeshAgentUtils.HasPath(navAgent) || !navAgent.pathPending)
			{
				if (AttackTargetController.CurState == EnemyController.State.Escape)
				{
					float num = (navAgent.transform.position - CurFollowPos).IsShorter(ManagerBase<FamilyManager>.Instance.PursuitDistanceMaximum) ? (-1f) : 1f;
					navAgent.speed = Mathf.Clamp(navAgent.speed + ManagerBase<FamilyManager>.Instance.PursuitDecceleration * Time.deltaTime * num, 0f, Math.Max(ManagerBase<PlayerManager>.Instance.curSpeed, ManagerBase<PlayerManager>.Instance.SpeedMedium));
					NavMeshUtils.SamplePositionIterative(CurFollowPos, out NavMeshHit navMeshHit, 2f, 100f, 4, -1);
					SetDestination(navMeshHit.position);
				}
				else
				{
					navAgent.speed = Mathf.Clamp(navAgent.speed + ManagerBase<FamilyManager>.Instance.PursuitDecceleration * Time.deltaTime, 0f, Math.Max(ManagerBase<PlayerManager>.Instance.curSpeed, ManagerBase<PlayerManager>.Instance.SpeedMedium));
					NavMeshUtils.SamplePositionIterative(AttackTarget.transform.position, out NavMeshHit navMeshHit2, 2f, 100f, 4, -1);
					SetDestination(navMeshHit2.position);
				}
			}
			yield return null;
		}
		ChangeState(State.Following, Following());
	}

	public void SwitchInvisibility(bool isEnabled)
	{
		if (isInited)
		{
			if (isEnabled)
			{
				InvisibilityEnterCommand command = new InvisibilityEnterCommand(this, Animator);
				AddCommand(command);
			}
			else
			{
				InvisibilityExitCommand command2 = new InvisibilityExitCommand(this, Animator);
				AddCommand(command2);
			}
		}
	}

	public void Eat(Food food, Action endCallback)
	{
		if (isInited)
		{
			EatCommand command = new EatCommand(this, food, endCallback);
			AddCommand(command);
			food.HoldFoodUnit(itemUser);
			holdedFood.Add(food);
		}
	}

    private float GetEatingDistance(Vector3 foodPos)
    {
        float magnitude = (PlayerGO.transform.position - foodPos).magnitude;
        float num = Mathf.Clamp(ManagerBase<PlayerManager>.Instance.ObstacleValue - magnitude, 0f, ManagerBase<PlayerManager>.Instance.ObstacleValue);
        return Mathf.Clamp(ManagerBase<FamilyManager>.Instance.EatingDistance * Mathf.Lerp(FamilyMemberParams.ModelScalePerc / 100f, 1f, 0.3f), num + this.navAgent.radius, float.MaxValue);
    }

    private Vector3 RotatedToFoodForward(Food food)
    {
        return Vector3.ProjectOnPlane(food.transform.position - transform.position, Vector3.up);
    }

    private IEnumerator Eating(Food food, Action endCallback)
	{
		CatModel curModel = Model;
		feedingState = FeedingState.None;
		NavMeshUtils.SamplePositionIterative(food.transform.position, out NavMeshHit foodNavMeshHit, 5f, 100f, 4, -1);
		_003C_003Ec__DisplayClass127_0 CS_0024_003C_003E8__locals0;
		float eatingDistance = GetEatingDistance(foodNavMeshHit.position);
		navAgent.stoppingDistance = eatingDistance;
		float updateEatingDistanceTimer = 0.2f;
		float curEatTime = 0f;
		while (curEatTime < ManagerBase<FamilyManager>.Instance.EatingDuration)
		{
			if (updateEatingDistanceTimer <= 0f)
			{
				eatingDistance = GetEatingDistance(foodNavMeshHit.position);
				navAgent.stoppingDistance = eatingDistance;
				updateEatingDistanceTimer = 0.2f;
			}
			if (!(base.transform.position - foodNavMeshHit.position).IsShorterOrEqual(eatingDistance))
			{
				feedingState = FeedingState.Move;
			}
			else if (Vector3.Angle(base.transform.forward, RotatedToFoodForward(food)) != 0f)
			{
				feedingState = FeedingState.Rotate;
			}
			else if (curEatTime < ManagerBase<FamilyManager>.Instance.EatingDuration)
			{
				feedingState = FeedingState.Feed;
			}
			if (feedingState == FeedingState.Move && !Animator.IsPlayingByName(AnimationHashes.eatExit.Value))
			{
				navAgent.speed = ManagerBase<PlayerManager>.Instance.SpeedMaximum;
				navAgent.acceleration = ManagerBase<FamilyManager>.Instance.Acceleration;
				if (!navAgent.pathPending)
				{
					SetDestination(foodNavMeshHit.position);
				}
			}
			else
			{
				ResetPath();
			}
			if (feedingState == FeedingState.Rotate)
			{
				float rotAngle = ManagerBase<PlayerManager>.Instance.EatDrinkRotationSpeed * Time.deltaTime;
				Quaternion rotation = QuaternionUtils.GetRotation(base.transform.forward, RotatedToFoodForward(food), rotAngle);
				base.transform.rotation *= rotation;
				if (rotation == Quaternion.identity)
				{
					feedingState = FeedingState.Feed;
				}
			}
			if (feedingState == FeedingState.Feed)
			{
				if (!Animator.GetBool("isEating"))
				{
					Animator.SetTrigger("Eat");
					Animator.SetBool("isEating", value: true);
				}
			}
			else
			{
				Animator.SetBool("isEating", value: false);
			}
			yield return null;
			if (feedingState == FeedingState.Feed)
			{
				curEatTime += Time.deltaTime;
			}
			updateEatingDistanceTimer -= Time.deltaTime;
		}
		feedingState = FeedingState.None;
		Animator.SetBool("isEating", value: false);
		if (IsNeededFood(food))
		{
			ManagerBase<FamilyManager>.Instance.AddExperience(ManagerBase<FamilyManager>.Instance.СhildEatingExperience * ManagerBase<FamilyManager>.Instance.DemandExpMulty, familyMemberData);
			SatisfyNeed();
		}
		else
		{
			ManagerBase<FamilyManager>.Instance.AddExperience(ManagerBase<FamilyManager>.Instance.СhildEatingExperience, familyMemberData);
		}
		food.EatFoodUnit();
		holdedFood.Remove(food);
		if (Animator.IsPlayingByTag(AnimationHashes.eat.Value) || Animator.IsPlayingNextByTag(AnimationHashes.eat.Value))
		{
			yield return new WaitWhile(() => !Animator.IsPlayingByName(AnimationHashes.eatExit.Value) && curModel == Model);
			yield return new WaitWhile(() => Animator.IsPlayingByName(AnimationHashes.eatExit.Value) && curModel == Model);
		}
		endCallback?.Invoke();
	}

	private void Drink_Initialize()
	{
		Combat.hitEvent += OnHit;
	}

	private void OnHit(ActorCombat attacker, ActorCombat target)
	{
		float num = ManagerBase<PlayerManager>.Instance.ThirstHitFallPerc / 100f * ManagerBase<PlayerManager>.Instance.ThirstMaximum;
		ChangeThirst(0f - num);
	}

	private void Drink_Destroy()
	{
		Combat.hitEvent -= OnHit;
	}

	public void Drink(Vector3 familyDrinkPoint, DrinkType drinkType, Water water)
	{
		if (isInited)
		{
			DrinkCommand command = new DrinkCommand(this, familyDrinkPoint, drinkType, water);
			AddCommand(command);
		}
	}

    private float GetDrinkingDistance(Vector3 drinkPos)
    {
        float magnitude = (this.PlayerGO.transform.position - drinkPos).magnitude;
        float obstacleValue = ManagerBase<PlayerManager>.Instance.ObstacleValue;
        obstacleValue += this.navAgent.radius;
        return Mathf.Clamp(obstacleValue - magnitude, 0f, obstacleValue);
    }

    private bool IsDrinkAnimationPlaying()
    {
        if (!Animator.IsPlayingByName(AnimationHashes.drinkExit.Value) && !Animator.IsPlayingByName(AnimationHashes.drinkEnter.Value))
        {
            return Animator.IsPlayingByName(AnimationHashes.drinkCycle.Value);
        }
        return true;
    }

    private IEnumerator Drinking(Vector3 familyDrinkPoint, DrinkType drinkType, Water water, Action endCallback)
	{
		CatModel curModel = Model;
		mySpeedMeasurements.Clear();
		if (drinkType == DrinkType.FromDrinkPosition)
		{
			float d = startNavAgentRadius - navAgent.radius;
			familyDrinkPoint += (water.Collider.transform.position - familyDrinkPoint).normalized * d;
		}
		NavMesh.GetAreaFromName("Water");
		float updateNavPosTimer = 0f;
		float curDrinkTime = 0f;
		feedingState = FeedingState.Move;
		bool rotateToWaterCenter = drinkType == DrinkType.FromDrinkPosition;
		bool onDrinkPos = false;
		bool mouthInWater = false;
		bool haveWaterNeed = familyMemberData.curNeed == "Water";
		NavMeshUtils.SamplePositionIterative(familyDrinkPoint, out NavMeshHit waterNavMeshHit, 5f, 100f, 4, -1);
		int quenchThirstCount = 0;
		_003C_003Ec__DisplayClass137_0 CS_0024_003C_003E8__locals0;
		while (HaveThirst())
		{
			if (updateNavPosTimer <= 0f)
			{
				updateNavPosTimer = 0.1f;
				NavMeshUtils.SamplePositionIterative(familyDrinkPoint, out waterNavMeshHit, 5f, 100f, 4, -1);
				Vector3 vector = water.Collider.ClosestPoint(Model.MouthDrinkPos.position);
				mouthInWater = (vector.x == Model.MouthDrinkPos.position.x && vector.z == Model.MouthDrinkPos.position.z && vector.y >= Model.MouthDrinkPos.position.y);
				onDrinkPos = (base.transform.position - waterNavMeshHit.position).IsShorterOrEqual(navAgent.stoppingDistance);
				navAgent.stoppingDistance = GetDrinkingDistance(waterNavMeshHit.position);
			}
			if (mouthInWater && drinkType == DrinkType.FromNearestWaterPoint)
			{
				feedingState = FeedingState.Feed;
			}
			else if (!onDrinkPos)
			{
				feedingState = FeedingState.Move;
			}
			else
			{
				feedingState = FeedingState.Rotate;
			}
			if (feedingState == FeedingState.Move && !IsDrinkAnimationPlaying())
			{
				navAgent.speed = ManagerBase<PlayerManager>.Instance.SpeedMaximum;
				navAgent.acceleration = ManagerBase<FamilyManager>.Instance.Acceleration;
				if (!navAgent.pathPending)
				{
					SetDestination(waterNavMeshHit.position);
				}
			}
			else
			{
				ResetPath();
			}
			if (feedingState != 0)
			{
				mySpeedMeasurements.Clear();
			}
			if (feedingState == FeedingState.Rotate)
			{
				float rotAngle = ManagerBase<PlayerManager>.Instance.EatDrinkRotationSpeed * Time.deltaTime;
				Vector3 zero = Vector3.zero;
				Quaternion quaternion = QuaternionUtils.GetRotation(endDir: (!rotateToWaterCenter) ? Vector3.ProjectOnPlane(waterNavMeshHit.position - base.transform.position, Vector3.up) : Vector3.ProjectOnPlane(water.Collider.transform.position - base.transform.position, Vector3.up), startDir: base.transform.forward, rotAngle: rotAngle);
				base.transform.rotation *= quaternion;
				if (quaternion == Quaternion.identity)
				{
					feedingState = FeedingState.Feed;
				}
			}
			if (feedingState == FeedingState.Feed)
			{
				if (!Animator.GetBool("isDrinking"))
				{
					Animator.SetBool("isDrinking", value: true);
					Animator.SetTrigger("Drink");
				}
			}
			else
			{
				Animator.SetBool("isDrinking", value: false);
			}
			yield return null;
			if (feedingState == FeedingState.Feed)
			{
				curDrinkTime += Time.deltaTime;
			}
			updateNavPosTimer -= Time.deltaTime;
			if (curDrinkTime >= ManagerBase<FamilyManager>.Instance.DrinkingDuration)
			{
				curDrinkTime = 0f;
				int num = quenchThirstCount + 1;
				quenchThirstCount = num;
				ChangeThirst(ManagerBase<PlayerManager>.Instance.ThirstEffect);
			}
		}
		if (haveWaterNeed)
		{
			ManagerBase<FamilyManager>.Instance.AddExperience(ManagerBase<FamilyManager>.Instance.ChildThirstExperience * ManagerBase<FamilyManager>.Instance.DemandExpMulty, familyMemberData);
		}
		else
		{
			ManagerBase<FamilyManager>.Instance.AddExperience(ManagerBase<FamilyManager>.Instance.ChildThirstExperience, familyMemberData);
		}
		if (haveWaterNeed)
		{
			SatisfyNeed();
		}
		feedingState = FeedingState.None;
		Animator.SetBool("isDrinking", value: false);
		if (Animator.IsPlayingByTag(AnimationHashes.drink.Value) || Animator.IsPlayingNextByTag(AnimationHashes.drink.Value))
		{
			yield return new WaitWhile(() => !Animator.IsPlayingByName(AnimationHashes.drinkExit.Value) && curModel == Model);
			yield return new WaitWhile(() => Animator.IsPlayingByName(AnimationHashes.drinkExit.Value) && curModel == Model);
		}
		FamilyMemberController.endDrinkEvent?.Invoke(this, quenchThirstCount);
		endCallback?.Invoke();
	}

	private void Drinking_Update()
	{
		if (Time.deltaTime != 0f)
		{
			ChangeThirst((0f - ManagerBase<FamilyManager>.Instance.ThirstFall) / 100f * ManagerBase<FamilyManager>.Instance.ThirstMaximum * Time.deltaTime);
		}
	}

	public void ChangeThirst(float value)
	{
		familyMemberData.thirst = Mathf.Clamp(familyMemberData.thirst + value, 0f, ManagerBase<FamilyManager>.Instance.ThirstMaximum);
	}

	private bool HaveThirst()
	{
		return familyMemberData.thirst < ManagerBase<FamilyManager>.Instance.ThirstMaximum - ManagerBase<PlayerManager>.Instance.ThirstEffect;
	}

	private void Sleep_Initialize()
	{
		commandExecutedEvent += Sleeping_OnCommandExecuted;
	}

	private void Sleep_Destroy()
	{
		commandExecutedEvent -= Sleeping_OnCommandExecuted;
	}

	private void Sleeping_OnCommandExecuted(FamilyMemberController familyMemberController)
	{
		if (familyMemberController == this)
		{
			UpdateCanSleepState();
		}
	}

	public bool CanSleep()
	{
		if (!HaveCommands())
		{
			return CurState != State.Disabled;
		}
		return false;
	}

	private void UpdateCanSleepState()
	{
		if (CanSleep() != prevCanSleep)
		{
			prevCanSleep = !prevCanSleep;
			changeCanSleepStateEvent?.Invoke(this);
		}
	}

	public void Sleep(Vector3 sleepPosition)
	{
		if (isInited && CanSleep())
		{
			AddCommand(new SleepCommand(this, sleepPosition));
		}
	}


    private IEnumerator Sleeping(Vector3 sleepPosition, Action sleepStartCallback)
	{
		CatModel curModel = Model;
		float moveTimer = 3f;
		navAgent.speed = FamilyMemberParams.SpeedMedium;
		navAgent.acceleration = ManagerBase<FamilyManager>.Instance.Acceleration;
		RecalculateStoppingDistance();
		SetDestination(sleepPosition);
		while (NavMeshAgentUtils.HasPath(navAgent) && moveTimer > 0f)
		{
			RecalculateStoppingDistance();
			yield return null;
			moveTimer -= Time.deltaTime;
		}
		ResetPath();
		NavAgent.stoppingDistance = 0f;
		if (Animator.GetInteger("IdleType") != 2)
		{
			Animator.SetInteger("IdleType", 0);
			yield return new WaitWhile(() => !Animator.IsPlayingByName(AnimationHashes.idle.Value) && curModel == Model);
		}
		Animator.SetInteger("IdleType", 2);
		Animator.SetBool("isSleeping", value: true);
		yield return new WaitWhile(() => !Animator.IsPlayingByName(AnimationHashes.sleep.Value) && curModel == Model);
		IsSleeping = true;
		sleepStartCallback?.Invoke();
	}

	public void AwakeFamilyMember()
	{
		if (isInited && IsSleeping)
		{
			AddCommand(new AwakeCommand(this));
		}
	}

	private IEnumerator Awaking(Action endCallback)
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 0.5f));
		Animator.SetInteger("IdleType", 0);
		Animator.SetBool("isSleeping", value: false);
		CatModel curModel = Model;
		yield return new WaitWhile(() => !Animator.IsPlayingByName(AnimationHashes.idle.Value) && curModel == Model);
		IsSleeping = false;
		endCallback?.Invoke();
		ChangeState(State.WalkingAround, WalkingAround());
	}

	private void Needs_Initialize()
	{
		if (IsFamilyMemberWithNeeds())
		{
			if (!IsNeedAvailableToSatisfy(familyMemberData.curNeed))
			{
				GenerateNeed();
			}
			Quest.startEvent += OnQuestStart;
		}
	}

	private bool IsFamilyMemberWithNeeds()
	{
		if (familyMemberData.role != 0)
		{
			return familyMemberData.role == FamilyManager.FamilyMemberRole.SecondStageChild;
		}
		return true;
	}

	public bool IsNeededFood(Food food)
	{
		if (IsFamilyMemberWithNeeds() && food != null)
		{
			return food.Id.ToString() == familyMemberData.curNeed;
		}
		return false;
	}

	public bool HaveFoodInEatQueue(Food food)
	{
		return (from x in commands
			where x is EatCommand
			select x as EatCommand).Any((EatCommand x) => x.Food.Name == food.Name);
	}

	private bool IsNeedAvailableToSatisfy(string need)
	{
		if (string.IsNullOrEmpty(need))
		{
			return false;
		}
		if (need == "Water")
		{
			return false;
		}
		if (ItemIds.Any((string x) => x.ToString() == need))
		{
			return true;
		}
		if (Singleton<EnemySpawner>.Instance == null)
		{
			return false;
		}
		if (Singleton<EnemySpawner>.Instance.Archetypes.Find((EnemyArchetype x) => x.name == need) == null)
		{
			return false;
		}
		return Singleton<EnemySpawner>.Instance.SpawnedEnemies.Count((EnemySpawner.EnemyInstance x) => x.logic.currentScheme.Archetype.name == need) >= 1;
	}

	private void GenerateNeed()
	{
		RemoveNeed();
		if (!IsFamilyMemberWithNeeds())
		{
			return;
		}
		List<string> availableNeeds = new List<string>(familyMemberData.Params.needs.FindAll((string need) => IsNeedAvailableToSatisfy(need)));
		if (availableNeeds.Count != 0)
		{
			if (UnityEngine.Random.Range(0f, ManagerBase<FamilyManager>.Instance.HuntDemandChance + ManagerBase<FamilyManager>.Instance.EatDemandChance) < ManagerBase<FamilyManager>.Instance.EatDemandChance)
			{
				string curNeed = (from x in availableNeeds
					where ItemIds.Contains(x)
					select x).Random().ToString();
				familyMemberData.curNeed = curNeed;
			}
			else
			{
				string name = ((from x in Singleton<EnemySpawner>.Instance.SpawnedEnemies
					where availableNeeds.Contains(x.logic.currentScheme.Archetype.name)
					select x)?.Select((EnemySpawner.EnemyInstance x) => x.logic.currentScheme.Archetype).Distinct()).Random().name;
				familyMemberData.curNeed = name;
			}
			FamilyMemberController.changeNeedEvent?.Invoke(this);
		}
	}

	public void SatisfyNeed()
	{
		RemoveNeed();
		FamilyMemberController.satisfyNeedEvent?.Invoke(this);
		if (needPauseCor != null)
		{
			StopCoroutine(needPauseCor);
			needPauseCor = null;
		}
		if (Singleton<QuestSpawner>.Instance != null && Singleton<QuestSpawner>.Instance.ActiveQuest is EducateChildQuest)
		{
			GenerateNeed();
		}
		else
		{
			needPauseCor = StartCoroutine(Timer(ManagerBase<FamilyManager>.Instance.DemandCooldown, delegate
			{
				GenerateNeed();
				needPauseCor = null;
			}));
		}
	}

	private IEnumerator Timer(float time, Action endCallback)
	{
		for (float curTime = 0f; curTime < time; curTime += Time.deltaTime)
		{
			yield return null;
		}
		endCallback?.Invoke();
	}

	private void RemoveNeed()
	{
		if (!string.IsNullOrEmpty(familyMemberData.curNeed))
		{
			familyMemberData.curNeed = null;
			FamilyMemberController.changeNeedEvent?.Invoke(this);
		}
	}

	private void Needs_Destroy()
	{
		Quest.startEvent -= OnQuestStart;
	}

	private void OnQuestStart(Quest quest)
	{
		if (IsFamilyMemberWithNeeds() && quest is EducateChildQuest && string.IsNullOrEmpty(familyMemberData.curNeed))
		{
			if (needPauseCor != null)
			{
				StopCoroutine(needPauseCor);
				needPauseCor = null;
			}
			GenerateNeed();
		}
	}

	private void Skin_Initialize()
	{
		skinner.Initialize();
	}

	[CompilerGenerated]
	private void _003CUpdate_003Eg__CalculacteAnimationMovementSpeed_007C42_0(float curMoveSpeed)
	{
		float num = 71f / (678f * (float)Math.PI) * ManagerBase<FamilyManager>.Instance.RotationSpeed;
		float num2 = 0f;
		float num3 = Mathf.Abs(curMoveSpeed);
		num2 = ((!(num3 > num)) ? Mathf.Clamp(num3 + Mathf.Abs(Animator.GetFloat("NormalizedRotationSpeed")) * num, 0f - num, num) : num3);
		Animator.SetFloat("MovementSpeed", Mathf.Abs(num2));
		if (navAgent.velocity.sqrMagnitude != 0f)
		{
			Animator.SetFloat("NormalizedMovementSpeed", 1f);
			Animator.SetFloat("SmoothNormalizedMovementSpeed", 1f);
		}
		else
		{
			Animator.SetFloat("NormalizedMovementSpeed", 0f);
			Animator.SetFloat("SmoothNormalizedMovementSpeed", 0f);
		}
	}

	[CompilerGenerated]
	private static bool _003COnStageUp_003Eg__IsMaxStage_007C49_0(ref _003C_003Ec__DisplayClass49_0 P_0)
	{
		if (P_0.familyMemberData.role != FamilyManager.FamilyMemberRole.Spouse)
		{
			return P_0.familyMemberData.role == FamilyManager.FamilyMemberRole.ThirdStageChild;
		}
		return true;
	}

	[CompilerGenerated]
	private static bool _003CWalkingAround_003Eg__IsRandomInBounds_007C103_3(ref MyRandom P_0)
	{
		if (P_0.randomNumber >= P_0.curRandomMin)
		{
			return P_0.randomNumber <= P_0.curRandomMax;
		}
		return false;
	}

	private static float _003CSleeping_003Eg__CalculateObstacleRadius_007C153_5(NavMeshAgent navMeshAgent)
	{
		return navMeshAgent.radius * Mathf.Max(navMeshAgent.transform.localScale.x, navMeshAgent.transform.localScale.z);
	}
}
