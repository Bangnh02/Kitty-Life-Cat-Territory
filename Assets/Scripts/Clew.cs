using System;
using System.Collections;
using UnityEngine;

public class Clew : MonoBehaviour
{
	private ProcessingSwitch processingSwitch;

	private ClewSpawnPoint spawnPoint;

	private ClewManager.Data _clewData;

	private Coroutine flyingCor;

	private Action unspawnCallback;

	private ClewManager.Data ClewData
	{
		get
		{
			if (_clewData == null)
			{
				_clewData = ManagerBase<ClewManager>.Instance.GetClewData(Id);
			}
			return _clewData;
		}
	}

	private int Id => spawnPoint.Id;

	public bool OnProcessingDistance => processingSwitch.OnProcessingDistance;

	public bool IsPicking => flyingCor != null;

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

	public static event Action pickEvent;

	public void Spawn(Action unspawnCallback, ClewSpawnPoint spawnPoint)
	{
		this.spawnPoint = spawnPoint;
		this.unspawnCallback = unspawnCallback;
		processingSwitch = GetComponent<ProcessingSwitch>();
	}

	public void Pick()
	{
		if (flyingCor == null && !ClewData.isPicked)
		{
			flyingCor = StartCoroutine(FlyingToPlayer());
		}
	}

	private IEnumerator FlyingToPlayer()
	{
		float curMoveSpeed = Singleton<CoinSpawner>.Instance.CoinPickStartSpeed;
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
				float t = (vector.magnitude - Singleton<CoinSpawner>.Instance.CoinCompletePickDistance) / (Singleton<CoinSpawner>.Instance.CoinStartPickDistance - Singleton<CoinSpawner>.Instance.CoinCompletePickDistance);
				float d = Mathf.Clamp(Mathf.Lerp(Singleton<CoinSpawner>.Instance.PickEndScale, 1f, t), Singleton<CoinSpawner>.Instance.PickEndScale, base.transform.localScale.x);
				base.transform.localScale = Vector3.one * d;
			}
			yield return null;
		}
		ClewData.isPicked = true;
		Clew.pickEvent?.Invoke();
		Action action = unspawnCallback;
		unspawnCallback = null;
		action?.Invoke();
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
