using Avelog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour, IInitializablePlayerComponent
{
	public delegate void MoveHandler(Vector3 moveDirectionPerFrame);

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Func<PlayerManager.SurfaceTag, bool> _003C_003E9__1_0;

		public static Func<PlayerManager.SurfaceTag, float> _003C_003E9__1_1;

		public static Func<PlayerManager.SurfaceTag, bool> _003C_003E9__78_0;

		public static Func<PlayerManager.SurfaceTag, float> _003C_003E9__78_1;

		public static Func<PlayerManager.SurfaceTag, bool> _003C_003E9__78_2;

		public static Func<PlayerManager.SurfaceTag, float> _003C_003E9__78_3;

		public static Func<PlayerManager.SurfaceTag, bool> _003C_003E9__78_4;

		public static Func<PlayerManager.SurfaceTag, bool> _003C_003E9__78_5;

		public static Func<PlayerManager.SurfaceTag, float> _003C_003E9__78_6;

		public static Func<PlayerManager.SurfaceTag, float> _003C_003E9__78_9;

		public static Action<FamilyMemberController> _003C_003E9__89_0;

		internal bool _003Cget_MaxMoveSpeed_003Eb__1_0(PlayerManager.SurfaceTag x)
		{
			return x.slowing;
		}

		internal float _003Cget_MaxMoveSpeed_003Eb__1_1(PlayerManager.SurfaceTag x)
		{
			return x.SlowingPart;
		}

		internal bool _003CApplyMove_003Eb__78_0(PlayerManager.SurfaceTag x)
		{
			return x.slide;
		}

		internal float _003CApplyMove_003Eb__78_1(PlayerManager.SurfaceTag x)
		{
			return x.SlideAngleMinimum;
		}

		internal bool _003CApplyMove_003Eb__78_2(PlayerManager.SurfaceTag x)
		{
			return x.slide;
		}

		internal float _003CApplyMove_003Eb__78_3(PlayerManager.SurfaceTag x)
		{
			return x.SlideSpeed;
		}

		internal bool _003CApplyMove_003Eb__78_4(PlayerManager.SurfaceTag x)
		{
			return x.slide;
		}

		internal bool _003CApplyMove_003Eb__78_5(PlayerManager.SurfaceTag x)
		{
			return x.slide;
		}

		internal float _003CApplyMove_003Eb__78_6(PlayerManager.SurfaceTag x)
		{
			return x.SlideAngleMinimum;
		}

		internal float _003CApplyMove_003Eb__78_9(PlayerManager.SurfaceTag x)
		{
			return x.AlignMaxAngle;
		}

		internal void _003CTeleport_003Eb__89_0(FamilyMemberController x)
		{
			x.HandlePlayerOutOfBounds();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass90_0
	{
		public ControllerColliderHit hit;

		internal bool _003COnControllerColliderHit_003Eb__0(PlayerManager.SurfaceTag x)
		{
			return hit.collider.CompareTag(x.tag);
		}
	}

	[Header("Ссылки")]
	[SerializeField]
	private PlayerCombat playerCombat;

	[SerializeField]
	private CharacterController charController;

	[SerializeField]
	private GameObject jumpBox;

	[SerializeField]
	private NavMeshObstacle navMeshObstacle;

	private Coroutine jumpingCor;

	private List<ControllerColliderHit> collisions = new List<ControllerColliderHit>();

	private List<PlayerManager.SurfaceTag> collisionSurfaceTags = new List<PlayerManager.SurfaceTag>();

	private List<Vector3> alignGroundNormals = new List<Vector3>();

	private float alignGroundNormalAngle;

	private Vector3 groundNormal = Vector3.up;

	private float groundNormalAngle;

	private float jumpCooldown;

	private bool isJumpHeight;

	private int jumpFrame = int.MinValue;

	private Collider[] jumpPlatformCols = new Collider[4];

	private float startSlopeLimit;

	private Vector3 moveDirection = Vector3.zero;

	private float jumpMoveSpeedBoost;

	private float moveInput;

	private float hitSlowTimer;

	private EnemyController observedEnemy;

	public float MaxMoveSpeed
	{
		get
		{
			float num = 0f;
			if (playerCombat.InCombat)
			{
				num = Mathf.Lerp(ManagerBase<PlayerManager>.Instance.SpeedMedium, ManagerBase<PlayerManager>.Instance.SpeedMaximum, playerCombat.LargerZoneInfluencePart);
			}
			else if (playerCombat.IsInvisibilityActive())
			{
				num = ManagerBase<PlayerManager>.Instance.SpeedMinimum;
				if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Bombay)
				{
					num += CalculationsHelpUtils.CalculateProp(num, ManagerBase<SkinManager>.Instance.BombayStealthMoveSpeedBonus, 100f);
				}
			}
			else
			{
				num = ManagerBase<PlayerManager>.Instance.SpeedMedium;
			}
			if (collisionSurfaceTags.Any((PlayerManager.SurfaceTag x) => x.slowing))
			{
				num *= 1f - collisionSurfaceTags.Min((PlayerManager.SurfaceTag x) => x.SlowingPart);
			}
			if (playerCombat.HavePursuitedEnemy)
			{
				float num2 = Vector3.Distance(base.transform.position, playerCombat.PursuitedEnemy.ModelTransform.position);
				if (num2 <= ManagerBase<PlayerManager>.Instance.SpeedLimitStartingDistance)
				{
					float t = (num2 - ManagerBase<PlayerManager>.Instance.SpeedLimitFinalDistance) / (ManagerBase<PlayerManager>.Instance.SpeedLimitStartingDistance - ManagerBase<PlayerManager>.Instance.SpeedLimitFinalDistance);
					num = Mathf.Lerp(playerCombat.PursuitedEnemy.NavAgent.velocity.magnitude, num, t);
				}
				else if (num2 == ManagerBase<PlayerManager>.Instance.SpeedLimitFinalDistance)
				{
					num = playerCombat.PursuitedEnemy.NavAgent.velocity.magnitude;
				}
			}
			return Mathf.Clamp(num, 0f - ManagerBase<PlayerManager>.Instance.MovementLimit, ManagerBase<PlayerManager>.Instance.MovementLimit);
		}
	}

	private Animator Animator => PlayerSpawner.PlayerInstance.Model.Animator;

	public NavMeshObstacle NavMeshObstacle => navMeshObstacle;

	public Vector3 AlignGroundNormal
	{
		get;
		private set;
	} = Vector3.up;


	public bool IsFalling
	{
		get;
		private set;
	}

	public bool IsSlidingAlongGround
	{
		get;
		private set;
	}

	public float PrevMoveSpeed
	{
		get;
		private set;
	}

	public float CurMoveSpeed
	{
		get
		{
			return ManagerBase<PlayerManager>.Instance.curSpeed;
		}
		private set
		{
			ManagerBase<PlayerManager>.Instance.curSpeed = value;
		}
	}

	private float MaxDistanceToObserving => ManagerBase<PlayerManager>.Instance.MaxDistanceToObserving * ManagerBase<PlayerManager>.Instance.MaxDistanceToObserving;

	public bool IsHelpInNavigationEnabled
	{
		get;
		private set;
	}

	public bool IsMovingOnWater
	{
		get;
		private set;
	}

	public bool IsWalkingAnimationPlaying
	{
		get
		{
			int layerIndex = Animator.GetLayerIndex("Base Layer");
			AnimatorStateInfo currentAnimatorStateInfo = Animator.GetCurrentAnimatorStateInfo(layerIndex);
			AnimatorStateInfo nextAnimatorStateInfo = Animator.GetNextAnimatorStateInfo(layerIndex);
			if (!currentAnimatorStateInfo.IsName("Walking"))
			{
				return nextAnimatorStateInfo.IsName("Walking");
			}
			return true;
		}
	}

	public bool IsRunningAnimationPlaying
	{
		get
		{
			int layerIndex = Animator.GetLayerIndex("Base Layer");
			AnimatorStateInfo currentAnimatorStateInfo = Animator.GetCurrentAnimatorStateInfo(layerIndex);
			AnimatorStateInfo nextAnimatorStateInfo = Animator.GetNextAnimatorStateInfo(layerIndex);
			if (!currentAnimatorStateInfo.IsName("Running"))
			{
				return nextAnimatorStateInfo.IsName("Running");
			}
			return true;
		}
	}

	public event Action jumpEvent;

	public event Action flyingStartEvent;

	public event Action flyingEndEvent;

	public event MoveHandler moveEvent;

	public void Initialize()
	{
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		startSlopeLimit = charController.slopeLimit;
		jumpCooldown = ManagerBase<PlayerManager>.Instance.JumpFrequency;
		playerCombat.changeLifeStateEvent += OnChangeLiveState;
		playerCombat.attackEvent += OnAttack;
	}

	private void OnDestroy()
	{
		playerCombat.changeLifeStateEvent -= OnChangeLiveState;
		playerCombat.attackEvent -= OnAttack;
	}

	private void OnChangeLiveState(ActorCombat.LifeState state)
	{
		if (state != 0)
		{
			moveDirection = new Vector3(0f, moveDirection.y, 0f);
			Animator.SetBool("isFalling", value: false);
		}
	}

	private void OnAttack()
	{
		hitSlowTimer = ManagerBase<PlayerManager>.Instance.HitSlowTime;
	}

	private void Update()
	{
		Vector3 position = base.transform.position;
		ApplyMove();
		Vector3 moveDirectionPerFrame = base.transform.position - position;
		this.moveEvent?.Invoke(moveDirectionPerFrame);
		if (jumpCooldown <= ManagerBase<PlayerManager>.Instance.JumpFrequency)
		{
			jumpCooldown += Time.deltaTime;
		}
		if (hitSlowTimer > 0f)
		{
			hitSlowTimer -= Time.deltaTime;
		}
	}

	private void ApplyMove()
	{
		if (Time.deltaTime == 0f)
		{
			return;
		}
		ApplyJump();
		float num = (0f - ManagerBase<PlayerManager>.Instance.Gravity) * Time.deltaTime;
		IsSlidingAlongGround = (moveDirection.y <= 0f && collisionSurfaceTags.Count > 0 && collisionSurfaceTags.All((PlayerManager.SurfaceTag x) => x.slide) && groundNormalAngle >= collisionSurfaceTags.Max((PlayerManager.SurfaceTag x) => x.SlideAngleMinimum));
		float num2 = IsSlidingAlongGround ? (from x in collisionSurfaceTags
			where x.slide
			select x).Min((PlayerManager.SurfaceTag x) => x.SlideSpeed) : 0f;
		float num3 = collisionSurfaceTags.Any((PlayerManager.SurfaceTag x) => x.slide) ? (from x in collisionSurfaceTags
			where x.slide
			select x).Min((PlayerManager.SurfaceTag x) => x.SlideAngleMinimum) : startSlopeLimit;
		if (charController.slopeLimit != num3)
		{
			charController.slopeLimit = num3;
		}
		collisions.Clear();
		collisionSurfaceTags.Clear();
		if (IsSlidingAlongGround)
		{
			Vector3 zero = Vector3.zero;
			float num4 = 0.1f;
			Vector3 vector = Vector3.Cross(groundNormal, Vector3.up);
			if (vector != Vector3.zero)
			{
				zero = Quaternion.AngleAxis(90f + groundNormalAngle - 0f, -vector) * Vector3.up;
				num4 = Mathf.Clamp(groundNormalAngle / 90f, 0.1f, 1f);
			}
			else
			{
				zero = base.transform.forward;
				num4 = 1f;
			}
			zero.Normalize();
			zero *= num2 * num4;
			charController.Move(zero * Time.deltaTime);
		}
		float y = base.transform.position.y;
		moveDirection.y += num / 2f;
		Vector3 motion = moveDirection * Time.deltaTime;
		charController.Move(motion);
		if (collisions.Count > 0)
		{
			if (moveDirection.y <= 0f)
			{
				float num5 = base.transform.position.y - y;
				if (num5 - moveDirection.y * Time.deltaTime > 0.001f)
				{
					moveDirection.y = Mathf.Clamp(num5 / Time.deltaTime, moveDirection.y, 0f);
				}
			}
			else if (moveDirection.y > 0f)
			{
				float num6 = base.transform.position.y - y;
				if (moveDirection.y * Time.deltaTime - num6 > 0.001f)
				{
					float max = num6 / Time.deltaTime;
					moveDirection.y = Mathf.Clamp(moveDirection.y, moveDirection.y, max);
				}
			}
		}
		moveDirection.y += num / 2f;
		_003CApplyMove_003Eg__CalcGroundNormal_007C78_7();
		for (int i = 0; i < jumpPlatformCols.Length; i++)
		{
			jumpPlatformCols[i] = null;
		}
		Physics.OverlapBoxNonAlloc(jumpBox.transform.position, jumpBox.transform.localScale / 2f, jumpPlatformCols, jumpBox.transform.rotation, 1 << Layers.ColliderLayer);
		isJumpHeight = (jumpPlatformCols.Count((Collider x) => x != null && x.transform != base.transform) > 0 || charController.isGrounded);
		if (playerCombat.CurLifeState != 0)
		{
			return;
		}
		bool isFalling = IsFalling;
		IsFalling = !isJumpHeight;
		Animator.SetBool("isFalling", IsFalling);
		if (IsFalling)
		{
			Animator.SetFloat("vertSpeed", moveDirection.y);
		}
		if (IsFalling != isFalling)
		{
			if (isFalling)
			{
				this.flyingEndEvent?.Invoke();
			}
			else
			{
				this.flyingStartEvent?.Invoke();
			}
		}
	}

	public void CalculateMove(float vertAxis)
	{
		float y = moveDirection.y;
		moveInput = vertAxis;
		bool num = vertAxis > 0f;
		bool flag = vertAxis < 0f;
		float num2 = vertAxis;
		if (num)
		{
			num2 *= MaxMoveSpeed;
		}
		else if (flag)
		{
			num2 *= 0f - ManagerBase<PlayerManager>.Instance.SpeedBackward;
		}
		float num3 = (0f - ManagerBase<PlayerManager>.Instance.SpeedAcceleration) * Time.deltaTime;
		jumpMoveSpeedBoost = _003CCalculateMove_003Eg__CalculateJumpBoost_007C79_0(num3 / 2f);
		if (num)
		{
			num2 = Mathf.Clamp(num2 + jumpMoveSpeedBoost, 0f - ManagerBase<PlayerManager>.Instance.MovementLimit, ManagerBase<PlayerManager>.Instance.MovementLimit);
		}
		jumpMoveSpeedBoost = _003CCalculateMove_003Eg__CalculateJumpBoost_007C79_0(num3 / 2f);
		if (hitSlowTimer > 0f)
		{
			num2 = num2 / 100f * (100f - ManagerBase<PlayerManager>.Instance.HitSlowSpeed);
		}
		PrevMoveSpeed = CurMoveSpeed;
		bool num4 = (num2 > 0f && CurMoveSpeed < 0f) || (num2 < 0f && CurMoveSpeed > 0f);
		Vector3 forward = base.transform.forward;
		if (num4)
		{
			CurMoveSpeed = 0f;
			moveDirection = Vector3.zero;
		}
		if (Mathf.Abs(CurMoveSpeed) < Mathf.Abs(num2))
		{
			float num5 = Mathf.Sign(num2 - CurMoveSpeed);
			MovementUtils.CalculateSpeed(ref ManagerBase<PlayerManager>.Instance.curSpeed, out float curFrameSpeed, Time.deltaTime, num5 * ManagerBase<PlayerManager>.Instance.SpeedAcceleration, Mathf.Min(CurMoveSpeed, num2), Mathf.Max(CurMoveSpeed, num2));
			moveDirection = forward * curFrameSpeed;
		}
		else
		{
			CurMoveSpeed = num2;
			moveDirection = forward * CurMoveSpeed;
		}
		moveDirection.y = y;
		CalculacteAnimationMovementSpeed();
		float num6 = (CurMoveSpeed > 0f) ? (CurMoveSpeed / MaxMoveSpeed) : ((0f - CurMoveSpeed) / ManagerBase<PlayerManager>.Instance.SpeedBackward);
		Animator.SetFloat("NormalizedMovementSpeed", num6);
		CalculateAnimationSmoothSpeed(num6, "SmoothNormalizedMovementSpeed");
	}

	private void CalculacteAnimationMovementSpeed()
	{
		float num = 71f / (678f * (float)Math.PI) * ManagerBase<PlayerManager>.Instance.RotationSpeed;
		float num2 = 0f;
		float num3 = Mathf.Abs(CurMoveSpeed);
		num2 = ((!(num3 > num)) ? Mathf.Clamp(num3 + Mathf.Abs(Animator.GetFloat("NormalizedRotationSpeed")) * num, 0f - num, num) : num3);
		Animator.SetFloat("MovementSpeed", Mathf.Abs(num2));
	}

	public void ResetVelocity()
	{
		CurMoveSpeed = 0f;
		jumpMoveSpeedBoost = 0f;
		moveDirection = Vector3.zero;
		Animator.SetFloat("MovementSpeed", 0f);
	}

	public void Rotate(float horAxis)
	{
		Animator.SetFloat("NormalizedRotationSpeed", 0f);
		TryHelpInNavigation();
		if (IsHelpInNavigationEnabled)
		{
			CalculateAnimationSmoothSpeed(0f, "SmoothNormalizedRotationSpeed");
			return;
		}
		float angle = horAxis * ManagerBase<PlayerManager>.Instance.RotationSpeed * Time.deltaTime;
		Rotate(angle, horAxis);
	}

	public void Rotate(float angle, float normalizedRotationSpeed)
	{
		charController.transform.RotateAround(charController.transform.position, charController.transform.up, angle);
		Animator.SetFloat("NormalizedRotationSpeed", normalizedRotationSpeed);
		CalculateAnimationSmoothSpeed(normalizedRotationSpeed, "SmoothNormalizedRotationSpeed");
		CalculacteAnimationMovementSpeed();
	}

	private void CalculateAnimationSmoothSpeed(float normalizedSpeed, string animatorSmoothSpeedName)
	{
		float @float = Animator.GetFloat(animatorSmoothSpeedName);
		float num = Mathf.Sign(normalizedSpeed - @float);
		float value = Mathf.Clamp(@float + 3f * num * Time.deltaTime, Mathf.Min(@float, normalizedSpeed), Mathf.Max(@float, normalizedSpeed));
		Animator.SetFloat(animatorSmoothSpeedName, value);
	}

	public bool Rotate(Vector3 targetForward, float deltaTime, float speed)
	{
		if (deltaTime == 0f)
		{
			Rotate(0f, 0f);
			return true;
		}
		Quaternion rotation = QuaternionUtils.GetRotation(Vector3.ProjectOnPlane(base.transform.forward, Vector3.up), targetForward, speed * Time.deltaTime);
		if (rotation == Quaternion.identity)
		{
			Rotate(0f, 0f);
			return false;
		}
		float num = EulerUtils.ToLowestAngle(rotation.eulerAngles.y);
		Rotate(num, Mathf.Sign(num));
		return true;
	}

	public void Jump()
	{
		if (jumpCooldown >= ManagerBase<PlayerManager>.Instance.JumpFrequency && !IsSlidingAlongGround)
		{
			List<Collider> list = new List<Collider>(Physics.OverlapBox(jumpBox.transform.position, jumpBox.transform.localScale / 2f, jumpBox.transform.rotation, 1 << Layers.ColliderLayer));
			list.RemoveAll((Collider x) => x.name == charController.transform.name);
			if (list.Count > 0)
			{
				jumpCooldown = 0f;
				jumpFrame = Time.frameCount;
				this.jumpEvent?.Invoke();
			}
		}
	}

	private void TryHelpInNavigation()
	{
		Vector3 point = new Vector3(Avelog.Input.HorAxis, 0f, Avelog.Input.VertAxis);
		point = base.transform.rotation * point;
		if (point == Vector3.zero)
		{
			if (observedEnemy != null)
			{
				if ((observedEnemy.ModelTransform.position - playerCombat.transform.position).sqrMagnitude > MaxDistanceToObserving || (playerCombat.PursuitedEnemy != null && _003CTryHelpInNavigation_003Eg__HaveObstaclesOnView_007C87_0()))
				{
					observedEnemy = null;
					IsHelpInNavigationEnabled = false;
				}
				else if (!playerCombat.AttackTargetChanged)
				{
					Vector3 endDir = Vector3.ProjectOnPlane(observedEnemy.ModelTransform.position - playerCombat.transform.position, Vector3.up);
					base.transform.rotation *= QuaternionUtils.GetRotation(base.transform.forward, endDir, ManagerBase<PlayerManager>.Instance.RotationSpeed * Time.deltaTime);
					IsHelpInNavigationEnabled = true;
				}
			}
			else if (playerCombat.HavePursuitedEnemy && (playerCombat.PursuitedEnemy.ModelTransform.position - playerCombat.transform.position).sqrMagnitude <= MaxDistanceToObserving)
			{
				observedEnemy = playerCombat.PursuitedEnemy;
				IsHelpInNavigationEnabled = true;
			}
			return;
		}
		if (observedEnemy != null)
		{
			observedEnemy = null;
			IsHelpInNavigationEnabled = false;
		}
		if (playerCombat.HavePursuitedEnemy)
		{
			Vector3 normalized = (playerCombat.PursuitedEnemy.ModelTransform.position - playerCombat.transform.position).normalized;
			normalized = new Vector3(normalized.x, 0f, normalized.z);
			float num = Vector3.Angle(normalized, point);
			float num2 = Mathf.Lerp(t: (Vector3.Distance(playerCombat.transform.position, playerCombat.PursuitedEnemy.ModelTransform.position) - ManagerBase<PlayerManager>.Instance.MinDistancePursuitAssistance) / (ManagerBase<PlayerManager>.Instance.MaxDistancePursuitAssistance - ManagerBase<PlayerManager>.Instance.MinDistancePursuitAssistance), a: ManagerBase<PlayerManager>.Instance.MinDistAnglePursuitAssistance, b: ManagerBase<PlayerManager>.Instance.MaxDistAnglePursuitAssistance);
			num2 /= 2f;
			if (num <= num2 && !playerCombat.AttackTargetChanged && playerCombat.PursuitedEnemy != null && !_003CTryHelpInNavigation_003Eg__HaveObstaclesOnView_007C87_0())
			{
				Vector3 endDir2 = Vector3.ProjectOnPlane(playerCombat.PursuitedEnemy.ModelTransform.position - playerCombat.transform.position, Vector3.up);
				base.transform.rotation *= QuaternionUtils.GetRotation(base.transform.forward, endDir2, ManagerBase<PlayerManager>.Instance.RotationSpeed * Time.deltaTime);
				IsHelpInNavigationEnabled = true;
			}
			else
			{
				IsHelpInNavigationEnabled = false;
			}
		}
		else
		{
			IsHelpInNavigationEnabled = false;
		}
	}

	private void ApplyJump()
	{
		if (jumpFrame + 1 != Time.frameCount)
		{
			return;
		}
		moveDirection.y = ManagerBase<PlayerManager>.Instance.JumpPower;
		if (CurMoveSpeed > 0f)
		{
			CurMoveSpeed += ManagerBase<PlayerManager>.Instance.JumpSpeedBoost;
			CurMoveSpeed = Mathf.Clamp(CurMoveSpeed, 0f - ManagerBase<PlayerManager>.Instance.SpeedBackward, Mathf.Clamp(MaxMoveSpeed + ManagerBase<PlayerManager>.Instance.JumpSpeedBoost, 0f, ManagerBase<PlayerManager>.Instance.MovementLimit));
			if (CurMoveSpeed > MaxMoveSpeed)
			{
				jumpMoveSpeedBoost = CurMoveSpeed - MaxMoveSpeed;
			}
			else
			{
				jumpMoveSpeedBoost = ManagerBase<PlayerManager>.Instance.JumpSpeedBoost;
			}
		}
	}

	public void Teleport(Vector3 position, Quaternion rotation, bool updateCamInstant = true, bool resetVelocity = true, bool teleportFamily = true)
	{
		charController.enabled = false;
		charController.transform.position = position;
		charController.transform.rotation = rotation;
		charController.enabled = true;
		IsMovingOnWater = false;
		PlayerSpawner.PlayerInstance.PlayerGraphic.UpdateGraphicTransform();
		PlayerSpawner.PlayerInstance.PlayerCamera.UpdateCamInstant();
		PlayerSpawner.PlayerInstance.PlayerMovement.ResetVelocity();
		if (PlayerSpawner.PlayerInstance.PlayerCombat.IsInvisibilityActive())
		{
			PlayerSpawner.PlayerInstance.PlayerCombat.SwitchInvisibility();
		}
		PlayerSpawner.PlayerInstance.PlayerFamilyController.FamilyMembersControllers.ForEach(delegate(FamilyMemberController x)
		{
			x.HandlePlayerOutOfBounds();
		});
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		collisions.Add(hit);
		PlayerManager.SurfaceTag surfaceTag = ManagerBase<PlayerManager>.Instance.SurfaceTags.Find((PlayerManager.SurfaceTag x) => hit.collider.CompareTag(x.tag));
		if (surfaceTag != null && !collisionSurfaceTags.Contains(surfaceTag))
		{
			collisionSurfaceTags.Add(surfaceTag);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((other.gameObject.layer & Layers.TriggerLayer) != 0 && other.gameObject.tag == "Water")
		{
			IsMovingOnWater = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((other.gameObject.layer & Layers.TriggerLayer) != 0 && other.gameObject.tag == "Water")
		{
			IsMovingOnWater = false;
		}
	}

	[CompilerGenerated]
	private void _003CApplyMove_003Eg__CalcGroundNormal_007C78_7()
	{
		if (charController.isGrounded && collisions.Count > 0)
		{
			alignGroundNormals.Clear();
			foreach (ControllerColliderHit collision in collisions)
			{
				bool flag = false;
				for (int i = 0; i < collisionSurfaceTags.Count; i++)
				{
					if (collisionSurfaceTags[i].align && collision.collider.CompareTag(collisionSurfaceTags[i].tag))
					{
						flag = true;
					}
				}
				if (flag && !alignGroundNormals.Contains(collision.normal))
				{
					alignGroundNormals.Add(collision.normal);
				}
			}
			AlignGroundNormal = Vector3.zero;
			foreach (Vector3 alignGroundNormal in alignGroundNormals)
			{
				AlignGroundNormal += alignGroundNormal;
			}
			AlignGroundNormal = AlignGroundNormal.normalized;
			if (alignGroundNormals.Count == 0)
			{
				AlignGroundNormal = Vector3.up;
			}
			alignGroundNormalAngle = Vector3.Angle(Vector3.up, AlignGroundNormal);
			float num = (collisionSurfaceTags.Count > 0) ? collisionSurfaceTags.Min((PlayerManager.SurfaceTag x) => x.AlignMaxAngle) : 0f;
			if (alignGroundNormalAngle > num)
			{
				AlignGroundNormal = Vector3.RotateTowards(AlignGroundNormal, Vector3.up, (alignGroundNormalAngle - num) * ((float)Math.PI / 180f), 0f);
				AlignGroundNormal = AlignGroundNormal.normalized;
				alignGroundNormalAngle = num;
			}
			groundNormal = Vector3.zero;
			collisions.ForEach(delegate(ControllerColliderHit x)
			{
				groundNormal += x.normal;
			});
			foreach (ControllerColliderHit collision2 in collisions)
			{
				groundNormal += collision2.normal;
			}
			groundNormal.Normalize();
			groundNormalAngle = Vector3.Angle(Vector3.up, groundNormal);
		}
		else if (!isJumpHeight)
		{
			AlignGroundNormal = Vector3.up;
			alignGroundNormalAngle = 0f;
			groundNormal = Vector3.up;
			groundNormalAngle = 0f;
		}
	}

	[CompilerGenerated]
	private float _003CCalculateMove_003Eg__CalculateJumpBoost_007C79_0(float speedDelta)
	{
		return Mathf.Clamp(jumpMoveSpeedBoost + speedDelta, 0f, ManagerBase<PlayerManager>.Instance.JumpSpeedBoost);
	}

	[CompilerGenerated]
	private bool _003CTryHelpInNavigation_003Eg__HaveObstaclesOnView_007C87_0()
	{
		if (!ManagerBase<PlayerManager>.Instance.UseRaycastToCheckObstacles)
		{
			return false;
		}
		Vector3 direction = playerCombat.PursuitedEnemy.ModelTransform.position + Vector3.up * (playerCombat.PursuitedEnemy.NavAgent.height / 2f) - CameraUtils.PlayerCamera.transform.position;
		return Physics.Raycast(CameraUtils.PlayerCamera.transform.position, direction, direction.magnitude, 1 << Layers.ColliderLayer);
	}
}
