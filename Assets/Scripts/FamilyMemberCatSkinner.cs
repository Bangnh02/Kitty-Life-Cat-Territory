using UnityEngine;

public class FamilyMemberCatSkinner : CatSkinner
{
	[SerializeField]
	private CatModel catModel;

	[SerializeField]
	private SkinnedMeshRenderer kittenMeshRenderer;

	[SerializeField]
	private SkinnedMeshRenderer kittenShadowMeshRenderer;

	[SerializeField]
	private CatModel kittenModel;

	private FamilyMemberController _familyMember;

	private FamilyMemberController FamilyMember
	{
		get
		{
			if (_familyMember == null)
			{
				_familyMember = GetComponent<FamilyMemberController>();
			}
			return _familyMember;
		}
	}

	public CatModel CurModel
	{
		get;
		private set;
	}

	public override void Initialize()
	{
		base.Initialize();
		if (FamilyMember.familyMemberData.role != FamilyManager.FamilyMemberRole.Spouse && FamilyMember.familyMemberData.role != FamilyManager.FamilyMemberRole.ThirdStageChild)
		{
			FamilyManager.stageUpEvent += OnChildStageUp;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		FamilyManager.stageUpEvent -= OnChildStageUp;
	}

	protected override void SetCurrentSkin()
	{
		if (FamilyMember.familyMemberData.role == FamilyManager.FamilyMemberRole.Spouse || FamilyMember.familyMemberData.role == FamilyManager.FamilyMemberRole.ThirdStageChild)
		{
			catModel.gameObject.SetActive(value: true);
			kittenModel.gameObject.SetActive(value: false);
			catMeshRenderer.sharedMesh = base.CurrentSkin.catMesh;
			shadowMeshRenderer.sharedMesh = base.CurrentSkin.catMesh;
			if (FamilyMember.familyMemberData.genderType == FamilyManager.GenderType.Female)
			{
				catMeshRenderer.sharedMaterial = base.CurrentSkin.catFemaleMaterial;
			}
			else
			{
				catMeshRenderer.sharedMaterial = base.CurrentSkin.catMaleMaterial;
			}
		}
		else
		{
			catModel.gameObject.SetActive(value: false);
			kittenModel.gameObject.SetActive(value: true);
			kittenMeshRenderer.sharedMesh = base.CurrentSkin.kittenMesh;
			kittenShadowMeshRenderer.sharedMesh = base.CurrentSkin.kittenMesh;
			if (FamilyMember.familyMemberData.genderType == FamilyManager.GenderType.Female)
			{
				kittenMeshRenderer.sharedMaterial = base.CurrentSkin.kittenFemaleMaterial;
			}
			else
			{
				kittenMeshRenderer.sharedMaterial = base.CurrentSkin.kittenMaleMaterial;
			}
		}
		CurModel = (catModel.gameObject.activeSelf ? catModel : kittenModel);
		UpdateModelOffset();
	}

	private void UpdateModelOffset()
	{
		CurModel.gameObject.transform.localPosition = new Vector3(0f, FamilyMember.familyMemberData.Params.ModelYOffset, 0f);
	}

	private void OnChildStageUp(FamilyManager.FamilyMemberData familyMemberData)
	{
		if (familyMemberData == FamilyMember.familyMemberData)
		{
			if (FamilyMember.familyMemberData.role == FamilyManager.FamilyMemberRole.ThirdStageChild)
			{
				SetCurrentSkin();
				FamilyManager.stageUpEvent -= OnChildStageUp;
			}
			else
			{
				UpdateModelOffset();
			}
		}
	}

	protected override void OnChangeGenderPressed()
	{
		if (FamilyMember.familyMemberData.role == FamilyManager.FamilyMemberRole.Spouse)
		{
			if (FamilyMember.familyMemberData.genderType == FamilyManager.GenderType.Female)
			{
				catMeshRenderer.sharedMaterial = base.CurrentSkin.catFemaleMaterial;
			}
			else
			{
				catMeshRenderer.sharedMaterial = base.CurrentSkin.catMaleMaterial;
			}
		}
	}
}
