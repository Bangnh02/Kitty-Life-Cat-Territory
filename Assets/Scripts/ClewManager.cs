using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClewManager : ManagerBase<ClewManager>
{
	[Serializable]
	public class Data
	{
		public int id;

		public bool isPicked;
	}

	[SerializeField]
	private int clewsCount = 10;

	[SerializeField]
	private int fullFeeCoinsReward = 700;

	[SerializeField]
	private float expiriencePerPick = 75f;

	[Header("Отладка")]
	[Save]
	public List<Data> clewsData = new List<Data>();

	public int FullFeeCoinsReward => fullFeeCoinsReward;

	public float ExpiriencePerPick => expiriencePerPick;

	public int ClewsCount => clewsCount;

	public int ClewsCollected => clewsData.Count((Data x) => x.isPicked);

	public bool IsAllClewsCollected => ClewsCount == ClewsCollected;

	public Data GetClewData(int id)
	{
		Data data = clewsData.Find((Data x) => x.id == id);
		if (data == null)
		{
			data = new Data
			{
				id = id,
				isPicked = false
			};
			clewsData.Add(data);
		}
		return data;
	}

	protected override void OnInit()
	{
	}
}
