using Avelog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

public class PlayerFamilyController : MonoBehaviour, IInitializablePlayerComponent
{
	[Serializable]
	public class Pursuer
	{
		public EnemyController enemyController;

		public ActorCombat actorCombat;

		public float takeDamageTime;

		public float attackTime;
	}

	[Serializable]
	public class FollowPos
	{
		public FamilyMemberController owner;

		public Vector3 localPos;

		public bool isCloseToPlayer;
	}

	private class FamilyMemberFollowPosData
	{
		public FamilyMemberController familyMemberController;

		public FollowPos followPos;

		public float sqrDistance;
	}

	public delegate void AddFamilyMemberHandler(FamilyManager.FamilyMemberData familyMember);

	private class FamilyMemberSatiety
	{
		public FamilyManager.FamilyMemberData familyMember;

		public float value;
	}

	public delegate void FamilyMemberEndEatHandler(float satietyInc, float foodEffect);

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Comparison<FollowPos> _003C_003E9__34_0;

		public static Comparison<FollowPos> _003C_003E9__34_1;

		public static Comparison<FollowPos> _003C_003E9__34_2;

		public static Action<FollowPos> _003C_003E9__37_2;

		public static Func<FamilyMemberController, bool> _003C_003E9__37_3;

		public static Predicate<FamilyMemberController> _003C_003E9__37_5;

		public static Comparison<FamilyMemberFollowPosData> _003C_003E9__37_4;

		public static Func<FamilyManager.FamilyMemberData, bool> _003C_003E9__52_0;

		public static Predicate<FamilyManager.FamilyMemberData> _003C_003E9__54_0;

		public static Predicate<FamilyMemberController> _003C_003E9__55_0;

		public static Predicate<FamilyManager.FamilyMemberData> _003C_003E9__55_1;

		public static Predicate<Pursuer> _003C_003E9__72_0;

		public static Predicate<Pursuer> _003C_003E9__72_1;

		public static Predicate<Pursuer> _003C_003E9__73_1;

		public static Func<Pursuer, bool> _003C_003E9__73_2;

		public static Comparison<Pursuer> _003C_003E9__73_3;

		public static Comparison<Pursuer> _003C_003E9__73_5;

		public static Func<Pursuer, bool> _003C_003E9__73_6;

		public static Comparison<Pursuer> _003C_003E9__73_7;

		public static Func<EnemyController, bool> _003C_003E9__73_0;

		public static Predicate<EnemyController> _003C_003E9__73_8;

		public static Predicate<(EnemyController enemy, float sqrDistance)> _003C_003E9__73_10;

		public static Comparison<(EnemyController enemy, float sqrDistance)> _003C_003E9__73_11;

		public static Predicate<FamilyMemberController> _003C_003E9__96_5;

		public static Comparison<(FamilyManager.FamilyMemberData familyMember, float eatenRatio)> _003C_003E9__96_0;

		public static Func<(FamilyManager.FamilyMemberData familyMember, float eatenRatio), bool> _003C_003E9__96_1;

		public static Comparison<(FamilyMemberController familyMemberController, Water.DrinkPoint waterDrinkPoint, float sqrDistance)> _003C_003E9__98_0;

		public static Action<FamilyMemberController> _003C_003E9__99_0;

		public static Action<FamilyMemberController> _003C_003E9__100_0;

		public static Func<FamilyMemberController, bool> _003C_003E9__105_0;

		public static Comparison<(FamilyMemberController controller, Vector3 sleepPos, float sqrDistance)> _003C_003E9__106_2;

		public static Comparison<(Vector3 sleepPos, float sqrDistance)> _003C_003E9__107_2;

		internal int _003CFamilyPositions_Initialize_003Eb__34_0(FollowPos x, FollowPos y)
		{
			return x.localPos.sqrMagnitude.CompareTo(y.localPos.sqrMagnitude);
		}

		internal int _003CFamilyPositions_Initialize_003Eb__34_1(FollowPos x, FollowPos y)
		{
			return x.localPos.sqrMagnitude.CompareTo(y.localPos.sqrMagnitude);
		}

		internal int _003CFamilyPositions_Initialize_003Eb__34_2(FollowPos x, FollowPos y)
		{
			return x.localPos.sqrMagnitude.CompareTo(y.localPos.sqrMagnitude);
		}

		internal void _003CFamilyPositions_Update_003Eb__37_2(FollowPos x)
		{
			x.owner = null;
		}

		internal bool _003CFamilyPositions_Update_003Eb__37_3(FamilyMemberController x)
		{
			return x.familyMemberData == ManagerBase<FamilyManager>.Instance.GrowingChild;
		}

		internal bool _003CFamilyPositions_Update_003Eb__37_5(FamilyMemberController x)
		{
			return x.familyMemberData == ManagerBase<FamilyManager>.Instance.GrowingChild;
		}

		internal int _003CFamilyPositions_Update_003Eb__37_4(FamilyMemberFollowPosData x, FamilyMemberFollowPosData y)
		{
			return x.sqrDistance.CompareTo(y.sqrDistance);
		}

		internal bool _003CCanMakeChild_003Eb__52_0(FamilyManager.FamilyMemberData x)
		{
			return x.role == FamilyManager.FamilyMemberRole.ThirdStageChild;
		}

		internal bool _003CMakeChild_003Eb__54_0(FamilyManager.FamilyMemberData x)
		{
			return x.role != FamilyManager.FamilyMemberRole.Spouse;
		}

		internal bool _003CTestMakeChild_003Eb__55_0(FamilyMemberController x)
		{
			if (x.familyMemberData.role != 0)
			{
				return x.familyMemberData.role == FamilyManager.FamilyMemberRole.SecondStageChild;
			}
			return true;
		}

		internal bool _003CTestMakeChild_003Eb__55_1(FamilyManager.FamilyMemberData x)
		{
			return x.role != FamilyManager.FamilyMemberRole.Spouse;
		}

		internal bool _003CAttack_Update_003Eb__72_0(Pursuer x)
		{
			if (x.enemyController.CurState != EnemyController.State.Attack)
			{
				return x.enemyController.CurState != EnemyController.State.Pursuit;
			}
			return false;
		}

		internal bool _003CAttack_Update_003Eb__72_1(Pursuer x)
		{
			return x.actorCombat.CurLifeState != ActorCombat.LifeState.Alive;
		}

		internal bool _003CUpdateAttackTarget_003Eb__73_1(Pursuer x)
		{
			if (x.actorCombat.CurLifeState == ActorCombat.LifeState.Alive)
			{
				return Time.time - x.takeDamageTime < ManagerBase<FamilyManager>.Instance.CounterAttackTimer;
			}
			return false;
		}

		internal bool _003CUpdateAttackTarget_003Eb__73_2(Pursuer x)
		{
			return x.attackTime != 0f;
		}

		internal int _003CUpdateAttackTarget_003Eb__73_3(Pursuer x, Pursuer y)
		{
			return x.attackTime.CompareTo(y.attackTime);
		}

		internal int _003CUpdateAttackTarget_003Eb__73_5(Pursuer x, Pursuer y)
		{
			return x.takeDamageTime.CompareTo(y.takeDamageTime);
		}

		internal bool _003CUpdateAttackTarget_003Eb__73_6(Pursuer x)
		{
			if (x.actorCombat.CurLifeState == ActorCombat.LifeState.Alive)
			{
				return x.attackTime != 0f;
			}
			return false;
		}

		internal int _003CUpdateAttackTarget_003Eb__73_7(Pursuer x, Pursuer y)
		{
			return x.attackTime.CompareTo(y.attackTime);
		}

		internal bool _003CUpdateAttackTarget_003Eb__73_0(EnemyController x)
		{
			return x.CurState == EnemyController.State.Escape;
		}

		internal bool _003CUpdateAttackTarget_003Eb__73_8(EnemyController x)
		{
			return x.CurState == EnemyController.State.Escape;
		}

		internal bool _003CUpdateAttackTarget_003Eb__73_10((EnemyController enemy, float sqrDistance) x)
		{
			return x.sqrDistance > ManagerBase<FamilyManager>.Instance.PursuitRadius * ManagerBase<FamilyManager>.Instance.PursuitRadius;
		}

		internal int _003CUpdateAttackTarget_003Eb__73_11((EnemyController enemy, float sqrDistance) x, (EnemyController enemy, float sqrDistance) y)
		{
			return x.sqrDistance.CompareTo(y.sqrDistance);
		}

		internal bool _003CEat_003Eb__96_5(FamilyMemberController x)
		{
			return x.familyMemberData == ManagerBase<FamilyManager>.Instance.GrowingChild;
		}

		internal int _003CEat_003Eb__96_0((FamilyManager.FamilyMemberData familyMember, float eatenRatio) x, (FamilyManager.FamilyMemberData familyMember, float eatenRatio) y)
		{
			return x.eatenRatio.CompareTo(y.eatenRatio);
		}

		internal bool _003CEat_003Eb__96_1((FamilyManager.FamilyMemberData familyMember, float eatenRatio) x)
		{
			return x.eatenRatio >= 1f;
		}

		internal int _003CDrink_003Eb__98_0((FamilyMemberController familyMemberController, Water.DrinkPoint waterDrinkPoint, float sqrDistance) x, (FamilyMemberController familyMemberController, Water.DrinkPoint waterDrinkPoint, float sqrDistance) y)
		{
			return x.sqrDistance.CompareTo(y.sqrDistance);
		}

		internal void _003CInvisibility_003Eb__99_0(FamilyMemberController x)
		{
			x.SwitchInvisibility(isEnabled: true);
		}

		internal void _003CCancelInvisibility_003Eb__100_0(FamilyMemberController x)
		{
			x.SwitchInvisibility(isEnabled: false);
		}

		internal bool _003CFamilyCanSleep_003Eb__105_0(FamilyMemberController x)
		{
			return x.CanSleep();
		}

		internal int _003CSleep_003Eb__106_2((FamilyMemberController controller, Vector3 sleepPos, float sqrDistance) x, (FamilyMemberController controller, Vector3 sleepPos, float sqrDistance) y)
		{
			return x.sqrDistance.CompareTo(y.sqrDistance);
		}

		internal int _003CGetPositionToSleep_003Eb__107_2((Vector3 sleepPos, float sqrDistance) x, (Vector3 sleepPos, float sqrDistance) y)
		{
			return x.sqrDistance.CompareTo(y.sqrDistance);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003C_003Ec__DisplayClass37_0
	{
		public List<FollowPos> distributedFollowPositions;
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003C_003Ec__DisplayClass37_1
	{
		public FamilyMemberFollowPosData followPosData;
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass37_2
	{
		public FamilyManager.FamilyMemberData familyMemberData;

		public bool closeToPlayer;

		public FamilyMemberFollowPosData nearestPosData;

		internal void _003CFamilyPositions_Update_003Eb__7(FamilyMemberFollowPosData followPosData)
		{
			if (followPosData.familyMemberController.familyMemberData == familyMemberData && (!closeToPlayer || followPosData.followPos.isCloseToPlayer) && (nearestPosData == null || followPosData.sqrDistance < nearestPosData.sqrDistance))
			{
				nearestPosData = followPosData;
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass38_0
	{
		public PlayerFamilyController _003C_003E4__this;

		public FamilyMemberController familyMemberController;

		internal void _003CFamilyPositions_OnSpawnFamilyMember_003Eb__0(FollowPos x)
		{
			_003C_003E4__this.followPosDatas.Add(new FamilyMemberFollowPosData
			{
				familyMemberController = familyMemberController,
				followPos = x,
				sqrDistance = 0f
			});
		}

		internal void _003CFamilyPositions_OnSpawnFamilyMember_003Eb__1(FollowPos x)
		{
			_003C_003E4__this.invisibilityFollowPosDatas.Add(new FamilyMemberFollowPosData
			{
				familyMemberController = familyMemberController,
				followPos = x,
				sqrDistance = 0f
			});
		}

		internal void _003CFamilyPositions_OnSpawnFamilyMember_003Eb__2(FollowPos x)
		{
			_003C_003E4__this.pursuitFollowPosDatas.Add(new FamilyMemberFollowPosData
			{
				familyMemberController = familyMemberController,
				followPos = x,
				sqrDistance = 0f
			});
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass39_0
	{
		public FamilyMemberController familyMemberController;

		internal bool _003CGetFollowPosition_003Eb__0(FollowPos x)
		{
			return x.owner == familyMemberController;
		}

		internal bool _003CGetFollowPosition_003Eb__1(FollowPos x)
		{
			return x.owner == familyMemberController;
		}

		internal bool _003CGetFollowPosition_003Eb__2(FollowPos x)
		{
			return x.owner == familyMemberController;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass56_0
	{
		public FamilyManager.FamilyMemberData familyMember;

		internal bool _003CSpawnFamily_003Eb__0(FamilyMemberController x)
		{
			return x.familyMemberData == familyMember;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass70_0
	{
		public ActorCombat attacker;

		internal bool _003COnTakeDamage_003Eb__0(Pursuer x)
		{
			return x.actorCombat == attacker;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass71_0
	{
		public ActorCombat target;

		internal bool _003COnPlayerHitEnemy_003Eb__0(Pursuer x)
		{
			return x.actorCombat == target;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass73_0
	{
		public List<(EnemyController enemy, float sqrDistance)> enemiesDinstance;

		public PlayerFamilyController _003C_003E4__this;

		internal void _003CUpdateAttackTarget_003Eb__9(EnemyController enemy)
		{
			enemiesDinstance.Add((enemy, (enemy.EnemyModel.transform.position - _003C_003E4__this.transform.position).sqrMagnitude));
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass90_0
	{
		public FamilyManager.FamilyMemberData familyMember;

		internal bool _003CSatietyController_OnAddFamilyMember_003Eb__0(FamilyManager.FamilyMemberParams x)
		{
			return x.Type == familyMember.role;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass91_0
	{
		public ActorCombat attacker;

		internal bool _003CSatietyController_OnHit_003Eb__0(FamilyMemberController x)
		{
			return x.Combat == attacker;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass93_0
	{
		public FamilyManager.FamilyMemberData familyMember;

		internal bool _003CUpdateEatersList_003Eb__0(FamilyMemberSatiety x)
		{
			return x.familyMember == familyMember;
		}

		internal bool _003CUpdateEatersList_003Eb__1(FamilyMemberSatiety x)
		{
			return x.familyMember == familyMember;
		}

		internal bool _003CUpdateEatersList_003Eb__2(FamilyManager.FamilyMemberParams x)
		{
			return x.Type == familyMember.role;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass96_0
	{
		public PlayerFamilyController _003C_003E4__this;

		public Action endEatingCallback;

		public FamilyManager.FamilyMemberData selectedFamilyMember;

		public Food foodToEat;

		internal bool _003CEat_003Eb__2(FamilyMemberSatiety x)
		{
			return x.familyMember == selectedFamilyMember;
		}

		internal bool _003CEat_003Eb__3(FamilyMemberController x)
		{
			return x.familyMemberData == selectedFamilyMember;
		}

		internal void _003CEat_003Eb__4()
		{
			_003C_003E4__this.OnFamilyMemberEndEat(foodToEat);
			endEatingCallback?.Invoke();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass98_0
	{
		public Water water;

		public Vector3 familyDrinkPoint;

		internal void _003CDrink_003Eb__2(FamilyMemberController x)
		{
			x.Drink(familyDrinkPoint, FamilyMemberController.DrinkType.FromNearestWaterPoint, water);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass98_1
	{
		public (FamilyMemberController familyMemberController, Water.DrinkPoint waterDrinkPoint, float sqrDistance) nearestPoint;

		internal bool _003CDrink_003Eb__1((FamilyMemberController familyMemberController, Water.DrinkPoint waterDrinkPoint, float sqrDistance) x)
		{
			if (!(x.familyMemberController == nearestPoint.familyMemberController))
			{
				return x.waterDrinkPoint.point.position == nearestPoint.waterDrinkPoint.point.position;
			}
			return true;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass106_0
	{
		public List<Vector3> sleepWorldPositions;

		public PlayerFamilyController _003C_003E4__this;

		public List<(FamilyMemberController controller, Vector3 sleepPos, float sqrDistance)> distancesToSleepPositions;

		internal void _003CSleep_003Eb__0(Vector3 x)
		{
			sleepWorldPositions.Add(_003C_003E4__this.transform.position + _003C_003E4__this.transform.rotation * x);
		}

		internal void _003CSleep_003Eb__1(FamilyMemberController controller)
		{
			sleepWorldPositions.ForEach(delegate(Vector3 sleepPos)
			{
				distancesToSleepPositions.Add((controller, sleepPos, (controller.transform.position - sleepPos).sqrMagnitude));
			});
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass106_1
	{
		public FamilyMemberController controller;

		public _003C_003Ec__DisplayClass106_0 CS_0024_003C_003E8__locals1;

		internal void _003CSleep_003Eb__3(Vector3 sleepPos)
		{
			CS_0024_003C_003E8__locals1.distancesToSleepPositions.Add((controller, sleepPos, (controller.transform.position - sleepPos).sqrMagnitude));
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass106_2
	{
		public (FamilyMemberController controller, Vector3 sleepPos, float sqrDistance) lowestDistance;

		internal bool _003CSleep_003Eb__4((FamilyMemberController controller, Vector3 sleepPos, float sqrDistance) x)
		{
			if (!(x.controller == lowestDistance.controller))
			{
				return x.sleepPos == lowestDistance.sleepPos;
			}
			return true;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass107_0
	{
		public List<Vector3> sleepWorldPositions;

		public PlayerFamilyController _003C_003E4__this;

		public List<(Vector3 sleepPos, float sqrDistance)> distancesToSleepPositions;

		public FamilyMemberController familyMember;

		internal void _003CGetPositionToSleep_003Eb__0(Vector3 x)
		{
			sleepWorldPositions.Add(_003C_003E4__this.transform.position + _003C_003E4__this.transform.rotation * x);
		}

		internal void _003CGetPositionToSleep_003Eb__1(Vector3 sleepPos)
		{
			distancesToSleepPositions.Add((sleepPos, (familyMember.transform.position - sleepPos).sqrMagnitude));
		}
	}

	[SerializeField]
	private GameObject familyMemberPrefab;

	[SerializeField]
	private Transform followerPos1;

	[SerializeField]
	private Transform followerPos2;

	[SerializeField]
	private Transform followerInvisibilityPos1;

	[SerializeField]
	private Transform followerInvisibilityPos2;

	[SerializeField]
	private Transform followerPursiutPos1;

	[SerializeField]
	private Transform followerPursuitPos2;

	private List<FollowPos> followPositions;

	private List<FollowPos> followInvisibilityPositions;

	private List<FollowPos> followInPursuitPositions;

	private List<FamilyMemberFollowPosData> followPosDatas = new List<FamilyMemberFollowPosData>();

	private List<FamilyMemberFollowPosData> invisibilityFollowPosDatas = new List<FamilyMemberFollowPosData>();

	private List<FamilyMemberFollowPosData> pursuitFollowPosDatas = new List<FamilyMemberFollowPosData>();

	private List<FamilyMemberController> familyMemberControllers = new List<FamilyMemberController>();

	private List<FollowPos> DistributedFollowPositions = new List<FollowPos>();

	private List<FollowPos> DistributedInvisibilityFollowPositions = new List<FollowPos>();

	private List<FollowPos> DistributedPursuitFollowPositions = new List<FollowPos>();

	private bool isFamilyPositionsInitialized;

	private ActorCombat _familyAttackTarget;

	private float updateAttackFrequency = 0.2f;

	private float updateAttackTimer;

	private List<FamilyMemberSatiety> eaten = new List<FamilyMemberSatiety>();

	private List<FamilyMemberSatiety> needToEat = new List<FamilyMemberSatiety>();

	[SerializeField]
	private Transform sleepPos1;

	[SerializeField]
	private Transform sleepPos2;

	private List<Vector3> sleepLocalPositions = new List<Vector3>();

	private PlayerCombat PlayerCombat => PlayerSpawner.PlayerInstance.PlayerCombat;

	private PlayerMovement PlayerMovement => PlayerSpawner.PlayerInstance.PlayerMovement;

	public List<FamilyMemberController> FamilyMembersControllers => familyMemberControllers;

	public List<Pursuer> pursuers
	{
		get;
		private set;
	} = new List<Pursuer>();


	public ActorCombat FamilyAttackTarget
	{
		get
		{
			return _familyAttackTarget;
		}
		private set
		{
			if (_familyAttackTarget != null)
			{
				_familyAttackTarget.changeLifeStateEvent -= OnFamilyAttackTargetChangeLifeState;
			}
			_familyAttackTarget = value;
			if (_familyAttackTarget != null)
			{
				FamilyAttackTargetController = _familyAttackTarget.GetComponentInParent<EnemyController>();
			}
			else
			{
				FamilyAttackTargetController = null;
			}
			if (_familyAttackTarget != null)
			{
				_familyAttackTarget.changeLifeStateEvent += OnFamilyAttackTargetChangeLifeState;
			}
		}
	}

	public EnemyController FamilyAttackTargetController
	{
		get;
		private set;
	}

	private PlayerEating PlayerEating => PlayerSpawner.PlayerInstance.PlayerEating;

	public static event AddFamilyMemberHandler addFamilyMemberEvent;

	public event FamilyMemberEndEatHandler familyMemberEndEatEvent;

	public static event Action familyChangeSatietyEvent;

	public void Initialize()
	{
		FamilyPositions_Initialize();
		SatietyController_Initialize();
		Attack_Initialize();
		MakeFamily_Initialize();
		Sleep_Initialize();
		SpawnFamily();
	}

	private void OnDestroy()
	{
		Attack_Destroy();
		MakeFamily_Destroy();
		SatietyController_Destroy();
	}

	private void Update()
	{
		if (Time.deltaTime != 0f)
		{
			FamilyPositions_Update();
			Attack_Update();
			SatietyController_Update();
		}
	}

	private void FamilyPositions_Initialize()
	{
		followPositions = new List<FollowPos>();
		followPositions.Add(new FollowPos
		{
			localPos = followerPos1.transform.localPosition,
			owner = null
		});
		followPositions.Add(new FollowPos
		{
			localPos = followerPos2.transform.localPosition,
			owner = null
		});
		followPositions.Add(new FollowPos
		{
			localPos = MirrorPosByForward(followerPos1.transform.localPosition),
			owner = null
		});
		followPositions.Add(new FollowPos
		{
			localPos = MirrorPosByForward(followerPos2.transform.localPosition),
			owner = null
		});
		followPositions.Sort((FollowPos x, FollowPos y) => x.localPos.sqrMagnitude.CompareTo(y.localPos.sqrMagnitude));
		followPositions[0].isCloseToPlayer = true;
		followPositions[1].isCloseToPlayer = true;
		followInvisibilityPositions = new List<FollowPos>();
		followInvisibilityPositions.Add(new FollowPos
		{
			localPos = followerInvisibilityPos1.transform.localPosition,
			owner = null
		});
		followInvisibilityPositions.Add(new FollowPos
		{
			localPos = followerInvisibilityPos2.transform.localPosition,
			owner = null
		});
		followInvisibilityPositions.Add(new FollowPos
		{
			localPos = MirrorPosByForward(followerInvisibilityPos1.transform.localPosition),
			owner = null
		});
		followInvisibilityPositions.Add(new FollowPos
		{
			localPos = MirrorPosByForward(followerInvisibilityPos2.transform.localPosition),
			owner = null
		});
		followInvisibilityPositions.Sort((FollowPos x, FollowPos y) => x.localPos.sqrMagnitude.CompareTo(y.localPos.sqrMagnitude));
		followInvisibilityPositions[0].isCloseToPlayer = true;
		followInvisibilityPositions[1].isCloseToPlayer = true;
		followInPursuitPositions = new List<FollowPos>();
		followInPursuitPositions.Add(new FollowPos
		{
			localPos = followerPursiutPos1.transform.localPosition,
			owner = null
		});
		followInPursuitPositions.Add(new FollowPos
		{
			localPos = followerPursuitPos2.transform.localPosition,
			owner = null
		});
		followInPursuitPositions.Add(new FollowPos
		{
			localPos = MirrorPosByForward(followerPursiutPos1.transform.localPosition),
			owner = null
		});
		followInPursuitPositions.Add(new FollowPos
		{
			localPos = MirrorPosByForward(followerPursuitPos2.transform.localPosition),
			owner = null
		});
		followInPursuitPositions.Sort((FollowPos x, FollowPos y) => x.localPos.sqrMagnitude.CompareTo(y.localPos.sqrMagnitude));
		followInPursuitPositions[0].isCloseToPlayer = true;
		followInPursuitPositions[1].isCloseToPlayer = true;
		PlayerCombat.InvisibilityAbility.switchInvisibilityEvent += OnSwitchInvisibility;
		isFamilyPositionsInitialized = true;
	}

	private Vector3 MirrorPosByForward(Vector3 sourcePos)
	{
		Quaternion rotation = Quaternion.FromToRotation(Vector3.ProjectOnPlane(sourcePos, Vector3.up), sourcePos);
		Quaternion rotation2 = Quaternion.LookRotation(sourcePos);
		return Quaternion.Inverse(rotation) * Quaternion.Inverse(rotation2) * Quaternion.Inverse(rotation2) * sourcePos;
	}

	private void OnSwitchInvisibility(bool state)
	{
		FamilyPositions_Update();
	}

	private void FamilyPositions_Update()
	{
		if (isFamilyPositionsInitialized)
		{
			if (ManagerBase<PlayerManager>.Instance.Invisibility.IsActive)
			{
				_003CFamilyPositions_Update_003Eg__DistributeFollowPositions_007C37_0(invisibilityFollowPosDatas, DistributedInvisibilityFollowPositions);
			}
			else if (PlayerMovement.IsHelpInNavigationEnabled)
			{
				_003CFamilyPositions_Update_003Eg__DistributeFollowPositions_007C37_0(pursuitFollowPosDatas, DistributedPursuitFollowPositions);
			}
			else
			{
				_003CFamilyPositions_Update_003Eg__DistributeFollowPositions_007C37_0(followPosDatas, DistributedFollowPositions);
			}
		}
	}

	private void FamilyPositions_OnSpawnFamilyMember(FamilyMemberController familyMemberController)
	{
		followPositions.ForEach(delegate(FollowPos x)
		{
			followPosDatas.Add(new FamilyMemberFollowPosData
			{
				familyMemberController = familyMemberController,
				followPos = x,
				sqrDistance = 0f
			});
		});
		followInvisibilityPositions.ForEach(delegate(FollowPos x)
		{
			invisibilityFollowPosDatas.Add(new FamilyMemberFollowPosData
			{
				familyMemberController = familyMemberController,
				followPos = x,
				sqrDistance = 0f
			});
		});
		followInPursuitPositions.ForEach(delegate(FollowPos x)
		{
			pursuitFollowPosDatas.Add(new FamilyMemberFollowPosData
			{
				familyMemberController = familyMemberController,
				followPos = x,
				sqrDistance = 0f
			});
		});
		FamilyPositions_Update();
	}

	public Vector3 GetFollowPosition(FamilyMemberController familyMemberController)
	{
		if (ManagerBase<PlayerManager>.Instance.Invisibility.IsActive)
		{
			FollowPos followPos = DistributedInvisibilityFollowPositions.Find((FollowPos x) => x.owner == familyMemberController);
			if (followPos != null)
			{
				return base.transform.position + base.transform.rotation * followPos.localPos;
			}
			return familyMemberController.transform.position;
		}
		if (PlayerMovement.IsHelpInNavigationEnabled)
		{
			FollowPos followPos2 = DistributedPursuitFollowPositions.Find((FollowPos x) => x.owner == familyMemberController);
			if (followPos2 != null)
			{
				return base.transform.position + base.transform.rotation * followPos2.localPos;
			}
			return familyMemberController.transform.position;
		}
		FollowPos followPos3 = DistributedFollowPositions.Find((FollowPos x) => x.owner == familyMemberController);
		if (followPos3 != null)
		{
			return base.transform.position + base.transform.rotation * followPos3.localPos;
		}
		return familyMemberController.transform.position;
	}

	private void MakeFamily_Initialize()
	{
		Avelog.Input.makeChildPressedEvent += OnMakeChildPressed;
		Avelog.Input.makeSpousePressedEvent += OnMakeSpousePressedEvent;
	}

	private void MakeFamily_Destroy()
	{
		Avelog.Input.makeChildPressedEvent -= OnMakeChildPressed;
		Avelog.Input.makeSpousePressedEvent -= OnMakeSpousePressedEvent;
	}

	private void OnMakeChildPressed(string childName)
	{
		if (CanMakeChild() && EnoughCoinsForChild())
		{
			MakeChild(childName);
		}
	}

	private void OnMakeSpousePressedEvent(string spouseName)
	{
		if (CanMakeSpouse())
		{
			MakeSpouse(spouseName);
		}
	}

	public bool CanMakeSpouse()
	{
		bool flag = ManagerBase<PlayerManager>.Instance.Level >= ManagerBase<FamilyManager>.Instance.SpouseLevelNeed;
		bool flag2 = Singleton<SpouseSpawner>.Instance != null && Singleton<SpouseSpawner>.Instance.PotentialSpouse != null && Singleton<SpouseSpawner>.Instance.PotentialSpouse.IsPlayerNear;
		if ((!ManagerBase<FamilyManager>.Instance.HaveSpouse && flag2) & flag)
		{
			return !ManagerBase<PlayerManager>.Instance.isSleeping;
		}
		return false;
	}

	public void MakeSpouse(string spouseName)
	{
		if (CanMakeSpouse())
		{
			FamilyManager.GenderType genderType = (ManagerBase<PlayerManager>.Instance.gender == FamilyManager.GenderType.Male) ? FamilyManager.GenderType.Female : FamilyManager.GenderType.Male;
			FamilyManager.FamilyMemberData familyMember = new FamilyManager.FamilyMemberData
			{
				name = spouseName,
				experience = 0f,
				role = FamilyManager.FamilyMemberRole.Spouse,
				genderType = genderType,
				thirst = ManagerBase<FamilyManager>.Instance.ThirstMaximum
			};
			AddFamilyMember(familyMember);
		}
	}

	public void TestMakeSpouse()
	{
		if (!ManagerBase<FamilyManager>.Instance.HaveSpouse)
		{
			FamilyManager.GenderType genderType = (ManagerBase<PlayerManager>.Instance.gender == FamilyManager.GenderType.Male) ? FamilyManager.GenderType.Female : FamilyManager.GenderType.Male;
			FamilyManager.FamilyMemberData familyMemberData = new FamilyManager.FamilyMemberData
			{
				name = "Spouse",
				experience = 0f,
				role = FamilyManager.FamilyMemberRole.Spouse,
				genderType = genderType,
				thirst = ManagerBase<FamilyManager>.Instance.ThirstMaximum
			};
			ManagerBase<FamilyManager>.Instance.family.Add(familyMemberData);
			SatietyController_OnAddFamilyMember(familyMemberData);
			UpdateEatersList();
			SpawnFamily();
			PlayerFamilyController.addFamilyMemberEvent?.Invoke(familyMemberData);
		}
	}

	private void AddFamilyMember(FamilyManager.FamilyMemberData familyMember)
	{
		ManagerBase<FamilyManager>.Instance.family.Add(familyMember);
		SatietyController_OnAddFamilyMember(familyMember);
		UpdateEatersList();
		SpawnFamily(spawnInFolowingPos: false);
		PlayerFamilyController.addFamilyMemberEvent?.Invoke(familyMember);
		ManagerBase<SaveManager>.Instance.SaveToLocal();
	}

	public bool CanMakeChild()
	{
		bool flag = ManagerBase<PlayerManager>.Instance.Level >= ManagerBase<FamilyManager>.Instance.ChildLevelNeed;
		bool flag2 = ManagerBase<FamilyManager>.Instance.Childs.Count() >= ManagerBase<FamilyManager>.Instance.MaxChilds;
		bool flag3 = ManagerBase<FamilyManager>.Instance.Childs.All((FamilyManager.FamilyMemberData x) => x.role == FamilyManager.FamilyMemberRole.ThirdStageChild);
		if ((ManagerBase<FamilyManager>.Instance.HaveSpouse && flag && !flag2) & flag3)
		{
			return !ManagerBase<PlayerManager>.Instance.isSleeping;
		}
		return false;
	}

	public bool EnoughCoinsForChild()
	{
		return ManagerBase<PlayerManager>.Instance.CurCoins >= ManagerBase<FamilyManager>.Instance.ChildCost;
	}

	public void MakeChild(string childName)
	{
		int num = ManagerBase<FamilyManager>.Instance.Childs.Count();
		bool flag = (num != 0 && num != 2) ? (ManagerBase<FamilyManager>.Instance.family.Find((FamilyManager.FamilyMemberData x) => x.role != FamilyManager.FamilyMemberRole.Spouse).genderType == FamilyManager.GenderType.Female) : (UnityEngine.Random.Range(0f, 1f) <= 0.5f);
		FamilyManager.FamilyMemberData familyMember = new FamilyManager.FamilyMemberData
		{
			name = childName,
			experience = 0f,
			role = FamilyManager.FamilyMemberRole.FirstStageChild,
			genderType = ((!flag) ? FamilyManager.GenderType.Female : FamilyManager.GenderType.Male),
			thirst = ManagerBase<FamilyManager>.Instance.ThirstMaximum
		};
		ManagerBase<PlayerManager>.Instance.ChangeCoins(-ManagerBase<FamilyManager>.Instance.ChildCost);
		AddFamilyMember(familyMember);
	}

	public void TestMakeChild()
	{
		int num = ManagerBase<FamilyManager>.Instance.Childs.Count();
		bool num2 = num >= ManagerBase<FamilyManager>.Instance.MaxChilds;
		FamilyMemberController familyMemberController = familyMemberControllers.Find((FamilyMemberController x) => (x.familyMemberData.role != 0) ? (x.familyMemberData.role == FamilyManager.FamilyMemberRole.SecondStageChild) : true);
		if (!num2 || !(familyMemberController == null))
		{
			if (familyMemberController != null)
			{
				ManagerBase<FamilyManager>.Instance.AddExperience(familyMemberController.familyMemberData.Params.LevelUpExperience - familyMemberController.familyMemberData.experience, familyMemberController.familyMemberData);
				return;
			}
			bool flag = (num != 0 && num != 2) ? (ManagerBase<FamilyManager>.Instance.family.Find((FamilyManager.FamilyMemberData x) => x.role != FamilyManager.FamilyMemberRole.Spouse).genderType == FamilyManager.GenderType.Female) : (UnityEngine.Random.Range(0f, 1f) <= 0.5f);
			FamilyManager.FamilyMemberData familyMemberData = new FamilyManager.FamilyMemberData
			{
				name = "child",
				experience = 0f,
				role = FamilyManager.FamilyMemberRole.FirstStageChild,
				genderType = ((!flag) ? FamilyManager.GenderType.Female : FamilyManager.GenderType.Male),
				thirst = ManagerBase<FamilyManager>.Instance.ThirstMaximum
			};
			ManagerBase<FamilyManager>.Instance.family.Add(familyMemberData);
			SatietyController_OnAddFamilyMember(familyMemberData);
			UpdateEatersList();
			SpawnFamily();
			PlayerFamilyController.addFamilyMemberEvent?.Invoke(familyMemberData);
		}
	}

	private void SpawnFamily(bool spawnInFolowingPos = true)
	{
		foreach (FamilyManager.FamilyMemberData familyMember in ManagerBase<FamilyManager>.Instance.family)
		{
			FamilyMemberController x2 = familyMemberControllers.Find((FamilyMemberController x) => x.familyMemberData == familyMember);
			if (!(x2 != null))
			{
				x2 = UnityEngine.Object.Instantiate(familyMemberPrefab, base.transform.position, base.transform.rotation, Singleton<PlayerSpawner>.Instance.SpawnedObjsParent).GetComponent<FamilyMemberController>();
				x2.familyMemberData = familyMember;
				x2.spawnInFolowingPos = spawnInFolowingPos;
				familyMemberControllers.Add(x2);
				FamilyPositions_OnSpawnFamilyMember(x2);
				SatietyController_OnSpawnFamilyMember(x2);
			}
		}
	}

	private void Attack_Initialize()
	{
		PlayerCombat.hitEvent += OnPlayerHitEnemy;
		PlayerCombat.takeDamageEvent += OnTakeDamage;
	}

	private void Attack_Destroy()
	{
		PlayerCombat.hitEvent -= OnPlayerHitEnemy;
		PlayerCombat.takeDamageEvent -= OnTakeDamage;
		if (FamilyAttackTarget != null)
		{
			FamilyAttackTarget.changeLifeStateEvent -= OnFamilyAttackTargetChangeLifeState;
		}
	}

	private void OnFamilyAttackTargetChangeLifeState(ActorCombat.LifeState state)
	{
		if (state != 0)
		{
			UpdateAttackTarget();
		}
	}

	private void OnTakeDamage(ActorCombat.TakeDamageType takeDamageType, float damage, ActorCombat attacker)
	{
		if (takeDamageType != ActorCombat.TakeDamageType.LoseHealth)
		{
			Pursuer pursuer = pursuers.Find((Pursuer x) => x.actorCombat == attacker);
			if (pursuer == null)
			{
				EnemyController componentInParent = attacker.GetComponentInParent<EnemyController>();
				pursuer = new Pursuer
				{
					enemyController = componentInParent,
					actorCombat = attacker
				};
				pursuers.Add(pursuer);
			}
			pursuer.takeDamageTime = Time.time;
		}
	}

	private void OnPlayerHitEnemy(ActorCombat attacker, ActorCombat target)
	{
		Pursuer pursuer = pursuers.Find((Pursuer x) => x.actorCombat == target);
		EnemyController enemyController = (pursuer != null) ? pursuer.enemyController : target.GetComponentInParent<EnemyController>();
		bool flag = enemyController.CurState == EnemyController.State.Attack || enemyController.CurState == EnemyController.State.Pursuit;
		if (pursuer == null)
		{
			if (flag)
			{
				pursuer = new Pursuer
				{
					enemyController = enemyController,
					actorCombat = target
				};
				pursuers.Add(pursuer);
			}
		}
		else if (!flag)
		{
			pursuers.Remove(pursuer);
		}
		if (flag)
		{
			pursuer.attackTime = Time.time;
		}
	}

	private void Attack_Update()
	{
		pursuers.RemoveAll((Pursuer x) => x.enemyController.CurState != EnemyController.State.Attack && x.enemyController.CurState != EnemyController.State.Pursuit);
		pursuers.RemoveAll((Pursuer x) => x.actorCombat.CurLifeState != ActorCombat.LifeState.Alive);
		if (ManagerBase<FamilyManager>.Instance.HaveFamily)
		{
			updateAttackTimer -= Time.deltaTime;
			if (updateAttackTimer <= 0f)
			{
				UpdateAttackTarget();
			}
		}
	}

	private void UpdateAttackTarget()
	{
		updateAttackTimer = updateAttackFrequency;
		if (pursuers.Count > 0)
		{
			List<Pursuer> list = pursuers.FindAll((Pursuer x) => x.actorCombat.CurLifeState == ActorCombat.LifeState.Alive && Time.time - x.takeDamageTime < ManagerBase<FamilyManager>.Instance.CounterAttackTimer);
			if (list.Count > 0)
			{
				if (list.Any((Pursuer x) => x.attackTime != 0f))
				{
					list.Sort((Pursuer x, Pursuer y) => x.attackTime.CompareTo(y.attackTime));
					FamilyAttackTarget = list[list.Count - 1].actorCombat;
				}
				else if (!(FamilyAttackTarget != null) || !list.Any((Pursuer x) => x.actorCombat == FamilyAttackTarget))
				{
					list.Sort((Pursuer x, Pursuer y) => x.takeDamageTime.CompareTo(y.takeDamageTime));
					FamilyAttackTarget = list[list.Count - 1].actorCombat;
				}
			}
			else
			{
				List<Pursuer> list2 = (from x in pursuers
					where x.actorCombat.CurLifeState == ActorCombat.LifeState.Alive && x.attackTime != 0f
					select x).ToList();
				if (list2.Count > 0)
				{
					list2.Sort((Pursuer x, Pursuer y) => x.attackTime.CompareTo(y.attackTime));
					FamilyAttackTarget = list2[list2.Count - 1].actorCombat;
				}
				else
				{
					FamilyAttackTarget = null;
				}
			}
		}
		else if (EnemyController.Instances.Any((EnemyController x) => x.CurState == EnemyController.State.Escape))
		{
			List<(EnemyController enemy, float sqrDistance)> enemiesDinstance = new List<(EnemyController, float)>();
			EnemyController.Instances.FindAll((EnemyController x) => x.CurState == EnemyController.State.Escape).ForEach(delegate(EnemyController enemy)
			{
				enemiesDinstance.Add((enemy, (enemy.EnemyModel.transform.position - base.transform.position).sqrMagnitude));
			});
			enemiesDinstance.RemoveAll(((EnemyController enemy, float sqrDistance) x) => x.sqrDistance > ManagerBase<FamilyManager>.Instance.PursuitRadius * ManagerBase<FamilyManager>.Instance.PursuitRadius);
			if (enemiesDinstance.Count > 0)
			{
				enemiesDinstance.Sort(((EnemyController enemy, float sqrDistance) x, (EnemyController enemy, float sqrDistance) y) => x.sqrDistance.CompareTo(y.sqrDistance));
				FamilyAttackTarget = enemiesDinstance[0].enemy.MyCombat;
			}
			else
			{
				FamilyAttackTarget = null;
			}
		}
		else
		{
			FamilyAttackTarget = null;
		}
	}

	private void SatietyController_Initialize()
	{
		UpdateEatersList();
		FamilyManager.stageUpEvent += SatietyController_OnStageUp;
	}

	private void SatietyController_Destroy()
	{
		FamilyManager.stageUpEvent -= SatietyController_OnStageUp;
		familyMemberControllers.ForEach(delegate(FamilyMemberController x)
		{
			if (x != null)
			{
				x.Combat.hitEvent -= SatietyController_OnHit;
			}
		});
	}

	private void SatietyController_OnStageUp(FamilyManager.FamilyMemberData familyMemberData)
	{
		UpdateEatersList();
	}

	private void SatietyController_OnSpawnFamilyMember(FamilyMemberController familyMemberController)
	{
		familyMemberController.Combat.hitEvent += SatietyController_OnHit;
	}

	private void SatietyController_OnAddFamilyMember(FamilyManager.FamilyMemberData familyMember)
	{
		FamilyManager.FamilyMemberParams familyMemberParams = ManagerBase<FamilyManager>.Instance.FamilyMembersParams.Find((FamilyManager.FamilyMemberParams x) => x.Type == familyMember.role);
		float value = ManagerBase<PlayerManager>.Instance.SatietyMaximum * (familyMemberParams.SatietyMaximumPerc / 100f);
		ChangeSatiety(value);
	}

	private void SatietyController_OnHit(ActorCombat attacker, ActorCombat target)
	{
		FamilyMemberController familyMemberController = familyMemberControllers.Find((FamilyMemberController x) => x.Combat == attacker);
		float num = ManagerBase<PlayerManager>.Instance.SatietyHitFallPerc * ManagerBase<FamilyManager>.Instance.familySatiety * familyMemberController.familyMemberData.HitPowerPerc / 100f;
		ChangeSatiety(0f - num);
	}

	private void SatietyController_Update()
	{
		if (Time.deltaTime != 0f && ManagerBase<FamilyManager>.Instance.family.Count > 0)
		{
			float value = (0f - ManagerBase<FamilyManager>.Instance.SatietyFall) / 100f * ManagerBase<FamilyManager>.Instance.FamilySatietyMaximum * Time.deltaTime;
			ChangeSatiety(value);
		}
	}

	private void UpdateEatersList()
	{
		foreach (FamilyManager.FamilyMemberData familyMember in ManagerBase<FamilyManager>.Instance.family)
		{
			FamilyMemberSatiety familyMemberSatiety = eaten.Find((FamilyMemberSatiety x) => x.familyMember == familyMember);
			if (familyMemberSatiety == null)
			{
				familyMemberSatiety = new FamilyMemberSatiety
				{
					familyMember = familyMember,
					value = 0f
				};
				eaten.Add(familyMemberSatiety);
			}
			FamilyMemberSatiety familyMemberSatiety2 = needToEat.Find((FamilyMemberSatiety x) => x.familyMember == familyMember);
			FamilyManager.FamilyMemberParams familyMemberParams = ManagerBase<FamilyManager>.Instance.FamilyMembersParams.Find((FamilyManager.FamilyMemberParams x) => x.Type == familyMember.role);
			if (familyMemberSatiety2 == null)
			{
				familyMemberSatiety2 = new FamilyMemberSatiety
				{
					familyMember = familyMember,
					value = ManagerBase<PlayerManager>.Instance.SatietyMaximum * (familyMemberParams.SatietyMaximumPerc / 100f)
				};
				needToEat.Add(familyMemberSatiety2);
			}
			else
			{
				familyMemberSatiety2.value = ManagerBase<PlayerManager>.Instance.SatietyMaximum * (familyMemberParams.SatietyMaximumPerc / 100f);
			}
		}
	}

	private float ChangeSatiety(float value)
	{
		float familySatiety = ManagerBase<FamilyManager>.Instance.familySatiety;
		ManagerBase<FamilyManager>.Instance.familySatiety = Mathf.Clamp(ManagerBase<FamilyManager>.Instance.familySatiety + value, 0f, ManagerBase<FamilyManager>.Instance.FamilySatietyMaximum);
		familySatiety = ManagerBase<FamilyManager>.Instance.familySatiety - familySatiety;
		PlayerFamilyController.familyChangeSatietyEvent?.Invoke();
		return familySatiety;
	}

	public bool CanEat()
	{
		if (PlayerEating.HaveAvailableFamilyFood)
		{
			return ManagerBase<FamilyManager>.Instance.HaveFamily;
		}
		return false;
	}

	public void Eat(Action endEatingCallback)
	{
		if (!CanEat())
		{
			return;
		}
		Food foodToEat = PlayerEating.GetNearestFreeFood(onlyGoodFood: true);
		if (foodToEat == null)
		{
			return;
		}
		if (PlayerSpawner.PlayerInstance.PlayerPicker.PickedItem == foodToEat)
		{
			PlayerSpawner.PlayerInstance.PlayerPicker.Drop();
		}
		FamilyManager.FamilyMemberData selectedFamilyMember = null;
		List<(FamilyManager.FamilyMemberData, float)> list = new List<(FamilyManager.FamilyMemberData, float)>();
		for (int i = 0; i < ManagerBase<FamilyManager>.Instance.family.Count; i++)
		{
			float item = eaten[i].value / needToEat[i].value;
			list.Add((eaten[i].familyMember, item));
		}
		if (ManagerBase<FamilyManager>.Instance.HaveGrowingChild)
		{
			FamilyMemberController familyMemberController = familyMemberControllers.Find((FamilyMemberController x) => x.familyMemberData == ManagerBase<FamilyManager>.Instance.GrowingChild);
			if (familyMemberController.IsNeededFood(foodToEat) && !familyMemberController.HaveFoodInEatQueue(foodToEat))
			{
				selectedFamilyMember = ManagerBase<FamilyManager>.Instance.GrowingChild;
			}
		}
		if (selectedFamilyMember == null)
		{
			list.Sort((Comparison<(FamilyManager.FamilyMemberData, float)>)(((FamilyManager.FamilyMemberData familyMember, float eatenRatio) x, (FamilyManager.FamilyMemberData familyMember, float eatenRatio) y) => x.eatenRatio.CompareTo(y.eatenRatio)));
			selectedFamilyMember = list[0].Item1;
		}
		if (((IEnumerable<(FamilyManager.FamilyMemberData, float)>)list).All((Func<(FamilyManager.FamilyMemberData, float), bool>)(((FamilyManager.FamilyMemberData familyMember, float eatenRatio) x) => x.eatenRatio >= 1f)))
		{
			for (int j = 0; j < ManagerBase<FamilyManager>.Instance.family.Count; j++)
			{
				eaten[j].value -= needToEat[j].value;
			}
		}
		eaten.Find((FamilyMemberSatiety x) => x.familyMember == selectedFamilyMember).value += foodToEat.FoodEffect;
		familyMemberControllers.Find((FamilyMemberController x) => x.familyMemberData == selectedFamilyMember).Eat(foodToEat, delegate
		{
			OnFamilyMemberEndEat(foodToEat);
			endEatingCallback?.Invoke();
		});
	}

	private void OnFamilyMemberEndEat(Food food)
	{
		float satietyInc = ChangeSatiety(food.FoodEffect);
		this.familyMemberEndEatEvent?.Invoke(satietyInc, food.FoodEffect);
	}

	public void Drink(Vector3 dirToNearestWaterPoint, Water water)
	{
		if (water.HaveDrinkPoints)
		{
			List<Water.DrinkPoint> list = new List<Water.DrinkPoint>();
			foreach (Water.DrinkPoint drinkPoint in water.DrinkPoints)
			{
				if ((base.transform.position - drinkPoint.point.position).IsLonger(ManagerBase<PlayerManager>.Instance.ObstacleValue))
				{
					list.Add(drinkPoint);
				}
			}
			List<(FamilyMemberController, Water.DrinkPoint, float)> list2 = new List<(FamilyMemberController, Water.DrinkPoint, float)>();
			foreach (FamilyMemberController familyMemberController in familyMemberControllers)
			{
				foreach (Water.DrinkPoint item in list)
				{
					list2.Add((familyMemberController, item, (familyMemberController.transform.position - item.point.position).sqrMagnitude));
				}
			}
			list2.Sort((Comparison<(FamilyMemberController, Water.DrinkPoint, float)>)(((FamilyMemberController familyMemberController, Water.DrinkPoint waterDrinkPoint, float sqrDistance) x, (FamilyMemberController familyMemberController, Water.DrinkPoint waterDrinkPoint, float sqrDistance) y) => x.sqrDistance.CompareTo(y.sqrDistance)));
			while (list2.Count != 0)
			{
				(FamilyMemberController familyMemberController, Water.DrinkPoint waterDrinkPoint, float sqrDistance) nearestPoint = list2[0];
				list2.RemoveAll((Predicate<(FamilyMemberController, Water.DrinkPoint, float)>)(((FamilyMemberController familyMemberController, Water.DrinkPoint waterDrinkPoint, float sqrDistance) x) => (!(x.familyMemberController == nearestPoint.familyMemberController)) ? (x.waterDrinkPoint.point.position == nearestPoint.waterDrinkPoint.point.position) : true));
				nearestPoint.familyMemberController.Drink(nearestPoint.waterDrinkPoint.point.position, FamilyMemberController.DrinkType.FromDrinkPosition, water);
			}
			return;
		}
		Vector3 familyDrinkPoint = base.transform.position;
		for (float num = ManagerBase<FamilyManager>.Instance.DrinkDistanceFromPlayer; num > ManagerBase<PlayerManager>.Instance.ObstacleValue; num -= 0.5f)
		{
			familyDrinkPoint = base.transform.position + dirToNearestWaterPoint.normalized * num;
			if (NavMeshUtils.SamplePositionIterative(familyDrinkPoint, out NavMeshHit navMeshHit, 2f, 100f, 10, -1) && water.Collider.ClosestPoint(navMeshHit.position) == navMeshHit.position)
			{
				break;
			}
		}
		familyMemberControllers.ForEach(delegate(FamilyMemberController x)
		{
			x.Drink(familyDrinkPoint, FamilyMemberController.DrinkType.FromNearestWaterPoint, water);
		});
	}

	public void Invisibility()
	{
		familyMemberControllers.ForEach(delegate(FamilyMemberController x)
		{
			x.SwitchInvisibility(isEnabled: true);
		});
	}

	public void CancelInvisibility()
	{
		familyMemberControllers.ForEach(delegate(FamilyMemberController x)
		{
			x.SwitchInvisibility(isEnabled: false);
		});
	}

	private void Sleep_Initialize()
	{
		sleepLocalPositions.Add(sleepPos1.localPosition);
		sleepLocalPositions.Add(sleepPos2.localPosition);
		sleepLocalPositions.Add(MirrorPosByForward(sleepPos1.localPosition));
		sleepLocalPositions.Add(MirrorPosByForward(sleepPos2.localPosition));
	}

	public bool FamilyCanSleep()
	{
		return familyMemberControllers.All((FamilyMemberController x) => x.CanSleep());
	}

	public void Sleep()
	{
		List<(FamilyMemberController controller, Vector3 sleepPos, float sqrDistance)> distancesToSleepPositions = new List<(FamilyMemberController, Vector3, float)>();
		List<Vector3> sleepWorldPositions = new List<Vector3>();
		sleepLocalPositions.ForEach(delegate(Vector3 x)
		{
			sleepWorldPositions.Add(base.transform.position + base.transform.rotation * x);
		});
		familyMemberControllers.ForEach(delegate(FamilyMemberController controller)
		{
			sleepWorldPositions.ForEach(delegate(Vector3 sleepPos)
			{
				distancesToSleepPositions.Add((controller, sleepPos, (controller.transform.position - sleepPos).sqrMagnitude));
			});
		});
		distancesToSleepPositions.Sort(((FamilyMemberController controller, Vector3 sleepPos, float sqrDistance) x, (FamilyMemberController controller, Vector3 sleepPos, float sqrDistance) y) => x.sqrDistance.CompareTo(y.sqrDistance));
		List<FamilyMemberController> list = new List<FamilyMemberController>();
		while (list.Count != familyMemberControllers.Count)
		{
			(FamilyMemberController controller, Vector3 sleepPos, float sqrDistance) lowestDistance = distancesToSleepPositions[0];
			lowestDistance.controller.Sleep(lowestDistance.sleepPos);
			list.Add(lowestDistance.controller);
			distancesToSleepPositions.RemoveAll(((FamilyMemberController controller, Vector3 sleepPos, float sqrDistance) x) => (!(x.controller == lowestDistance.controller)) ? (x.sleepPos == lowestDistance.sleepPos) : true);
		}
	}

	public Vector3 GetPositionToSleep(FamilyMemberController familyMember)
	{
		List<(Vector3 sleepPos, float sqrDistance)> distancesToSleepPositions = new List<(Vector3, float)>();
		List<Vector3> sleepWorldPositions = new List<Vector3>();
		sleepLocalPositions.ForEach(delegate(Vector3 x)
		{
			sleepWorldPositions.Add(base.transform.position + base.transform.rotation * x);
		});
		sleepWorldPositions.ForEach(delegate(Vector3 sleepPos)
		{
			distancesToSleepPositions.Add((sleepPos, (familyMember.transform.position - sleepPos).sqrMagnitude));
		});
		distancesToSleepPositions.Sort(((Vector3 sleepPos, float sqrDistance) x, (Vector3 sleepPos, float sqrDistance) y) => x.sqrDistance.CompareTo(y.sqrDistance));
		return distancesToSleepPositions[0].sleepPos;
	}

	public void AwakeFamily()
	{
		foreach (FamilyMemberController familyMemberController in familyMemberControllers)
		{
			if (familyMemberController.IsSleeping)
			{
				familyMemberController.AwakeFamilyMember();
			}
		}
	}

	[CompilerGenerated]
	private void _003CFamilyPositions_Update_003Eg__DistributeFollowPositions_007C37_0(List<FamilyMemberFollowPosData> followPosDatas, List<FollowPos> distributedFollowPositions)
	{
		_003C_003Ec__DisplayClass37_0 _003C_003Ec__DisplayClass37_ = default(_003C_003Ec__DisplayClass37_0);
		_003C_003Ec__DisplayClass37_.distributedFollowPositions = distributedFollowPositions;
		_003C_003Ec__DisplayClass37_.distributedFollowPositions.ForEach(delegate(FollowPos x)
		{
			x.owner = null;
		});
		_003C_003Ec__DisplayClass37_.distributedFollowPositions.Clear();
		for (int i = 0; i < followPosDatas.Count; i++)
		{
			followPosDatas[i].sqrDistance = (followPosDatas[i].familyMemberController.transform.position - (base.transform.position + base.transform.rotation * followPosDatas[i].followPos.localPos)).sqrMagnitude;
		}
		bool flag = familyMemberControllers.Any((FamilyMemberController x) => x.familyMemberData == ManagerBase<FamilyManager>.Instance.GrowingChild);
		if (ManagerBase<FamilyManager>.Instance.HaveGrowingChild && flag)
		{
			FamilyMemberFollowPosData familyMemberFollowPosData = _003CFamilyPositions_Update_003Eg__FindNearestFollowPos_007C37_1(followPosDatas, ManagerBase<FamilyManager>.Instance.GrowingChild, closeToPlayer: true);
			familyMemberFollowPosData.followPos.owner = familyMemberControllers.Find((FamilyMemberController x) => x.familyMemberData == ManagerBase<FamilyManager>.Instance.GrowingChild);
			_003C_003Ec__DisplayClass37_.distributedFollowPositions.Add(familyMemberFollowPosData.followPos);
		}
		followPosDatas.Sort((FamilyMemberFollowPosData x, FamilyMemberFollowPosData y) => x.sqrDistance.CompareTo(y.sqrDistance));
		bool flag2 = familyMemberControllers.Count <= 2;
		_003C_003Ec__DisplayClass37_1 _003C_003Ec__DisplayClass37_2 = default(_003C_003Ec__DisplayClass37_1);
		for (int j = 0; j < followPosDatas.Count; j++)
		{
			_003C_003Ec__DisplayClass37_2.followPosData = followPosDatas[j];
			if ((!flag2 || _003C_003Ec__DisplayClass37_2.followPosData.followPos.isCloseToPlayer) && _003C_003Ec__DisplayClass37_2.followPosData.followPos.owner == null && !_003CFamilyPositions_Update_003Eg__HaveDistributedPosition_007C37_6(_003C_003Ec__DisplayClass37_2.followPosData.familyMemberController, ref _003C_003Ec__DisplayClass37_, ref _003C_003Ec__DisplayClass37_2))
			{
				_003C_003Ec__DisplayClass37_2.followPosData.followPos.owner = _003C_003Ec__DisplayClass37_2.followPosData.familyMemberController;
				_003C_003Ec__DisplayClass37_.distributedFollowPositions.Add(_003C_003Ec__DisplayClass37_2.followPosData.followPos);
			}
		}
	}

	[CompilerGenerated]
	private static bool _003CFamilyPositions_Update_003Eg__HaveDistributedPosition_007C37_6(FamilyMemberController familyMemberController, ref _003C_003Ec__DisplayClass37_0 P_1, ref _003C_003Ec__DisplayClass37_1 P_2)
	{
		for (int i = 0; i < P_1.distributedFollowPositions.Count; i++)
		{
			if (P_1.distributedFollowPositions[i].owner == P_2.followPosData.familyMemberController)
			{
				return true;
			}
		}
		return false;
	}

	[CompilerGenerated]
	private static FamilyMemberFollowPosData _003CFamilyPositions_Update_003Eg__FindNearestFollowPos_007C37_1(List<FamilyMemberFollowPosData> followPosDatas, FamilyManager.FamilyMemberData familyMemberData, bool closeToPlayer)
	{
		FamilyMemberFollowPosData nearestPosData = null;
		followPosDatas.ForEach(delegate(FamilyMemberFollowPosData followPosData)
		{
			if (followPosData.familyMemberController.familyMemberData == familyMemberData && (!closeToPlayer || followPosData.followPos.isCloseToPlayer) && (nearestPosData == null || followPosData.sqrDistance < nearestPosData.sqrDistance))
			{
				nearestPosData = followPosData;
			}
		});
		return nearestPosData;
	}
}
