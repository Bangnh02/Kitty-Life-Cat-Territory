using UnityEngine;

public class Food : Item
{
	public delegate void FoodHandler(Food food);

	[SerializeField]
	[Header("Настройка еды")]
	protected float foodEffect;

	[SerializeField]
	private bool isBadFood;

	private const int countFoodUnits = 1;

	[SerializeField]
	private Vector3 worldPanelOffset = Vector3.zero;

	[Header("Отладка")]
	[SerializeField]
	[ReadonlyInspector]
	private int _curCountFoodUnits;

	[SerializeField]
	[ReadonlyInspector]
	private int _countOccupiedFoodUnits;

	private SpawnPoint myPoint;

	public float FoodEffect => foodEffect;

	public bool IsBadFood => isBadFood;

	public Vector3 WorldPanelOffset => worldPanelOffset;

	public int CurCountFoodUnits
	{
		get
		{
			return _curCountFoodUnits;
		}
		protected set
		{
			_curCountFoodUnits = value;
			FireUpdatePickableStateEvent();
		}
	}

	public int CountOccupiedFoodUnits
	{
		get
		{
			return _countOccupiedFoodUnits;
		}
		private set
		{
			_countOccupiedFoodUnits = value;
			FireUpdatePickableStateEvent();
		}
	}

	public bool IsFoodOccupied => CountOccupiedFoodUnits > 0;

	public virtual bool Eatable => true;

	public virtual string Name => base.Id.ToString();

	public static event FoodHandler holdFoodEvent;

	public static event FoodHandler unholdFoodEvent;

	public static event FoodHandler unspawnEvent;

	private void Awake()
	{
		base.ProcessingSwitch = GetComponent<ProcessingSwitch>();
		base.ProcessingSwitch.maxProcessingDistance = Singleton<ItemSpawner>.Instance.ProcessingDistance;
		base.ProcessingSwitch.UpdateProcessingState();
	}

	protected override void Start()
	{
		base.Start();
		PlayerSpawner.spawnPlayerEvent += OnSpawnPlayer;
		if (PlayerSpawner.IsPlayerSpawned)
		{
			OnSpawnPlayer();
		}
	}

	private void OnDestroy()
	{
		PlayerSpawner.spawnPlayerEvent -= OnSpawnPlayer;
		if (PlayerSpawner.PlayerInstance != null)
		{
			PlayerSpawner.PlayerInstance.PlayerCombat.changeLifeStateEvent -= OnPlayerChangeLifeState;
		}
	}

	private void OnSpawnPlayer()
	{
		PlayerSpawner.PlayerInstance.PlayerCombat.changeLifeStateEvent += OnPlayerChangeLifeState;
	}

	private void OnPlayerChangeLifeState(ActorCombat.LifeState state)
	{
		if (state != 0)
		{
			CountOccupiedFoodUnits = 0;
			Food.unholdFoodEvent?.Invoke(this);
		}
	}

	private void OnEnable()
	{
		CurCountFoodUnits = 1;
		myPoint = GetComponentInParent<SpawnPoint>();
	}

	public void HoldFoodUnit(ItemUser ItemUser)
	{
		int num = ++CountOccupiedFoodUnits;
		base.CurItemUser = ItemUser;
		Food.holdFoodEvent?.Invoke(this);
	}

	public void UnholdFoodUnit()
	{
		int num = --CountOccupiedFoodUnits;
		base.CurItemUser = null;
		Food.unholdFoodEvent?.Invoke(this);
	}

	public virtual void EatFoodUnit()
	{
		CurCountFoodUnits--;
		UnholdFoodUnit();
		if (CurCountFoodUnits == 0 && CountOccupiedFoodUnits == 0)
		{
			Unspawn();
		}
	}

	public void Unspawn()
	{
		if (myPoint != null)
		{
			myPoint.IsBusy = false;
		}
		ResetTransform();
		base.gameObject.SetActive(value: false);
		base.MeshRenderer.enabled = true;
		if (this is EnemyFood)
		{
			Singleton<ItemSpawner>.Instance.AddNonSpawnedEnemyItem(base.gameObject, base.Id);
		}
		else
		{
			Singleton<ItemSpawner>.Instance.AddNonSpawnedItem(base.gameObject, base.Id);
		}
		Food.unspawnEvent?.Invoke(this);
		FireUpdatePickableStateEvent();
	}

	public override bool CanBePicked(ItemUser itemUser)
	{
		if (base.CanBePicked(itemUser))
		{
			return CurCountFoodUnits > 0;
		}
		return false;
	}
}
