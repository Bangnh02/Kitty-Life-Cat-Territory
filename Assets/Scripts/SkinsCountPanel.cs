using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SkinsCountPanel : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private Text text;

	public void OnInitializeUI()
	{
		SkinManager.buySkinEvent += UpdatePanel;
		SaveManager.LoadEndEvent += UpdatePanel;
		UpdatePanel();
	}

	private void OnDestroy()
	{
		SkinManager.buySkinEvent -= UpdatePanel;
		SaveManager.LoadEndEvent -= UpdatePanel;
	}

	private void UpdatePanel()
	{
		text.text = $"{ManagerBase<SkinManager>.Instance.Skins.Count((SkinManager.Skin x) => x.isPurchased)}/{ManagerBase<SkinManager>.Instance.Skins.Count}";
	}
}
