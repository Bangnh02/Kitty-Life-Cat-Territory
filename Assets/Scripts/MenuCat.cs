using Avelog;
using System.Collections.Generic;
using UnityEngine;

public class MenuCat : MonoBehaviour
{
	[SerializeField]
	private GameObject catModel;

	[SerializeField]
	private SkinnedMeshRenderer catMeshRenderer;

	[SerializeField]
	private SkinnedMeshRenderer catShadowMeshRenderer;

	[SerializeField]
	private Animator catAnimator;

	[SerializeField]
	private GameObject kittenModel;

	[SerializeField]
	private SkinnedMeshRenderer kittenMeshRenderer;

	[SerializeField]
	private SkinnedMeshRenderer kittenShadowMeshRenderer;

	[SerializeField]
	private Animator kittenAnimator;

	public float animationNumber;

	private float animationTime;

	private bool isOldCat;

	private FamilyManager.GenderType gender;

	private FamilyManager.FamilyMemberData familyMemberData;

	public static List<MenuCat> Instances
	{
		get;
		private set;
	} = new List<MenuCat>();


	public GameObject CatModel => catModel;

	public GameObject KittenModel => kittenModel;

	private Animator CurrentAnimator
	{
		get
		{
			if (!catModel.activeSelf)
			{
				return kittenAnimator;
			}
			return catAnimator;
		}
	}

	public Vector3 StartModelScale
	{
		get;
		private set;
	}

	public FamilyManager.FamilyMemberData FamilyMemberData => familyMemberData;

	public bool IsPlayerCat
	{
		get;
		private set;
	}

	public bool IsSpouseCat
	{
		get;
		private set;
	}

	public void Spawn(bool isOldCat, bool isPlayerCat, bool isSpouseCat, FamilyManager.GenderType gender, FamilyManager.FamilyMemberData familyMemberData = null)
	{
		Instances.Add(this);
		this.isOldCat = isOldCat;
		this.gender = gender;
		IsPlayerCat = isPlayerCat;
		IsSpouseCat = isSpouseCat;
		catModel.SetActive(isOldCat);
		kittenModel.SetActive(!isOldCat);
		SetPreviewSkin(ManagerBase<SkinManager>.Instance.CurrentSkinIndex);
		this.familyMemberData = familyMemberData;
		if (!isOldCat)
		{
			StartModelScale = KittenModel.transform.localScale;
			FamilyManager.stageUpEvent += OnKittenStageUp;
			FamilyManager.gotExperienceEvent += OnKittenGotExperience;
		}
		catAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
		kittenAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
		CurrentAnimator.SetFloat("IdleType", animationNumber);
		CurrentAnimator.Play("Idle", 0, UnityEngine.Random.Range(0f, 1f));
		SceneController.changeActiveSceneEvent += OnChangeActiveScene;
		SkinManager.previewSkinChangedEvent += OnPreviewSkinChanged;
		Avelog.Input.changeGenderPressedEvent += OnChangeGenderPressed;
	}

	private void OnChangeActiveScene(SceneController.SceneType newActiveScene)
	{
		if (newActiveScene == SceneController.SceneType.Menu)
		{
			CurrentAnimator.SetFloat("IdleType", animationNumber);
			CurrentAnimator.Play("Idle", 0, UnityEngine.Random.Range(0f, 1f));
		}
	}

	private void OnPreviewSkinChanged()
	{
		SetPreviewSkin(ManagerBase<SkinManager>.Instance.PreviewSkinIndex);
	}

	public void SetPreviewSkin(int skinIndex)
	{
		SkinManager.Skin skin = ManagerBase<SkinManager>.Instance.Skins[skinIndex];
		if (isOldCat)
		{
			catMeshRenderer.sharedMesh = skin.catMesh;
			catShadowMeshRenderer.sharedMesh = skin.catMesh;
			if (gender == FamilyManager.GenderType.Male)
			{
				catMeshRenderer.sharedMaterial = skin.catMaleMaterial;
			}
			else
			{
				catMeshRenderer.sharedMaterial = skin.catFemaleMaterial;
			}
		}
		else
		{
			kittenMeshRenderer.sharedMesh = skin.kittenMesh;
			kittenShadowMeshRenderer.sharedMesh = skin.kittenMesh;
			if (gender == FamilyManager.GenderType.Male)
			{
				kittenMeshRenderer.sharedMaterial = skin.kittenMaleMaterial;
			}
			else
			{
				kittenMeshRenderer.sharedMaterial = skin.kittenFemaleMaterial;
			}
		}
	}

	private void OnKittenStageUp(FamilyManager.FamilyMemberData familyMemberData)
	{
		if (familyMemberData == this.familyMemberData && familyMemberData.role == FamilyManager.FamilyMemberRole.ThirdStageChild)
		{
			isOldCat = true;
			catModel.SetActive(isOldCat);
			catModel.transform.localScale = catModel.transform.localScale * familyMemberData.CurScalePart;
			kittenModel.SetActive(!isOldCat);
			SetPreviewSkin(ManagerBase<SkinManager>.Instance.CurrentSkinIndex);
			FamilyManager.stageUpEvent -= OnKittenStageUp;
			FamilyManager.gotExperienceEvent -= OnKittenGotExperience;
		}
	}

	private void OnKittenGotExperience(FamilyManager.FamilyMemberData familyMemberData, float experience, float experiencePercent)
	{
		if (familyMemberData == this.familyMemberData)
		{
			KittenModel.transform.localScale = StartModelScale * familyMemberData.CurScalePart;
		}
	}

	private void OnChangeGenderPressed()
	{
		if (IsPlayerCat)
		{
			catMeshRenderer.sharedMaterial = ((ManagerBase<PlayerManager>.Instance.gender == FamilyManager.GenderType.Male) ? ManagerBase<SkinManager>.Instance.PreviewSkin.catMaleMaterial : ManagerBase<SkinManager>.Instance.PreviewSkin.catFemaleMaterial);
			gender = ManagerBase<PlayerManager>.Instance.gender;
		}
		if (IsSpouseCat)
		{
			catMeshRenderer.sharedMaterial = ((ManagerBase<PlayerManager>.Instance.gender == FamilyManager.GenderType.Male) ? ManagerBase<SkinManager>.Instance.PreviewSkin.catFemaleMaterial : ManagerBase<SkinManager>.Instance.PreviewSkin.catMaleMaterial);
			gender = ((ManagerBase<PlayerManager>.Instance.gender == FamilyManager.GenderType.Male) ? FamilyManager.GenderType.Female : FamilyManager.GenderType.Male);
		}
	}

	private void OnDestroy()
	{
		FamilyManager.stageUpEvent -= OnKittenStageUp;
		FamilyManager.gotExperienceEvent -= OnKittenGotExperience;
		SceneController.changeActiveSceneEvent -= OnChangeActiveScene;
		Avelog.Input.changeGenderPressedEvent -= OnChangeGenderPressed;
	}
}
