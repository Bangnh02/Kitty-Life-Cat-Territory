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

[RequireComponent(typeof(EnemyCurrentScheme))]
public class EnemyController : MonoBehaviour
{
	public enum State
	{
		None,
		Patrol,
		Attack,
		Escape,
		Pursuit,
		Die
	}

	private class PursuitData
	{
		private EnemyController enemyController;

		private float cantReachPlayerTimer;

		private const float cantReachPlayerMaxTime = 5f;

		private float recalcReachPlayerTimer;

		private const float recalcReachPlayerFrequency = 1f;

		private bool canReachPlayer = true;

		public bool IsPlayerUnreachable
		{
			get;
			private set;
		}

		public PursuitData(EnemyController enemyController)
		{
			this.enemyController = enemyController;
			cantReachPlayerTimer = 5f;
			recalcReachPlayerTimer = 1f;
		}

		public void Update(float deltaTime)
		{
			if (deltaTime == 0f)
			{
				return;
			}
			recalcReachPlayerTimer -= deltaTime;
			if (recalcReachPlayerTimer <= 0f)
			{
				recalcReachPlayerTimer = 1f;
				canReachPlayer = enemyController.CanReachPlayerPosition(enemyController.NavAgent.transform.position);
				if (canReachPlayer)
				{
					cantReachPlayerTimer = 5f;
				}
			}
			if (!canReachPlayer)
			{
				cantReachPlayerTimer -= deltaTime;
			}
			if (cantReachPlayerTimer <= 0f)
			{
				IsPlayerUnreachable = true;
			}
		}
	}

	private class PatrolData
	{
		private EnemyController enemyController;

		private float recalcReachPlayerTimer;

		private const float recalcReachPlayerFrequency = 0.2f;

		public bool CanReachPlayer
		{
			get;
			private set;
		} = true;


		public bool IsNeedRotateToPlayer
		{
			get
			{
				if (enemyController.isPlayerHeard)
				{
					return CanReachPlayer;
				}
				return false;
			}
		}

		public PatrolData(EnemyController enemyController)
		{
			this.enemyController = enemyController;
			CanReachPlayer = ((enemyController.isPlayerHeard || (!enemyController.PlayerCombat.IsHearable && enemyController.IsPlayerVisible())) && enemyController.CanReachPlayerPosition(enemyController.NavAgent.transform.position));
			recalcReachPlayerTimer = 0.2f;
		}

		public void Update(float deltaTime)
		{
			if (deltaTime != 0f)
			{
				recalcReachPlayerTimer -= deltaTime;
				if (recalcReachPlayerTimer <= 0f)
				{
					recalcReachPlayerTimer = 0.2f;
					CanReachPlayer = ((enemyController.isPlayerHeard || (!enemyController.PlayerCombat.IsHearable && enemyController.IsPlayerVisible())) && enemyController.CanReachPlayerPosition(enemyController.NavAgent.transform.position));
				}
			}
		}
	}

	public delegate void CreationOrSpawnAIHandler(EnemyController enemyController);

	public delegate void ChangeStateHandler(EnemyController enemyController, State newState);

	public delegate void ChangeMoveModeHandler(bool isNavAgentMode);

	public delegate void SelfTransitionToAnyAttackStateHandler(bool isInAttack);

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass201_0
	{
		public EnemyController _003C_003E4__this;

		public List<EnemyArchetype> myAllyArchetypes;

		internal bool _003Cget_Allies_003Eb__0(EnemySpawner.ArchetypeAllies x)
		{
			return x.archetype == _003C_003E4__this.MyArchetype;
		}

		internal bool _003Cget_Allies_003Eb__1(EnemyController x)
		{
			if (myAllyArchetypes.Contains(x.MyArchetype) && x.CurState != State.Die && x.gameObject.activeInHierarchy && !x.IsBossScheme)
			{
				return !x.IsBossAssistantScheme;
			}
			return false;
		}
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Comparison<(EnemyController ally, float sqrMagnitude)> _003C_003E9__227_1;

		public static Comparison<(EnemyHabitatArea area, float sqrMagnitude)> _003C_003E9__227_3;

		public static Predicate<EnemyController> _003C_003E9__233_0;

		public static Predicate<EnemyController> _003C_003E9__255_0;

		public static Comparison<float> _003C_003E9__262_1;

		internal int _003CGetNextEscapePosition_003Eb__227_1((EnemyController ally, float sqrMagnitude) item1, (EnemyController ally, float sqrMagnitude) item2)
		{
			return item1.sqrMagnitude.CompareTo(item2.sqrMagnitude);
		}

		internal int _003CGetNextEscapePosition_003Eb__227_3((EnemyHabitatArea area, float sqrMagnitude) item1, (EnemyHabitatArea area, float sqrMagnitude) item2)
		{
			return item1.sqrMagnitude.CompareTo(item2.sqrMagnitude);
		}

		internal bool _003CHaveAllyInFight_003Eb__233_0(EnemyController x)
		{
			if (x.CurState != State.Pursuit)
			{
				return x.CurState == State.Attack;
			}
			return true;
		}

		internal bool _003CUpdate_003Eb__255_0(EnemyController habitant)
		{
			return habitant.CurState == State.Patrol;
		}

		internal int _003CPatrol_003Eb__262_1(float x, float y)
		{
			return x.CompareTo(y);
		}
	}

	[CompilerGenerated]
	private sealed class _003CTraverseNavLinkParabola_003Ed__259 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public NavMeshAgent agent;

		public float height;

		public float duration;

		public EnemyController _003C_003E4__this;

		private Vector3 _003CstartPos_003E5__2;

		private Vector3 _003CendPos_003E5__3;

		private Vector3 _003CstartForward_003E5__4;

		private Vector3 _003CendForward_003E5__5;

		private float _003CnormalizedTime_003E5__6;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CTraverseNavLinkParabola_003Ed__259(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			EnemyController enemyController = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				OffMeshLinkData currentOffMeshLinkData = agent.currentOffMeshLinkData;
				_003CstartPos_003E5__2 = agent.transform.position;
				_003CendPos_003E5__3 = currentOffMeshLinkData.endPos + Vector3.up * agent.baseOffset;
				_003CstartForward_003E5__4 = agent.transform.forward;
				_003CendForward_003E5__5 = (new Vector3(_003CendPos_003E5__3.x, agent.transform.position.y, _003CendPos_003E5__3.z) - agent.transform.position).normalized;
				_003CnormalizedTime_003E5__6 = 0f;
				break;
			}
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (_003CnormalizedTime_003E5__6 < 1f)
			{
				float d = height * 4f * (_003CnormalizedTime_003E5__6 - _003CnormalizedTime_003E5__6 * _003CnormalizedTime_003E5__6);
				Vector3 position = agent.transform.position;
				agent.transform.position = Vector3.Lerp(_003CstartPos_003E5__2, _003CendPos_003E5__3, _003CnormalizedTime_003E5__6) + d * Vector3.up;
				Vector3.Lerp(_003CstartForward_003E5__4, _003CendForward_003E5__5, Mathf.Clamp(_003CnormalizedTime_003E5__6 * 2f, 0f, 1f));
				agent.transform.LookAt(agent.transform.position + _003CendForward_003E5__5);
				_003CnormalizedTime_003E5__6 += Time.deltaTime / duration;
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			enemyController.traverseNavLinkCor = null;
			if (enemyController.NavAgent.isOnNavMesh)
			{
				enemyController.NavAgent.CompleteOffMeshLink();
			}
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
	private sealed class _003CRotationToGroundNormal_003Ed__260 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public EnemyController _003C_003E4__this;

		private Quaternion _003CstartRotation_003E5__2;

		private Quaternion _003CendRotation_003E5__3;

		private float _003Ctimer_003E5__4;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CRotationToGroundNormal_003Ed__260(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			EnemyController enemyController = _003C_003E4__this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				_003C_003E1__state = -1;
			}
			else
			{
				_003C_003E1__state = -1;
				Physics.Raycast(enemyController.ModelTransform.position, Vector3.down, out RaycastHit hitInfo, 100f, 1 << Layers.ColliderLayer);
				_003CstartRotation_003E5__2 = enemyController.ModelTransform.rotation;
				_003CendRotation_003E5__3 = Quaternion.FromToRotation(enemyController.ModelTransform.up, hitInfo.normal) * enemyController.ModelTransform.rotation;
				_003Ctimer_003E5__4 = 0f;
			}
			_003Ctimer_003E5__4 = Mathf.Clamp(_003Ctimer_003E5__4 + Time.deltaTime, 0f, 0.5f);
			float t = _003Ctimer_003E5__4 / 0.5f;
			enemyController.ModelTransform.rotation = Quaternion.Slerp(_003CstartRotation_003E5__2, _003CendRotation_003E5__3, t);
			if (enemyController.ModelTransform.rotation == _003CendRotation_003E5__3)
			{
				return false;
			}
			_003C_003E2__current = null;
			_003C_003E1__state = 1;
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

	

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003C_003Ec__DisplayClass262_0
	{
		public EnemyController _003C_003E4__this;

		public bool needBossRotate;

		public float checkPosAndRotBossTimer;
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003C_003Ec__DisplayClass262_1
	{
		public Vector3 needForward;
	}

	[CompilerGenerated]
	private sealed class _003CPatrol_003Ed__262 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public EnemyController _003C_003E4__this;

		private _003C_003Ec__DisplayClass262_0 _003C_003E8__1;

		private WaitForSeconds _003Cwfs_003E5__2;

		private float _003CcurStopCooldown_003E5__3;

		private float _003CpatrolInterval_003E5__4;

		private Vector3 _003CnextPosition_003E5__5;

		private List<float> _003CobstacleFromAgents_003E5__6;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CPatrol_003Ed__262(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			EnemyController enemyController = _003C_003E4__this;
			bool flag;
			switch (num)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
				_003Cwfs_003E5__2 = new WaitForSeconds(enemyController.timeToWaitForNextAction);
				enemyController.needMove = true;
				enemyController.isPlayerHeard = enemyController.IsPlayerHeard();
				enemyController.ResetMove();
				float num2 = 0f;
				_003CcurStopCooldown_003E5__3 = 0f;
				_003CpatrolInterval_003E5__4 = 0f;
				enemyController.lastPatrolPoint = Vector3.zero;
				_003CnextPosition_003E5__5 = enemyController.lastPatrolPoint;
				_003CobstacleFromAgents_003E5__6 = new List<float>();
				_003C_003E8__1.needBossRotate = true;
				_003C_003E8__1.checkPosAndRotBossTimer = 0f;
				goto IL_00d7;
			}
			case 1:
				_003C_003E1__state = -1;
				enemyController.needMove = false;
				enemyController.myAgent.SetDestination(_003CnextPosition_003E5__5);
				_003C_003E2__current = null;
				_003C_003E1__state = 2;
				return true;
			case 2:
				_003C_003E1__state = -1;
				goto IL_00d7;
			case 3:
				_003C_003E1__state = -1;
				goto IL_00d7;
			case 4:
				_003C_003E1__state = -1;
				goto IL_00d7;
			case 5:
				_003C_003E1__state = -1;
				goto IL_00d7;
			case 6:
				_003C_003E1__state = -1;
				goto IL_00d7;
			case 7:
				_003C_003E1__state = -1;
				goto IL_00d7;
			case 8:
				{
					_003C_003E1__state = -1;
					goto IL_00d7;
				}
				IL_00d7:
				flag = (!enemyController.needMove && !NavMeshAgentUtils.HasPath(enemyController.myAgent) && !enemyController.patrolData.IsNeedRotateToPlayer);
				if (!enemyController.IsBossScheme && !enemyController.IsBossAssistantScheme)
				{
					if (NavMeshAgentUtils.HasPath(enemyController.myAgent) && enemyController.enemyHabitatArea.Inhabitants.Count > 1 && enemyController.EnemyProcessingSwitch.OnProcessingDistance)
					{
						_003CobstacleFromAgents_003E5__6.Clear();
						foreach (EnemyController inhabitant in enemyController.enemyHabitatArea.Inhabitants)
						{
							if (inhabitant != enemyController)
							{
								_003CobstacleFromAgents_003E5__6.Add(enemyController._003CPatrol_003Eg__CalculateObstacle_007C262_2(enemyController.myAgent.destination, inhabitant.NavAgent, ref _003C_003E8__1));
							}
						}
						_003CobstacleFromAgents_003E5__6.Sort((float x, float y) => x.CompareTo(y));
						float stoppingDistance = _003CobstacleFromAgents_003E5__6[_003CobstacleFromAgents_003E5__6.Count - 1];
						enemyController.myAgent.stoppingDistance = stoppingDistance;
					}
					else
					{
						enemyController.myAgent.stoppingDistance = 0f;
					}
				}
				if (!NavMeshAgentUtils.HasPath(enemyController.myAgent) && enemyController.needMove && !enemyController.patrolData.IsNeedRotateToPlayer && !enemyController.stoppedByAlly)
				{
					enemyController.myAgent.ResetPath();
					_003CcurStopCooldown_003E5__3 = 0f;
					if (enemyController.IsBossScheme)
					{
						_003CnextPosition_003E5__5 = enemyController.GetSampledPosition(Singleton<EnemySpawner>.Instance.BossSpawnPoint.transform.position);
					}
					else if (enemyController.IsBossAssistantScheme)
					{
						_003CnextPosition_003E5__5 = enemyController.GetSampledPosition(enemyController.bossAssistantSpawnPoint.Position);
					}
					else if (enemyController.lastPatrolPoint != Vector3.zero)
					{
						_003CnextPosition_003E5__5 = enemyController.lastPatrolPoint;
						enemyController.lastPatrolPoint = Vector3.zero;
					}
					else
					{
						_003CnextPosition_003E5__5 = enemyController.GetNextPatrolPosition();
					}
					_003C_003E2__current = enemyController.StartCoroutine(enemyController.WaitForExitExtraIdleAnimation());
					_003C_003E1__state = 1;
					return true;
				}
				if (flag && !enemyController.stoppedByAlly)
				{
					if (enemyController.returnFromPursuit)
					{
						enemyController.returnFromPursuit = false;
					}
					if (enemyController.IsBossScheme || enemyController.IsBossAssistantScheme)
					{
						if (_003C_003E8__1.needBossRotate)
						{
							_003C_003Ec__DisplayClass262_1 _003C_003Ec__DisplayClass262_ = default(_003C_003Ec__DisplayClass262_1);
							_003C_003Ec__DisplayClass262_.needForward = (enemyController.IsBossScheme ? Singleton<EnemySpawner>.Instance.BossSpawnPoint.transform.forward : enemyController.bossAssistantSpawnPoint.transform.forward);
							enemyController._003CPatrol_003Eg__RotateCurModel_007C262_3(ref _003C_003E8__1, ref _003C_003Ec__DisplayClass262_);
							_003C_003E2__current = null;
							_003C_003E1__state = 3;
							return true;
						}
						if (_003C_003E8__1.checkPosAndRotBossTimer == 0f)
						{
							Vector3 sourcePos = enemyController.IsBossScheme ? Singleton<EnemySpawner>.Instance.BossSpawnPoint.transform.position : enemyController.bossAssistantSpawnPoint.Position;
							Vector3 sampledPosition = enemyController.GetSampledPosition(sourcePos);
							enemyController._003CPatrol_003Eg__CheckModelPosition_007C262_0(sampledPosition, ref _003C_003E8__1);
							_003C_003E8__1.checkPosAndRotBossTimer = Singleton<EnemySpawner>.Instance.GlobalEnemyParams.checkPosAndRotBossInterval;
						}
						_003C_003E8__1.checkPosAndRotBossTimer = Mathf.Clamp(_003C_003E8__1.checkPosAndRotBossTimer - enemyController.timeToWaitForNextAction, 0f, Singleton<EnemySpawner>.Instance.GlobalEnemyParams.checkPosAndRotBossInterval);
						_003C_003E2__current = _003Cwfs_003E5__2;
						_003C_003E1__state = 4;
						return true;
					}
					if (_003CcurStopCooldown_003E5__3 == 0f)
					{
						if (UnityEngine.Random.Range(0f, 100f) >= enemyController.IdleChance)
						{
							enemyController.needMove = true;
						}
						else
						{
							enemyController.ResetMove();
							if (enemyController.CountIdleAnimations > 1)
							{
								int value = UnityEngine.Random.Range(0, enemyController.CountIdleAnimations);
								enemyController.myAnimator.SetInteger("IdleType", value);
							}
							_003CpatrolInterval_003E5__4 = UnityEngine.Random.Range(enemyController.IdleTimeMin, enemyController.IdleTimeMax);
						}
					}
					if (_003CcurStopCooldown_003E5__3 >= _003CpatrolInterval_003E5__4)
					{
						enemyController.needMove = true;
					}
					_003CcurStopCooldown_003E5__3 += enemyController.timeToWaitForNextAction;
					_003C_003E2__current = _003Cwfs_003E5__2;
					_003C_003E1__state = 5;
					return true;
				}
				if (enemyController.isPlayerAlive && enemyController.patrolData.IsNeedRotateToPlayer && !ManagerBase<PlayerManager>.Instance.isSleeping)
				{
					if (enemyController.returnFromPursuit)
					{
						enemyController.returnFromPursuit = false;
					}
					if (!enemyController.PlayerCombat.IsHearable)
					{
						enemyController.isPlayerHeard = false;
						enemyController.needMove = true;
					}
					else
					{
						if (NavMeshAgentUtils.HasPath(enemyController.myAgent) && enemyController.lastPatrolPoint == Vector3.zero)
						{
							enemyController.lastPatrolPoint = enemyController.myAgent.pathEndPosition;
						}
						enemyController.ResetMove();
						float num3 = Vector3.Distance(enemyController.ModelTransform.position, enemyController.PlayerTransform.position);
						float num2 = (enemyController.RotationSpeed + CalculationsHelpUtils.CalculateProp(enemyController.RotationSpeedFight - enemyController.RotationSpeed, enemyController.HearingRadius - num3, enemyController.HearingRadius)) * Time.deltaTime;
						Vector3 endDir = Vector3.ProjectOnPlane(enemyController.PlayerTransform.position - enemyController.ModelTransform.position, Vector3.up);
						Quaternion rotation = QuaternionUtils.GetRotation(enemyController.ModelTransform.forward, endDir, num2);
						enemyController.ModelTransform.rotation *= rotation;
						if (rotation == Quaternion.identity)
						{
							enemyController.needMove = true;
						}
					}
					_003C_003E2__current = null;
					_003C_003E1__state = 6;
					return true;
				}
				if (!flag && !enemyController.stoppedByAlly)
				{
					if (enemyController.EnemyProcessingSwitch.OnNavAgentMoveModeDistance)
					{
						float y2 = Vector3.Distance(enemyController.ModelTransform.position, _003CnextPosition_003E5__5);
						float speed = Mathf.Clamp(enemyController.SpeedMinimum + CalculationsHelpUtils.CalculateProp(enemyController.SpeedMedium - enemyController.SpeedMinimum, y2, enemyController.WalkDistanceMaximum), enemyController.SpeedMinimum, enemyController.SpeedMedium);
						enemyController.ChangeMovementSpeed(speed);
					}
					_003C_003E2__current = _003Cwfs_003E5__2;
					_003C_003E1__state = 7;
					return true;
				}
				_003C_003E2__current = null;
				_003C_003E1__state = 8;
				return true;
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
	private sealed class _003CPursuit_003Ed__263 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public EnemyController _003C_003E4__this;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CPursuit_003Ed__263(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			EnemyController enemyController = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				if (enemyController.PrevState != State.Escape && enemyController.PrevState != State.Attack)
				{
					enemyController.CurStamina = enemyController.CalculateStaminaByHealth();
				}
				if (NavMeshAgentUtils.HasPath(enemyController.myAgent))
				{
					enemyController.ResetMove();
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				goto IL_0076;
			case 1:
				_003C_003E1__state = -1;
				goto IL_0076;
			case 2:
				_003C_003E1__state = -1;
				break;
			case 3:
				{
					_003C_003E1__state = -1;
					break;
				}
				IL_0076:
				enemyController.ChangeMovementSpeed(enemyController.speedMaximum);
				enemyController.myAgent.acceleration = enemyController.acceleration;
				_003C_003E2__current = enemyController.StartCoroutine(enemyController.WaitForExitExtraIdleAnimation());
				_003C_003E1__state = 2;
				return true;
			}
			if ((enemyController.PlayerTransform.position - enemyController.ModelTransform.position).IsLongerOrEqual(enemyController.attackRange) && !enemyController.myAgent.pathPending)
			{
				enemyController.myAgent.SetDestination(enemyController.GetNextPursuitPosition());
			}
			_003C_003E2__current = null;
			_003C_003E1__state = 3;
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



	private EnemyHabitatArea enemyHabitatArea;

	private NavMeshAgent myAgent;

	private Animator myAnimator;

	private Coroutine myCoroutine;

	private Coroutine traverseNavLinkCor;

	private EnemyController curAlly;

	private float speedMaximum;

	private float escapeMediumSpeed;

	private float acceleration;

	private float viewDistance;

	private float viewHeight;

	private float viewAngle;

	private float viewStealthDistance;

	private float viewStealthHeight;

	private float viewStealthAngle;

	private float realHittedSlowSpeed;

	private float hearing;

	private float hearingMaximum;

	private float stamina;

	private float maxHealth;

	private float attackPower;

	private float attackRange;

	private bool cowardice;

	[SerializeField]
	private float timeToWaitForNextAction = 0.1f;

	[SerializeField]
	[Header("Визуализация слуха и зрения")]
	private bool drawGizmos = true;

	[SerializeField]
	private Color viewZoneCol;

	[SerializeField]
	private Color hearingZoneCol;

	private bool isPlayerHeard;

	private bool isPlayerAlive;

	private bool needMove;

	private bool forcePursuit;

	private bool attackIsOver = true;

	private bool stoppedByAlly;

	private bool returnFromPursuit;

	private float patrolTimer;

	private float slowTimer;

	private float habitatPopulationCheckTimer;

	private float processingDisableTimer;

	private float curStamina;

	private float curSpeed;

	private float healthDeltaInc;

	private float startNavAgentRadius;

	private float startNavAgentHeight;

	private PursuitData pursuitData;

	private PatrolData patrolData;

	private const float pursuit_minDistToSetDestination = 10f;

	private const float pursuit_calcPathCallFrequency = 1f;

	private const float disable_updateTransformFrequency = 1f;

	private Vector3 disable_startPos;

	private Vector3 disable_nextCorner;

	private float disable_distance;

	private float disable_distancePerStep;

	private float disable_distanceTraveled;

	private Vector3 velocity;

	private Material startMaterial;

	private Vector3 startScale = Vector3.zero;

	private Vector3 startColliderSize = Vector3.zero;

	private Vector3 prevPos;

	private Vector3 lastPatrolPoint = Vector3.zero;

	private BossAssistantSpawnPoint bossAssistantSpawnPoint;

	private bool isInited;

	private EnemySpawner.SpawnHandler unspawnCallback;

	private Mesh viewMesh;

	public EnemyCurrentScheme CurrentScheme
	{
		get;
		set;
	}

	public EnemyModel EnemyModel
	{
		get;
		set;
	}

	public NavMeshAgent NavAgent => myAgent;

	public EnemyCombat MyCombat
	{
		get;
		private set;
	}

	public Transform ModelTransform => EnemyModel?.transform;

	public EnemyProcessingSwitch EnemyProcessingSwitch => EnemyModel?.ProcessingSwitch;

	private Transform PlayerTransform => PlayerSpawner.PlayerInstance.transform;

	private PlayerCombat PlayerCombat => PlayerSpawner.PlayerInstance.PlayerCombat;

	private PlayerMovement PlayerMovement => PlayerSpawner.PlayerInstance.PlayerMovement;

	private List<PatrolZone> PatrolZones => enemyHabitatArea.PatrolZones;

	public State CurState
	{
		get;
		private set;
	}

	public State PrevState
	{
		get;
		private set;
	}

	private EnemyArchetype MyArchetype => EnemyModel.Archetype;

	private float SpeedMinimum => CurrentScheme.Archetype.speedMinimum;

	public float SpeedMedium => CurrentScheme.Archetype.speedMedium;

	private float RotationSpeed => Singleton<EnemySpawner>.Instance.GlobalEnemyParams.rotationSpeed;

	private float WalkDistanceMaximum => Singleton<EnemySpawner>.Instance.GlobalEnemyParams.walkDistanceMaximum;

	private float RotationSpeedFight => Singleton<EnemySpawner>.Instance.GlobalEnemyParams.rotationSpeedFight;

	private float JumpTime => Singleton<EnemySpawner>.Instance.GlobalEnemyParams.jumpTime;

	private float AllyAngle => Singleton<EnemySpawner>.Instance.GlobalEnemyParams.allyAngle;

	private float HelpRadius => Singleton<EnemySpawner>.Instance.GlobalEnemyParams.helpRadius;

	private float HealthRecoveryTime => Singleton<EnemySpawner>.Instance.GlobalEnemyParams.recoveryTime;

	private float IdleChance => Singleton<EnemySpawner>.Instance.GlobalEnemyParams.idleChance;

	private float IdleTimeMax => Singleton<EnemySpawner>.Instance.GlobalEnemyParams.idleTimeMax;

	private float IdleTimeMin => Singleton<EnemySpawner>.Instance.GlobalEnemyParams.idleTimeMin;

	private float HittedSlowSpeed => Singleton<EnemySpawner>.Instance.GlobalEnemyParams.hitSlowingSpeed;

	private float HittedSlowTime => Singleton<EnemySpawner>.Instance.GlobalEnemyParams.hitSlowingTime;

	private float HitFrequency => CurrentScheme.Archetype.hitFrequency;

	public float ScaleMod => 1f + CurrentScheme.Scheme.modelScale / 100f;

	private float EnduranceHealthFallMod => Singleton<EnemySpawner>.Instance.GlobalEnemyParams.enduranceHealthFallMod;

	private float CheckHearAndViewInterval => Singleton<EnemySpawner>.Instance.GlobalEnemyParams.checkHearAndViewInterval;

	private int CountIdleAnimations => EnemyModel.CountIdleAnimations;

	private int NumberIdleAnimInCombat => MyCombat.NumberIdleAnimInCombat;

	private bool Eatable => CurrentScheme.Scheme.Edible;

	private Material Material => CurrentScheme.Scheme.materials.Random();

	private float HearingMinimum => Singleton<EnemySpawner>.Instance.GlobalEnemyParams.hearingMinimum;

	private float ProcessingDistance
	{
		get
		{
			if (CurrentScheme.Scheme.Type == EnemyScheme.SchemeType.Boss)
			{
				return Singleton<EnemySpawner>.Instance.GlobalEnemyParams.bossProcessingDistance;
			}
			return Singleton<EnemySpawner>.Instance.GlobalEnemyParams.processingDistance;
		}
	}

	private float DistanceToSwitchMoveMode
	{
		get
		{
			if (CurrentScheme.Scheme.Type == EnemyScheme.SchemeType.Boss)
			{
				return Singleton<EnemySpawner>.Instance.GlobalEnemyParams.bossDistanceToSwitchMoveMode;
			}
			return Singleton<EnemySpawner>.Instance.GlobalEnemyParams.distanceToSwitchMoveMode;
		}
	}

	public bool DrawGizmos => drawGizmos;

	public Color ViewZoneCol => viewZoneCol;

	public Color HearingZoneCol => hearingZoneCol;

	public static List<EnemyController> Instances
	{
		get;
		private set;
	} = new List<EnemyController>();


	private bool IsBossScheme => CurrentScheme.Scheme.Type == EnemyScheme.SchemeType.Boss;

	private bool IsBossAssistantScheme => CurrentScheme.Scheme.Type == EnemyScheme.SchemeType.BossAssistant;

	public float CurStamina
	{
		get
		{
			return curStamina;
		}
		private set
		{
			curStamina = Mathf.Clamp(value, 0f, MaxStamina);
			this.staminaChangeEvent?.Invoke();
		}
	}

	private float MinPursuitSpeed => Mathf.Clamp((SpeedMedium + speedMaximum) / 2f, Singleton<EnemySpawner>.Instance.GlobalEnemyParams.minPursuitSpeed, float.MaxValue);

	public float MaxStamina => stamina;

	public float HearingRadius
	{
		get
		{
			if (PlayerCombat != null && !PlayerCombat.IsHearable)
			{
				return 0f;
			}
			if (CurState == State.Escape)
			{
				return hearingMaximum;
			}
			if (returnFromPursuit)
			{
				return HearingMinimum;
			}
			if (CurState == State.Pursuit || CurState == State.Attack)
			{
				if (CurStamina > 0f)
				{
					return hearingMaximum;
				}
				return HearingMinimum;
			}
			return hearing;
		}
	}

	public float ViewDistance
	{
		get
		{
			if (returnFromPursuit)
			{
				return 0f;
			}
			if (PlayerCombat.IsInvisibilityActive())
			{
				return Mathf.Max(viewStealthDistance, HearingRadius);
			}
			return Mathf.Max(viewDistance, HearingRadius);
		}
	}

	public float ViewHeight
	{
		get
		{
			if (PlayerCombat.IsInvisibilityActive())
			{
				return viewStealthHeight;
			}
			return viewHeight;
		}
	}

	public float ViewAngle
	{
		get
		{
			if (PlayerCombat.IsInvisibilityActive())
			{
				return viewStealthAngle;
			}
			return viewAngle;
		}
	}

	private List<EnemyController> Allies
	{
		get
		{
			List<EnemyArchetype> myAllyArchetypes = Singleton<EnemySpawner>.Instance.AllyMatrix.Find((EnemySpawner.ArchetypeAllies x) => x.archetype == MyArchetype).allies;
			List<EnemyController> list = (from x in Instances
				where myAllyArchetypes.Contains(x.MyArchetype) && x.CurState != State.Die && x.gameObject.activeInHierarchy && !x.IsBossScheme && !x.IsBossAssistantScheme
				select x).ToList();
			list.Remove(this);
			return list;
		}
	}

	public event Action staminaChangeEvent;

	public static event CreationOrSpawnAIHandler creationEvent;

	public static event CreationOrSpawnAIHandler spawnEvent;

	public static event ChangeStateHandler changeStateEvent;

	public event ChangeMoveModeHandler changeMoveModeEvent;

	public static event SelfTransitionToAnyAttackStateHandler selfTransitionToAnyAttackStateEvent;

	public static event EnemySpawner.SpawnHandler unspawnEvent;

	private float GetViewDistance()
	{
		return Singleton<EnemySpawner>.Instance.GlobalEnemyParams.visionDistance.GetValue(CurrentScheme.level);
	}

	private float GetViewAngle()
	{
		return Singleton<EnemySpawner>.Instance.GlobalEnemyParams.visionAngle.GetValue(CurrentScheme.level);
	}

	private float GetViewHeight()
	{
		return Singleton<EnemySpawner>.Instance.GlobalEnemyParams.visionHeight.GetValue(CurrentScheme.level);
	}

	private float GetViewStealthDistance()
	{
		return Singleton<EnemySpawner>.Instance.GlobalEnemyParams.visionStealthDistance.GetValue(CurrentScheme.level);
	}

	private float GetViewStealthAngle()
	{
		return Singleton<EnemySpawner>.Instance.GlobalEnemyParams.visionStealthAngle.GetValue(CurrentScheme.level);
	}

	private float GetViewStealthHeight()
	{
		return Singleton<EnemySpawner>.Instance.GlobalEnemyParams.visionStealthHeight.GetValue(CurrentScheme.level);
	}

	private float GetHearing()
	{
		return Singleton<EnemySpawner>.Instance.GlobalEnemyParams.hearing.GetValue(CurrentScheme.level);
	}

	private float GetHearingMaximum()
	{
		return Singleton<EnemySpawner>.Instance.GlobalEnemyParams.hearingMaximum.GetValue(CurrentScheme.level);
	}

	private float GetSpeedMaximum()
	{
		return CurrentScheme.Archetype.speedMaximum.GetValue(CurrentScheme.level);
	}

	private float GetEscapeMediumSpeed()
	{
		return CalculationsHelpUtils.CalculateProp(SpeedMedium, Singleton<EnemySpawner>.Instance.GlobalEnemyParams.escapeMediumSpeed, 100f);
	}

	private float GetRealHittedSlowSpeed()
	{
		return CalculationsHelpUtils.CalculateProp(speedMaximum, HittedSlowSpeed, 100f);
	}

	private float GetStamina()
	{
		return Singleton<EnemySpawner>.Instance.GlobalEnemyParams.endurance.GetValue(CurrentScheme.level);
	}

	private float GetAcceleration()
	{
		return Singleton<EnemySpawner>.Instance.GlobalEnemyParams.acceleration.GetValue(CurrentScheme.level);
	}

	private float GetMaxHealth()
	{
		float health = CurrentScheme.Scheme.health;
		float healthLevelMod = Singleton<EnemySpawner>.Instance.GlobalEnemyParams.healthLevelMod;
		float num = health * healthLevelMod / 100f;
		num *= (float)(CurrentScheme.level - 1);
		return health + num;
	}

	private float GetAttackPower()
	{
		float hitPower = CurrentScheme.Scheme.hitPower;
		float hitPowerLevelMod = Singleton<EnemySpawner>.Instance.GlobalEnemyParams.hitPowerLevelMod;
		float num = hitPower * hitPowerLevelMod / 100f;
		num *= (float)(CurrentScheme.level - 1);
		return hitPower + num;
	}

	public float GetExperienceForKill()
	{
		float experience = CurrentScheme.Scheme.experience;
		int level = CurrentScheme.level;
		int level2 = ManagerBase<PlayerManager>.Instance.Level;
		if (level >= level2)
		{
			return experience;
		}
		int num = level2 - level;
		float num2 = Mathf.Clamp(CalculationsHelpUtils.CalculateProp(experience, Singleton<EnemySpawner>.Instance.GlobalEnemyParams.experienceDec * (float)num, 100f), 0f, float.MaxValue);
		return experience - num2;
	}

	private bool GetCowardice()
	{
		return CurrentScheme.Scheme.Coward;
	}

	private float GetAttackRange()
	{
		return myAgent.radius + Singleton<EnemySpawner>.Instance.GlobalEnemyParams.hitDistance + PlayerMovement.NavMeshObstacle.radius;
	}

	private float GetHealthDeltaInc()
	{
		return CalculationsHelpUtils.CalculateProp(MyCombat.MaxHealth, Time.deltaTime, HealthRecoveryTime);
	}

	private void ChangeState(State newState, IEnumerator routine)
	{
		if (newState != CurState)
		{
			if (CurState == State.Patrol)
			{
				myAgent.stoppingDistance = 0f;
			}
			switch (newState)
			{
			case State.Pursuit:
				pursuitData = new PursuitData(this);
				break;
			case State.Patrol:
				patrolData = new PatrolData(this);
				break;
			}
			ResetAllyStopping();
			if (myCoroutine != null)
			{
				StopCoroutine(myCoroutine);
			}
			PrevState = CurState;
			CurState = newState;
			if (routine != null)
			{
				myCoroutine = StartCoroutine(routine);
			}
			EnemyController.changeStateEvent?.Invoke(this, newState);
		}
	}

	private void ResetAllyStopping()
	{
		if (CurState == State.Escape && curAlly != null)
		{
			curAlly.stoppedByAlly = false;
			curAlly = null;
		}
	}

	private void ChangeMovementSpeed(float speed)
	{
		myAgent.isStopped = false;
		myAgent.acceleration = acceleration;
		myAgent.speed = speed;
	}

	private void ResetMove(bool stoppedByAlly = false)
	{
		if (myAgent.enabled)
		{
			myAgent.ResetPath();
			myAgent.isStopped = true;
			myAgent.acceleration = 9999f;
			myAgent.speed = 0f;
			this.stoppedByAlly = stoppedByAlly;
		}
	}

	private Vector3 GetSampledPosition(Vector3 sourcePos)
	{
		NavMeshUtils.SamplePositionIterative(sourcePos, out NavMeshHit navMeshHit, 2f, 100f, 4, -1);
		return navMeshHit.position;
	}

	private Vector3 GetNextPatrolPosition()
	{
		Vector3 randomPoint = PatrolZones.Random().GetRandomPoint();
		return GetSampledPosition(randomPoint);
	}

	private bool GetNextEscapePosition(out Vector3 escapePosition)
	{
		escapePosition = Vector3.zero;
		lastPatrolPoint = Vector3.zero;
		if (curAlly == null)
		{
			List<EnemyController> allies = Allies;
			List<(EnemyController, float)> list = new List<(EnemyController, float)>();
			foreach (EnemyController item6 in allies)
			{
				list.Add((item6, (ModelTransform.position - item6.ModelTransform.position).sqrMagnitude));
			}
			list.Sort((Comparison<(EnemyController, float)>)(((EnemyController ally, float sqrMagnitude) item1, (EnemyController ally, float sqrMagnitude) item2) => item1.sqrMagnitude.CompareTo(item2.sqrMagnitude)));
			foreach (var item7 in list)
			{
				EnemyController item3 = item7.Item1;
				Vector3 normalized = (ModelTransform.position - PlayerTransform.position).normalized;
				Vector3 normalized2 = (item3.ModelTransform.position - ModelTransform.position).normalized;
				if (Mathf.Abs(Vector3.Angle(normalized, normalized2)) < 180f - AllyAngle)
				{
					curAlly = item3;
					break;
				}
			}
			if (curAlly != null)
			{
				_003CGetNextEscapePosition_003Eg__SetAllyHabbitatAndEscapePosition_007C227_0(ref escapePosition);
				return true;
			}
			List<EnemyHabitatArea> list2 = Singleton<EnemySpawner>.Instance.EnemyHabitats.FindAll((EnemyHabitatArea x) => x.Inhabitants.Count == 0 && x.ZoneId == enemyHabitatArea.ZoneId);
			if (list2.Count != 0)
			{
				List<(EnemyHabitatArea, float)> list3 = new List<(EnemyHabitatArea, float)>();
				foreach (EnemyHabitatArea item8 in list2)
				{
					list3.Add((item8, (ModelTransform.position - item8.HabitatCenter).sqrMagnitude));
				}
				list3.Sort((Comparison<(EnemyHabitatArea, float)>)(((EnemyHabitatArea area, float sqrMagnitude) item1, (EnemyHabitatArea area, float sqrMagnitude) item2) => item1.sqrMagnitude.CompareTo(item2.sqrMagnitude)));
				EnemyHabitatArea item4 = list3[list3.Count - 1].Item1;
				enemyHabitatArea.Inhabitants.Remove(this);
				enemyHabitatArea = item4;
				enemyHabitatArea.Inhabitants.Add(this);
				PatrolZone patrolZone = item4.PatrolZones.Random();
				escapePosition = GetSampledPosition(patrolZone.GetRandomPoint());
				return true;
			}
			foreach (var item9 in list)
			{
				EnemyController item5 = item9.Item1;
				Vector3 normalized3 = (ModelTransform.position - PlayerTransform.position).normalized;
				Vector3 normalized4 = (item5.ModelTransform.position - ModelTransform.position).normalized;
				if (Mathf.Abs(Vector3.Angle(normalized3, normalized4)) > 180f - AllyAngle)
				{
					curAlly = item5;
					break;
				}
			}
			if (curAlly != null)
			{
				_003CGetNextEscapePosition_003Eg__SetAllyHabbitatAndEscapePosition_007C227_0(ref escapePosition);
				return true;
			}
			escapePosition = ModelTransform.position;
			return false;
		}
		Vector3 vector = escapePosition = GetSampledPosition(curAlly.ModelTransform.position);
		return true;
	}

	private Vector3 GetNextPursuitPosition()
	{
		Vector3 position = PlayerTransform.position;
		return GetSampledPosition(position);
	}

	public float CalculcatePBF(float value, float pbf)
	{
		return CalculationsHelpUtils.CalculcatePBF(value, pbf, CurrentScheme.level);
	}

	private bool IsPlayerVisible()
	{
		if (PlayerSpawner.PlayerInstance.PlayerCombat.CurLifeState != 0)
		{
			return false;
		}
		if (PlayerTransform.position.y - ModelTransform.position.y > ViewHeight)
		{
			return false;
		}
		Vector3 vector = new Vector3(PlayerTransform.position.x, 0f, PlayerTransform.position.z);
		Vector3 vector2 = new Vector3(ModelTransform.position.x, 0f, ModelTransform.position.z);
		if ((vector2 - vector).IsLonger(ViewDistance))
		{
			return false;
		}
		if (Vector3.Angle(new Vector3(ModelTransform.forward.x, 0f, ModelTransform.forward.z), vector - vector2) <= ViewAngle / 2f)
		{
			return true;
		}
		return false;
	}

	private bool IsPlayerHeard()
	{
		if (PlayerSpawner.PlayerInstance.PlayerCombat.CurLifeState != 0)
		{
			return false;
		}
		if (PlayerTransform.position.y - ModelTransform.position.y > ViewHeight)
		{
			return false;
		}
		Vector3 b = new Vector3(PlayerTransform.position.x, 0f, PlayerTransform.position.z);
		Vector3 a = new Vector3(ModelTransform.position.x, 0f, ModelTransform.position.z);
		if (PlayerCombat.IsHearable)
		{
			return (a - b).IsShorterOrEqual(HearingRadius);
		}
		return false;
	}

	private bool IsAllyInHelpRadius(out EnemyController ally)
	{
		ally = Allies.Find((EnemyController x) => (x.ModelTransform.position - ModelTransform.position).IsShorterOrEqual(HelpRadius));
		return ally != null;
	}

	private bool HaveAllyInFight()
	{
		return Allies.Find((EnemyController x) => (x.CurState != State.Pursuit) ? (x.CurState == State.Attack) : true) != null;
	}

	private bool HavePursuedAlly()
	{
		return Allies.Find((EnemyController x) => (x.ModelTransform.position - ModelTransform.position).IsShorterOrEqual(x.HearingRadius) && (x.ModelTransform.position - x.PlayerTransform.position).IsShorterOrEqual(x.HearingRadius) && ((x.CurState != State.Attack) ? (x.CurState == State.Pursuit) : true)) != null;
	}

	private float CalculateStaminaByHealth()
	{
		if (MyCombat.CurHealth == MyCombat.MaxHealth)
		{
			return MaxStamina;
		}
		float y = CalculationsHelpUtils.CalculateProp(100f, MyCombat.CurHealth, MyCombat.MaxHealth);
		return CalculationsHelpUtils.CalculateProp(MaxStamina, y, 100f);
	}

	private void ApplyModelScale()
	{
		if (ScaleMod != 1f)
		{
			EnemyModel.ModelObj.transform.localScale = startScale * ScaleMod;
			EnemyModel.Collider.size = startColliderSize * ScaleMod;
			myAgent.radius = startNavAgentRadius * ScaleMod;
			myAgent.height = startNavAgentHeight * ScaleMod;
		}
	}

	private void CancelModelScale()
	{
		if (ScaleMod != 1f)
		{
			EnemyModel.ModelObj.transform.localScale = startScale;
			EnemyModel.Collider.size = startColliderSize;
			myAgent.radius = startNavAgentRadius;
			myAgent.height = startNavAgentHeight;
		}
	}

	public void FindNewHabitat()
	{
		EnemyHabitatArea enemyHabitatArea = Singleton<EnemySpawner>.Instance.GetEnemyHabitatArea(this);
		this.enemyHabitatArea.Inhabitants.Remove(this);
		this.enemyHabitatArea = enemyHabitatArea;
		this.enemyHabitatArea.Inhabitants.Add(this);
	}

	public void Spawn(Vector3 spawnPos, bool teleportToHabitat, EnemySpawner.SpawnHandler unspawnCallback)
	{
		Init();
		myAgent = GetComponentInChildren<NavMeshAgent>();
		myAnimator = GetComponentInChildren<Animator>();
		MyCombat = GetComponentInChildren<EnemyCombat>();
		startScale = EnemyModel.ModelObj.transform.localScale;
		startColliderSize = EnemyModel.Collider.size;
		startNavAgentRadius = myAgent.radius;
		startNavAgentHeight = myAgent.height;
		ApplyModelScale();
		speedMaximum = GetSpeedMaximum();
		escapeMediumSpeed = GetEscapeMediumSpeed();
		realHittedSlowSpeed = GetRealHittedSlowSpeed();
		viewDistance = GetViewDistance();
		viewAngle = GetViewAngle();
		viewHeight = GetViewHeight();
		viewStealthDistance = GetViewStealthDistance();
		viewStealthAngle = GetViewStealthAngle();
		viewStealthHeight = GetViewStealthHeight();
		hearing = GetHearing();
		hearingMaximum = GetHearingMaximum();
		stamina = GetStamina();
		acceleration = GetAcceleration();
		cowardice = GetCowardice();
		maxHealth = GetMaxHealth();
		attackPower = GetAttackPower();
		attackRange = GetAttackRange();
		healthDeltaInc = GetHealthDeltaInc();
		startMaterial = EnemyModel.Renderer.material;
		if (Material != null)
		{
			EnemyModel.Renderer.material = Material;
		}
		myAgent.acceleration = acceleration;
		disable_distancePerStep = SpeedMedium * 1f;
		EnemyProcessingSwitch.maxProcessingDistance = ProcessingDistance;
		EnemyProcessingSwitch.distanceToSwitchMoveMode = DistanceToSwitchMoveMode;
		EnemyProcessingSwitch.UpdateProcessingState();
		MyCombat.takeDamageEvent += OnTakeDamage;
		MyCombat.changeLifeStateEvent += OnChangeLifeState;
		EnemyProcessingSwitch.switchMoveModeEvent += OnSwitchMoveModeState;
		ProcessingSwitch.switchEvent += OnProcessingSwitch;
		EnemyCurrentScheme currentScheme = CurrentScheme;
		currentScheme.correctZoneEvent = (Action)Delegate.Combine(currentScheme.correctZoneEvent, new Action(OnSchemeZoneCorrect));
		EnemyCurrentScheme currentScheme2 = CurrentScheme;
		currentScheme2.correctLevelEvent = (Action)Delegate.Combine(currentScheme2.correctLevelEvent, new Action(OnSchemeLevelCorrect));
		PlayerCombat.InvisibilityAbility.switchInvisibilityEvent += OnSwitchInvisibility;
		this.unspawnCallback = unspawnCallback;
		MyCombat.Configure(maxHealth, attackPower, HitFrequency);
		MyCombat.Respawn();
		CurStamina = CalculateStaminaByHealth();
		myAgent.enabled = false;
		ModelTransform.position = spawnPos;
		prevPos = ModelTransform.position;
		if (CurrentScheme.Scheme.Type == EnemyScheme.SchemeType.Simple || CurrentScheme.Scheme.Type == EnemyScheme.SchemeType.BossAssistant)
		{
			EnemyModel.MiniBossEyes.SetActive(value: false);
			EnemyModel.BossEyes.SetActive(value: false);
		}
		else if (CurrentScheme.Scheme.Type == EnemyScheme.SchemeType.MiniBoss)
		{
			EnemyModel.MiniBossEyes.SetActive(value: true);
			EnemyModel.BossEyes.SetActive(value: false);
		}
		else if (CurrentScheme.Scheme.Type == EnemyScheme.SchemeType.Boss)
		{
			EnemyModel.MiniBossEyes.SetActive(value: false);
			EnemyModel.BossEyes.SetActive(value: true);
		}
		if (IsBossScheme)
		{
			spawnPos = GetSampledPosition(spawnPos);
			myAgent.transform.position = spawnPos;
			myAgent.enabled = true;
			myAgent.Warp(spawnPos);
		}
		else if (IsBossAssistantScheme)
		{
			bossAssistantSpawnPoint = Singleton<EnemySpawner>.Instance.GetFreeBossAssistantSpawnPoint();
			bossAssistantSpawnPoint.CurrentAssistant = this;
			spawnPos = GetSampledPosition(bossAssistantSpawnPoint.Position);
			myAgent.transform.position = spawnPos;
			myAgent.enabled = true;
			myAgent.Warp(spawnPos);
		}
		else
		{
			enemyHabitatArea = Singleton<EnemySpawner>.Instance.GetEnemyHabitatArea(this);
			enemyHabitatArea.Inhabitants.Add(this);
			if (teleportToHabitat)
			{
				NavMeshPath navMeshPath = new NavMeshPath();
				NavMeshUtils.SamplePositionIterative(spawnPos, out NavMeshHit navMeshHit, 5f, 100f, 5, -1);
				if (float.IsInfinity(navMeshHit.position.x))
				{
					UnityEngine.Debug.LogError("Навмеш не построен либо точка спавна находится очень далеко от навмеша");
					return;
				}
				Vector3 zero = Vector3.zero;
				do
				{
					zero = enemyHabitatArea.PatrolZones.Random().GetRandomPoint();
					NavMeshUtils.SamplePositionIterative(zero, out NavMeshHit navMeshHit2, 5f, 100f, 5, -1);
					zero = navMeshHit2.position;
					NavMesh.CalculatePath(navMeshHit.position, zero, -1, navMeshPath);
				}
				while (navMeshPath.status != 0);
				myAgent.transform.position = zero;
				myAgent.enabled = true;
				myAgent.Warp(zero);
			}
			else
			{
				spawnPos = GetSampledPosition(spawnPos);
				myAgent.transform.position = spawnPos;
				myAgent.enabled = true;
				myAgent.Warp(spawnPos);
			}
		}
		if (PlayerSpawner.IsPlayerSpawned)
		{
			ChangeState(State.Patrol, Patrol());
		}
		else
		{
			PlayerSpawner.spawnPlayerEvent += OnSpawnPlayer;
		}
		EnemyProcessingSwitch.CheckSwitchMoveMode();
		EnemyModel.Collider.enabled = true;
		EnemyController.spawnEvent?.Invoke(this);
	}

	public void Unspawn()
	{
		MyCombat.takeDamageEvent -= OnTakeDamage;
		MyCombat.changeLifeStateEvent -= OnChangeLifeState;
		EnemyProcessingSwitch.switchMoveModeEvent -= OnSwitchMoveModeState;
		ProcessingSwitch.switchEvent -= OnProcessingSwitch;
		EnemyCurrentScheme currentScheme = CurrentScheme;
		currentScheme.correctZoneEvent = (Action)Delegate.Remove(currentScheme.correctZoneEvent, new Action(OnSchemeZoneCorrect));
		EnemyCurrentScheme currentScheme2 = CurrentScheme;
		currentScheme2.correctLevelEvent = (Action)Delegate.Remove(currentScheme2.correctLevelEvent, new Action(OnSchemeLevelCorrect));
		PlayerCombat.InvisibilityAbility.switchInvisibilityEvent -= OnSwitchInvisibility;
		ChangeState(State.None, null);
		CancelModelScale();
		EnemyModel.Collider.enabled = false;
		EnemyModel.Renderer.material = startMaterial;
		if (!IsBossScheme && !IsBossAssistantScheme)
		{
			enemyHabitatArea.Inhabitants.Remove(this);
			enemyHabitatArea = null;
		}
		myAgent.enabled = false;
		EnemySpawner.SpawnHandler spawnHandler = unspawnCallback;
		unspawnCallback = null;
		spawnHandler?.Invoke(this);
		EnemyController.unspawnEvent?.Invoke(this);
	}

	private void OnSchemeZoneCorrect()
	{
		if (!IsBossScheme && !IsBossAssistantScheme)
		{
			FindNewHabitat();
			Vector3 zero = Vector3.zero;
			NavMeshPath navMeshPath = new NavMeshPath();
			do
			{
				zero = enemyHabitatArea.PatrolZones.Random().GetRandomPoint();
				NavMeshUtils.SamplePositionIterative(zero, out NavMeshHit navMeshHit, 5f, 100f, 5, -1);
				zero = navMeshHit.position;
				NavMesh.CalculatePath(ModelTransform.position, zero, -1, navMeshPath);
			}
			while (navMeshPath.status != 0);
			myAgent.Warp(zero);
		}
	}

	private void OnSchemeLevelCorrect()
	{
		speedMaximum = GetSpeedMaximum();
		realHittedSlowSpeed = GetRealHittedSlowSpeed();
		viewDistance = GetViewDistance();
		viewAngle = GetViewAngle();
		viewHeight = GetViewHeight();
		viewStealthDistance = GetViewStealthDistance();
		viewStealthAngle = GetViewStealthAngle();
		viewStealthHeight = GetViewStealthHeight();
		hearing = GetHearing();
		hearingMaximum = GetHearingMaximum();
		acceleration = GetAcceleration();
		stamina = GetStamina();
		cowardice = GetCowardice();
		maxHealth = GetMaxHealth();
		attackPower = GetAttackPower();
		attackRange = GetAttackRange();
		healthDeltaInc = GetHealthDeltaInc();
		ApplyModelScale();
		startMaterial = EnemyModel.Renderer.material;
		if (Material != null)
		{
			EnemyModel.Renderer.material = Material;
		}
		myAgent.acceleration = acceleration;
		disable_distancePerStep = SpeedMedium * 1f;
		MyCombat.Configure(maxHealth, attackPower, HitFrequency);
	}

	private void OnSwitchInvisibility(bool state)
	{
		if (returnFromPursuit)
		{
			returnFromPursuit = false;
		}
	}

	private void Start()
	{
		Init();
	}

	private void Init()
	{
		if (!isInited)
		{
			isInited = true;
			Instances.Add(this);
			EnemyController.creationEvent?.Invoke(this);
		}
	}

	private void OnSpawnPlayer()
	{
		ChangeState(State.Patrol, Patrol());
	}

	private void OnSwitchMoveModeState(bool isNavAgentMode)
	{
		if (isNavAgentMode)
		{
			if (NavMeshAgentUtils.HasPath(myAgent))
			{
				float y = Vector3.Distance(ModelTransform.position, myAgent.pathEndPosition);
				float speed = Mathf.Clamp(SpeedMinimum + CalculationsHelpUtils.CalculateProp(SpeedMedium - SpeedMinimum, y, WalkDistanceMaximum), SpeedMinimum, SpeedMedium);
				ChangeMovementSpeed(speed);
			}
		}
		else
		{
			disable_nextCorner = Vector3.zero;
			disable_startPos = Vector3.zero;
			disable_distance = 0f;
			disable_distanceTraveled = 0f;
			if (!NavMeshAgentUtils.HasPath(myAgent))
			{
				if (IsBossScheme)
				{
					myAgent.SetDestination(Singleton<EnemySpawner>.Instance.BossSpawnPoint.transform.position);
				}
				else if (IsBossAssistantScheme)
				{
					myAgent.SetDestination(bossAssistantSpawnPoint.Position);
				}
				else
				{
					myAgent.SetDestination(GetNextPatrolPosition());
				}
			}
			ChangeMovementSpeed(0f);
		}
		this.changeMoveModeEvent?.Invoke(isNavAgentMode);
	}

	private void OnProcessingSwitch(ProcessingSwitch processingSwitch)
	{
		if (!(processingSwitch != EnemyProcessingSwitch) && curSpeed > 0f && (myAnimator.GetCurrentAnimatorStateInfo(0).tagHash == AnimationHashes.extraIdle.Value || myAnimator.GetNextAnimatorStateInfo(0).tagHash == AnimationHashes.extraIdle.Value))
		{
			myAnimator.SetInteger("IdleType", 0);
		}
	}

	private void OnDestroy()
	{
		PlayerSpawner.spawnPlayerEvent -= OnSpawnPlayer;
		MyCombat.takeDamageEvent -= OnTakeDamage;
		MyCombat.changeLifeStateEvent -= OnChangeLifeState;
		EnemyProcessingSwitch.switchMoveModeEvent -= OnSwitchMoveModeState;
		ProcessingSwitch.switchEvent -= OnProcessingSwitch;
		PlayerCombat.InvisibilityAbility.switchInvisibilityEvent -= OnSwitchInvisibility;
	}

	private void Update()
	{
		if (Time.deltaTime == 0f)
		{
			return;
		}
		if (CurState == State.Patrol && MyCombat.CurHealth != MyCombat.MaxHealth)
		{
			MyCombat.Heal(healthDeltaInc);
			CurStamina = CalculateStaminaByHealth();
		}
		if (MyCombat.CurLifeState == ActorCombat.LifeState.Alive && EnemyProcessingSwitch.OnProcessingDistance)
		{
			float num = (ModelTransform.position - prevPos).magnitude / Time.deltaTime / ScaleMod;
			float value = num / SpeedMedium;
			float value2 = num / speedMaximum;
			myAnimator.SetFloat("NormalizedWalkSpeed", value);
			myAnimator.SetFloat("NormalizedRunSpeed", value2);
			myAnimator.SetFloat("Speed", num);
			myAnimator.SetFloat("SpeedAnimMultiplier", Mathf.Clamp(num, SpeedMinimum, float.MaxValue));
			if ((CurState == State.Escape || CurState == State.Pursuit || CurState == State.Attack) && myAnimator.GetInteger("IdleType") != NumberIdleAnimInCombat)
			{
				myAnimator.SetInteger("IdleType", NumberIdleAnimInCombat);
			}
			prevPos = ModelTransform.position;
		}
		if (EnemyProcessingSwitch.OnNavAgentMoveModeDistance)
		{
			if (myAgent.isOnOffMeshLink && traverseNavLinkCor == null)
			{
				traverseNavLinkCor = StartCoroutine(TraverseNavLinkParabola(myAgent, myAgent.height, JumpTime));
				if (myAgent.isOnNavMesh)
				{
					myAgent.CompleteOffMeshLink();
				}
			}
			else
			{
				bool flag = CurState == State.Attack || CurState == State.Escape || CurState == State.Pursuit;
				if (flag)
				{
					NavMeshAgentUtils.UpdateVelocity(myAgent, ref velocity, ref curSpeed, Time.deltaTime, flag ? RotationSpeedFight : RotationSpeed, Singleton<EnemySpawner>.Instance.GlobalEnemyParams.corneringSlowing);
				}
				else
				{
					velocity = NavAgent.velocity;
					curSpeed = NavAgent.velocity.magnitude;
				}
			}
		}
		else if (processingDisableTimer >= 1f)
		{
			processingDisableTimer = 0f;
			if (NavMeshAgentUtils.HasPath(myAgent))
			{
				if (disable_nextCorner != myAgent.steeringTarget)
				{
					disable_nextCorner = myAgent.steeringTarget;
					disable_startPos = ModelTransform.position;
					disable_distance = Vector3.Distance(ModelTransform.position, disable_nextCorner);
					disable_distanceTraveled = 0f;
				}
				disable_distanceTraveled += disable_distancePerStep;
				float t = Mathf.Clamp(disable_distanceTraveled / disable_distance, 0f, 1f);
				ModelTransform.position = Vector3.Lerp(disable_startPos, disable_nextCorner, t);
			}
		}
		else
		{
			processingDisableTimer += Time.deltaTime;
		}
		if (habitatPopulationCheckTimer >= ManagerBase<EnemyManager>.Instance.HabitatPopulationCheckFrequency)
		{
			habitatPopulationCheckTimer = 0f;
			if (!IsBossScheme && !IsBossAssistantScheme && enemyHabitatArea.Inhabitants.FindAll((EnemyController habitant) => habitant.CurState == State.Patrol).Count > ManagerBase<EnemyManager>.Instance.MaxPopulationInHabitat)
			{
				FindNewHabitat();
			}
		}
		else
		{
			habitatPopulationCheckTimer += Time.deltaTime;
		}
		if (CurState == State.Patrol)
		{
			isPlayerAlive = (PlayerCombat.CurLifeState == ActorCombat.LifeState.Alive);
			if (EnemyProcessingSwitch.OnProcessingDistance)
			{
				patrolData.Update(Time.deltaTime);
				if (patrolTimer >= CheckHearAndViewInterval)
				{
					patrolTimer = 0f;
					isPlayerHeard = IsPlayerHeard();
					if (IsPlayerVisible() && isPlayerAlive && !_003CUpdate_003Eg__HaveObstaclesOnView_007C255_1() && patrolData.CanReachPlayer && !ManagerBase<PlayerManager>.Instance.isSleeping)
					{
						bool flag2 = !cowardice || HaveAllyInFight();
						if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Alien)
						{
							flag2 = (CurrentScheme.Scheme.Type != EnemyScheme.SchemeType.Simple);
						}
						EnemyController.selfTransitionToAnyAttackStateEvent?.Invoke(flag2);
						if (flag2)
						{
							ChangeState(State.Pursuit, Pursuit());
						}
						else
						{
							ChangeState(State.Escape, Escape());
						}
						return;
					}
				}
				patrolTimer += Time.deltaTime;
			}
		}
		if (CurState == State.Pursuit)
		{
			if (forcePursuit)
			{
				forcePursuit = false;
			}
			if (CurStamina > 0f)
			{
				CurStamina -= Time.deltaTime;
			}
			if (CurStamina <= 0f && myAgent.speed != MinPursuitSpeed)
			{
				ChangeMovementSpeed(MinPursuitSpeed);
			}
			if ((PlayerTransform.position - ModelTransform.position).IsShorter(attackRange))
			{
				ChangeState(State.Attack, Attack());
				return;
			}
			pursuitData.Update(Time.deltaTime);
			isPlayerHeard = IsPlayerHeard();
			if ((!isPlayerHeard || pursuitData.IsPlayerUnreachable) && !HavePursuedAlly())
			{
				returnFromPursuit = true;
				ChangeState(State.Patrol, Patrol());
				return;
			}
		}
		if (CurState == State.Attack)
		{
			if (!IsOnAttackRange() && attackIsOver)
			{
				ChangeState(State.Pursuit, Pursuit());
				return;
			}
			if (PlayerCombat.CurLifeState != 0)
			{
				ChangeState(State.Patrol, Patrol());
				return;
			}
		}
		if (CurState != State.Escape)
		{
			return;
		}
		isPlayerHeard = IsPlayerHeard();
		CurStamina -= Time.deltaTime;
		if (CurStamina == 0f && PlayerCombat.HaveEnemyInAttackBox)
		{
			bool flag3 = false;
			foreach (PlayerCombat.EnemyForAttackInfo enemyForAttackInBoxInfo in PlayerCombat.EnemyForAttackInBoxInfos)
			{
				if (enemyForAttackInBoxInfo.enemyController == this)
				{
					flag3 = true;
					break;
				}
			}
			if (flag3)
			{
				ChangeState(State.Pursuit, Pursuit());
				return;
			}
		}
		if (forcePursuit)
		{
			ChangeState(State.Pursuit, Pursuit());
			return;
		}
		if (IsAllyInHelpRadius(out EnemyController ally))
		{
			ally.ChangeState(State.Pursuit, ally.Pursuit());
		}
		if (HaveAllyInFight())
		{
			ChangeState(State.Pursuit, Pursuit());
			return;
		}
		if (!NavMeshAgentUtils.HasPath(myAgent))
		{
			ChangeState(State.Pursuit, Pursuit());
			return;
		}
		if (slowTimer > 0f)
		{
			slowTimer -= Time.deltaTime;
		}
		if (!isPlayerHeard)
		{
			ChangeState(State.Patrol, Patrol());
		}
	}

	private bool CanReachPlayerPosition(Vector3 sourcePos)
	{
		if (NavMeshUtils.SamplePositionIterative(PlayerSpawner.PlayerInstance.transform.position, out NavMeshHit navMeshHit, 2f, 100f, 10, -1))
		{
			NavMeshPath navMeshPath = new NavMeshPath();
			NavMesh.CalculatePath(sourcePos, navMeshHit.position, -1, navMeshPath);
			return navMeshPath.status == NavMeshPathStatus.PathComplete;
		}
		return false;
	}

	private void OnTakeDamage(ActorCombat.TakeDamageType takeDamageType, float damage, ActorCombat attacker)
	{
		if (CurStamina > 0f)
		{
			float y = CalculationsHelpUtils.CalculateProp(100f, damage, MyCombat.MaxHealth) * EnduranceHealthFallMod / 100f;
			float num = CalculationsHelpUtils.CalculateProp(MaxStamina, y, 100f);
			CurStamina -= num;
		}
		if (CurState == State.Escape)
		{
			if (CurStamina <= 0f)
			{
				ChangeState(State.Pursuit, Pursuit());
				return;
			}
			if (attacker is PlayerCombat)
			{
				slowTimer = HittedSlowTime;
			}
		}
		if (CurState == State.Patrol)
		{
			if (returnFromPursuit)
			{
				returnFromPursuit = false;
			}
			bool flag = !cowardice;
			if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Alien)
			{
				flag = (CurrentScheme.Scheme.Type != EnemyScheme.SchemeType.Simple);
			}
			if (flag)
			{
				ChangeState(State.Pursuit, Pursuit());
			}
			else
			{
				ChangeState(State.Escape, Escape());
			}
		}
	}

	private void OnChangeLifeState(ActorCombat.LifeState state)
	{
		switch (state)
		{
		case ActorCombat.LifeState.Dying:
			if (myCoroutine != null)
			{
				StopCoroutine(myCoroutine);
			}
			myAgent.ResetPath();
			myAgent.enabled = false;
			ChangeState(State.Die, null);
			myCoroutine = StartCoroutine(RotationToGroundNormal());
			break;
		case ActorCombat.LifeState.Dead:
		{
			if (myCoroutine != null)
			{
				StopCoroutine(myCoroutine);
			}
			Mesh mesh = new Mesh();
			((SkinnedMeshRenderer)EnemyModel.Renderer).BakeMesh(mesh);
			Singleton<ItemSpawner>.Instance.SpawnEnemyItem(MyArchetype, CurrentScheme.Scheme.Edible, ScaleMod, EnemyModel.Renderer.transform.position, mesh, EnemyModel.Renderer.material, ModelTransform.rotation);
			Unspawn();
			break;
		}
		}
		if (state != 0)
		{
			myAnimator.SetFloat("Speed", 0f);
		}
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
		if (NavAgent.isOnNavMesh)
		{
			NavAgent.CompleteOffMeshLink();
		}
	}

	private IEnumerator RotationToGroundNormal()
	{
		Physics.Raycast(ModelTransform.position, Vector3.down, out RaycastHit hitInfo, 100f, 1 << Layers.ColliderLayer);
		Quaternion startRotation = ModelTransform.rotation;
		Quaternion endRotation = Quaternion.FromToRotation(ModelTransform.up, hitInfo.normal) * ModelTransform.rotation;
		float timer = 0f;
		while (true)
		{
			timer = Mathf.Clamp(timer + Time.deltaTime, 0f, 0.5f);
			float t = timer / 0.5f;
			ModelTransform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
			if (ModelTransform.rotation == endRotation)
			{
				break;
			}
			yield return null;
		}
	}

	private IEnumerator WaitForExitExtraIdleAnimation()
	{
		myAnimator.SetInteger("IdleType", 0);
		if (myAnimator.GetCurrentAnimatorStateInfo(0).tagHash == AnimationHashes.extraIdle.Value)
		{
			yield return new WaitWhile(() => myAnimator.GetCurrentAnimatorStateInfo(0).tagHash == AnimationHashes.extraIdle.Value);
		}
		else if (myAnimator.GetNextAnimatorStateInfo(0).tagHash == AnimationHashes.extraIdle.Value)
		{
			yield return new WaitWhile(() => myAnimator.GetCurrentAnimatorStateInfo(0).tagHash != AnimationHashes.extraIdle.Value);
			yield return new WaitWhile(() => myAnimator.GetCurrentAnimatorStateInfo(0).tagHash == AnimationHashes.extraIdle.Value);
		}
	}

	private IEnumerator Patrol()
	{
		_003C_003Ec__DisplayClass262_0 _003C_003Ec__DisplayClass262_ = default(_003C_003Ec__DisplayClass262_0);
		_003C_003Ec__DisplayClass262_._003C_003E4__this = this;
		WaitForSeconds wfs = new WaitForSeconds(timeToWaitForNextAction);
		needMove = true;
		isPlayerHeard = IsPlayerHeard();
		ResetMove();
		float curStopCooldown = 0f;
		float patrolInterval = 0f;
		lastPatrolPoint = Vector3.zero;
		Vector3 nextPosition = lastPatrolPoint;
		List<float> obstacleFromAgents = new List<float>();
		_003C_003Ec__DisplayClass262_.needBossRotate = true;
		_003C_003Ec__DisplayClass262_.checkPosAndRotBossTimer = 0f;
		_003C_003Ec__DisplayClass262_1 _003C_003Ec__DisplayClass262_2 = default(_003C_003Ec__DisplayClass262_1);
		while (true)
		{
			bool flag = !needMove && !NavMeshAgentUtils.HasPath(myAgent) && !patrolData.IsNeedRotateToPlayer;
			if (!IsBossScheme && !IsBossAssistantScheme)
			{
				if (NavMeshAgentUtils.HasPath(myAgent) && enemyHabitatArea.Inhabitants.Count > 1 && EnemyProcessingSwitch.OnProcessingDistance)
				{
					obstacleFromAgents.Clear();
					foreach (EnemyController inhabitant in enemyHabitatArea.Inhabitants)
					{
						if (inhabitant != this)
						{
							obstacleFromAgents.Add(_003CPatrol_003Eg__CalculateObstacle_007C262_2(myAgent.destination, inhabitant.NavAgent, ref _003C_003Ec__DisplayClass262_));
						}
					}
					obstacleFromAgents.Sort((float x, float y) => x.CompareTo(y));
					float stoppingDistance = obstacleFromAgents[obstacleFromAgents.Count - 1];
					myAgent.stoppingDistance = stoppingDistance;
				}
				else
				{
					myAgent.stoppingDistance = 0f;
				}
			}
			if (!NavMeshAgentUtils.HasPath(myAgent) && needMove && !patrolData.IsNeedRotateToPlayer && !stoppedByAlly)
			{
				myAgent.ResetPath();
				curStopCooldown = 0f;
				if (IsBossScheme)
				{
					nextPosition = GetSampledPosition(Singleton<EnemySpawner>.Instance.BossSpawnPoint.transform.position);
				}
				else if (IsBossAssistantScheme)
				{
					nextPosition = GetSampledPosition(bossAssistantSpawnPoint.Position);
				}
				else if (lastPatrolPoint != Vector3.zero)
				{
					nextPosition = lastPatrolPoint;
					lastPatrolPoint = Vector3.zero;
				}
				else
				{
					nextPosition = GetNextPatrolPosition();
				}
				yield return StartCoroutine(WaitForExitExtraIdleAnimation());
				needMove = false;
				myAgent.SetDestination(nextPosition);
				yield return null;
			}
			else if (flag && !stoppedByAlly)
			{
				if (returnFromPursuit)
				{
					returnFromPursuit = false;
				}
				if (IsBossScheme || IsBossAssistantScheme)
				{
					if (_003C_003Ec__DisplayClass262_.needBossRotate)
					{
						_003C_003Ec__DisplayClass262_2.needForward = (IsBossScheme ? Singleton<EnemySpawner>.Instance.BossSpawnPoint.transform.forward : bossAssistantSpawnPoint.transform.forward);
						_003CPatrol_003Eg__RotateCurModel_007C262_3(ref _003C_003Ec__DisplayClass262_, ref _003C_003Ec__DisplayClass262_2);
						yield return null;
						continue;
					}
					if (_003C_003Ec__DisplayClass262_.checkPosAndRotBossTimer == 0f)
					{
						Vector3 sourcePos = IsBossScheme ? Singleton<EnemySpawner>.Instance.BossSpawnPoint.transform.position : bossAssistantSpawnPoint.Position;
						Vector3 sampledPosition = GetSampledPosition(sourcePos);
						_003CPatrol_003Eg__CheckModelPosition_007C262_0(sampledPosition, ref _003C_003Ec__DisplayClass262_);
						_003C_003Ec__DisplayClass262_.checkPosAndRotBossTimer = Singleton<EnemySpawner>.Instance.GlobalEnemyParams.checkPosAndRotBossInterval;
					}
					_003C_003Ec__DisplayClass262_.checkPosAndRotBossTimer = Mathf.Clamp(_003C_003Ec__DisplayClass262_.checkPosAndRotBossTimer - timeToWaitForNextAction, 0f, Singleton<EnemySpawner>.Instance.GlobalEnemyParams.checkPosAndRotBossInterval);
					yield return wfs;
					continue;
				}
				if (curStopCooldown == 0f)
				{
					if (UnityEngine.Random.Range(0f, 100f) >= IdleChance)
					{
						needMove = true;
					}
					else
					{
						ResetMove();
						if (CountIdleAnimations > 1)
						{
							int value = UnityEngine.Random.Range(0, CountIdleAnimations);
							myAnimator.SetInteger("IdleType", value);
						}
						patrolInterval = UnityEngine.Random.Range(IdleTimeMin, IdleTimeMax);
					}
				}
				if (curStopCooldown >= patrolInterval)
				{
					needMove = true;
				}
				curStopCooldown += timeToWaitForNextAction;
				yield return wfs;
			}
			else if (isPlayerAlive && patrolData.IsNeedRotateToPlayer && !ManagerBase<PlayerManager>.Instance.isSleeping)
			{
				if (returnFromPursuit)
				{
					returnFromPursuit = false;
				}
				if (!PlayerCombat.IsHearable)
				{
					isPlayerHeard = false;
					needMove = true;
				}
				else
				{
					if (NavMeshAgentUtils.HasPath(myAgent) && lastPatrolPoint == Vector3.zero)
					{
						lastPatrolPoint = myAgent.pathEndPosition;
					}
					ResetMove();
					float num = Vector3.Distance(ModelTransform.position, PlayerTransform.position);
					float rotAngle = (RotationSpeed + CalculationsHelpUtils.CalculateProp(RotationSpeedFight - RotationSpeed, HearingRadius - num, HearingRadius)) * Time.deltaTime;
					Vector3 endDir = Vector3.ProjectOnPlane(PlayerTransform.position - ModelTransform.position, Vector3.up);
					Quaternion rotation = QuaternionUtils.GetRotation(ModelTransform.forward, endDir, rotAngle);
					ModelTransform.rotation *= rotation;
					if (rotation == Quaternion.identity)
					{
						needMove = true;
					}
				}
				yield return null;
			}
			else if (!flag && !stoppedByAlly)
			{
				if (EnemyProcessingSwitch.OnNavAgentMoveModeDistance)
				{
					float y2 = Vector3.Distance(ModelTransform.position, nextPosition);
					float speed = Mathf.Clamp(SpeedMinimum + CalculationsHelpUtils.CalculateProp(SpeedMedium - SpeedMinimum, y2, WalkDistanceMaximum), SpeedMinimum, SpeedMedium);
					ChangeMovementSpeed(speed);
				}
				yield return wfs;
			}
			else
			{
				yield return null;
			}
		}
	}

	private IEnumerator Pursuit()
	{
		if (PrevState != State.Escape && PrevState != State.Attack)
		{
			CurStamina = CalculateStaminaByHealth();
		}
		if (NavMeshAgentUtils.HasPath(myAgent))
		{
			ResetMove();
			yield return null;
		}
		ChangeMovementSpeed(speedMaximum);
		myAgent.acceleration = acceleration;
		yield return StartCoroutine(WaitForExitExtraIdleAnimation());
		while (true)
		{
			if ((PlayerTransform.position - ModelTransform.position).IsLongerOrEqual(attackRange) && !myAgent.pathPending)
			{
				myAgent.SetDestination(GetNextPursuitPosition());
			}
			yield return null;
		}
	}

	private IEnumerator Attack()
	{
		new WaitForSeconds(timeToWaitForNextAction);
		myAnimator.SetInteger("IdleType", 0);
		ResetMove();
		while (true)
		{
			Vector3 vector = Vector3.ProjectOnPlane(PlayerTransform.position - ModelTransform.position, Vector3.up);
			ModelTransform.rotation *= QuaternionUtils.GetRotation(ModelTransform.forward, vector, RotationSpeedFight * Time.deltaTime);
			bool flag = Vector3.Angle(ModelTransform.forward, vector) <= 5f;
			if (MyCombat.IsAttackTime() && flag && IsOnAttackRange())
			{
				attackIsOver = false;
				MyCombat.Attack(PlayerCombat, delegate
				{
					attackIsOver = true;
				});
			}
			yield return null;
		}
	}

	private bool IsOnAttackRange()
	{
		return (PlayerTransform.position - ModelTransform.position).IsShorter(attackRange);
	}

	private IEnumerator Escape()
	{
		WaitForSeconds wfs = new WaitForSeconds(timeToWaitForNextAction);
		CurStamina = CalculateStaminaByHealth();
		ResetMove();
		curAlly = null;
		slowTimer = 0f;
		bool isSlowed = false;
		forcePursuit = !GetNextEscapePosition(out Vector3 escapePos);
		if (forcePursuit)
		{
			yield return null;
		}
		myAgent.SetDestination(escapePos);
		if (curAlly != null)
		{
			curAlly.ResetMove(stoppedByAlly: true);
		}
		yield return StartCoroutine(WaitForExitExtraIdleAnimation());
		while (true)
		{
			if (PlayerCombat.HavePursuitedEnemy && PlayerCombat.PursuitedEnemy == this && PlayerMovement.IsHelpInNavigationEnabled)
			{
				if (slowTimer > 0f)
				{
					if (!isSlowed)
					{
						ChangeMovementSpeed(myAgent.speed - realHittedSlowSpeed);
						isSlowed = true;
					}
				}
				else
				{
					if (isSlowed)
					{
						isSlowed = false;
					}
					if (CurStamina > 0f)
					{
						float num = Vector3.Distance(ModelTransform.position, PlayerTransform.position);
						float speed = SpeedMedium + Mathf.Clamp(CalculationsHelpUtils.CalculateProp(speedMaximum - SpeedMedium, 1f - (num - hearing) / (hearingMaximum - hearing), 1f), 0f, speedMaximum - SpeedMedium);
						ChangeMovementSpeed(speed);
					}
					else
					{
						ChangeMovementSpeed((SpeedMedium + speedMaximum) / 2f);
					}
				}
			}
			else
			{
				ChangeMovementSpeed(escapeMediumSpeed);
			}
			yield return wfs;
		}
	}

	private void OnDrawGizmos()
	{
		if (drawGizmos && ModelTransform != null)
		{
			Mesh triangleMesh = GizmosUtils.GetTriangleMesh(ref viewMesh, viewAngle, ModelTransform.forward);
			Gizmos.color = viewZoneCol;
			Gizmos.DrawMesh(triangleMesh, ModelTransform.position, Quaternion.identity, Vector3.one * ViewDistance);
			Gizmos.color = hearingZoneCol;
			Gizmos.DrawWireSphere(ModelTransform.position, HearingRadius);
		}
	}

	[CompilerGenerated]
	private void _003CGetNextEscapePosition_003Eg__SetAllyHabbitatAndEscapePosition_007C227_0(ref Vector3 newEscapePosition)
	{
		Vector3 sampledPosition = GetSampledPosition(curAlly.ModelTransform.position);
		enemyHabitatArea.Inhabitants.Remove(this);
		enemyHabitatArea = curAlly.enemyHabitatArea;
		enemyHabitatArea.Inhabitants.Add(this);
		newEscapePosition = sampledPosition;
	}

	[CompilerGenerated]
	private bool _003CUpdate_003Eg__HaveObstaclesOnView_007C255_1()
	{
		Vector3 direction = ModelTransform.position + Vector3.up * (NavAgent.height / 2f) - CameraUtils.PlayerCamera.transform.position;
		return Physics.Raycast(CameraUtils.PlayerCamera.transform.position, direction, direction.magnitude, 1 << Layers.ColliderLayer);
	}

	[CompilerGenerated]
	private float _003CPatrol_003Eg__CalculateObstacle_007C262_2(Vector3 destination, NavMeshAgent navMeshAgent, ref _003C_003Ec__DisplayClass262_0 P_2)
	{
		float num = myAgent.radius + navMeshAgent.radius;
		if ((navMeshAgent.transform.position - destination).IsShorter(num))
		{
			return num;
		}
		return 0f;
	}

	[CompilerGenerated]
	private void _003CPatrol_003Eg__RotateCurModel_007C262_3(ref _003C_003Ec__DisplayClass262_0 P_0, ref _003C_003Ec__DisplayClass262_1 P_1)
	{
		Quaternion rotation = QuaternionUtils.GetRotation(ModelTransform.forward, P_1.needForward, RotationSpeed * Time.deltaTime);
		ModelTransform.rotation *= rotation;
		if (rotation.eulerAngles == Vector3.zero)
		{
			P_0.needBossRotate = false;
			P_0.checkPosAndRotBossTimer = Singleton<EnemySpawner>.Instance.GlobalEnemyParams.checkPosAndRotBossInterval;
		}
	}

	[CompilerGenerated]
	private void _003CPatrol_003Eg__CheckModelPosition_007C262_0(Vector3 needPosition, ref _003C_003Ec__DisplayClass262_0 P_1)
	{
		if (ModelTransform.position != needPosition)
		{
			needMove = true;
		}
		else
		{
			P_1.needBossRotate = true;
		}
	}
}
