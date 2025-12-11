using Avelog.UI;
using UnityEngine;
using UnityEngine.UI;

public class FamilySatietyPanel : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private Bar bar;

	[SerializeField]
	private Text attackPowerPerText;

	[SerializeField]
	private Image paramStateImage;

	private const int notDefinedAttackPercent = -1;

	private int curAttackPowerPercent = -1;

	public void OnInitializeUI()
	{
		PlayerFamilyController.addFamilyMemberEvent += OnAddFamilyMember;
		PlayerFamilyController.familyChangeSatietyEvent += UpdatePanel;
		UpdatePanel();
		bar.SetValueInstantly(ManagerBase<FamilyManager>.Instance.SatietyPart);
	}

	private void OnDestroy()
	{
		PlayerFamilyController.addFamilyMemberEvent -= OnAddFamilyMember;
		PlayerFamilyController.familyChangeSatietyEvent -= UpdatePanel;
	}

	private void OnAddFamilyMember(FamilyManager.FamilyMemberData familyMember)
	{
		bar.SetValueInstantly(ManagerBase<FamilyManager>.Instance.SatietyPart);
		UpdatePanel();
	}

	private void UpdatePanel()
	{
		if (base.gameObject.activeSelf != ManagerBase<FamilyManager>.Instance.HaveFamily)
		{
			base.gameObject.SetActive(ManagerBase<FamilyManager>.Instance.HaveFamily);
		}
		if (curAttackPowerPercent != (int)ManagerBase<FamilyManager>.Instance.AttackPowerPercentFromSatiety)
		{
			curAttackPowerPercent = (int)ManagerBase<FamilyManager>.Instance.AttackPowerPercentFromSatiety;
			attackPowerPerText.text = curAttackPowerPercent.ToString() + "%";
			bar.SetValue(ManagerBase<FamilyManager>.Instance.SatietyPart);
		}
		if (ManagerBase<FamilyManager>.Instance.SatietyPart == 0f && !paramStateImage.enabled)
		{
			paramStateImage.enabled = true;
		}
		else if (ManagerBase<FamilyManager>.Instance.SatietyPart != 0f && paramStateImage.enabled)
		{
			paramStateImage.enabled = false;
		}
	}
}
