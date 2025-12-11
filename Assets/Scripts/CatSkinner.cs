using Avelog;
using UnityEngine;

public abstract class CatSkinner : MonoBehaviour
{
	[SerializeField]
	protected SkinnedMeshRenderer catMeshRenderer;

	[SerializeField]
	protected SkinnedMeshRenderer shadowMeshRenderer;

	protected SkinManager.Skin CurrentSkin => ManagerBase<SkinManager>.Instance.CurrentSkin;

	public virtual void Initialize()
	{
		SetCurrentSkin();
		SkinManager.currentSkinChangedEvent += SetCurrentSkin;
		Avelog.Input.changeGenderPressedEvent += OnChangeGenderPressed;
	}

	protected virtual void OnDestroy()
	{
		SkinManager.currentSkinChangedEvent -= SetCurrentSkin;
		Avelog.Input.changeGenderPressedEvent -= OnChangeGenderPressed;
	}

	protected abstract void SetCurrentSkin();

	protected abstract void OnChangeGenderPressed();
}
