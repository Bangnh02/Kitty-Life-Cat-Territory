using Avelog;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OutOfBoundsHandler : MonoBehaviour
{
	private enum State
	{
		None,
		Blocking,
		Unblocking
	}

	public delegate void BlockHandler(float duration);

	[SerializeField]
	private float outOfBoundsY = -50f;

	[SerializeField]
	private float blockingDuration = 0.3f;

	[SerializeField]
	private float unblockingDuration = 1f;

	private CharacterController playerCharacterController;

	private List<GameObject> playerSafePlaces;

	private State curState;

	private Coroutine handlingCor;

	public static event BlockHandler startBlockingGameEvent;

	public static event BlockHandler startUnblockingGameEvent;

	private void Start()
	{
		playerSafePlaces = GameObject.FindGameObjectsWithTag("PlayerSafePlace").ToList();
		if (PlayerSpawner.IsPlayerSpawned)
		{
			OnSpawnPlayer();
		}
		PlayerSpawner.spawnPlayerEvent += OnSpawnPlayer;
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		playerSafePlaces = GameObject.FindGameObjectsWithTag("PlayerSafePlace").ToList();
	}

	private void OnDestroy()
	{
		PlayerSpawner.spawnPlayerEvent -= OnSpawnPlayer;
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSpawnPlayer()
	{
		playerCharacterController = PlayerSpawner.PlayerInstance.PlayerMovement.GetComponentInChildren<CharacterController>();
	}

	private void Update()
	{
		if (!(playerCharacterController == null) && curState != State.Blocking && playerCharacterController.bounds.center.y - playerCharacterController.bounds.extents.y < outOfBoundsY)
		{
			if (handlingCor != null)
			{
				StopCoroutine(handlingCor);
			}
			handlingCor = StartCoroutine(HandleOutOfBounds());
		}
	}

	private IEnumerator HandleOutOfBounds()
	{
		curState = State.Blocking;
		OutOfBoundsHandler.startBlockingGameEvent?.Invoke(blockingDuration);
		yield return new WaitForSecondsRealtime(blockingDuration);
		GameObject gameObject;
		if (playerSafePlaces != null && playerSafePlaces.Count > 0)
		{
			List<(GameObject safePlace, float sqrDist)> playerSafePlaceDisances = new List<(GameObject, float)>();
			playerSafePlaces.ForEach(delegate(GameObject x)
			{
				playerSafePlaceDisances.Add((x, (x.transform.position - playerCharacterController.transform.position).sqrMagnitude));
			});
			playerSafePlaceDisances.Sort(((GameObject safePlace, float sqrDist) x, (GameObject safePlace, float sqrDist) y) => x.sqrDist.CompareTo(y.sqrDist));
			gameObject = playerSafePlaceDisances[0].safePlace;
		}
		else
		{
			UnityEngine.Debug.Log("Игрок вылетел из границ карты. Безопасных мест нет (копируй AvsenScene.PlayerSafePlaces), перенос игрока на точку спавна");
			gameObject = UnityEngine.Object.FindObjectOfType<PlayerSpawnPoint>().gameObject;
		}
		float maxDistance = 1000f;
		Physics.Raycast(gameObject.transform.position, Vector3.down, out RaycastHit hitInfo, maxDistance, 1 << Layers.ColliderLayer);
		PlayerSpawner.PlayerInstance.PlayerMovement.Teleport(hitInfo.point, gameObject.transform.rotation);
		curState = State.Unblocking;
		OutOfBoundsHandler.startUnblockingGameEvent?.Invoke(unblockingDuration);
		yield return new WaitForSecondsRealtime(unblockingDuration);
		handlingCor = null;
		curState = State.None;
	}
}
