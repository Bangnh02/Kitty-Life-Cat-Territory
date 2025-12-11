using Avelog;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManager : ManagerBase<UIManager>
{
	[Serializable]
	private class ItemSprite
	{
		public ItemId id;

		public Sprite sprite;
	}

	[Serializable]
	public class ArchetypeSprite
	{
		public EnemyArchetypeReference enemyArchetypeRef;

		public Sprite sprite;

		public Sprite foodSprite;
	}

	[Serializable]
	private class RoleSprite
	{
		public FamilyManager.FamilyMemberRole role;

		public FamilyManager.GenderType gender;

		public Sprite sprite;
	}

	[Serializable]
	public class QuestHints
	{
		public string questType;

		public List<Sprite> sprites;
	}

	public enum Demand
	{
		None,
		Eat,
		Drink,
		Hunt
	}

	[Serializable]
	public class DemandSprite
	{
		public Demand demand;

		public Sprite sprite;
	}

	[Serializable]
	public class FarmResidentSprite
	{
		public FarmResidentId id;

		public Sprite sprite;
	}

	[Serializable]
	public class SuperBonusSprite
	{
		public SuperBonus.Id id;

		public Sprite sprite;
	}

	[Serializable]
	public class HelpHintSprites
	{
		public HelpManager.Hint hint;

		public List<Sprite> sprites;
	}

	[Serializable]
	public class ChildSprite
	{
		public FamilyManager.FamilyMemberRole role;

		public Sprite maleSprite;

		public Sprite femaleSprite;
	}

	[Header("Графика")]
	[SerializeField]
	private Sprite waterSprite;

	[SerializeField]
	private Sprite coinIcon;

	[SerializeField]
	private Sprite smallCoinSprite;

	[SerializeField]
	private Sprite clewIcon;

	[SerializeField]
	private Sprite experienceIcon;

	[SerializeField]
	private Sprite shovelIcon;

	[SerializeField]
	private List<ItemSprite> itemSprites;

	[SerializeField]
	private List<ArchetypeSprite> archetypeSprites;

	[SerializeField]
	private List<QuestHints> questHints;

	[SerializeField]
	private List<RoleSprite> roleSprites;

	[SerializeField]
	private List<FarmResidentSprite> farmResidentSprites;

	[SerializeField]
	private List<SuperBonusSprite> superBonusSprites;

	[SerializeField]
	private List<HelpHintSprites> helpHintSprites;

	[SerializeField]
	private List<ChildSprite> childSprites;

	[SerializeField]
	private Sprite paramZeroSprite;

	[SerializeField]
	private Sprite paramFallSprite;

	[SerializeField]
	private Color temporarySuperBonusFillColor;

	[SerializeField]
	private Color permanentSuperBonusFillColor;

	[SerializeField]
	private Color enoughCoinsTextColor;

	[SerializeField]
	private Color notEnoughCoinsTextColor;

	private List<ItemId> itemIds;

	[Header("Звуки")]
	[SerializeField]
	private AudioSource uiAudioSource;

	[SerializeField]
	private AudioClipVolume mapButtonSound;

	[SerializeField]
	private AudioClipVolume skinsButtonSound;

	[SerializeField]
	private AudioClipVolume buyButtonSound;

	[SerializeField]
	private AudioClipVolume universalButtonSound;

	[SerializeField]
	private AudioClipVolume hintOpenSound;

	public Sprite CoinIcon => coinIcon;

	public Sprite SmallCoinSprite => smallCoinSprite;

	public Sprite ClewIcon => clewIcon;

	public Sprite ExperienceIcon => experienceIcon;

	public Sprite ShovelIcon => shovelIcon;

	public Sprite ParamZeroSprite => paramZeroSprite;

	public Sprite ParamFallSprite => paramFallSprite;

	public Color TemporarySuperBonusFillColor => temporarySuperBonusFillColor;

	public Color PermanentSuperBonusFillColor => permanentSuperBonusFillColor;

	public Color EnoughCoinsTextColor => enoughCoinsTextColor;

	public Color NotEnoughCoinsTextColor => notEnoughCoinsTextColor;

	protected override void OnInit()
	{
		itemIds = EnumUtils.ToList<ItemId>();
		itemIds.RemoveAll((ItemId x) => (!(x.ToString() == "None")) ? (x.ToString() == "Enemy") : true);
		uiAudioSource.ignoreListenerPause = true;
	}

	public Sprite GetFarmResidentNeedSprite(string need)
	{
		if (string.IsNullOrEmpty(need))
		{
			return null;
		}
		if (need.ToLower() == "water")
		{
			return waterSprite;
		}
		if (!itemIds.Any((ItemId x) => x.ToString() == need))
		{
			return GetArchetypeFoodSprite(need);
		}
		return GetFoodSprite(need);
	}

	public Sprite GetChildNeedSprite(string need)
	{
		if (string.IsNullOrEmpty(need))
		{
			return null;
		}
		if (need.ToLower() == "water")
		{
			return waterSprite;
		}
		if (!itemIds.Any((ItemId x) => x.ToString() == need))
		{
			return GetArchetypeSprite(need);
		}
		return GetFoodSprite(need);
	}

	public Sprite GetArchetypeSprite(EnemyArchetype enemyArchetype)
	{
		return GetArchetypeSprite(enemyArchetype.name);
	}

	public Sprite GetArchetypeSprite(string archetypeName)
	{
		return archetypeSprites.Find((ArchetypeSprite x) => x.enemyArchetypeRef.Value.gameObject.name == archetypeName).sprite;
	}

	public Sprite GetArchetypeFoodSprite(EnemyArchetype enemyArchetype)
	{
		return GetArchetypeFoodSprite(enemyArchetype.name);
	}

	public Sprite GetArchetypeFoodSprite(string archetypeName)
	{
		return archetypeSprites.Find((ArchetypeSprite x) => x.enemyArchetypeRef.Value.gameObject.name == archetypeName).foodSprite;
	}

	public Sprite GetFoodSprite(string itemId)
	{
		return itemSprites.Find((ItemSprite x) => x.id.ToString() == itemId).sprite;
	}

	public Sprite GetFoodSprite(ItemId itemId)
	{
		return itemSprites.Find((ItemSprite x) => x.id == itemId).sprite;
	}

	public List<Sprite> GetQuestHints(Quest quest)
	{
		return questHints.Find((QuestHints x) => x.questType == quest.name).sprites;
	}

	public List<Sprite> GetHelpHintsSprites(HelpManager.Hint hint)
	{
		return helpHintSprites.Find((HelpHintSprites x) => x.hint == hint).sprites;
	}

	public Sprite GetChildSprite(FamilyManager.FamilyMemberRole role, FamilyManager.GenderType gender)
	{
		ChildSprite childSprite = childSprites.Find((ChildSprite x) => x.role == role);
		if (gender == FamilyManager.GenderType.Male)
		{
			return childSprite.maleSprite;
		}
		return childSprite.femaleSprite;
	}

	public Sprite GetRoleSprite(FamilyManager.FamilyMemberRole role, FamilyManager.GenderType gender)
	{
		return (roleSprites?.Find((RoleSprite x) => x.role == role && x.gender == gender))?.sprite;
	}

	public Sprite GetFarmResidentSprite(FarmResidentId farmResidentId)
	{
		return farmResidentSprites.Find((FarmResidentSprite x) => x.id == farmResidentId).sprite;
	}

	public Sprite GetSuperBonusSprite(SuperBonus.Id superBonusId)
	{
		return superBonusSprites.Find((SuperBonusSprite x) => x.id == superBonusId).sprite;
	}

	public void PlayMapButtonSound()
	{
		uiAudioSource.clip = mapButtonSound.clip;
		uiAudioSource.volume = mapButtonSound.Volume * ManagerBase<SettingsManager>.Instance.effectsVolume;
		uiAudioSource.PlayDelayed(0f);
	}

	public void PlaySkinsButtonSound()
	{
		uiAudioSource.clip = skinsButtonSound.clip;
		uiAudioSource.volume = skinsButtonSound.Volume * ManagerBase<SettingsManager>.Instance.effectsVolume;
		uiAudioSource.PlayDelayed(0f);
	}

	public void PlayBuyButtonSound()
	{
		uiAudioSource.clip = buyButtonSound.clip;
		uiAudioSource.volume = buyButtonSound.Volume * ManagerBase<SettingsManager>.Instance.effectsVolume;
		uiAudioSource.PlayDelayed(0f);
	}

	public void PlayUniversalButtonSound()
	{
		uiAudioSource.clip = universalButtonSound.clip;
		uiAudioSource.volume = universalButtonSound.Volume * ManagerBase<SettingsManager>.Instance.effectsVolume;
		uiAudioSource.PlayDelayed(0f);
	}

	public void PlayHintOpenSound()
	{
		uiAudioSource.clip = hintOpenSound.clip;
		uiAudioSource.volume = hintOpenSound.Volume * ManagerBase<SettingsManager>.Instance.effectsVolume;
		uiAudioSource.PlayDelayed(0f);
	}
}
