using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour, IInitializableUI
{
	public delegate void WindowOpenedHandler(Window prevWindow);

	[SerializeField]
	public bool pauseTimeWhileOpen;

	private static List<Window> instances;

	public static List<Window> Instances
	{
		get
		{
			if (instances == null)
			{
				instances = new List<Window>();
			}
			return instances;
		}
	}

	public static Window CurWindow
	{
		get;
		private set;
	}

	public Window PrevWindow
	{
		get;
		private set;
	}

	public bool IsOpened
	{
		get;
		private set;
	}

	public static event WindowOpenedHandler windowOpenedEvent;

	public void OnInitializeUI()
	{
		if (!(Instances.Find((Window x) => x.name == base.gameObject.name) != null))
		{
			Instances.Add(this);
			Initialze();
		}
	}

	protected virtual void Initialze()
	{
	}

	public void Open()
	{
		if (!IsOpened)
		{
			PrevWindow = CurWindow;
			foreach (Window instance in Instances)
			{
				if (instance != this)
				{
					instance.Close();
				}
			}
			CurWindow = this;
			IsOpened = true;
			Time.timeScale = (pauseTimeWhileOpen ? 0f : 1f);
			AudioListener.pause = (pauseTimeWhileOpen ? true : false);
			Window.windowOpenedEvent?.Invoke(PrevWindow);
			base.gameObject.SetActive(value: true);
		}
	}

	public void Close()
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
			IsOpened = false;
		}
	}
}
