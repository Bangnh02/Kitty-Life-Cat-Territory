using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelPanel : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private Text levelText;

	public void OnInitializeUI()
	{
		PlayerManager.levelChangeEvent += OnLevelUp;
		SaveManager.LoadEndEvent += UpdateLevelText;
		UpdateLevelText();
	}

	private void OnDestroy()
	{
		PlayerManager.levelChangeEvent -= OnLevelUp;
	}

	private void OnLevelUp()
	{
		UpdateLevelText();
	}

	private void UpdateLevelText()
	{
		levelText.text = ManagerBase<PlayerManager>.Instance.Level.ToString();
	}
}
