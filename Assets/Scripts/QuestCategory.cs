using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestCategory : MonoBehaviour
{
	[Serializable]
	public class SaveData
	{
		public string categoryName;
	}

	private SaveData _saveData;

	protected List<Quest> categoryQuests;

	protected SaveData MySaveData
	{
		get
		{
			if (_saveData == null)
			{
				_saveData = ManagerBase<QuestManager>.Instance.questCategoriesData.Find((SaveData x) => x.categoryName == base.gameObject.name);
				if (_saveData == null)
				{
					_saveData = CreateSaveData();
					ManagerBase<QuestManager>.Instance.questCategoriesData.Add(_saveData);
				}
			}
			return _saveData;
		}
	}

	protected virtual SaveData CreateSaveData()
	{
		return new SaveData
		{
			categoryName = base.gameObject.name
		};
	}

	public void Init()
	{
		categoryQuests = new List<Quest>(GetComponentsInChildren<Quest>());
		OnInit();
	}

	protected abstract void OnInit();

	public abstract Quest SelectQuest();
}
