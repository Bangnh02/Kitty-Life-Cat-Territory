using UnityEngine;
using UnityEngine.UI;

public class MenuClewsPanel : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private Text text;

	public void OnInitializeUI()
	{
		Clew.pickEvent += UpdatePanel;
		SaveManager.LoadEndEvent += UpdatePanel;
		UpdatePanel();
	}

	private void OnDestroy()
	{
		Clew.pickEvent -= UpdatePanel;
		SaveManager.LoadEndEvent -= UpdatePanel;
	}

	private void UpdatePanel()
	{
		text.text = $"{ManagerBase<ClewManager>.Instance.ClewsCollected}/{ManagerBase<ClewManager>.Instance.ClewsCount}";
	}
}
