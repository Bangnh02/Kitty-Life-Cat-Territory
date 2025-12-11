using UnityEngine;

public abstract class Quest : MonoBehaviour
{
	public class QuestReward
	{
		private float experience;

		private int coins;

		public float Experience => experience;

		public int Coins
		{
			get
			{
				if (ManagerBase<SkinManager>.Instance.CurrentSkin.id == SkinManager.SkinId.Siamese)
				{
					return coins + ManagerBase<SkinManager>.Instance.SiameseCoinIncBonus;
				}
				return coins;
			}
		}

		public QuestReward(float experience, int coins)
		{
			this.experience = experience;
			this.coins = coins;
		}
	}

	public delegate void QuestHandler(Quest quest);

	private QuestCategory _category;

	private float curProgress;

	[SerializeField]
	private float experience = 50f;

	[SerializeField]
	private int coins = 20;

	public QuestReward Reward;

	public new string name
	{
		get
		{
			return base.gameObject.name;
		}
		private set
		{
			base.gameObject.name = value;
		}
	}

	public string DescriptionTerm => "QuestDescriptions/" + name;

	public QuestCategory Category
	{
		get
		{
			if (_category == null)
			{
				_category = GetComponentInParent<QuestCategory>();
			}
			return _category;
		}
	}

	public float CurProgress
	{
		get
		{
			return curProgress;
		}
		protected set
		{
			curProgress = Mathf.Clamp(value, 0f, MaxProgress);
			if (IsInProgress)
			{
				Quest.updateProgressEvent?.Invoke(this);
			}
			if (curProgress == MaxProgress && IsInProgress)
			{
				CompleteQuest();
			}
		}
	}

	public float MaxProgress
	{
		get;
		private set;
	}

	public bool IsInProgress
	{
		get;
		private set;
	}

	public static event QuestHandler startEvent;

	public static event QuestHandler updateProgressEvent;

	public static event QuestHandler completeEvent;

	public static event QuestHandler cancelEvent;

	public abstract bool CanStart();

	public bool StartQuest()
	{
		if (IsInProgress || !CanStart())
		{
			return false;
		}
		MaxProgress = CalculateMaxProgress();
		float baseExperience = experience;
		int baseCoins = coins;
		Reward = CalculateReward(baseExperience, baseCoins);
		IsInProgress = true;
		OnStart();
		Quest.startEvent?.Invoke(this);
		return true;
	}

	protected abstract float CalculateMaxProgress();

	private void CompleteQuest()
	{
		OnEnd();
		IsInProgress = false;
		CurProgress = 0f;
		Quest.completeEvent?.Invoke(this);
	}

	public void CancelQuest()
	{
		OnEnd();
		IsInProgress = false;
		CurProgress = 0f;
		Quest.cancelEvent?.Invoke(this);
	}

	protected virtual QuestReward CalculateReward(float baseExperience, int baseCoins)
	{
		return new QuestReward(baseExperience, baseCoins);
	}

	protected abstract void OnStart();

	protected abstract void OnEnd();
}
