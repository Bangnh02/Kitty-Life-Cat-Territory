using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyModel : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private int countIdleAnimations = 1;

	[SerializeField]
	private GameObject modelObj;

	[SerializeField]
	private BoxCollider collider;

	[SerializeField]
	[Tooltip("При старте игры из координаты Y вычтется высота агента. Для работы нового алгоритма")]
	private Vector3 worldPanelOffset = Vector3.zero;

	[SerializeField]
	private Renderer renderer;

	[SerializeField]
	private GameObject miniBossEyes;

	[SerializeField]
	private GameObject bossEyes;

	private EnemyController enemyController;

	private EnemyProcessingSwitch processingSwitch;

	private NavMeshAgent navAgent;

	public EnemyArchetype Archetype
	{
		get;
		set;
	}

	public Animator Animator => animator;

	public int CountIdleAnimations => countIdleAnimations;

	public GameObject ModelObj => modelObj;

	public BoxCollider Collider => collider;

	public Vector3 WorldPanelOffset => worldPanelOffset;

	public Renderer Renderer => renderer;

	public GameObject MiniBossEyes => miniBossEyes;

	public GameObject BossEyes => bossEyes;

	public EnemyController EnemyController
	{
		get
		{
			if (enemyController == null || enemyController.EnemyModel != this)
			{
				enemyController = GetComponentInParent<EnemyController>();
			}
			return enemyController;
		}
	}

	public EnemyProcessingSwitch ProcessingSwitch
	{
		get
		{
			if (processingSwitch == null)
			{
				processingSwitch = GetComponent<EnemyProcessingSwitch>();
			}
			return processingSwitch;
		}
	}

	public NavMeshAgent NavAgent
	{
		get
		{
			if (navAgent == null)
			{
				navAgent = GetComponent<NavMeshAgent>();
			}
			return navAgent;
		}
	}

	public event Action changeEnemyControllerEvent;

	private void Awake()
	{
		worldPanelOffset = new Vector3(worldPanelOffset.x, worldPanelOffset.y - NavAgent.height, worldPanelOffset.z);
	}

	public void SetController(EnemyController enemyController)
	{
		this.enemyController = enemyController;
		this.changeEnemyControllerEvent?.Invoke();
	}
}
