using System;
using System.Collections.Generic;
using UnityEngine;

public class SkinManager : ManagerBase<SkinManager>
{
	public enum SkinId
	{
		Abyssinian,
		Alien,
		Bengal,
		Bombay,
		Burmilla,
		Egyptian,
		Pixibob,
		Russian,
		Siamese,
		Simple,
		Mainecoon,
		Scottish
	}

	[Serializable]
	public class Skin
	{
		public SkinId id;

		public string nameTerm;

		public string descriptionTerm;

		public List<string> bonusTerms;

		[SerializeField]
		private int unlockLevel;

		public int price;

		public bool isPurchased;

		public Mesh catMesh;

		public Material catMaleMaterial;

		public Material catFemaleMaterial;

		public Mesh kittenMesh;

		public Material kittenMaleMaterial;

		public Material kittenFemaleMaterial;

		public int UnlockLevel
		{
			get
			{
				if (!ManagerBase<SkinManager>.Instance.haveLevelLimits)
				{
					return 1;
				}
				return unlockLevel;
			}
		}
	}

	[Save]
	[ReadonlyInspector]
	private int currentSkinIndex;

	private int newCurrentSkinIndex;

	[NonSerialized]
	private int previewSkinIndex;

	[Save]
	private List<(SkinId id, bool state)> skinsPurchaseState = new List<(SkinId, bool)>();

	[SerializeField]
	private List<Skin> skins = new List<Skin>();

	public bool haveLevelLimits = true;

	[Header("Бонусы от скинов")]
	[SerializeField]
	private int siameseCoinIncBonus = 3;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private float siameseHealthPerc = 70f;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private float siameseHitPowerBonus = 15f;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private float bombayStealthHitPowerBonus = 30f;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private float bombayStealthMoveSpeedBonus = 20f;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private float russianMaxHealthBonus = 20f;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private float russianFarmResidentExpBonus = 200f;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private float burmillaHealthByFamilyCountBonus = 8f;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private int burmillaFarmResidentCoinsMultBonus = 2;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private float abyssinianThirstAndsatietyBonus = 20f;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private float abyssinianMoveSpeedBonus = 10f;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private float egyptianMoveSpeedBonus = 25f;

	[SerializeField]
	private float egyptianJumpPowerBonus = 25f;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private float egyptianDrinkSpeedBonus = 50f;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private float bengalHitPowerBonus = 25f;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private float bengalMoveSpeedBonus = 70f;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private float pixibobEatingSpeedBonus = 50f;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private float pixibobEnemyHealthPerc = 30f;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private float alienMaxHealthBonus = 100f;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private float alienHitPowerBonus = 100f;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private float mainecoonMaxHealthBonus = 25f;

	[SerializeField]
	private int mainecoonCoinsFromMiniBossBonus = 25;

	[SerializeField]
	private int scottishHitCountsPerCoinDropBonus = 20;

	[SerializeField]
	private float scottishCoinsPickDistanceBonus = 3f;

	public int CurrentSkinIndex
	{
		get
		{
			return currentSkinIndex;
		}
		set
		{
			currentSkinIndex = value;
			SkinManager.currentSkinChangedEvent?.Invoke();
		}
	}

	public Skin CurrentSkin => skins[currentSkinIndex];

	public int PreviewSkinIndex
	{
		get
		{
			return previewSkinIndex;
		}
		set
		{
			previewSkinIndex = value;
			SkinManager.previewSkinChangedEvent?.Invoke();
		}
	}

	public Skin PreviewSkin => skins[PreviewSkinIndex];

	public List<Skin> Skins => skins;

	public int SiameseCoinIncBonus => siameseCoinIncBonus;

	public float SiameseHealthPart => siameseHealthPerc / 100f;

	public float SiameseHitPowerBonus => siameseHitPowerBonus;

	public float BombayStealthHitPowerBonus => bombayStealthHitPowerBonus;

	public float BombayStealthMoveSpeedBonus => bombayStealthMoveSpeedBonus;

	public float RussianMaxHealthBonus => russianMaxHealthBonus;

	public float RussianFarmResidentExpBonus => russianFarmResidentExpBonus;

	public float BurmillaHealthByFamilyCountBonus => burmillaHealthByFamilyCountBonus;

	public int BurmillaFarmResidentCoinsMultBonus => burmillaFarmResidentCoinsMultBonus;

	public float AbyssinianThirstAndSatietyBonus => abyssinianThirstAndsatietyBonus;

	public float AbyssinianMoveSpeedBonus => abyssinianMoveSpeedBonus;

	public float EgyptianMoveSpeedBonus => egyptianMoveSpeedBonus;

	public float EgyptianJumpPowerBonus => egyptianJumpPowerBonus;

	public float EgyptianDrinkSpeedBonus => egyptianDrinkSpeedBonus;

	public float BengalHitPowerBonus => bengalHitPowerBonus;

	public float BengalMoveSpeedBonus => bengalMoveSpeedBonus;

	public float PixibobEatingSpeedBonus => pixibobEatingSpeedBonus;

	public float PixibobEnemyHealthPerc => pixibobEnemyHealthPerc / 100f;

	public float AlienMaxHealthBonus => alienMaxHealthBonus;

	public float AlienHitPowerBonus => alienHitPowerBonus;

	public float MainecoonMaxHealthBonus => mainecoonMaxHealthBonus;

	public int MainecoonCoinsFromMiniBossBonus => mainecoonCoinsFromMiniBossBonus;

	public int ScottishHitCountsPerCoinDropBonus => scottishHitCountsPerCoinDropBonus;

	public float ScottishCoinsPickDistanceBonus => scottishCoinsPickDistanceBonus;

	public static event Action currentSkinChangedEvent;

	public static event Action previewSkinChangedEvent;

	public static event Action buySkinEvent;

	protected override void OnInit()
	{
		if (Skins.Count > skinsPurchaseState.Count)
		{
			Skin currentSkin = CurrentSkin;
			Skins.Sort((Skin o1, Skin o2) => o1.price.CompareTo(o2.price));
			CurrentSkinIndex = Skins.IndexOf(currentSkin);
			newCurrentSkinIndex = CurrentSkinIndex;
		}
		else
		{
			Skins.Sort((Skin o1, Skin o2) => o1.price.CompareTo(o2.price));
		}
		SaveManager.SaveStartEvent += OnSaveStart;
		SaveManager.LocalSaveStartEvent += OnSaveStart;
		SaveManager.LoadEndEvent += OnLoadEnd;
		if (ManagerBase<SaveManager>.Instance.IsDataLoaded)
		{
			OnLoadEnd();
		}
	}

	private void OnDestroy()
	{
		SaveManager.SaveStartEvent -= OnSaveStart;
		SaveManager.LocalSaveStartEvent -= OnSaveStart;
		SaveManager.LoadEndEvent -= OnLoadEnd;
	}

	private void OnSaveStart()
	{
		skinsPurchaseState.Clear();
		foreach (Skin skin in Skins)
		{
			skinsPurchaseState.Add((skin.id, skin.isPurchased));
		}
	}

	private void OnLoadEnd()
	{
		if (Skins.Count > skinsPurchaseState.Count)
		{
			CurrentSkinIndex = newCurrentSkinIndex;
		}
		foreach (var item in skinsPurchaseState)
		{
			(SkinId id, bool state) skinPurchaseState = item;
			if (skinPurchaseState.state)
			{
				Skins.Find((Skin x) => x.id == skinPurchaseState.id).isPurchased = skinPurchaseState.state;
			}
		}
	}

	public void BuySkin(Skin skin)
	{
		if (ManagerBase<PlayerManager>.Instance.CurCoins >= skin.price)
		{
			ManagerBase<PlayerManager>.Instance.ChangeCoins(-skin.price);
			skin.isPurchased = true;
			SkinManager.buySkinEvent?.Invoke();
			ManagerBase<SaveManager>.Instance.SaveToLocal();
		}
	}
}
