using System.Collections.Generic;
using UnityEngine;

public class MenuObjectsController : MonoBehaviour
{
	private List<Renderer> renderers;

	private List<Light> lights;

	private void Start()
	{
		renderers = new List<Renderer>(GetComponentsInChildren<Renderer>());
		lights = new List<Light>(GetComponentsInChildren<Light>());
		SceneController.changeActiveSceneEvent += OnChangeActiveScene;
		MenuCatSpawner.menuCatSpawnEvent += OnMenuCatSpawn;
	}

	private void OnDestroy()
	{
		SceneController.changeActiveSceneEvent -= OnChangeActiveScene;
		MenuCatSpawner.menuCatSpawnEvent -= OnMenuCatSpawn;
	}

	private void OnChangeActiveScene(SceneController.SceneType newActiveScene)
	{
		switch (newActiveScene)
		{
		case SceneController.SceneType.Menu:
			base.gameObject.SetActive(value: true);
			renderers.ForEach(delegate(Renderer renderer)
			{
				renderer.enabled = true;
			});
			lights.ForEach(delegate(Light light)
			{
				light.enabled = true;
			});
			break;
		case SceneController.SceneType.Game:
			base.gameObject.SetActive(value: false);
			renderers.ForEach(delegate(Renderer renderer)
			{
				renderer.enabled = false;
			});
			lights.ForEach(delegate(Light light)
			{
				light.enabled = false;
			});
			break;
		}
	}

	private void OnMenuCatSpawn(GameObject menuCatObject)
	{
		renderers.AddRange(menuCatObject.GetComponentsInChildren<Renderer>(includeInactive: true));
	}
}
