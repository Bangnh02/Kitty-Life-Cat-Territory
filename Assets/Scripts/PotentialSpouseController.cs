using Avelog;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PotentialSpouseController : MonoBehaviour
{
	public delegate void MeetPlayerHandler(bool isMeetingPlayer);

	public delegate void PlayerNeaHandler(bool isPlayerNear);

	[SerializeField]
	private float meetingSpeed = 15f;

	[SerializeField]
	private float meetingDistance = 40f;

	[SerializeField]
	private float meetingStopMoveDistance = 6f;

	[SerializeField]
	private float distanceToEnableSpouseButton = 7f;

	[SerializeField]
	private float rotationSpeed = 180f;

	[Postfix(PostfixAttribute.Id.Percents)]
	[SerializeField]
	private float corneringSlowing = 60f;

	[SerializeField]
	private Vector3 worldPanelOffset = Vector3.zero;

	[SerializeField]
	private NavMeshAgent navAgent;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private CatSkinner catSkinner;

	private Vector3 spawnPoint;

	private Coroutine walkingCor;

	private ProcessingSwitch _processingSwitch;

	private const int sittingAnimationNumber = 0;

	private const int standingAnimationNumber = 1;

	private bool _isMeetingPlayer;

	private bool _isPlayerNear;

	private Vector3 velocity = Vector3.zero;

	private Vector3 prevPos = Vector3.zero;

	private float curSpeed;

	private Coroutine traverseNavLinkCor;

	public static List<PotentialSpouseController> Instances
	{
		get;
		private set;
	} = new List<PotentialSpouseController>();


	public Vector3 WorldPanelOffset => worldPanelOffset;

	private PlayerBrain Player => PlayerSpawner.PlayerInstance;

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

	public bool IsMeetingPlayer
	{
		get
		{
			return _isMeetingPlayer;
		}
		private set
		{
			bool isMeetingPlayer = _isMeetingPlayer;
			_isMeetingPlayer = value;
			if (isMeetingPlayer != _isMeetingPlayer)
			{
				PotentialSpouseController.switchMeetPlayerStateEvent?.Invoke(_isMeetingPlayer);
			}
		}
	}

	public bool IsPlayerNear
	{
		get
		{
			return _isPlayerNear;
		}
		private set
		{
			bool isPlayerNear = _isPlayerNear;
			_isPlayerNear = value;
			if (isPlayerNear != _isPlayerNear)
			{
				PotentialSpouseController.switchPlayerNearEvent?.Invoke(_isPlayerNear);
			}
		}
	}

	public static event Action unspawnEvent;

	public static event MeetPlayerHandler switchMeetPlayerStateEvent;

	public static event PlayerNeaHandler switchPlayerNearEvent;

	public void Spawn(Vector3 spawnPoint)
	{
		this.spawnPoint = spawnPoint;
		ProcessingSwitch.maxProcessingDistance = Singleton<SpouseSpawner>.Instance.ProcessingDistance;
		ProcessingSwitch.UpdateProcessingState();
		animator.SetInteger("IdleType", 0);
		navAgent.angularSpeed = rotationSpeed;
		navAgent.speed = meetingSpeed;
		catSkinner.Initialize();
		if (PlayerSpawner.IsPlayerSpawned)
		{
			walkingCor = StartCoroutine(Walking());
		}
		else
		{
			PlayerSpawner.spawnPlayerEvent += OnSpawnPlayer;
		}
		if (!Instances.Contains(this))
		{
			Instances.Add(this);
		}
	}

	private void OnSpawnPlayer()
	{
		walkingCor = StartCoroutine(Walking());
	}

	private void OnDestroy()
	{
		PlayerSpawner.spawnPlayerEvent -= OnSpawnPlayer;
		if (Instances.Contains(this))
		{
			Instances.Remove(this);
		}
	}

	public void Unspawn()
	{
		if (walkingCor != null)
		{
			StopCoroutine(walkingCor);
		}
		PotentialSpouseController.unspawnEvent?.Invoke();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Update()
	{
		if (Time.deltaTime == 0f)
		{
			return;
		}
		float value = (base.transform.position - prevPos).magnitude / Time.deltaTime;
		animator.SetFloat("MovementSpeed", value);
		prevPos = base.transform.position;
		if (IsMeetingPlayer)
		{
			IsPlayerNear = (Player.PlayerCenter.position - base.transform.position).IsShorterOrEqual(distanceToEnableSpouseButton);
		}
		else
		{
			IsPlayerNear = false;
		}
		if (navAgent.isOnOffMeshLink && traverseNavLinkCor == null)
		{
			traverseNavLinkCor = StartCoroutine(TraverseNavLinkParabola(navAgent, 2f, ManagerBase<FamilyManager>.Instance.JumpTime));
			if (navAgent.isOnNavMesh)
			{
				navAgent.CompleteOffMeshLink();
			}
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
		if (navAgent.isOnNavMesh)
		{
			navAgent.CompleteOffMeshLink();
		}
	}

    private IEnumerator Walking()
    {
        bool idle = true;
        float updateMeetPathTimer = 0.3f;
        float startMeetingTime = Time.time - updateMeetPathTimer;
        WaitForSeconds longWait = new WaitForSeconds(0.3f);

        // ---- Local helpers (mapping 1-1 theo decompile)
        bool IsNeedUpdateMeeting() => Time.time - startMeetingTime > updateMeetPathTimer;

        void MoveToPlayer()
        {
            Vector3 position = Player.PlayerHead.position;
            NavMeshHit hit;
            NavMeshUtils.SamplePositionIterative(position, out hit, 5f, 100f, 5, -1);
            SetDestination(hit.position);
            idle = false;
        }

        void MoveToIdlePos()
        {
            NavMeshHit hit;
            NavMeshUtils.SamplePositionIterative(spawnPoint, out hit, 5f, 100f, 5, -1);
            SetDestination(hit.position);
            idle = true;
        }

        while (true)
        {
            // Đang ở trong vùng "meeting" quanh spawn?
            IsMeetingPlayer = (Player.PlayerCenter.position - spawnPoint).IsShorter(meetingDistance);

            if (!IsMeetingPlayer)
            {
                // Idle type 0 và quay về idle pos nếu đang không ở trạng thái idle
                if (animator.GetInteger("IdleType") != 0)
                    animator.SetInteger("IdleType", 0);

                if (!idle)
                    MoveToIdlePos();

                yield return longWait;
                continue;
            }

            // Trong vùng meeting
            if (animator.GetInteger("IdleType") != 1)
                animator.SetInteger("IdleType", 1);

            if (navAgent.speed != meetingSpeed)
                navAgent.speed = meetingSpeed;

            // Đã đủ gần để dừng di chuyển và xoay đối diện player?
            if ((Player.PlayerHead.position - transform.position).IsShorter(meetingStopMoveDistance) &&
                !navAgent.isOnOffMeshLink)
            {
                if (HasPath())
                    ResetPath();

                if (navAgent.velocity == Vector3.zero)
                {
                    Vector3 endDir = Vector3.ProjectOnPlane(Player.transform.position - transform.position, Vector3.up);
                    transform.rotation *= QuaternionUtils.GetRotation(transform.forward, endDir, rotationSpeed * Time.deltaTime);
                }

                yield return null;
                continue;
            }

            // Cần cập nhật đường đi tới player định kỳ
            if (IsNeedUpdateMeeting())
            {
                startMeetingTime = Time.time;

                // Nếu đang/chuẩn bị ở extraIdle, thoát ra an toàn trước khi move
                if (animator.GetCurrentAnimatorStateInfo(0).tagHash == AnimationHashes.extraIdle.Value)
                {
                    animator.SetTrigger("ExitExtraAnimation");
                    yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).tagHash == AnimationHashes.extraIdle.Value);
                }
                else if (animator.GetNextAnimatorStateInfo(0).tagHash == AnimationHashes.extraIdle.Value)
                {
                    yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).tagHash != AnimationHashes.extraIdle.Value);
                    animator.SetTrigger("ExitExtraAnimation");
                    yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).tagHash == AnimationHashes.extraIdle.Value);
                }

                MoveToPlayer();
            }

            yield return null;
        }
    }

    private bool HasPath()
	{
		if (!navAgent.pathPending && (!navAgent.hasPath || !(navAgent.remainingDistance > navAgent.stoppingDistance)))
		{
			return navAgent.pathStatus == NavMeshPathStatus.PathInvalid;
		}
		return true;
	}

	private void ResetPath()
	{
		navAgent.velocity = Vector3.zero;
		navAgent.ResetPath();
		navAgent.isStopped = true;
	}

	private void SetDestination(Vector3 destination)
	{
		navAgent.SetDestination(destination);
		navAgent.isStopped = false;
	}
}
