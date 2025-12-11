using UnityEngine;
using UnityEngine.UI;

public class MenuCoinsPanel : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private Text text;

	public void OnInitializeUI()
	{
		PlayerManager.coinsChangeEvent += OnCoinsChange;
		SaveManager.LoadEndEvent += UpdatePanel;
		UpdatePanel();
	}

	private void OnDestroy()
	{
		PlayerManager.coinsChangeEvent -= OnCoinsChange;
		SaveManager.LoadEndEvent -= UpdatePanel;
	}

	private void OnCoinsChange(int coinsInc = 0)
	{
		UpdatePanel();
	}

	private void UpdatePanel()
	{
		text.text = ManagerBase<PlayerManager>.Instance.CurCoins.ToString();
	}
}
