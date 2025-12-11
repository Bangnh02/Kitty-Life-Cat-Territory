using Avelog.UI;
using UnityEngine;
using UnityEngine.UI;

public class GrowingChildPanel : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private Bar bar;

	[SerializeField]
	private Text progressTextUI;

	[SerializeField]
	private Image growingIcon;

	public void OnInitializeUI()
	{
		UpdateState();
		SetBarInstantly();
		PlayerFamilyController.addFamilyMemberEvent += OnAddFamilyMember;
		FamilyManager.gotExperienceEvent += OnFamilyMemberGotExperience;
		FamilyManager.stageUpEvent += OnFamilyMemberStageUp;
		SaveManager.LoadEndEvent += UpdateState;
	}

	private void OnDestroy()
	{
		FamilyManager.gotExperienceEvent -= OnFamilyMemberGotExperience;
		FamilyManager.stageUpEvent -= OnFamilyMemberStageUp;
		PlayerFamilyController.addFamilyMemberEvent -= OnAddFamilyMember;
		SaveManager.LoadEndEvent -= UpdateState;
	}

	private void OnFamilyMemberGotExperience(FamilyManager.FamilyMemberData familyMemberData, float experience, float experiencePercent)
	{
		UpdateState();
	}

	private void OnFamilyMemberStageUp(FamilyManager.FamilyMemberData familyMemberData)
	{
		SetBarInstantly();
		UpdateState();
	}

	private void OnAddFamilyMember(FamilyManager.FamilyMemberData familyMember)
	{
		if (familyMember.role != FamilyManager.FamilyMemberRole.Spouse)
		{
			SetBarInstantly();
			UpdateState();
		}
	}

	private void SetBarInstantly()
	{
		FamilyManager.FamilyMemberData growingChild = ManagerBase<FamilyManager>.Instance.GrowingChild;
		if (growingChild != null)
		{
			float valueInstantly = Mathf.Clamp(growingChild.experience / growingChild.Params.LevelUpExperience, 0f, 1f);
			bar.SetValueInstantly(valueInstantly);
		}
	}

	private void UpdateState()
	{
		bool flag = ManagerBase<FamilyManager>.Instance.GrowingChild != null;
		base.gameObject.SetActive(flag);
		if (flag)
		{
			FamilyManager.FamilyMemberData growingChild = ManagerBase<FamilyManager>.Instance.GrowingChild;
			growingIcon.sprite = ManagerBase<UIManager>.Instance.GetChildSprite(growingChild.role, growingChild.genderType);
			float num = Mathf.Clamp(growingChild.experience / growingChild.Params.LevelUpExperience, 0f, 1f);
			bar.SetValue(num);
			progressTextUI.text = ((int)(num * 100f)).ToString() + "%";
		}
	}
}
