using I2.Loc;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuperBonusPanel : ExtraPanel, IInitializableUI
{
	private class SuperBonusTerms
	{
		public SuperBonus.Id id;

		public string nameTerm;

		public string descriptionTerm;

		public SuperBonusTerms(SuperBonus.Id id, string nameTerm, string descriptionTerm)
		{
			this.id = id;
			this.nameTerm = nameTerm;
			this.descriptionTerm = descriptionTerm;
		}
	}

	[Header("SuperBonusPanel")]
	[SerializeField]
	private Localize superBonusNameLoc;

	[SerializeField]
	private Localize superBonusDescriptionLoc;

	[SerializeField]
	private LocalizationParamsManager superBonusDescriptionParam;

	[SerializeField]
	private Text superBonusCountText;

	[SerializeField]
	private Image superBonusIcon;

	private const string superBonusesTermCategory = "SuperBonuses/";

	private List<SuperBonusTerms> bonusTerms = new List<SuperBonusTerms>
	{
		new SuperBonusTerms(SuperBonus.Id.Acorn, "AcornName", "AcornDescription"),
		new SuperBonusTerms(SuperBonus.Id.BootsWalkers, "BootsName", "BootsDescription"),
		new SuperBonusTerms(SuperBonus.Id.Milk, "MilkName", "MilkDescription")
	};

	private const string timeParam = "time";

	public static SuperBonusPanel Instance
	{
		get;
		private set;
	}

	public static event Action startProcessingEvent;

	public void OnInitializeUI()
	{
		Instance = this;
		SuperBonus.pickEvent += OnSuperBonusPick;
		canvasRenderers = new List<CanvasRenderer>(GetComponentsInChildren<CanvasRenderer>(includeInactive: true));
		curState = State.Disabled;
		ManagerBase<FarmResidentManager>.Instance.superBonusLifeTime = changeScaleTime + viewPanelTime + changeAlphaTime;
		ManagerBase<FarmResidentManager>.Instance.superBonusFirstLifeTime = changeScaleTime + viewPanelTime * firstViewPanleTimeMulty + changeAlphaTime;
	}

	private void OnDestroy()
	{
		SuperBonus.pickEvent -= OnSuperBonusPick;
	}

	private void OnSuperBonusPick(FarmResidentManager.SuperBonusData superBonusData)
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		SuperBonusTerms superBonusTerms = bonusTerms.Find((SuperBonusTerms x) => x.id == superBonusData.id);
		superBonusNameLoc.SetTerm("SuperBonuses/" + superBonusTerms.nameTerm);
		superBonusDescriptionLoc.SetTerm("SuperBonuses/" + superBonusTerms.descriptionTerm);
		superBonusDescriptionParam.SetParameterValue("time", (ManagerBase<FarmResidentManager>.Instance.SuperBonusTime / 60f).ToString());
		superBonusIcon.sprite = ManagerBase<UIManager>.Instance.GetSuperBonusSprite(superBonusTerms.id);
		superBonusCountText.text = GetHelpProggres(superBonusTerms.id);
		canvasRenderers.ForEach(delegate(CanvasRenderer x)
		{
			x.SetAlpha(1f);
		});
		base.transform.localScale = Vector3.zero;
		CheckFirstEnablePanel(superBonusTerms.id);
		if (coroutine != null)
		{
			StopCoroutine(coroutine);
		}
		if (base.gameObject.activeInHierarchy)
		{
			coroutine = StartCoroutine(ChangeScaleCoroutine());
		}
		else
		{
			curState = State.ChangeScale;
		}
		SuperBonusPanel.startProcessingEvent?.Invoke();
	}

	private void CheckFirstEnablePanel(SuperBonus.Id superBonusId)
	{
		switch (superBonusId)
		{
		case SuperBonus.Id.Acorn:
			isFirstEnablePanel = (ManagerBase<FarmResidentManager>.Instance.FarmResidentsData.Find((FarmResidentManager.FarmResidentData x) => x.farmResidentId == FarmResidentId.Pig).helpProgressCurrent == 1);
			break;
		case SuperBonus.Id.BootsWalkers:
			isFirstEnablePanel = (ManagerBase<FarmResidentManager>.Instance.FarmResidentsData.Find((FarmResidentManager.FarmResidentData x) => x.farmResidentId == FarmResidentId.Farmer).helpProgressCurrent == 1);
			break;
		case SuperBonus.Id.Milk:
			isFirstEnablePanel = (ManagerBase<FarmResidentManager>.Instance.FarmResidentsData.Find((FarmResidentManager.FarmResidentData x) => x.farmResidentId == FarmResidentId.Goat).helpProgressCurrent == 1);
			break;
		}
	}

	private string GetHelpProggres(SuperBonus.Id superBonusId)
	{
		string str = "";
		switch (superBonusId)
		{
		case SuperBonus.Id.Acorn:
			str = ManagerBase<FarmResidentManager>.Instance.FarmResidentsData.Find((FarmResidentManager.FarmResidentData x) => x.farmResidentId == FarmResidentId.Pig).helpProgressCurrent.ToString();
			break;
		case SuperBonus.Id.BootsWalkers:
			str = ManagerBase<FarmResidentManager>.Instance.FarmResidentsData.Find((FarmResidentManager.FarmResidentData x) => x.farmResidentId == FarmResidentId.Farmer).helpProgressCurrent.ToString();
			break;
		case SuperBonus.Id.Milk:
			str = ManagerBase<FarmResidentManager>.Instance.FarmResidentsData.Find((FarmResidentManager.FarmResidentData x) => x.farmResidentId == FarmResidentId.Goat).helpProgressCurrent.ToString();
			break;
		}
		return str + $"/{ManagerBase<FarmResidentManager>.Instance.HelpProgressMaximum}";
	}
}
