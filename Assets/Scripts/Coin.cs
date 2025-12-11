using Avelog;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Coin : MonoBehaviour
{
	public delegate void PickCoinHandler(bool isSpawnedCoin);

	[SerializeField]
	private bool useRotating = true;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Seconds)]
	private float timeToFullScale = 1f;

	[SerializeField]
	private float rotationSpeed = 180f;

	[SerializeField]
	private bool useFloating = true;

	[SerializeField]
	private AnimationCurve floatingCurve;

	[SerializeField]
	private float graphicLerp = 10f;

	[Header("Ссылки")]
	[SerializeField]
	private GameObject physicGO;

	[SerializeField]
	private ProcessingSwitch processingSwitch;

	[SerializeField]
	private ColliderEvents colliderEventsBeh;

	[SerializeField]
	private SphereCollider collider;

	[SerializeField]
	private Rigidbody rigidbody;

	private Action unspawnCallback;

	private Action applyForceEndCallback;

	private Coroutine moveCor;

	private Coroutine rotatingCor;

	private Coroutine floatingCor;

	private Coroutine applyForceCor;

	private int value;

	private bool applyingForce;

	private Transform startPhysicParent;

	private Vector3 startScale;

	private Quaternion physicLocalRot;

	public bool IsPicking => moveCor != null;

	public bool OnProcessingDistance => processingSwitch.OnProcessingDistance;

	public bool IsSpawnedCoin
	{
		get;
		private set;
	}

	public float StartPickDistance
	{
		get
		{
			if (!applyingForce)
			{
				return Singleton<CoinSpawner>.Instance.CoinStartPickDistance;
			}
			return 0f;
		}
	}

	public Vector3 Position
	{
		get
		{
			return base.transform.position;
		}
		set
		{
			base.transform.position = value;
		}
	}

	public static event PickCoinHandler pickEvent;

	private void Start()
	{
	}

	private void OnDestroy()
	{
		colliderEventsBeh.collisionEnterEvent -= OnCollisionEnterEvent;
		colliderEventsBeh.collisionStayEvent -= OnCollisionStayEvent;
	}

	private void OnCollisionEnterEvent(Collision collision)
	{
		CheckCollision(collision);
	}

	private void OnCollisionStayEvent(Collision collision)
	{
		CheckCollision(collision);
	}

	private void Update()
	{
	}

	private void OnDisable()
	{
	}

	public void Spawn(Action unspawnCallback, int coinValue, bool isSpawnedCoin)
	{
		startPhysicParent = base.transform;
		startScale = base.transform.localScale;
		physicLocalRot = physicGO.transform.localRotation;
		IsSpawnedCoin = isSpawnedCoin;
		value = coinValue;
		this.unspawnCallback = unspawnCallback;
		processingSwitch.enabled = true;
		processingSwitch.MeasureDistance();
		if (!processingSwitch.OnProcessingDistance)
		{
			processingSwitch.Switch(isEnabled: false, forceUpdate: true, instantEnabling: true, instantDisabling: true);
		}
		StartRotating();
		StartFloating(randomizeStartY: true, 0f);
	}

	private void Unspawn()
	{
		CompletePick();
		StopApplyForce();
		StopFloating();
		StopRotating();
		StopAllCoroutines();
		base.transform.localScale = startScale;
		Action action = unspawnCallback;
		unspawnCallback = null;
		action?.Invoke();
	}

	public void AttachToGround()
	{
		Position = GetGroundedPosition(Position, collider.transform.rotation);
		Position += Vector3.up * (collider.transform.lossyScale.y * collider.radius * 2f + 0.1f);
	}

	public Vector3 GetGroundedPosition(Vector3 sourcePos, Quaternion rotation)
	{
		Vector3 a = sourcePos;
		float num = collider.radius * Mathf.Max(collider.transform.lossyScale.x, collider.transform.lossyScale.y, collider.transform.lossyScale.z);
		RaycastHit hitInfo2;
		if (Physics.CheckSphere(sourcePos, num, 1 << Layers.ColliderLayer))
		{
			a = ((!Physics.Raycast(sourcePos, Vector3.down, out RaycastHit hitInfo, num, 1 << Layers.ColliderLayer)) ? sourcePos : hitInfo.point);
		}
		else if (Physics.SphereCast(sourcePos, num, Vector3.down, out hitInfo2, 100f, 1 << Layers.ColliderLayer))
		{
			a = hitInfo2.point;
		}
		else if (Physics.SphereCast(sourcePos, num, Vector3.up, out hitInfo2, 100f, 1 << Layers.ColliderLayer))
		{
			a = hitInfo2.point;
		}
		Vector3 b = Vector3.Project(a - sourcePos, Vector3.up);
		return sourcePos + b;
	}

	public void ApplyForce(Vector3 force, Action endCallback)
	{
		if (applyForceCor != null)
		{
			StopCoroutine(applyForceCor);
		}
		applyForceCor = StartCoroutine(ApplyingForce(force, endCallback));
	}

	private IEnumerator ApplyingForce(Vector3 force, Action endCallback)
	{
		applyForceEndCallback = endCallback;
		processingSwitch.Switch(isEnabled: true, forceUpdate: true, instantEnabling: true, instantDisabling: true);
		StopFloating();
		physicGO.transform.parent = base.transform.parent;
		colliderEventsBeh.collisionEnterEvent += OnCollisionEnterEvent;
		colliderEventsBeh.collisionStayEvent += OnCollisionStayEvent;
		collider.enabled = true;
		rigidbody.isKinematic = false;
		applyingForce = true;
		rigidbody.AddForce(force, ForceMode.VelocityChange);
		Vector3 lastMoveDir = Vector3.zero;
		base.transform.localScale = Vector3.zero;
		float lastDeltaTime = Time.deltaTime;
		float scaleLerp2 = 0f;
		float flyTime = 0f;
		while (collider.enabled)
		{
			Vector3 position = base.transform.position;
			base.transform.position = Vector3.Lerp(base.transform.position, physicGO.transform.position, graphicLerp * Time.deltaTime);
			physicGO.transform.rotation = base.transform.rotation * physicLocalRot;
			lastMoveDir = base.transform.position - position;
			lastDeltaTime = Time.deltaTime;
			yield return null;
			flyTime += Time.deltaTime;
			if (base.transform.localScale != startScale)
			{
				scaleLerp2 = Mathf.Lerp(0f, 1f, flyTime / timeToFullScale);
				base.transform.localScale = startScale * scaleLerp2;
			}
			if (flyTime >= 5f)
			{
				Unspawn();
				yield break;
			}
		}
		float lastMoveSpeed = (lastMoveDir / lastDeltaTime).magnitude;
		if (lastMoveSpeed != 0f)
		{
			while (physicGO.transform.position != base.transform.position)
			{
				Vector3 vector = physicGO.transform.position - base.transform.position;
				Vector3 vector2 = vector.normalized * lastMoveSpeed * Time.deltaTime;
				if (vector.magnitude < vector2.magnitude)
				{
					vector2 = vector;
				}
				physicGO.transform.rotation = base.transform.rotation * physicLocalRot;
				base.transform.position += vector2;
				yield return null;
			}
		}
		else
		{
			physicGO.transform.rotation = base.transform.rotation * physicLocalRot;
			base.transform.position = physicGO.transform.position;
		}
		if (scaleLerp2 != 1f)
		{
			while (base.transform.localScale != startScale)
			{
				flyTime += Time.deltaTime;
				scaleLerp2 = Mathf.Lerp(0f, 1f, flyTime / timeToFullScale);
				base.transform.localScale = startScale * scaleLerp2;
				yield return null;
			}
		}
		StopApplyForce();
	}

	private void CheckCollision(Collision collision)
	{
		if (collision.collider.gameObject.layer != Layers.ColliderLayer || Vector3.Dot(rigidbody.velocity.normalized, Vector3.up) > 0f)
		{
			return;
		}
		int num = 0;
		while (true)
		{
			if (num < collision.contactCount)
			{
				collision.GetContact(num);
				if (Vector3.Angle(collision.GetContact(0).normal, Vector3.up) < 90f)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		collider.enabled = false;
		rigidbody.isKinematic = true;
	}

	private void StopApplyForce()
	{
		if (applyingForce)
		{
			physicGO.transform.parent = startPhysicParent;
			physicGO.transform.position = base.transform.position;
			physicGO.transform.rotation = base.transform.rotation * physicLocalRot;
			if (applyForceCor != null)
			{
				StopCoroutine(applyForceCor);
			}
			applyForceCor = null;
			colliderEventsBeh.collisionEnterEvent -= OnCollisionEnterEvent;
			colliderEventsBeh.collisionStayEvent -= OnCollisionStayEvent;
			collider.enabled = false;
			rigidbody.isKinematic = true;
			applyingForce = false;
			Action action = applyForceEndCallback;
			applyForceEndCallback = null;
			action?.Invoke();
			if (!IsPicking && base.gameObject.activeSelf)
			{
				StartFloating(randomizeStartY: false, Singleton<CoinSpawner>.Instance.DropHeightOffset);
			}
		}
	}

	public void Pick()
	{
		if (moveCor == null)
		{
			StopApplyForce();
			StopFloating();
			StopRotating();
			processingSwitch.enabled = false;
			moveCor = StartCoroutine(MoveToPlayer(CompletePick));
		}
	}

	private void CompletePick()
	{
		if (IsPicking)
		{
			moveCor = null;
			ManagerBase<PlayerManager>.Instance.ChangeCoins(value);
			Unspawn();
			Coin.pickEvent?.Invoke(IsSpawnedCoin);
		}
	}

	private IEnumerator MoveToPlayer(Action endCallback)
	{
		float curMoveSpeed = Singleton<CoinSpawner>.Instance.CoinPickStartSpeed;
		Vector3 startScale = base.transform.localScale;
		float startScaleMagnitude = startScale.magnitude;
		float endScaleMagnitude = startScaleMagnitude * Singleton<CoinSpawner>.Instance.PickEndScale;
		while (true)
		{
			curMoveSpeed += Time.deltaTime * Singleton<CoinSpawner>.Instance.CoinPickAcceleration;
			Vector3 vector = PlayerSpawner.PlayerInstance.PlayerCenter.position - Position;
			float num = curMoveSpeed * Time.deltaTime;
			if (!vector.IsLonger(Mathf.Max(Singleton<CoinSpawner>.Instance.CoinCompletePickDistance, num)))
			{
				break;
			}
			Position += vector.normalized * num;
			if (Singleton<CoinSpawner>.Instance.ScaleOnPick)
			{
				float num2 = (vector.magnitude - Singleton<CoinSpawner>.Instance.CoinCompletePickDistance) / (Singleton<CoinSpawner>.Instance.CoinStartPickDistance - Singleton<CoinSpawner>.Instance.CoinCompletePickDistance);
				num2 = 1f - num2;
				float d = Mathf.Clamp(Mathf.Lerp(startScaleMagnitude, endScaleMagnitude, num2), Singleton<CoinSpawner>.Instance.PickEndScale, base.transform.localScale.magnitude);
				base.transform.localScale = startScale.normalized * d;
			}
			yield return null;
		}
		endCallback?.Invoke();
	}

	private void StartFloating(bool randomizeStartY, float offset)
	{
		if (floatingCor == null)
		{
			floatingCor = StartCoroutine(Floating(randomizeStartY, offset));
		}
	}

	private IEnumerator Floating(bool randomizeStartY, float offset)
	{
		float startTime = Time.time;
		float curAnimationY = 0f;
		float randomTimeOffset = 0f;
		float startHeightMulty = 1f;
		if (randomizeStartY)
		{
			randomTimeOffset = UnityEngine.Random.Range(0f, 100f);
		}
		if (offset != 0f)
		{
			Keyframe keyframe = floatingCurve.keys.FirstOrDefault((Keyframe x) => x.value == floatingCurve.keys.Min((Keyframe y) => y.value));
			Keyframe keyframe2 = floatingCurve.keys.FirstOrDefault((Keyframe x) => x.value == floatingCurve.keys.Max((Keyframe y) => y.value));
			Mathf.Abs(keyframe.time - keyframe2.time);
			float num = Mathf.Abs(keyframe.value - keyframe2.value);
			startHeightMulty = (num + offset) / num;
		}
		while (true)
		{
			if (useFloating)
			{
				float num2 = curAnimationY;
				Position -= Vector3.up * curAnimationY;
				curAnimationY = floatingCurve.Evaluate(Time.time - startTime + randomTimeOffset) * startHeightMulty;
				Position += Vector3.up * curAnimationY;
				if (num2 > curAnimationY && startHeightMulty != 1f)
				{
					curAnimationY /= startHeightMulty;
					startHeightMulty = 1f;
				}
			}
			yield return null;
		}
	}

	private void StopFloating()
	{
		if (floatingCor != null)
		{
			StopCoroutine(floatingCor);
			floatingCor = null;
		}
	}

	private void StartRotating()
	{
		if (rotatingCor == null)
		{
			rotatingCor = StartCoroutine(Rotating());
		}
	}

	private IEnumerator Rotating()
	{
		float y = UnityEngine.Random.Range(0f, 360f);
		base.transform.rotation *= Quaternion.Euler(0f, y, 0f);
		while (true)
		{
			if (useRotating)
			{
				base.transform.rotation *= Quaternion.Euler(0f, rotationSpeed * Time.deltaTime, 0f);
			}
			yield return null;
		}
	}

	private void StopRotating()
	{
		if (rotatingCor != null)
		{
			StopCoroutine(rotatingCor);
			rotatingCor = null;
		}
	}
}
