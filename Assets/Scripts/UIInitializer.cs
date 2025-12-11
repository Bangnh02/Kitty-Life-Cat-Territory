using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIInitializer : MonoBehaviour
{
	private List<Window> windows;

	private void Awake()
	{
		windows = GetComponentsInChildren<Window>(includeInactive: true).ToList();
		windows.ForEach(delegate(Window x)
		{
			x.Close();
		});
	}

	private void Start()
	{
		windows.ForEach(delegate(Window x)
		{
			x.OnInitializeUI();
		});
		List<IInitializableUI> initializableUIElems = GetComponentsInChildren<IInitializableUI>(includeInactive: true).ToList();
		windows.ForEach(delegate(Window x)
		{
			initializableUIElems.Remove(x);
		});
		foreach (IInitializableUI item in initializableUIElems)
		{
			item.OnInitializeUI();
		}
	}

	private void OnDestroy()
	{
		SceneController.initializeEvent -= OpenStartWindow;
	}

	private void OpenStartWindow()
	{
	}
}
