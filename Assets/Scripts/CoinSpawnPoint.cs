using Avelog.Spawn;
using UnityEngine;

public class CoinSpawnPoint : MonoBehaviour, ISpawnPoint
{
	[SerializeField]
	[ReadonlyInspector]
	private Coin owner;

	public bool IsBusy => owner != null;

	public float FreeTime
	{
		get;
		private set;
	}

	public void Occupy(Coin coin)
	{
		if (owner != null)
		{
			UnityEngine.Debug.LogError("Попытка занять уже занятую точку спавна");
		}
		else
		{
			owner = coin;
		}
	}

	public void Free()
	{
		if (owner != null)
		{
			FreeTime = Time.time;
		}
		owner = null;
	}

	public Vector3 GetPosition()
	{
		return base.transform.position;
	}
}
