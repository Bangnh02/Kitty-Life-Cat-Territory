using Avelog.Spawn;
using UnityEngine;

public class SpawnPoint : MonoBehaviour, ISpawnPoint
{
	[SerializeField]
	private ItemId itemId;

	[ReadonlyInspector]
	[SerializeField]
	private bool isBusy;

	public static SpawnDistributor<SpawnPoint>.EventHandler freeEvent;

	public static SpawnDistributor<SpawnPoint>.EventHandler occupyEvent;

	public ItemId ItemId => itemId;

	public bool IsBusy
	{
		get
		{
			return isBusy;
		}
		set
		{
			if (isBusy != value)
			{
				isBusy = value;
				if (value)
				{
					FreeTime = Time.time;
				}
				if (value)
				{
					occupyEvent?.Invoke(this);
				}
				else
				{
					freeEvent?.Invoke(this);
				}
			}
		}
	}

	public int CurPriority
	{
		get;
		private set;
	}

	public float FreeTime
	{
		get;
		private set;
	}

	private void Awake()
	{
		if (base.transform.childCount != 0)
		{
			base.transform.GetChild(0).gameObject.SetActive(value: false);
		}
	}

	public Vector3 GetPosition()
	{
		return base.transform.position;
	}
}
