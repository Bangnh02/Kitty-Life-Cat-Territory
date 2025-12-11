using UnityEngine;

public class PlayerCoinsController : MonoBehaviour, IInitializablePlayerComponent
{
	public void Initialize()
	{
		Quest.completeEvent += OnCompleteQuest;
		Clew.pickEvent += OnClewPick;
	}

	private void OnDestroy()
	{
		Quest.completeEvent -= OnCompleteQuest;
		Clew.pickEvent -= OnClewPick;
	}

	private void OnCompleteQuest(Quest completedQuest)
	{
		ManagerBase<PlayerManager>.Instance.ChangeCoins(completedQuest.Reward.Coins);
	}

	private void OnClewPick()
	{
		if (ManagerBase<ClewManager>.Instance.IsAllClewsCollected)
		{
			ManagerBase<PlayerManager>.Instance.ChangeCoins(ManagerBase<ClewManager>.Instance.FullFeeCoinsReward, isRewardedCoins: true);
		}
	}

	private void Update()
	{
		if (Singleton<CoinSpawner>.Instance == null)
		{
			return;
		}
		for (int i = 0; i < Singleton<CoinSpawner>.Instance.SpawnedCoins.Count; i++)
		{
			Coin coin = Singleton<CoinSpawner>.Instance.SpawnedCoins[i];
			if (coin.OnProcessingDistance && !coin.IsPicking && (coin.Position - PlayerSpawner.PlayerInstance.PlayerCenter.position).IsShorterOrEqual(coin.StartPickDistance))
			{
				coin.Pick();
			}
		}
	}
}
