using UnityEngine;

public class GetCoinsButton : MonoBehaviour
{
	[SerializeField]
	private int coins = 100;

	public void GetCoins()
	{
		ManagerBase<PlayerManager>.Instance.ChangeCoins(coins);
	}
}
