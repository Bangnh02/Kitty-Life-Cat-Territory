using System;
using UnityEngine;

public abstract class StandartQuestCategory : QuestCategory
{
	[Serializable]
	public new class SaveData : QuestCategory.SaveData
	{
		public int сurPriority;
	}

	[SerializeField]
	private int priority;

	private SaveData MyData => base.MySaveData as SaveData;

	public int Priority => priority;

	public int CurPriority
	{
		get
		{
			return MyData.сurPriority;
		}
		set
		{
			MyData.сurPriority = Mathf.Clamp(value, 0, Priority);
		}
	}

	protected override QuestCategory.SaveData CreateSaveData()
	{
		return new SaveData
		{
			categoryName = base.gameObject.name,
			сurPriority = priority
		};
	}
}
