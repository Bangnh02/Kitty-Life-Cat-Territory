using Avelog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class FarmResident : MonoBehaviour
{
	public enum State
	{
		None,
		Walking,
		MeetingPlayer,
		ChangingHabitat
	}

	[Serializable]
	private class ExtraAnimation
	{
		public string trigger;

		public float actionTimeMinimum;

		public float actionTimeMaximum;

		public bool isDiggingAnimation;
	}

	public delegate void FarmResidentEventHandler(FarmResident farmResident);

	public class PathIterator : IEnumerator<Vector3>, IEnumerator, IDisposable
	{
		private class PathLineSegmentPoint
		{
			public Vector3 point;

			public PathWaypoint waypoint1;

			public PathWaypoint waypoint2;
		}

		private PathLineSegmentPoint startSegmentPoint;

		private PathLineSegmentPoint endSegmentPoint;

		private Vector3 startPosition;

		private Vector3 endPosition;

		private List<Vector3> path;

		private bool useFirstWaypoint = true;

		private bool useLastWaypoint = true;

		private bool usePath = true;

		private bool startPositionPassed;

		private bool startSegmentPointPassed;

		private bool endSegmentPointPassed;

		private bool endPositionPassed;

		private int curPathIndex;

		private bool PathPassed
		{
			get
			{
				if (!usePath)
				{
					return true;
				}
				if (useLastWaypoint)
				{
					return curPathIndex > path.Count - 1;
				}
				return curPathIndex > path.Count - 2;
			}
		}

		public Vector3 Current
		{
			get
			{
				if (!startPositionPassed)
				{
					return startPosition;
				}
				if (!startSegmentPointPassed)
				{
					return startSegmentPoint.point;
				}
				if (usePath && !PathPassed)
				{
					return path[curPathIndex];
				}
				if (!endSegmentPointPassed)
				{
					return endSegmentPoint.point;
				}
				return endPosition;
			}
		}

		object IEnumerator.Current => Current;

		public PathIterator(Vector3 startPosition, Vector3 endPosition)
		{
			this.startPosition = startPosition;
			this.endPosition = endPosition;
			startSegmentPoint = GetNearestPathPoint(startPosition);
			endSegmentPoint = GetNearestPathPoint(endPosition);
			if ((startSegmentPoint.waypoint1 == endSegmentPoint.waypoint1 && startSegmentPoint.waypoint2 == endSegmentPoint.waypoint2) || (startSegmentPoint.waypoint1 == endSegmentPoint.waypoint2 && startSegmentPoint.waypoint2 == endSegmentPoint.waypoint1))
			{
				usePath = false;
				return;
			}
			path = (from x in Pathfinding.FindPath(startSegmentPoint.waypoint1, endSegmentPoint.waypoint1)
				select x.transform.position).ToList();
			if (path.Count > 1)
			{
				if ((path[0] == startSegmentPoint.waypoint1.transform.position && path[1] == startSegmentPoint.waypoint2.transform.position) || (path[0] == startSegmentPoint.waypoint2.transform.position && path[1] == startSegmentPoint.waypoint1.transform.position))
				{
					useFirstWaypoint = false;
				}
				if ((path[path.Count - 1] == endSegmentPoint.waypoint1.transform.position && path[path.Count - 2] == endSegmentPoint.waypoint2.transform.position) || (path[path.Count - 1] == endSegmentPoint.waypoint2.transform.position && path[path.Count - 2] == endSegmentPoint.waypoint1.transform.position))
				{
					useLastWaypoint = false;
				}
			}
		}

		private PathLineSegmentPoint GetNearestPathPoint(Vector3 sourcePosition)
		{
			float num = float.MaxValue;
			PathWaypoint pathWaypoint = PathWaypoint.Instances[0];
			foreach (PathWaypoint instance in PathWaypoint.Instances)
			{
				float sqrMagnitude = (instance.transform.position - sourcePosition).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					pathWaypoint = instance;
					num = sqrMagnitude;
				}
			}
			List<(Vector3, float, PathWaypoint)> list = new List<(Vector3, float, PathWaypoint)>();
			foreach (Pathfinding.IPathNode neighbor in pathWaypoint.Neighbors)
			{
				PathWaypoint pathWaypoint2 = neighbor as PathWaypoint;
				Vector3 vector = sourcePosition.ProjectOnLineSegment(pathWaypoint.transform.position, pathWaypoint2.transform.position);
				float sqrMagnitude2 = (vector - sourcePosition).sqrMagnitude;
				list.Add((vector, sqrMagnitude2, pathWaypoint2));
			}
			list.Sort((Comparison<(Vector3, float, PathWaypoint)>)(((Vector3 project, float sqrDistance, PathWaypoint waypoint) x, (Vector3 project, float sqrDistance, PathWaypoint waypoint) y) => x.sqrDistance.CompareTo(y.sqrDistance)));
			return new PathLineSegmentPoint
			{
				point = list[0].Item1,
				waypoint1 = pathWaypoint,
				waypoint2 = list[0].Item3
			};
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			if (!startPositionPassed)
			{
				startPositionPassed = true;
				return true;
			}
			if (!startSegmentPointPassed)
			{
				startSegmentPointPassed = true;
				curPathIndex = 0;
				if (!useFirstWaypoint)
				{
					curPathIndex++;
				}
				return true;
			}
			if (usePath && !PathPassed)
			{
				curPathIndex++;
				if (!useLastWaypoint && curPathIndex == path.Count - 1)
				{
					curPathIndex++;
				}
				return true;
			}
			if (!endSegmentPointPassed)
			{
				endSegmentPointPassed = true;
				return true;
			}
			if (!endPositionPassed)
			{
				endPositionPassed = true;
				return true;
			}
			return false;
		}

		public void Reset()
		{
		}

		public bool IsLastElement()
		{
			return endSegmentPointPassed;
		}
	}

	private class WalkingData
	{
		public Vector3 destination;

		public Transform farmerDigPoint;

		public PathIterator farmerPathIterator;

		public bool extraAnimationAfterReachPath;

		public float curWaitStartTime;

		public float curWaitDuration;

		public bool waiting;

		public ExtraAnimation extraAnimation;

		public Vector3 extraAnimationForward;

		private bool DestinationIsDiggingPos
		{
			get
			{
				if (extraAnimation != null)
				{
					return extraAnimation.isDiggingAnimation;
				}
				return false;
			}
		}
	}

	[SerializeField]
	private float meetingDistance = 15f;

	[SerializeField]
	private float playerViewAngle = 20f;

	[SerializeField]
	private bool haveMeetingAnimation;

	[SerializeField]
	private float checkMeetingPlayerFrequency = 1f;

	private float checkMeetingPlayerTimer;

	[SerializeField]
	private float rotationSpeed = 180f;

	[Postfix(PostfixAttribute.Id.Percents)]
	[SerializeField]
	private float corneringSlowing = 60f;

	[SerializeField]
	private float rotationSpeedOnHabitatChange = 30f;

	[Postfix(PostfixAttribute.Id.Percents)]
	[SerializeField]
	private float corneringSlowingOnHabitatChange = 10f;

	[SerializeField]
	private float idleTimeMinimum = 3f;

	[SerializeField]
	private float idleTimeMaximum = 6f;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private float extraAnimationsChance;

	[SerializeField]
	private List<ExtraAnimation> extraAnimations = new List<ExtraAnimation>();

	[Header("Настройка оффсета панелей информации.")]
	[SerializeField]
	private Vector3 worldFullOffset = Vector3.zero;

	[SerializeField]
	private Vector3 worldIconOffset;

	[SerializeField]
	[Header("Ссылки")]
	private NavMeshAgent navAgent;

	[SerializeField]
	private Animator animator;

	private FarmResidentManager.FarmResidentParams farmResidentParams;

	private FarmResidentHabitatArea _habitatArea;

	private ProcessingSwitch _processingSwitch;

	private bool isFirstWaypointPassed;

	private Coroutine curStateCor;

	private Coroutine changeHabitatTimerCor;

	private Vector3 velocity = Vector3.zero;

	private float curSpeed;

	private Vector3 prevPos;

	private WalkingData walkingData;

	private static List<ItemId> _itemIds;

	public Vector3 WorldFullOffset => worldFullOffset;

	public Vector3 WorldIconOffset => worldIconOffset;

	public NavMeshAgent NavAgent => navAgent;

	public FarmResidentManager.FarmResidentData FarmResidentData => farmResidentParams.FarmResidentData;

	private FarmResidentHabitatArea HabitatArea
	{
		get
		{
			return _habitatArea;
		}
		set
		{
			_habitatArea = value;
			farmResidentParams.FarmResidentData.curHabitatId = _habitatArea.HabitatId;
			farmResidentParams.FarmResidentData.curZoneId = _habitatArea.ZoneId;
		}
	}

	private ProcessingSwitch ProcessingSwitch
	{
		get
		{
			if (_processingSwitch == null)
			{
				_processingSwitch = GetComponent<ProcessingSwitch>();
			}
			return _processingSwitch;
		}
	}

	private SuperBonus.Id MySuperBonus
	{
		get
		{
			SuperBonus.Id result = SuperBonus.Id.BootsWalkers;
			if (FarmResidentData.farmResidentId == FarmResidentId.Goat)
			{
				result = SuperBonus.Id.Milk;
			}
			else if (FarmResidentData.farmResidentId == FarmResidentId.Pig)
			{
				result = SuperBonus.Id.Acorn;
			}
			return result;
		}
	}

	private PlayerBrain Player => PlayerSpawner.PlayerInstance;

	public State CurState
	{
		get;
		private set;
	}

	public bool ForceShowPanel
	{
		get;
		set;
	}

	private static List<ItemId> ItemIds
	{
		get
		{
			if (_itemIds == null)
			{
				_itemIds = EnumUtils.ToList<ItemId>();
				_itemIds.Remove(ItemId.Enemy);
				_itemIds.Remove(ItemId.None);
			}
			return _itemIds;
		}
	}

	public static event FarmResidentEventHandler changeStateEvent;

	public static event FarmResidentEventHandler changeNeedEvent;

	public static event FarmResidentEventHandler updateNeedProgressEvent;

	public void Spawn(FarmResidentManager.FarmResidentParams farmResidentParams, FarmResidentHabitatArea startHabitatArea)
	{
		ProcessingSwitch.maxProcessingDistance = Singleton<FarmResidentSpawner>.Instance.ProcessingDistance;
		ProcessingSwitch.UpdateProcessingState();
		this.farmResidentParams = farmResidentParams;
		HabitatArea = startHabitatArea;
		HabitatArea.Inhabitant = this;
		navAgent.angularSpeed = rotationSpeed;
		if (!IsNeedAvailableToSatisfy(farmResidentParams.FarmResidentData.curNeed))
		{
			RemoveNeed();
			GenerateNeed();
		}
		if (ManagerBase<FarmResidentManager>.Instance.SuperBonusesData.Find((FarmResidentManager.SuperBonusData x) => x.id == MySuperBonus).IsActive)
		{
			RemoveNeed();
			StartCoroutine(Timer(ManagerBase<FarmResidentManager>.Instance.SuperBonusesData.Find((FarmResidentManager.SuperBonusData x) => x.id == MySuperBonus).Timer, GenerateNeed));
		}
		ChangeState(State.Walking, Walking());
	}

	private void ChangeState(State newState, IEnumerator routine)
	{
		navAgent.autoBraking = (newState != State.ChangingHabitat);
		navAgent.stoppingDistance = 0f;
		if (curStateCor != null)
		{
			StopCoroutine(curStateCor);
		}
		CurState = newState;
		if (routine != null)
		{
			curStateCor = StartCoroutine(routine);
		}
		FarmResident.changeStateEvent?.Invoke(this);
	}

	private void Update()
	{
		if (Time.deltaTime != 0f)
		{
			float value = (base.transform.position - prevPos).magnitude / Time.deltaTime / navAgent.speed;
			animator?.SetFloat("normalizedSpeed", value);
			prevPos = base.transform.position;
		}
	}

	private bool HasPath()
	{
		return NavMeshAgentUtils.HasPath(navAgent);
	}

	private void ResetPath(bool instantResetVelocity = true)
	{
		if (instantResetVelocity)
		{
			navAgent.velocity = Vector3.zero;
		}
		navAgent.ResetPath();
		navAgent.autoBraking = true;
		navAgent.isStopped = true;
	}

	private void SetDestination(Vector3 destination)
	{
		NavMeshPath path = new NavMeshPath();
		navAgent.CalculatePath(destination, path);
		navAgent.isStopped = false;
		navAgent.SetPath(path);
	}

	private bool CheckMeetingPlayer()
	{
		if (HaveValidNeed())
		{
			return (Player.transform.position - base.transform.position).IsShorter(meetingDistance);
		}
		return false;
	}

	private bool CheckLookingPlayer()
	{
		Vector3 a = new Vector3(base.transform.position.x, 0f, base.transform.position.z);
		if (Vector3.Angle(to: a - new Vector3(Player.transform.position.x, 0f, Player.transform.position.z), from: new Vector3(Player.transform.forward.x, 0f, Player.transform.forward.z)) <= playerViewAngle / 2f)
		{
			return true;
		}
		return false;
	}

	private IEnumerator MeetingPlayer()
	{
		checkMeetingPlayerTimer = 0f;
		ResetPath();
		while (true)
		{
			if (checkMeetingPlayerTimer <= 0f)
			{
				checkMeetingPlayerTimer = checkMeetingPlayerFrequency;
				if (!CheckMeetingPlayer())
				{
					break;
				}
			}
			if (navAgent.velocity == Vector3.zero)
			{
				Vector3 endDir = Vector3.ProjectOnPlane(Player.transform.position - base.transform.position, Vector3.up);
				Quaternion rotation = QuaternionUtils.GetRotation(base.transform.forward, endDir, rotationSpeed * Time.deltaTime);
				base.transform.rotation *= rotation;
				if (rotation == Quaternion.identity && haveMeetingAnimation && !animator.GetBool("MeetPlayer"))
				{
					animator.SetBool("MeetPlayer", value: true);
				}
			}
			yield return null;
			checkMeetingPlayerTimer -= Time.deltaTime;
		}
		if (haveMeetingAnimation)
		{
			animator.SetBool("MeetPlayer", value: false);
		}
		yield return new WaitUntil(() => animator.IsPlayingByTag(AnimationHashes.idle.Value) && !animator.IsPlayingNextByTag(AnimationHashes.meetingPlayer.Value));
		ChangeState(State.Walking, Walking());
	}

	private IEnumerator ChangingHabitat()
	{
		FarmResidentHabitatArea newHabitat = Singleton<FarmResidentSpawner>.Instance.GetNewHabitat(FarmResidentData.farmResidentId);
		FarmResidentHabitatArea.MigratePath migratePath = HabitatArea.GetMigratePath(newHabitat);
		if (migratePath != null)
		{
			HabitatArea.Inhabitant = null;
			HabitatArea = newHabitat;
			HabitatArea.Inhabitant = this;
			isFirstWaypointPassed = false;
			int num = 0;
			int num2 = 0;
			float num3 = float.MaxValue;
			do
			{
				float sqrMagnitude = (migratePath.waypoints[num].transform.position - base.transform.position).sqrMagnitude;
				if (!(sqrMagnitude < num3))
				{
					break;
				}
				num2 = num;
				num3 = sqrMagnitude;
				num++;
			}
			while (num < migratePath.waypoints.Count);
			int curMigrateWaypointIndex = num2;
			navAgent.stoppingDistance = navAgent.radius;
			navAgent.isStopped = false;
			WaitWhile waitWhileHasPath = new WaitWhile(() => HasPath());
			while (curMigrateWaypointIndex < migratePath.waypoints.Count)
			{
				PathWaypoint pathWaypoint = migratePath.waypoints[curMigrateWaypointIndex];
				if (NavMeshUtils.SamplePositionIterative(pathWaypoint.transform.position, out NavMeshHit navMeshHit, 5f, 100f, 8, -1))
				{
					navAgent.SetDestination(navMeshHit.position);
					yield return waitWhileHasPath;
					if (!isFirstWaypointPassed)
					{
						isFirstWaypointPassed = true;
					}
					int num4 = curMigrateWaypointIndex + 1;
					curMigrateWaypointIndex = num4;
					continue;
				}
				UnityEngine.Debug.LogError("Can't hit navMesh from - " + pathWaypoint.gameObject.name + ". Maybe navMesh not built");
				yield break;
			}
			navAgent.stoppingDistance = 0f;
		}
		else
		{
			HabitatArea.Inhabitant = null;
			HabitatArea = newHabitat;
			HabitatArea.Inhabitant = this;
		}
		if (!ManagerBase<FarmResidentManager>.Instance.SuperBonusesData.Find((FarmResidentManager.SuperBonusData x) => x.id == MySuperBonus).IsActive)
		{
			GenerateNeed();
		}
		else
		{
			StartCoroutine(Timer(ManagerBase<FarmResidentManager>.Instance.SuperBonusesData.Find((FarmResidentManager.SuperBonusData x) => x.id == MySuperBonus).Timer, GenerateNeed));
		}
		ChangeState(State.Walking, Walking());
	}

	private IEnumerator Walking()
	{
		if (walkingData != null)
		{
			if (walkingData.waiting)
			{
				ResetPath();
				if (walkingData.extraAnimation != null)
				{
					while (true)
					{
						Quaternion rotation = QuaternionUtils.GetRotation(base.transform.forward, walkingData.extraAnimationForward, rotationSpeed * Time.deltaTime);
						base.transform.rotation *= rotation;
						if (rotation == Quaternion.identity)
						{
							break;
						}
						yield return null;
					}
					_003CWalking_003Eg__PlayExtraAnimation_007C76_5();
					yield return new WaitUntil(() => animator.IsPlayingByTag(AnimationHashes.extraIdle.Value));
				}
			}
			else
			{
				SetDestination(walkingData.destination);
			}
		}
		else
		{
			walkingData = new WalkingData
			{
				curWaitStartTime = Time.time,
				curWaitDuration = 0f,
				waiting = false,
				destination = base.transform.position,
				extraAnimation = null,
				extraAnimationAfterReachPath = false,
				extraAnimationForward = base.transform.forward,
				farmerDigPoint = null,
				farmerPathIterator = null
			};
			_003CWalking_003Eg__MoveToNextPos_007C76_4();
		}
		new WaitForSeconds(0.3f);
		checkMeetingPlayerTimer = 0f;
		float prevTime = Time.time;
		while (true)
		{
			if (checkMeetingPlayerTimer <= 0f)
			{
				checkMeetingPlayerTimer = checkMeetingPlayerFrequency;
				if (CheckMeetingPlayer() && CheckLookingPlayer() && !Player.PlayerCombat.InCombat)
				{
					break;
				}
			}
			if (walkingData.waiting)
			{
				navAgent.isStopped = true;
				if (!_003CWalking_003Eg__IsWaitTime_007C76_3())
				{
					StopExtraAnimation();
					walkingData.extraAnimation = null;
					yield return new WaitWhile(() => !animator.IsPlayingByTag(AnimationHashes.idle.Value));
					_003CWalking_003Eg__MoveToNextPos_007C76_4();
				}
			}
			else if (!HasPath())
			{
				if (walkingData.farmerPathIterator != null && walkingData.farmerPathIterator.MoveNext())
				{
					Vector3 current = walkingData.farmerPathIterator.Current;
					NavMeshUtils.SamplePositionIterative(current, out NavMeshHit _, 5f, 100f, 5, -1);
					SetDestination(current);
					walkingData.destination = current;
					if (walkingData.farmerPathIterator.IsLastElement())
					{
						navAgent.autoBraking = true;
						navAgent.stoppingDistance = 0f;
					}
				}
				else
				{
					ResetPath();
					walkingData.waiting = true;
					walkingData.curWaitStartTime = Time.time;
					if (walkingData.farmerDigPoint != null)
					{
						Vector3 digForward = new Vector3(walkingData.farmerDigPoint.forward.x, 0f, walkingData.farmerDigPoint.forward.z);
						while (true)
						{
							Quaternion rotation2 = QuaternionUtils.GetRotation(base.transform.forward, digForward, rotationSpeed * Time.deltaTime);
							base.transform.rotation *= rotation2;
							if (rotation2 == Quaternion.identity)
							{
								break;
							}
							yield return null;
						}
					}
					if (walkingData.extraAnimationAfterReachPath)
					{
						walkingData.curWaitDuration = UnityEngine.Random.Range(walkingData.extraAnimation.actionTimeMinimum, walkingData.extraAnimation.actionTimeMaximum);
						walkingData.extraAnimationForward = base.transform.forward;
						_003CWalking_003Eg__PlayExtraAnimation_007C76_5();
					}
					else
					{
						walkingData.curWaitDuration = UnityEngine.Random.Range(idleTimeMinimum, idleTimeMaximum);
					}
				}
			}
			else
			{
				navAgent.isStopped = false;
			}
			yield return null;
			checkMeetingPlayerTimer -= Time.time - prevTime;
			prevTime = Time.time;
		}
		ResetPath();
		if (animator.IsPlayingByTag(AnimationHashes.extraIdle.Value))
		{
			StopExtraAnimation();
			yield return new WaitUntil(() => animator.IsPlayingByTag(AnimationHashes.idle.Value));
		}
		ChangeState(State.MeetingPlayer, MeetingPlayer());
	}

	private void StopExtraAnimation()
	{
		if (walkingData != null && walkingData.extraAnimation != null && animator.IsPlayingByTag(AnimationHashes.extraIdle.Value))
		{
			animator?.SetTrigger("ExitExtraAnimation");
		}
	}

	public bool IsNeededFood(Food food)
	{
		if (food != null)
		{
			if (food.Id != ItemId.Enemy || !((food as EnemyFood).EnemyArchetype.name == FarmResidentData.curNeed))
			{
				return food.Id.ToString() == FarmResidentData.curNeed;
			}
			return true;
		}
		return false;
	}

	private bool IsNeedAvailableToSatisfy(string need)
	{
		if (string.IsNullOrEmpty(need))
		{
			return false;
		}
		if (ItemIds.Any((ItemId x) => x.ToString() == need))
		{
			return true;
		}
		if (Singleton<EnemySpawner>.Instance == null)
		{
			return false;
		}
		EnemyArchetype enemyArchetype = Singleton<EnemySpawner>.Instance.Archetypes.Find((EnemyArchetype x) => x.name == need);
		if (enemyArchetype == null)
		{
			return false;
		}
		if (!enemyArchetype.SimpleScheme.Edible)
		{
			return false;
		}
		return true;
	}

	private void GenerateNeed()
	{
		List<string> availableNeeds = new List<string>(farmResidentParams.needs.FindAll((string need) => IsNeedAvailableToSatisfy(need)));
		if (availableNeeds.Count != 0)
		{
			ManagerBase<FarmResidentManager>.Instance.FarmResidentsData.ForEach(delegate(FarmResidentManager.FarmResidentData farmResident)
			{
				if (availableNeeds.Count > 1 && availableNeeds.Contains(farmResident.curNeed))
				{
					availableNeeds.Remove(farmResident.curNeed);
				}
			});
			if (!FarmResidentData.startNeedUsed && availableNeeds.Contains(farmResidentParams.startNeed))
			{
				FarmResidentData.curNeed = farmResidentParams.startNeed;
			}
			else
			{
				FarmResidentData.curNeed = availableNeeds.Random();
			}
			FarmResidentData.startNeedUsed = true;
			ManagerBase<FarmResidentManager>.Instance.lastNeeds.Add(FarmResidentData.curNeed);
			while (ManagerBase<FarmResidentManager>.Instance.lastNeeds.Count > 30)
			{
				ManagerBase<FarmResidentManager>.Instance.lastNeeds.RemoveAt(0);
			}
			FarmResidentData.needProgressMaximum = ManagerBase<FarmResidentManager>.Instance.GetNeedCount(FarmResidentData.farmResidentId);
			FarmResident.changeNeedEvent?.Invoke(this);
		}
	}

	public bool HaveValidNeed()
	{
		return IsNeedAvailableToSatisfy(FarmResidentData.curNeed);
	}

	public void SatisfyNeed(Food food)
	{
		if (!IsNeededFood(food))
		{
			return;
		}
		food.Unspawn();
		FarmResidentData.needProgressCurrent++;
		FarmResident.updateNeedProgressEvent?.Invoke(this);
		float num = ManagerBase<FarmResidentManager>.Instance.ExperiencePerNeedUnit;
		if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Russian)
		{
			num += CalculationsHelpUtils.CalculateProp(num, ManagerBase<SkinManager>.Instance.RussianFarmResidentExpBonus, 100f);
		}
		ManagerBase<PlayerManager>.Instance.AddExperience(num);
		if (FarmResidentData.needProgressCurrent >= FarmResidentData.needProgressMaximum)
		{
			RemoveNeed();
			FarmResidentData.needProgressCurrent = 0;
			bool num2 = FarmResidentData.helpProgressCurrent >= ManagerBase<FarmResidentManager>.Instance.HelpProgressMaximum;
			int num3 = FarmResidentData.needProgressMaximum * ManagerBase<FarmResidentManager>.Instance.CoinsPerNeedUnit;
			if (num2)
			{
				num3 = (int)((float)num3 * ManagerBase<FarmResidentManager>.Instance.ProgressMaximumCoinsMulty);
			}
			else
			{
				FarmResidentData.helpProgressCurrent++;
			}
			Vector3 dropDirection = base.transform.position - Player.transform.position;
			if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Burmilla)
			{
				num3 *= ManagerBase<SkinManager>.Instance.BurmillaFarmResidentCoinsMultBonus;
			}
			Singleton<CoinSpawner>.Instance.DropCoins(base.transform.position, dropDirection, num3);
			if (!ManagerBase<FarmResidentManager>.Instance.SuperBonusesData.Find((FarmResidentManager.SuperBonusData x) => x.id == MySuperBonus).IsPermanent)
			{
				Singleton<SuperBonusSpawner>.Instance.Spawn(MySuperBonus, base.transform.position, dropDirection);
			}
			if (changeHabitatTimerCor == null)
			{
				changeHabitatTimerCor = StartCoroutine(Timer(ManagerBase<FarmResidentManager>.Instance.PauseBetweenChangeHabitat, delegate
				{
					StopExtraAnimation();
					walkingData = null;
					ChangeState(State.ChangingHabitat, ChangingHabitat());
					changeHabitatTimerCor = null;
				}));
			}
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
		farmResidentParams.FarmResidentData.curNeed = null;
		FarmResident.changeNeedEvent?.Invoke(this);
	}

	[CompilerGenerated]
	private bool _003CWalking_003Eg__IsWaitTime_007C76_3()
	{
		return Time.time - walkingData.curWaitStartTime < walkingData.curWaitDuration;
	}

	[CompilerGenerated]
	private void _003CWalking_003Eg__MoveToNextPos_007C76_4()
	{
		walkingData.extraAnimationAfterReachPath = (UnityEngine.Random.Range(0f, 100f) <= extraAnimationsChance);
		if (walkingData.extraAnimationAfterReachPath)
		{
			List<ExtraAnimation> list = (HabitatArea.DigZones.Count > 0) ? extraAnimations : extraAnimations.FindAll((ExtraAnimation x) => !x.isDiggingAnimation);
			if (list.Count > 0)
			{
				int index = UnityEngine.Random.Range(0, list.Count);
				walkingData.extraAnimation = list[index];
			}
			else
			{
				walkingData.extraAnimationAfterReachPath = false;
			}
		}
		Vector3 position = base.transform.position;
		walkingData.farmerPathIterator = null;
		if (walkingData.extraAnimationAfterReachPath && walkingData.extraAnimation.isDiggingAnimation)
		{
			if (FarmResidentData.farmResidentId == FarmResidentId.Farmer)
			{
				walkingData.farmerDigPoint = HabitatArea.DigZones.Random().GetRandomFarmerDigPoint();
				position = walkingData.farmerDigPoint.position;
			}
			else
			{
				position = HabitatArea.DigZones.Random().GetRandomPoint();
			}
		}
		else
		{
			position = HabitatArea.PatrolZones.Random().GetRandomPoint();
			walkingData.farmerDigPoint = null;
		}
		NavMeshUtils.SamplePositionIterative(position, out NavMeshHit navMeshHit, 5f, 100f, 5, -1);
		if (FarmResidentData.farmResidentId != FarmResidentId.Farmer)
		{
			position = navMeshHit.position;
		}
		else
		{
			walkingData.farmerPathIterator = new PathIterator(base.transform.position, navMeshHit.position);
			position = walkingData.farmerPathIterator.Current;
			navAgent.autoBraking = false;
			navAgent.stoppingDistance = navAgent.radius;
		}
		SetDestination(position);
		walkingData.destination = position;
		walkingData.waiting = false;
	}

	[CompilerGenerated]
	private void _003CWalking_003Eg__PlayExtraAnimation_007C76_5()
	{
		animator?.SetTrigger(walkingData.extraAnimation.trigger);
	}
}
