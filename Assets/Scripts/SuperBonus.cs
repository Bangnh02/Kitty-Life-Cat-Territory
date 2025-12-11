using Avelog;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SuperBonus : MonoBehaviour
{
	public enum Id
	{
		BootsWalkers,
		Acorn,
		Milk
	}

	[Serializable]
	public class Params
	{
		public Id id;

		public GameObject prefab;
	}

	public delegate void PickHandler(FarmResidentManager.SuperBonusData superBonusData);

	[SerializeField]
	private SphereCollider collider;

	[SerializeField]
	private Rigidbody rigidbody;

	[SerializeField]
	private GameObject graphicGO;

	private bool isApplyingForce;

	private Id id;

	private FarmResidentManager.SuperBonusData superBonusData;

	private Vector3 startScale;

	private float timer;

	private Vector3 Position
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

	public static event PickHandler pickEvent;

	public void Spawn(Id id, Vector3 force)
	{
		this.id = id;
		startScale = graphicGO.transform.localScale;
		superBonusData = ManagerBase<FarmResidentManager>.Instance.SuperBonusesData.Find((FarmResidentManager.SuperBonusData x) => x.id == id);
		StartCoroutine(Life(force));
	}

	private IEnumerator Life(Vector3 force)
	{
		AttachToGround();
		isApplyingForce = false;
		ApplyForce(force);
		while (isApplyingForce)
		{
			yield return null;
		}
		StopApplyForce();
		float curMoveSpeed = Singleton<CoinSpawner>.Instance.CoinPickStartSpeed;
		while (true)
		{
			curMoveSpeed += Time.deltaTime * Singleton<CoinSpawner>.Instance.CoinPickAcceleration;
			Vector3 vector = PlayerSpawner.PlayerInstance.SuperBonusPickTransform.position - Position;
			float num = curMoveSpeed * Time.deltaTime;
			if (!vector.IsLonger(Mathf.Max(Singleton<CoinSpawner>.Instance.CoinCompletePickDistance, num)))
			{
				break;
			}
			Position += vector.normalized * num;
			bool scaleOnPick = Singleton<CoinSpawner>.Instance.ScaleOnPick;
			yield return null;
		}
		ManagerBase<FarmResidentManager>.Instance.UpgradeSuperBonus(superBonusData);
		SuperBonus.pickEvent?.Invoke(superBonusData);
		FarmResidentId farmResidentId = ManagerBase<FarmResidentManager>.Instance.ConvertBonusToFarmResident(superBonusData.id);
		if (ManagerBase<FarmResidentManager>.Instance.FarmResidentsData.Find((FarmResidentManager.FarmResidentData x) => x.farmResidentId == farmResidentId).helpProgressCurrent == 1)
		{
			timer = ManagerBase<FarmResidentManager>.Instance.superBonusFirstLifeTime;
		}
		else
		{
			timer = ManagerBase<FarmResidentManager>.Instance.superBonusLifeTime;
		}
		while (true)
		{
			Position = PlayerSpawner.PlayerInstance.SuperBonusPickTransform.position;
			timer = Mathf.Clamp(timer - Time.deltaTime, 0f, float.MaxValue);
			if (timer == 0f)
			{
				break;
			}
			yield return null;
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void DestroyObject()
	{
		StopAllCoroutines();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void AttachToGround()
	{
		if (NavMeshUtils.SamplePositionIterative(Position, out NavMeshHit navMeshHit, 1f, 10f, 4, -1))
		{
			Position = navMeshHit.position + Vector3.up * Mathf.Max(collider.bounds.size.x, collider.bounds.size.y, collider.bounds.size.z);
		}
	}

	private void ApplyForce(Vector3 force)
	{
		isApplyingForce = true;
		collider.enabled = true;
		rigidbody.isKinematic = false;
		rigidbody.AddForce(force, ForceMode.VelocityChange);
	}

	private void StopApplyForce()
	{
		isApplyingForce = false;
		collider.enabled = false;
		rigidbody.isKinematic = true;
	}

	private void OnCollisionEnter(Collision collision)
	{
		CheckCollision(collision);
	}

	private void OnCollisionStay(Collision collision)
	{
		CheckCollision(collision);
	}

	private void CheckCollision(Collision collision)
	{
		if (collision.collider.gameObject.layer != Layers.ColliderLayer)
		{
			return;
		}
		int num = 0;
		while (true)
		{
			if (num < collision.contactCount)
			{
				collision.GetContact(num);
				if (Vector3.Angle(collision.GetContact(0).normal, Vector3.up) <= 90f)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		StopApplyForce();
	}
}
