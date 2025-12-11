using Avelog;
using System.Linq;
using UnityEngine;
using Input = UnityEngine.Input;

public class PlayerBrain : MonoBehaviour, IInitializablePlayerComponent
{
	public delegate void StartCommandDelegate(CommandBase command);

	[SerializeField]
	[Header("Настройка управления")]
	private KeyCode mainActionKey = KeyCode.V;

	[SerializeField]
	private KeyCode eatKey = KeyCode.F;

	[SerializeField]
	private KeyCode drinkKey = KeyCode.G;

	[SerializeField]
	private KeyCode pickOrDropKey = KeyCode.E;

	[SerializeField]
	private CatModel model;

	[SerializeField]
	private PlayerGraphic playerGraphic;

	[SerializeField]
	private Transform playerCenter;

	[SerializeField]
	private Transform superBonusPickTransform;

	[SerializeField]
	private Transform playerHead;

	private XORButtonsController mainXORButtons;

	private PlayerEatCommand eatCommand = new PlayerEatCommand();

	private PlayerJumpCommand jumpCommand = new PlayerJumpCommand();

	private PlayerDrinkCommand drinkCommand = new PlayerDrinkCommand();

	private PlayerPickCommand pickCommand = new PlayerPickCommand();

	private PlayerDropCommand dropCommand = new PlayerDropCommand();

	private PlayerAttackCommand attackCommand = new PlayerAttackCommand();

	private PlayerInvisibilityCommand invisibilityCommand = new PlayerInvisibilityCommand();

	private PlayerSleepCommand sleepCommand = new PlayerSleepCommand();

	private FamilyEatCommand familyEatCommand = new FamilyEatCommand();

	public CatModel Model => model;

	public PlayerGraphic PlayerGraphic => playerGraphic;

	public Transform PlayerCenter => playerCenter;

	public Transform SuperBonusPickTransform => superBonusPickTransform;

	public Transform PlayerHead => playerHead;

	public PlayerCombat PlayerCombat
	{
		get;
		private set;
	}

	public PlayerMovement PlayerMovement
	{
		get;
		private set;
	}

	public PlayerEating PlayerEating
	{
		get;
		private set;
	}

	public ActorPicker PlayerPicker
	{
		get;
		private set;
	}

	public PlayerFamilyController PlayerFamilyController
	{
		get;
		private set;
	}

	public PlayerExperienceController PlayerExperienceController
	{
		get;
		private set;
	}

	public PlayerPlanter PlayerPlanter
	{
		get;
		private set;
	}

	public PlayerFarmResident PlayerFarmResident
	{
		get;
		private set;
	}

	public ItemUser ItemUser
	{
		get;
		private set;
	}

	public PlayerCamera PlayerCamera
	{
		get;
		private set;
	}

	public PlayerSleepController PlayerSleepController
	{
		get;
		private set;
	}

	public PlayerIdleController PlayerIdleController
	{
		get;
		private set;
	}

	public CommandBase CurCommand
	{
		get;
		private set;
	}

	public event StartCommandDelegate beforeStartCommandEvent;

	public event StartCommandDelegate startCommandEvent;

	public event StartCommandDelegate endCommandEvent;

	public void Initialize()
	{
		PlayerCombat = GetComponent<PlayerCombat>();
		PlayerMovement = GetComponent<PlayerMovement>();
		PlayerEating = GetComponent<PlayerEating>();
		PlayerPicker = GetComponent<ActorPicker>();
		PlayerFamilyController = GetComponent<PlayerFamilyController>();
		PlayerExperienceController = GetComponent<PlayerExperienceController>();
		PlayerPlanter = GetComponent<PlayerPlanter>();
		PlayerFarmResident = GetComponent<PlayerFarmResident>();
		PlayerCamera = GetComponentInChildren<PlayerCamera>(includeInactive: true);
		PlayerSleepController = GetComponent<PlayerSleepController>();
		PlayerIdleController = GetComponent<PlayerIdleController>();
		ItemUser = GetComponent<ItemUser>();
		Avelog.Input.jumpPressedEvent += OnJumpPressed;
		Avelog.Input.attackPressedEvent += OnAttackPressed;
		Avelog.Input.invisibilityPressedEvent += OnInvisibilityPressed;
		Avelog.Input.eatPressedEvent += OnEatPressed;
		Avelog.Input.drinkPressedEvent += OnDrinkPressed;
		Avelog.Input.pickPressedEvent += OnPickPressed;
		Avelog.Input.dropPressedEvent += OnDropPressed;
		Avelog.Input.familyEatPressedEvent += OnFamilyEatPressed;
		Avelog.Input.sleepButtonPressedEvent += OnSleepButtonPressed;
		PlayerCombat.changeLifeStateEvent += OnChangeLifeState;
		PlayerCombat.takeDamageEvent += OnTakeDamage;
	}

	private void OnDestroy()
	{
		Avelog.Input.jumpPressedEvent -= OnJumpPressed;
		Avelog.Input.attackPressedEvent -= OnAttackPressed;
		Avelog.Input.invisibilityPressedEvent -= OnInvisibilityPressed;
		Avelog.Input.eatPressedEvent -= OnEatPressed;
		Avelog.Input.drinkPressedEvent -= OnDrinkPressed;
		Avelog.Input.pickPressedEvent -= OnPickPressed;
		Avelog.Input.dropPressedEvent -= OnDropPressed;
		Avelog.Input.familyEatPressedEvent -= OnFamilyEatPressed;
		Avelog.Input.sleepButtonPressedEvent -= OnSleepButtonPressed;
		PlayerCombat.changeLifeStateEvent -= OnChangeLifeState;
		PlayerCombat.takeDamageEvent -= OnTakeDamage;
	}

	private void OnJumpPressed()
	{
		TryExecuteCommand(jumpCommand);
	}

	private void OnAttackPressed()
	{
		if (!PlayerCombat.IsAttackAnimationPlaying)
		{
			TryExecuteCommand(attackCommand);
		}
	}

	private void OnInvisibilityPressed()
	{
		TryExecuteCommand(invisibilityCommand);
	}

	private void OnEatPressed()
	{
		TryExecuteCommand(eatCommand);
	}

	private void OnDrinkPressed()
	{
		TryExecuteCommand(drinkCommand);
	}

	private void OnPickPressed()
	{
		TryExecuteCommand(pickCommand);
	}

	private void OnDropPressed()
	{
		TryExecuteCommand(dropCommand);
	}

	private void OnFamilyEatPressed()
	{
		TryExecuteCommand(familyEatCommand);
	}

	private void OnSleepButtonPressed()
	{
		TryExecuteCommand(sleepCommand);
	}

	private void OnChangeLifeState(ActorCombat.LifeState state)
	{
		if (CurCommand != null && CurCommand.IsExecuting)
		{
			CancelCommand();
		}
	}

	private void OnTakeDamage(ActorCombat.TakeDamageType takeDamageType, float damage, ActorCombat attacker)
	{
		if (CurCommand != null && CurCommand.IsExecuting && takeDamageType == ActorCombat.TakeDamageType.AttackedByEnemy)
		{
			CancelCommand();
		}
	}

	private void Update()
	{
		if (PlayerCombat.CurLifeState != 0 || Time.deltaTime == 0f)
		{
			return;
		}
		if (mainXORButtons == null)
		{
			mainXORButtons = WindowSingleton<GameWindow>.Instance.GetComponentsInChildren<XORButtonsController>(includeInactive: true).FirstOrDefault((XORButtonsController x) => x.CompareTag("MainXORButtons"));
		}
		if (mainXORButtons != null && mainXORButtons.XORButtons != null)
		{
			if (UnityEngine.Input.GetKeyDown(mainActionKey))
			{
				XORButton xORButton = mainXORButtons.XORButtons.LastOrDefault((XORButton x) => x.gameObject.activeInHierarchy);
				if (xORButton != null)
				{
					if (xORButton is FamilyEatButton)
					{
						(xORButton as FamilyEatButton).FamilyEat();
					}
					else if (xORButton is PlantButton)
					{
						(xORButton as PlantButton).Plant();
					}
					else if (xORButton is MakeSpouseButton)
					{
						(xORButton as MakeSpouseButton).TryMakeSpouse();
					}
					else if (xORButton is SleepButton)
					{
						(xORButton as SleepButton).Sleep();
					}
					else if (xORButton is AttackButton)
					{
						(xORButton as AttackButton).OnPointerDown();
					}
				}
			}
			if (UnityEngine.Input.GetKeyUp(mainActionKey))
			{
				XORButton xORButton2 = mainXORButtons.XORButtons.LastOrDefault((XORButton x) => x.gameObject.activeInHierarchy);
				if (xORButton2 != null && xORButton2 is AttackButton)
				{
					(xORButton2 as AttackButton).OnPointerUp();
				}
			}
		}
		if (!PlayerCombat.IsAttackAnimationPlaying && UnityEngine.Input.GetKeyDown(KeyCode.Space))
		{
			TryExecuteCommand(jumpCommand);
		}
		if (UnityEngine.Input.GetKeyDown(drinkKey) && PlayerEating.CanDrink())
		{
			TryExecuteCommand(drinkCommand);
		}
		else if (UnityEngine.Input.GetKeyDown(eatKey) && PlayerEating.HaveEatableNearFood)
		{
			TryExecuteCommand(eatCommand);
		}
		if (UnityEngine.Input.GetKeyDown(pickOrDropKey) && PlayerPicker.HavePickableItems && !PlayerPicker.HavePickedItem)
		{
			TryExecuteCommand(pickCommand);
		}
		else if (UnityEngine.Input.GetKeyDown(pickOrDropKey) && PlayerPicker.HavePickedItem)
		{
			TryExecuteCommand(dropCommand);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.C))
		{
			TryExecuteCommand(invisibilityCommand);
		}
		if (sleepCommand.ReadyToAutoSleep())
		{
			TryExecuteCommand(sleepCommand);
		}
		if (CurCommand != null && CurCommand.IsExecuting)
		{
			PlayerMovement.CalculateMove(0f);
			if (CurCommand == sleepCommand)
			{
				PlayerMovement.Rotate(0f);
			}
		}
		else
		{
			
			//PlayerMovement.CalculateMove(Avelog.Input.VertAxis);
			PlayerMovement.Rotate(Avelog.Input.HorAxis);
			PlayerMovement.CalculateMove( UltimateJoystick.GetVerticalAxis( "Joystick" ));
			PlayerMovement.Rotate(UltimateJoystick.GetHorizontalAxis( "Joystick" ));
			
		}
		if (CurCommand != null && !CurCommand.IsExecuting)
		{
			this.endCommandEvent?.Invoke(CurCommand);
			CurCommand = null;
		}
	}

	private bool TryExecuteCommand(CommandBase command)
	{
		if (command == CurCommand || PlayerCombat.CurLifeState != 0 || PlayerMovement.IsFalling)
		{
			return false;
		}
		if (CurCommand != null && CurCommand.IsExecuting && !command.CanForceRun)
		{
			if ((command.CancelMask & CurCommand.CommandId) == CommandId.None)
			{
				return false;
			}
			CancelCommand();
		}
		this.beforeStartCommandEvent?.Invoke(command);
		command.Execute();
		this.startCommandEvent?.Invoke(command);
		if (command.IsExecuting)
		{
			CurCommand = command;
		}
		return true;
	}

	private void CancelCommand()
	{
		if (CurCommand != null)
		{
			this.endCommandEvent?.Invoke(CurCommand);
			CurCommand.Cancel();
		}
	}

	public bool IsCommandExecuting()
	{
		if (CurCommand != null)
		{
			return CurCommand.IsExecuting;
		}
		return false;
	}
}
