using Avelog;
using UnityEngine;

public class MakeSpouseWindow : WindowSingleton<MakeSpouseWindow>
{
	[SerializeField]
	private GameObject spouseInformationPanel;

	[SerializeField]
	private GameObject familyInstructionPanel;

	private PotentialSpouseController PotentialSpouse => Singleton<SpouseSpawner>.Instance.PotentialSpouse;

	protected override void OnInitialize()
	{
	}

	private void OnEnable()
	{
		spouseInformationPanel.SetActive(value: true);
		familyInstructionPanel.SetActive(value: false);
	}

	public void OpenFamilyInstructionPanel()
	{
		spouseInformationPanel.SetActive(value: false);
		familyInstructionPanel.SetActive(value: true);
	}

	public void MakeSpouse()
	{
		Avelog.Input.FireMakeSpousePressed("Spouse");
		PotentialSpouse.Unspawn();
		WindowSingleton<GameWindow>.Instance.Open();
	}
}
