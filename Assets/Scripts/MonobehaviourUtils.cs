using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MonobehaviourUtils
{
	public static List<T> GetAllSceneComponents<T>() where T : Object
	{
		return GetAllSceneComponents<T>(SceneManager.GetActiveScene());
	}

	public static List<T> GetAllSceneComponents<T>(Scene scene) where T : Object
	{
		GameObject[] rootGameObjects = scene.GetRootGameObjects();
		List<T> list = new List<T>();
		GameObject[] array = rootGameObjects;
		foreach (GameObject gameObject in array)
		{
			list.AddRange(gameObject.GetComponentsInChildren<T>(includeInactive: true));
		}
		return list;
	}
}
