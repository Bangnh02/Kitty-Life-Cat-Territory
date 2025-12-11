using Avelog;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlantVegetableQuest : Quest
{
	[SerializeField]
	private float coinsExceedsCostMulty = 3f;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Minutes)]
	private float questFrequencyMin = 20f;

	public float QuestFrequency => questFrequencyMin * 60f;

	public override bool CanStart()
	{
		if (ManagerBase<VegetableManager>.Instance.vegetablesData.Count <= 0)
		{
			return false;
		}
		IEnumerable<VegetableManager.VegetableData> source = from x in ManagerBase<VegetableManager>.Instance.vegetablesData
			where !x.isPlanted
			select x;
		bool flag = source.Count() > 0;
		bool flag2 = source.Any((VegetableManager.VegetableData x) => (float)ManagerBase<PlayerManager>.Instance.CurCoins >= (float)x.vegetableParams.cost * coinsExceedsCostMulty);
		if ((base.Category as PlantingQuestCategory).MySaveData.completePlantQuestTime > TimeUtils.GetDeviceTime(TimeUtils.TimeType.System))
		{
			(base.Category as PlantingQuestCategory).MySaveData.completePlantQuestTime = TimeUtils.GetDeviceTime(TimeUtils.TimeType.System);
		}
		if (flag && flag2)
		{
			return (base.Category as PlantingQuestCategory).MySaveData.completePlantQuestTime + (long)QuestFrequency < TimeUtils.GetDeviceTime(TimeUtils.TimeType.System);
		}
		return false;
	}

	protected override float CalculateMaxProgress()
	{
		return 1f;
	}

	protected override void OnStart()
	{
		VegetableBehaviour.plantEvent += OnPlant;
		PlayerManager.coinsChangeEvent += OnCoinsChange;
	}

	private void OnPlant(VegetableBehaviour vegetable)
	{
		base.CurProgress = base.MaxProgress;
	}

	private void OnCoinsChange(int coinsInc = 0)
	{
		int num = ManagerBase<VegetableManager>.Instance.vegetablesParams.Min((VegetableManager.VegetableParams x) => x.cost);
		if (ManagerBase<PlayerManager>.Instance.CurCoins < num)
		{
			Singleton<QuestSpawner>.Instance.CancelQuest();
		}
	}

	protected override void OnEnd()
	{
		VegetableBehaviour.plantEvent -= OnPlant;
		PlayerManager.coinsChangeEvent -= OnCoinsChange;
	}
}
