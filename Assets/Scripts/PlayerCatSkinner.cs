public class PlayerCatSkinner : CatSkinner, IInitializablePlayerComponent
{
	protected override void SetCurrentSkin()
	{
		catMeshRenderer.sharedMesh = base.CurrentSkin.catMesh;
		shadowMeshRenderer.sharedMesh = base.CurrentSkin.catMesh;
		SelectMaterial();
	}

	protected override void OnChangeGenderPressed()
	{
		SelectMaterial();
	}

	private void SelectMaterial()
	{
		if (ManagerBase<PlayerManager>.Instance.gender == FamilyManager.GenderType.Male)
		{
			catMeshRenderer.sharedMaterial = base.CurrentSkin.catMaleMaterial;
		}
		else
		{
			catMeshRenderer.sharedMaterial = base.CurrentSkin.catFemaleMaterial;
		}
	}
}
