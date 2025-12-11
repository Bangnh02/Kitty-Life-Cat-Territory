using Avelog;
using I2.Loc;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SkinsWindow : WindowSingleton<SkinsWindow>
{
	[Serializable]
	public class GenderButton
	{
		public Button button;

		public GameObject selectedRingObj;

		public void SetActiveButton()
		{
			button.interactable = true;
			selectedRingObj.SetActive(value: true);
		}

		public void SetInactiveButton()
		{
			button.interactable = true;
			selectedRingObj.SetActive(value: false);
		}

		public void DisableButton()
		{
			button.interactable = false;
			selectedRingObj.SetActive(value: false);
		}
	}

	[SerializeField]
	private Localize skinNameTerm;

	[SerializeField]
	private Localize skinDescriptionTerm;

	[SerializeField]
	private Localize bonusTerm1;

	[SerializeField]
	private LocalizationParamsManager bonusTermParam1;

	[SerializeField]
	private Localize bonusTerm2;

	[SerializeField]
	private LocalizationParamsManager bonusTermParam2;

	[SerializeField]
	private GameObject skinClosedPanel;

	[SerializeField]
	private LocalizationParamsManager closedLevelTermParam;

	[SerializeField]
	private Button purchaseSkinButton;

	[SerializeField]
	private CanvasGroup purchaseSkinButtonCanvasGroup;

	[SerializeField]
	private float disableButtonAlpha = 0.5f;

	[SerializeField]
	private GameObject pricePanel;

	[SerializeField]
	private Text priceText;

	[SerializeField]
	private Text skinNumberText;

	[SerializeField]
	private GenderButton maleGenderButton;

	[SerializeField]
	private GenderButton femaleGenderButton;

	private int browsedSkinIndex;

	private const string lockLevelLocParam = "level";

	private const string bonusParam = "bonus";

	private const string valueParam = "value";

	private int BrowsedSkinIndex
	{
		get
		{
			return browsedSkinIndex;
		}
		set
		{
			browsedSkinIndex = value;
			ManagerBase<SkinManager>.Instance.PreviewSkinIndex = value;
		}
	}

	private SkinManager.Skin BrowsedSkin => ManagerBase<SkinManager>.Instance.Skins[BrowsedSkinIndex];

	protected override void OnInitialize()
	{
		SaveManager.LoadEndEvent += OnLoadEnd;
		GameConfigManager.remoteConfigsLoadedEvent += OnRemoteConfigsLoaded;
	}

	private void OnDestroy()
	{
		SaveManager.LoadEndEvent -= OnLoadEnd;
		GameConfigManager.remoteConfigsLoadedEvent -= OnRemoteConfigsLoaded;
	}

	private void OnLoadEnd()
	{
		if (Window.CurWindow == this)
		{
			OnEnable();
		}
	}

	private void OnRemoteConfigsLoaded()
	{
		if (Window.CurWindow == this)
		{
			OnEnable();
		}
	}

	private void OnEnable()
	{
		BrowsedSkinIndex = ManagerBase<SkinManager>.Instance.CurrentSkinIndex;
		UpdateUI();
		if (ManagerBase<PlayerManager>.Instance.gender == FamilyManager.GenderType.Male)
		{
			maleGenderButton.SetActiveButton();
			femaleGenderButton.SetInactiveButton();
		}
		else
		{
			femaleGenderButton.SetActiveButton();
			maleGenderButton.SetInactiveButton();
		}
	}

	private void UpdateUI()
	{
		skinNameTerm.SetTerm(BrowsedSkin.nameTerm);
		skinDescriptionTerm.SetTerm(BrowsedSkin.descriptionTerm);
		if (BrowsedSkin.bonusTerms.Count == 2)
		{
			bonusTerm1.gameObject.SetActive(value: true);
			bonusTerm2.gameObject.SetActive(value: true);
			bonusTerm1.SetTerm(BrowsedSkin.bonusTerms[0]);
			bonusTerm2.SetTerm(BrowsedSkin.bonusTerms[1]);
			UpdateBonusParms(BrowsedSkin.id);
		}
		else
		{
			bonusTerm1.gameObject.SetActive(value: true);
			bonusTerm2.gameObject.SetActive(value: false);
			bonusTerm1.SetTerm(BrowsedSkin.bonusTerms[0]);
			UpdateBonusParms(BrowsedSkin.id);
		}
		skinNumberText.text = $"{BrowsedSkinIndex + 1} / {ManagerBase<SkinManager>.Instance.Skins.Count}";
		if (BrowsedSkin.isPurchased)
		{
			purchaseSkinButton.gameObject.SetActive(value: false);
			pricePanel.SetActive(value: false);
			skinClosedPanel.SetActive(value: false);
			return;
		}
		bool flag = ManagerBase<PlayerManager>.Instance.Level < BrowsedSkin.UnlockLevel;
		bool flag2 = ManagerBase<PlayerManager>.Instance.CurCoins >= BrowsedSkin.price;
		skinClosedPanel.SetActive(flag);
		pricePanel.SetActive(!flag);
		purchaseSkinButton.gameObject.SetActive(!flag);
		if (flag)
		{
			closedLevelTermParam.SetParameterValue("level", BrowsedSkin.UnlockLevel.ToString());
		}
		else
		{
			priceText.color = (flag2 ? ManagerBase<UIManager>.Instance.EnoughCoinsTextColor : ManagerBase<UIManager>.Instance.NotEnoughCoinsTextColor);
			priceText.text = BrowsedSkin.price.ToString();
		}
		purchaseSkinButton.interactable = (flag2 && !flag);
		purchaseSkinButtonCanvasGroup.alpha = ((flag2 && !flag) ? 1f : disableButtonAlpha);
	}

	private void UpdateBonusParms(SkinManager.SkinId skinId)
	{
		switch (skinId)
		{
		case SkinManager.SkinId.Abyssinian:
			bonusTermParam1.SetParameterValue("bonus", ManagerBase<SkinManager>.Instance.AbyssinianThirstAndSatietyBonus.ToString());
			bonusTermParam2.SetParameterValue("bonus", ManagerBase<SkinManager>.Instance.AbyssinianMoveSpeedBonus.ToString());
			break;
		case SkinManager.SkinId.Alien:
			bonusTermParam1.SetParameterValue("bonus", ManagerBase<SkinManager>.Instance.AlienMaxHealthBonus.ToString());
			bonusTermParam1.SetParameterValue("value", ManagerBase<SkinManager>.Instance.AlienHitPowerBonus.ToString());
			break;
		case SkinManager.SkinId.Bengal:
			bonusTermParam1.SetParameterValue("bonus", ManagerBase<SkinManager>.Instance.BengalHitPowerBonus.ToString());
			bonusTermParam2.SetParameterValue("bonus", ManagerBase<SkinManager>.Instance.BengalMoveSpeedBonus.ToString());
			break;
		case SkinManager.SkinId.Bombay:
			bonusTermParam1.SetParameterValue("bonus", ManagerBase<SkinManager>.Instance.BombayStealthHitPowerBonus.ToString());
			bonusTermParam2.SetParameterValue("bonus", ManagerBase<SkinManager>.Instance.BombayStealthMoveSpeedBonus.ToString());
			break;
		case SkinManager.SkinId.Burmilla:
			bonusTermParam1.SetParameterValue("bonus", ManagerBase<SkinManager>.Instance.BurmillaHealthByFamilyCountBonus.ToString());
			bonusTermParam2.SetParameterValue("bonus", ManagerBase<SkinManager>.Instance.BurmillaFarmResidentCoinsMultBonus.ToString());
			break;
		case SkinManager.SkinId.Egyptian:
			bonusTermParam1.SetParameterValue("bonus", ManagerBase<SkinManager>.Instance.EgyptianMoveSpeedBonus.ToString());
			break;
		case SkinManager.SkinId.Pixibob:
			bonusTermParam1.SetParameterValue("bonus", ManagerBase<SkinManager>.Instance.PixibobEatingSpeedBonus.ToString());
			bonusTermParam2.SetParameterValue("bonus", (ManagerBase<SkinManager>.Instance.PixibobEnemyHealthPerc * 100f).ToString());
			break;
		case SkinManager.SkinId.Russian:
			bonusTermParam1.SetParameterValue("bonus", ManagerBase<SkinManager>.Instance.RussianMaxHealthBonus.ToString());
			bonusTermParam2.SetParameterValue("bonus", ManagerBase<SkinManager>.Instance.RussianFarmResidentExpBonus.ToString());
			break;
		case SkinManager.SkinId.Siamese:
			bonusTermParam1.SetParameterValue("bonus", ManagerBase<SkinManager>.Instance.SiameseCoinIncBonus.ToString());
			bonusTermParam2.SetParameterValue("value", (ManagerBase<SkinManager>.Instance.SiameseHealthPart * 100f).ToString());
			bonusTermParam2.SetParameterValue("bonus", ManagerBase<SkinManager>.Instance.SiameseHitPowerBonus.ToString());
			break;
		case SkinManager.SkinId.Mainecoon:
			bonusTermParam1.SetParameterValue("bonus", ManagerBase<SkinManager>.Instance.MainecoonMaxHealthBonus.ToString());
			bonusTermParam2.SetParameterValue("bonus", ManagerBase<SkinManager>.Instance.MainecoonCoinsFromMiniBossBonus.ToString());
			break;
		case SkinManager.SkinId.Scottish:
			bonusTermParam1.SetParameterValue("value", ManagerBase<SkinManager>.Instance.ScottishHitCountsPerCoinDropBonus.ToString());
			break;
		}
	}

	public void BrowseOtherProduct(bool nextProduct)
	{
		if (nextProduct)
		{
			browsedSkinIndex++;
		}
		else
		{
			browsedSkinIndex--;
		}
		if (browsedSkinIndex < 0)
		{
			BrowsedSkinIndex = ManagerBase<SkinManager>.Instance.Skins.Count - 1;
		}
		else if (BrowsedSkinIndex >= ManagerBase<SkinManager>.Instance.Skins.Count)
		{
			BrowsedSkinIndex = 0;
		}
		else
		{
			BrowsedSkinIndex = browsedSkinIndex;
		}
		if (BrowsedSkin.isPurchased)
		{
			SelectSkin();
		}
		UpdateUI();
	}

	public void ChangeGender(bool isMaleGender)
	{
		if ((!isMaleGender || ManagerBase<PlayerManager>.Instance.gender != 0) && (isMaleGender || ManagerBase<PlayerManager>.Instance.gender != FamilyManager.GenderType.Female))
		{
			if (isMaleGender)
			{
				maleGenderButton.SetActiveButton();
				femaleGenderButton.SetInactiveButton();
			}
			else
			{
				maleGenderButton.SetInactiveButton();
				femaleGenderButton.SetActiveButton();
			}
			ManagerBase<PlayerManager>.Instance.gender = ((!isMaleGender) ? FamilyManager.GenderType.Female : FamilyManager.GenderType.Male);
			if (ManagerBase<FamilyManager>.Instance.HaveSpouse)
			{
				ManagerBase<FamilyManager>.Instance.family.Find((FamilyManager.FamilyMemberData x) => x.role == FamilyManager.FamilyMemberRole.Spouse).genderType = (isMaleGender ? FamilyManager.GenderType.Female : FamilyManager.GenderType.Male);
			}
			Avelog.Input.FireChangeGenderPressed();
		}
	}

	public void SelectSkin()
	{
		if (BrowsedSkin.isPurchased)
		{
			ManagerBase<SkinManager>.Instance.CurrentSkinIndex = BrowsedSkinIndex;
			UpdateUI();
		}
	}

	public void BuySkin()
	{
		if (!BrowsedSkin.isPurchased && ManagerBase<PlayerManager>.Instance.CurCoins >= BrowsedSkin.price)
		{
			ManagerBase<SkinManager>.Instance.BuySkin(BrowsedSkin);
			SelectSkin();
		}
	}

	public void CloseWindow()
	{
		if (BrowsedSkinIndex != ManagerBase<SkinManager>.Instance.CurrentSkinIndex)
		{
			BrowsedSkinIndex = ManagerBase<SkinManager>.Instance.CurrentSkinIndex;
		}
		WindowSingleton<MenuWindow>.Instance.Open();
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		{
			CloseWindow();
		}
	}
}
