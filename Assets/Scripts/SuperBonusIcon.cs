using UnityEngine;
using UnityEngine.UI;

public class SuperBonusIcon : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private SuperBonus.Id id;

	[SerializeField]
	private Image fill;

	private FarmResidentManager.SuperBonusData superBonusData;

	private bool isMenuIcon;

	public void OnInitializeUI()
	{
		Transform parent = base.transform.parent;
		while (parent != null && parent != WindowSingleton<MenuWindow>.Instance.transform && parent != WindowSingleton<GameWindow>.Instance.transform)
		{
			parent = parent.parent;
		}
		isMenuIcon = (parent == WindowSingleton<MenuWindow>.Instance.transform);
		superBonusData = ManagerBase<FarmResidentManager>.Instance.SuperBonusesData.Find((FarmResidentManager.SuperBonusData x) => x.id == id);
		if (superBonusData.IsActive)
		{
			if (superBonusData.IsPermanent)
			{
				fill.color = ManagerBase<UIManager>.Instance.PermanentSuperBonusFillColor;
			}
			else
			{
				fill.color = ManagerBase<UIManager>.Instance.TemporarySuperBonusFillColor;
			}
		}
		FarmResidentManager.SuperBonusData.switchActiveStateEvent += OnSuperBonusSwitchActive;
		FarmResidentManager.SuperBonusData.updateTimerEvent += OnSuperBonusUpdateTimer;
		UpdateIcon();
		SaveManager.LoadEndEvent += OnLoadEnd;
	}

	private void OnLoadEnd()
	{
		if (superBonusData.IsActive)
		{
			if (superBonusData.IsPermanent)
			{
				fill.color = ManagerBase<UIManager>.Instance.PermanentSuperBonusFillColor;
			}
			else
			{
				fill.color = ManagerBase<UIManager>.Instance.TemporarySuperBonusFillColor;
			}
		}
		UpdateIcon();
	}

	private void OnDestroy()
	{
		FarmResidentManager.SuperBonusData.switchActiveStateEvent -= OnSuperBonusSwitchActive;
		FarmResidentManager.SuperBonusData.updateTimerEvent -= OnSuperBonusUpdateTimer;
	}

	private void OnSuperBonusUpdateTimer(SuperBonus.Id id)
	{
		if (id == this.id)
		{
			UpdateIcon();
		}
	}

	private void OnSuperBonusSwitchActive(SuperBonus.Id id)
	{
		if (id == this.id)
		{
			if (superBonusData.IsActive)
			{
				base.transform.SetAsFirstSibling();
			}
			if (superBonusData.IsPermanent)
			{
				fill.color = ManagerBase<UIManager>.Instance.PermanentSuperBonusFillColor;
			}
			else
			{
				fill.color = ManagerBase<UIManager>.Instance.TemporarySuperBonusFillColor;
			}
			UpdateIcon();
		}
	}

	private void UpdateIcon()
	{
		fill.fillAmount = (superBonusData.IsPermanent ? 1f : (superBonusData.Timer / ManagerBase<FarmResidentManager>.Instance.SuperBonusTime));
		if (!isMenuIcon)
		{
			base.gameObject.SetActive(superBonusData.IsActive);
		}
	}
}
