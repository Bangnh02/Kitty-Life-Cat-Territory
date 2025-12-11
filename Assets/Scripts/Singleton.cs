using UnityEngine;

public abstract class Singleton<ConcreteSingleton> : MonoBehaviour where ConcreteSingleton : Singleton<ConcreteSingleton>
{
	private static ConcreteSingleton instance;

	public static ConcreteSingleton Instance
	{
		get
		{
			if ((Object)instance == (Object)null)
			{
				instance = UnityEngine.Object.FindObjectOfType<ConcreteSingleton>();
				if ((Object)instance != (Object)null)
				{
					instance.Init();
				}
			}
			return instance;
		}
	}

	protected bool IsInited
	{
		get;
		private set;
	}

	public static bool IsExist => (Object)Instance != (Object)null;

	private void Start()
	{
		if ((Object)instance == (Object)null)
		{
			instance = (this as ConcreteSingleton);
			Init();
		}
	}

	private void Init()
	{
		OnInit();
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

	protected abstract void OnInit();
}
