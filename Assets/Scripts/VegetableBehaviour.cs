using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VegetableBehaviour : MonoBehaviour
{
	public delegate void VegetableEventHandler(VegetableBehaviour vegetable);

	[SerializeField]
	private VegetableManager.VegetableType type;

	[Header("Ссылки")]
	[SerializeField]
	private Collider trigger;

	[SerializeField]
	private GameObject growingModel;

	[SerializeField]
	private GameObject growedUpModel;

	[SerializeField]
	private Vector3 worldPanelOffset = Vector3.zero;

	private const int notDefinedId = -1;

	private int _vegetableId = -1;

	private int _gardenId = -1;

	private VegetableManager.VegetableData _vegetableData;

	private VegetableBehaviour _prevVegetableBeh;

	private VegetableGrowedUpRenderers _vegetableGrowedUpRenderers;

	private ProcessingSwitch _processingSwitch;

	public static List<VegetableBehaviour> Instances = new List<VegetableBehaviour>();

	public Vector3 WorldPanelOffset => worldPanelOffset;

	public GameObject SignalModel
	{
		get;
		private set;
	}

	public float DistanceToUpdateState
	{
		get;
		private set;
	}

	private int VegetableId
	{
		get
		{
			if (_vegetableId == -1)
			{
				List<VegetableBehaviour> list = (from x in base.transform.parent.GetComponentsInChildren<VegetableBehaviour>()
					where x.type == type
					select x).ToList();
				list.Sort((VegetableBehaviour x, VegetableBehaviour y) => x.transform.GetSiblingIndex().CompareTo(y.transform.GetSiblingIndex()));
				_vegetableId = list.IndexOf(this);
			}
			return _vegetableId;
		}
	}

	private int GardenId
	{
		get
		{
			if (_gardenId == -1)
			{
				Transform parent = base.transform.parent;
				while (!parent.CompareTag("Garden") && parent != null)
				{
					parent = parent.parent;
				}
				_gardenId = parent.GetSiblingIndex();
			}
			return _gardenId;
		}
	}

	public VegetableManager.VegetableData VegetableData
	{
		get
		{
			InitVegetableData();
			return _vegetableData;
		}
	}

	private VegetableManager.VegetableParams VegetableParams => VegetableData.vegetableParams;

	private VegetableManager.VegetableData PrevVegetableData => PrevVegetableBeh?.VegetableData;

	private VegetableBehaviour PrevVegetableBeh
	{
		get
		{
			if (_prevVegetableBeh == null && VegetableId != 0)
			{
				_prevVegetableBeh = base.transform.parent.GetComponentsInChildren<VegetableBehaviour>().ToList().Find((VegetableBehaviour x) => x.type == type && x.VegetableId == VegetableId - 1);
			}
			return _prevVegetableBeh;
		}
	}

	private VegetableGrowedUpRenderers VegetableGrowedUpRenderers
	{
		get
		{
			if (_vegetableGrowedUpRenderers == null)
			{
				_vegetableGrowedUpRenderers = growedUpModel.GetComponent<VegetableGrowedUpRenderers>();
			}
			return _vegetableGrowedUpRenderers;
		}
	}

	public int PlantCost => VegetableData.vegetableParams.cost;

	public bool IsNextToPlant
	{
		get
		{
			if (PrevVegetableData == null || PrevVegetableData.isPlanted)
			{
				return !VegetableData.isPlanted;
			}
			return false;
		}
	}

	private bool IsPlayerInCombat
	{
		get
		{
			if (PlayerSpawner.PlayerInstance == null)
			{
				return false;
			}
			return PlayerSpawner.PlayerInstance.PlayerCombat.InCombat;
		}
	}

	private ProcessingSwitch ProcessingSwitch
	{
		get
		{
			if (_processingSwitch == null)
			{
				_processingSwitch = GetComponent<ProcessingSwitch>();
			}
			return _processingSwitch;
		}
	}

	private Vector3 PlayerPosition => PlayerSpawner.PlayerInstance.PlayerCenter.position;

	public bool IsPlayerInTrigger
	{
		get;
		private set;
	}

	public static event VegetableEventHandler plantEvent;

	public static event VegetableEventHandler grownEvent;

	public event Action updateStateEvent;

	public event Action switchPlayerInTriggerEvent;

	public static event VegetableEventHandler spawnEvent;

	private void Start()
	{
		ProcessingSwitch.maxProcessingDistance = ManagerBase<VegetableManager>.Instance.ProcessingDistance;
		ProcessingSwitch.UpdateProcessingState();
		DistanceToUpdateState = ((SphereCollider)trigger).radius;
		trigger.enabled = false;
		InitVegetableData();
		InitMaterials();
		if (!VegetableData.isPlanted && PrevVegetableBeh != null)
		{
			plantEvent += OnVegetablePlanted;
		}
		PlayerCombat.changeInCombatStateEvent += OnChangeInCombatState;
		UpdateState();
		Instances.Add(this);
		VegetableBehaviour.spawnEvent?.Invoke(this);
	}

	private void OnDestroy()
	{
		Instances?.Remove(this);
		plantEvent -= OnVegetablePlanted;
		PlayerCombat.changeInCombatStateEvent -= OnChangeInCombatState;
	}

	private void InitVegetableData()
	{
		if (_vegetableData == null)
		{
			_vegetableData = ManagerBase<VegetableManager>.Instance.vegetablesData.Find((VegetableManager.VegetableData x) => x.type == type && x.gardenId == GardenId && x.vegetableId == VegetableId);
			if (_vegetableData == null)
			{
				VegetableManager.VegetableParams vegetableParams = ManagerBase<VegetableManager>.Instance.vegetablesParams.Find((VegetableManager.VegetableParams x) => x.type == type);
				_vegetableData = new VegetableManager.VegetableData
				{
					vegetableParams = vegetableParams,
					vegetableId = VegetableId,
					gardenId = GardenId,
					type = type,
					curGrowthTime = 0f,
					isPlanted = false
				};
				ManagerBase<VegetableManager>.Instance.vegetablesData.Add(_vegetableData);
				_vegetableData = ManagerBase<VegetableManager>.Instance.vegetablesData.Find((VegetableManager.VegetableData x) => x.type == type && x.gardenId == GardenId && x.vegetableId == VegetableId);
			}
		}
	}

	private void InitMaterials()
	{
		if (!VegetableData.isPlanted)
		{
			VegetableGrowedUpRenderers.TopMeshRenderer.material = ManagerBase<VegetableManager>.Instance.PreviewMaterial.topMaterial;
			VegetableGrowedUpRenderers.VegetableMeshRenderer.material = ManagerBase<VegetableManager>.Instance.PreviewMaterial.vegetableMaterial;
		}
	}

	private void OnVegetablePlanted(VegetableBehaviour vegetable)
	{
		if (!(vegetable != PrevVegetableBeh))
		{
			plantEvent -= OnVegetablePlanted;
			UpdateState();
		}
	}

	private void OnChangeInCombatState(bool inCombat)
	{
		UpdateState();
	}

	private void OnEnable()
	{
		UpdateState();
	}

	private void UpdateState()
	{
		if (VegetableData.isPlanted)
		{
			bool flag = VegetableData.curGrowthTime < VegetableParams.growthTime;
			if (flag)
			{
				float t = VegetableData.curGrowthTime / VegetableParams.growthTime;
				float d = Mathf.Lerp(VegetableParams.growingModelMinScale, 1f, t);
				growingModel.transform.localScale = Vector3.one * d;
			}
			growingModel.SetActive(flag);
			growedUpModel.SetActive(!flag);
			SignalModel?.SetActive(value: false);
		}
		else
		{
			if (IsPlayerInCombat)
			{
				SignalModel?.SetActive(value: false);
				growedUpModel.SetActive(value: false);
			}
			else
			{
				SignalModel?.SetActive(IsNextToPlant && !IsPlayerInTrigger);
				growedUpModel.SetActive(IsNextToPlant && IsPlayerInTrigger);
			}
			growingModel.SetActive(value: false);
		}
		this.updateStateEvent?.Invoke();
	}

	public void SetSignalModel(GameObject signalModel)
	{
		SignalModel = signalModel;
	}

	public bool CanPlant()
	{
		if (!VegetableData.isPlanted && IsNextToPlant)
		{
			return ManagerBase<PlayerManager>.Instance.CurCoins >= VegetableParams.cost;
		}
		return false;
	}

	public void Plant()
	{
		if (CanPlant())
		{
			VegetableData.isPlanted = true;
			ManagerBase<VegetableManager>.Instance.vegetablesData.Find((VegetableManager.VegetableData x) => x.gardenId == GardenId && x.vegetableId == VegetableId);
			VegetableGrowedUpRenderers.TopMeshRenderer.material = ManagerBase<VegetableManager>.Instance.PlantedMaterial.topMaterial;
			VegetableGrowedUpRenderers.VegetableMeshRenderer.material = ManagerBase<VegetableManager>.Instance.PlantedMaterial.vegetableMaterial;
			UpdateState();
			VegetableBehaviour.plantEvent?.Invoke(this);
		}
	}

	private void Update()
	{
		if (VegetableData.isPlanted && !VegetableData.IsGrowedUp)
		{
			bool isGrowedUp = VegetableData.IsGrowedUp;
			VegetableData.curGrowthTime += Time.deltaTime;
			UpdateState();
			if (VegetableData.IsGrowedUp && !isGrowedUp)
			{
				VegetableBehaviour.grownEvent?.Invoke(this);
			}
		}
		if (IsNextToPlant || VegetableData.isPlanted)
		{
			if ((base.transform.position - PlayerPosition).IsShorterOrEqual(DistanceToUpdateState) && !IsPlayerInTrigger)
			{
				IsPlayerInTrigger = true;
				this.switchPlayerInTriggerEvent?.Invoke();
				UpdateState();
			}
			else if ((base.transform.position - PlayerPosition).IsLonger(DistanceToUpdateState) && IsPlayerInTrigger)
			{
				IsPlayerInTrigger = false;
				this.switchPlayerInTriggerEvent?.Invoke();
				UpdateState();
			}
		}
	}
}
