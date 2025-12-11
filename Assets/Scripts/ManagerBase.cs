using System;
using UnityEngine;

public abstract class ManagerBase<ChildClass> : MonoBehaviour, ISaveable where ChildClass : ManagerBase<ChildClass>
{
	private static ChildClass instance;

	[NonSerialized]
	private bool isInited;

	public static ChildClass Instance
	{
		get
		{
			if ((UnityEngine.Object)instance == (UnityEngine.Object)null)
			{
				UnityEngine.Object.FindObjectOfType<ChildClass>().Init();
			}
			return instance;
		}
	}

	protected void Start()
	{
		if (!isInited)
		{
			Init();
		}
	}

	private void OnEnable()
	{
		if (!isInited)
		{
			Init();
		}
	}

	public void Init()
	{
		if (!isInited && !((UnityEngine.Object)instance != (UnityEngine.Object)null))
		{
			if ((UnityEngine.Object)instance == (UnityEngine.Object)null)
			{
				instance = (this as ChildClass);
			}
			OnInit();
			isInited = true;
		}
	}

	protected abstract void OnInit();
}
