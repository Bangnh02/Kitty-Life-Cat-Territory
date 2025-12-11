using Avelog;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
	public enum SceneType
	{
		None,
		Menu,
		Game
	}

	public delegate void ChangeActiveSceneHandler(SceneType newActiveScene);

	public delegate void SceneLoadingHandler(SceneType sceneType);

	private static SceneController instance;

	[Header("Отладка")]
	[ReadonlyInspector]
	[SerializeField]
	private SceneType _curActiveScene;

	private const string menuSceneName = "Menu";

	private int menuSceneIndex;

	private int gameSceneIndex = 1;

	private Scene menuScene;

	private Scene gameScene;

	private Coroutine sceneLoadingCor;

	public static SceneController Instance
	{
		get
		{
			if (instance == null)
			{
				UnityEngine.Object.FindObjectOfType<SceneController>().Initialize();
			}
			return instance;
		}
	}

	public SceneType CurActiveScene
	{
		get
		{
			return _curActiveScene;
		}
		set
		{
			_curActiveScene = value;
		}
	}

	public SceneType StartScene
	{
		get;
		private set;
	}

	public bool IsGameSceneLoaded
	{
		get;
		private set;
	}

	public bool IsInitialized
	{
		get;
		private set;
	}

	public bool IsSceneLoading
	{
		get;
		private set;
	}

	public static event ChangeActiveSceneHandler changeActiveSceneEvent;

	public static event Action initializeEvent;

	public static event SceneLoadingHandler sceneLoadingEvent;

	private void Awake()
	{
		if (instance != this && instance != null)
		{
			base.gameObject.SetActive(value: false);
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		Initialize();
	}

	private void Initialize()
	{
		if (instance == this)
		{
			return;
		}
		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			Scene sceneAt = SceneManager.GetSceneAt(i);
			if (sceneAt.IsValid())
			{
				if (sceneAt.name == "Menu")
				{
					menuScene = sceneAt;
					menuSceneIndex = sceneAt.buildIndex;
					gameSceneIndex = ((menuSceneIndex == 0) ? 1 : 0);
				}
				else
				{
					gameScene = sceneAt;
					gameSceneIndex = sceneAt.buildIndex;
					menuSceneIndex = ((gameSceneIndex == 0) ? 1 : 0);
				}
			}
		}
		instance = this;
		Avelog.Input.startGamePressedEvent += OnStartGamePressed;
		Avelog.Input.toMainMenuPressedEvent += OnMainMenuPressed;
		if (gameScene.IsValid())
		{
			ChangeActiveScene(SceneType.Game);
		}
		else
		{
			ChangeActiveScene(SceneType.Menu);
		}
	}

	private void OnDestroy()
	{
		Avelog.Input.startGamePressedEvent -= OnStartGamePressed;
		Avelog.Input.toMainMenuPressedEvent -= OnMainMenuPressed;
	}

	private void OnStartGamePressed()
	{
		ChangeActiveScene(SceneType.Game);
	}

	private void OnMainMenuPressed()
	{
		ChangeActiveScene(SceneType.Menu);
	}

	public void ChangeActiveScene(SceneType newActiveSceneType)
	{
		if (newActiveSceneType == CurActiveScene || IsSceneLoading)
		{
			return;
		}
		if ((newActiveSceneType == SceneType.Game && !gameScene.IsValid()) || (newActiveSceneType == SceneType.Menu && !menuScene.IsValid()))
		{
			IsSceneLoading = true;
			int sceneBuildIndex = (newActiveSceneType == SceneType.Menu) ? menuSceneIndex : gameSceneIndex;
			sceneLoadingCor = StartCoroutine(LoadScene(sceneBuildIndex, newActiveSceneType, delegate
			{
				ChangeActiveScene(newActiveSceneType);
			}));
			return;
		}
		if (newActiveSceneType == SceneType.Menu)
		{
			SceneManager.SetActiveScene(menuScene);
		}
		else
		{
			SceneManager.SetActiveScene(gameScene);
		}
		Scene newActiveScene = (newActiveSceneType == SceneType.Menu) ? menuScene : gameScene;
		UnityEngine.Object.FindObjectsOfType<Light>().ToList().ForEach(delegate(Light x)
		{
			x.enabled = (x.gameObject.scene == newActiveScene);
		});
		UnityEngine.Object.FindObjectsOfType<Terrain>().ToList().ForEach(delegate(Terrain x)
		{
			x.enabled = (x.gameObject.scene == newActiveScene);
		});
		UnityEngine.Object.FindObjectsOfType<Renderer>().ToList().ForEach(delegate(Renderer x)
		{
			x.enabled = (x.gameObject.scene == newActiveScene);
		});
		CurActiveScene = newActiveSceneType;
		if (CurActiveScene == SceneType.Game && !IsGameSceneLoaded)
		{
			IsGameSceneLoaded = true;
		}
		if (!IsInitialized)
		{
			StartScene = newActiveSceneType;
			IsInitialized = true;
			SceneController.initializeEvent?.Invoke();
		}
		SceneController.changeActiveSceneEvent?.Invoke(CurActiveScene);
	}

	private IEnumerator LoadScene(int sceneBuildIndex, SceneType sceneType, Action endCallback)
	{
		SceneController.sceneLoadingEvent?.Invoke(sceneType);
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Additive);
		yield return new WaitUntil(() => asyncOperation.isDone);
		if (sceneBuildIndex == menuSceneIndex)
		{
			menuScene = SceneManager.GetSceneByBuildIndex(sceneBuildIndex);
		}
		else
		{
			gameScene = SceneManager.GetSceneByBuildIndex(sceneBuildIndex);
		}
		IsSceneLoading = false;
		sceneLoadingCor = null;
		endCallback?.Invoke();
	}
}
