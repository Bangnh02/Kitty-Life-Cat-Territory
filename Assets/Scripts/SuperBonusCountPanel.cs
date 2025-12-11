using UnityEngine;
using UnityEngine.UI;

public class SuperBonusCountPanel : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private SuperBonus.Id id;

	[SerializeField]
	private Text text;

	private FarmResidentManager.FarmResidentData farmResidentData;

	private bool isMenuIcon;

	public void OnInitializeUI()
	{
		FarmResidentManager.SuperBonusData superBonusData = ManagerBase<FarmResidentManager>.Instance.SuperBonusesData.Find((FarmResidentManager.SuperBonusData x) => x.id == id);
		FarmResidentId farmResidentId = ManagerBase<FarmResidentManager>.Instance.ConvertBonusToFarmResident(superBonusData.id);
		farmResidentData = ManagerBase<FarmResidentManager>.Instance.FarmResidentsData.Find((FarmResidentManager.FarmResidentData x) => x.farmResidentId == farmResidentId);
		FarmResidentManager.upgradeSuperBonusEvent += OnUpgradeSuperBonus;
		SaveManager.LoadEndEvent += UpdatePanel;
		UpdatePanel();
	}

	private void OnDestroy()
	{
		FarmResidentManager.upgradeSuperBonusEvent -= OnUpgradeSuperBonus;
		SaveManager.LoadEndEvent -= UpdatePanel;
	}

	private void OnUpgradeSuperBonus(SuperBonus.Id id)
	{
		if (id == this.id)
		{
			UpdatePanel();
		}
	}

	private void UpdatePanel()
	{
		text.text = $"{farmResidentData.helpProgressCurrent}/{ManagerBase<FarmResidentManager>.Instance.HelpProgressMaximum}";
	}
}
