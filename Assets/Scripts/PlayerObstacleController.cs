using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PlayerObstacleController : MonoBehaviour, IInitializablePlayerComponent
{
	[SerializeField]
	private float distanceToEnableObstacle = 4f;

	[Header("Отладка")]
	[SerializeField]
	private List<NavMeshAgent> agents = new List<NavMeshAgent>();

	private NavMeshObstacle NavMeshObstacle => PlayerSpawner.PlayerInstance.PlayerMovement.NavMeshObstacle;

	public void Initialize()
	{
		NavMeshObstacle.enabled = false;
		EnemyController.spawnEvent += OnEnemySpawn;
		EnemyController.unspawnEvent += OnEnemyUnspawn;
		FarmResidentSpawner.spawnEvent += OnFarmResidentSpawnEvent;
		FamilyMemberController.spawnEvent += OnFamilyMemberSpawn;
	}

	private void OnDestroy()
	{
		EnemyController.spawnEvent -= OnEnemySpawn;
		EnemyController.unspawnEvent -= OnEnemyUnspawn;
		FarmResidentSpawner.spawnEvent -= OnFarmResidentSpawnEvent;
		FamilyMemberController.spawnEvent -= OnFamilyMemberSpawn;
	}

	private void OnFamilyMemberSpawn(FamilyMemberController familyMemberController)
	{
		NavMeshAgent navAgent = familyMemberController.NavAgent;
		AddNewAgentToList(navAgent);
	}

	private void OnFarmResidentSpawnEvent(FarmResident farmResident)
	{
		NavMeshAgent navAgent = farmResident.NavAgent;
		AddNewAgentToList(navAgent);
	}

	private void OnEnemyUnspawn(EnemyController enemyController)
	{
		NavMeshAgent navAgent = enemyController.NavAgent;
		if (agents.Contains(navAgent))
		{
			agents.Remove(navAgent);
		}
	}

	private void OnEnemySpawn(EnemyController enemyController)
	{
		NavMeshAgent navAgent = enemyController.NavAgent;
		AddNewAgentToList(navAgent);
	}

	private void AddNewAgentToList(NavMeshAgent newAgent)
	{
		if (!agents.Contains(newAgent))
		{
			agents.Add(newAgent);
		}
	}

	private void Update()
	{
		if (agents.Any((NavMeshAgent agent) => (agent.transform.position - base.transform.position).IsShorterOrEqual(distanceToEnableObstacle)))
		{
			if (!NavMeshObstacle.enabled)
			{
				NavMeshObstacle.enabled = true;
			}
		}
		else if (NavMeshObstacle.enabled)
		{
			NavMeshObstacle.enabled = false;
		}
	}
}
