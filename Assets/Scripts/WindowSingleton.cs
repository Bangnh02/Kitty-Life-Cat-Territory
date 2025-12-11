using UnityEngine;

public abstract class WindowSingleton<ConcreteSingleton> : Window where ConcreteSingleton : WindowSingleton<ConcreteSingleton>
{
	private static ConcreteSingleton instance;

	public static ConcreteSingleton Instance => instance;

	protected bool IsInited
	{
		get;
		private set;
	}

	public static bool IsExist => (Object)Instance != (Object)null;

	protected override void Initialze()
	{
		base.Initialze();
		if ((Object)instance == (Object)null)
		{
			instance = (this as ConcreteSingleton);
		}
		OnInitialize();
		if (IsInited)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			UnityEngine.Debug.LogError("Multiple init of " + instance.ToString());
		}
		else
		{
			IsInited = true;
		}
	}

	protected abstract void OnInitialize();
}
