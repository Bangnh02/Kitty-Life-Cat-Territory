using UnityEngine;

public class CollectCoinQuest : Quest
{
	[SerializeField]
	[Tooltip("Число для задания случайного необходимого прогресса (минимум)")]
	private int progressRandomVal1;

	[SerializeField]
	[Tooltip("Число для задания случайного необходимого прогресса (максимум)")]
	private int progressRandomVal2;

	protected int ProgressRandomVal1 => progressRandomVal1;

	protected int ProgressRandomVal2 => progressRandomVal2;

	public override bool CanStart()
	{
		return true;
	}

	protected override float CalculateMaxProgress()
	{
		int max = Mathf.Min(ProgressRandomVal1 + (base.Category as CollectionQuestCategory).MySaveData.completedQuestsCount, ProgressRandomVal2 + 1);
		return Mathf.Clamp(Random.Range(ProgressRandomVal1, ProgressRandomVal2 + 1), ProgressRandomVal1, max);
	}

	protected override void OnStart()
	{
		Coin.pickEvent += OnCoinPick;
	}

	protected override void OnEnd()
	{
		Coin.pickEvent -= OnCoinPick;
	}

	private void OnCoinPick(bool isSpawnedCoin)
	{
		if (isSpawnedCoin)
		{
			float num = base.CurProgress += 1f;
		}
	}
}
