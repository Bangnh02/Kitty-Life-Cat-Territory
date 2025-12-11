using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlantCountPanel : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private Text text;

	public void OnInitializeUI()
	{
		VegetableBehaviour.plantEvent += OnPlant;
		SaveManager.LoadEndEvent += UpdatePanel;
		UpdatePanel();
	}

	private void OnDestroy()
	{
		VegetableBehaviour.plantEvent -= OnPlant;
		SaveManager.LoadEndEvent -= UpdatePanel;
	}

	private void OnPlant(VegetableBehaviour vegetable)
	{
		UpdatePanel();
	}

	private void UpdatePanel()
	{
		text.text = $"{ManagerBase<VegetableManager>.Instance.vegetablesData.Count((VegetableManager.VegetableData x) => x.isPlanted)}/{ManagerBase<VegetableManager>.Instance.VegetablesCount}";
	}
}
