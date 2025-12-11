using Avelog;
using System;
using UnityEngine;

[Serializable]
public class CoinsData
{
	[SerializeField]
	private int coinsCount;

	[SerializeField]
	[HideInInspector]
	private int coinsHash;

	public int CoinsCount
	{
		get
		{
			ValidateCoins();
			return coinsCount;
		}
		set
		{
			if (ValidateCoins())
			{
				coinsCount = value;
				coinsHash = BitConverter.ToInt32(HashUtils.Compute(coinsCount), 0);
			}
		}
	}

	public static event Action coinsValidateFailedEvent;

	public bool ValidateCoins()
	{
		if (coinsCount == 0)
		{
			return true;
		}
		int num = BitConverter.ToInt32(HashUtils.Compute(coinsCount), 0);
		if (coinsHash != num)
		{
			UnityEngine.Debug.LogError("Coins hash invalid. Coins reseted");
			coinsCount = 0;
			coinsHash = BitConverter.ToInt32(HashUtils.Compute(coinsCount), 0);
			CoinsData.coinsValidateFailedEvent?.Invoke();
			return false;
		}
		return true;
	}

	public void LoadFromPrefs()
	{
	}
}
