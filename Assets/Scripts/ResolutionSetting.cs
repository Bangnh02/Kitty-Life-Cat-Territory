using Avelog;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionSetting : MonoBehaviour
{
	private float ratio;

	[HideInInspector]
	public List<Vector2> resolutions;

	private static ResolutionSetting instance;

	public Vector2 StartResolution
	{
		get;
		private set;
	}

	public static ResolutionSetting Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType<ResolutionSetting>();
				instance.Init();
			}
			return instance;
		}
		private set
		{
			instance = value;
		}
	}

	public static bool IsInited
	{
		get;
		private set;
	}

	private int CurResolutionWidth
	{
		get
		{
			return ManagerBase<SettingsManager>.Instance.curResolutionWidth;
		}
		set
		{
			ManagerBase<SettingsManager>.Instance.curResolutionWidth = value;
		}
	}

	private int CurResolutionHeight
	{
		get
		{
			return ManagerBase<SettingsManager>.Instance.curResolutionHeight;
		}
		set
		{
			ManagerBase<SettingsManager>.Instance.curResolutionHeight = value;
		}
	}

	private int CurResolutionIndex
	{
		get
		{
			return ManagerBase<SettingsManager>.Instance.curResolutionIndex;
		}
		set
		{
			ManagerBase<SettingsManager>.Instance.curResolutionIndex = value;
		}
	}

	private void Start()
	{
		Init();
	}

	private void Init()
	{
		if (!IsInited)
		{
			List<Resolution> list = new List<Resolution>(Screen.resolutions);
			StartResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
			ratio = StartResolution.x / StartResolution.y;
			if (CurResolutionWidth == 0 || CurResolutionHeight == 0)
			{
				CurResolutionWidth = Screen.currentResolution.width;
				CurResolutionHeight = Screen.currentResolution.height;
			}
			resolutions = new List<Vector2>();
			list.ForEach(delegate(Resolution resolution)
			{
				Vector3 v = new Vector2(resolution.width, resolution.height);
				if (!resolutions.Contains(v))
				{
					resolutions.Add(v);
				}
			});
			if (CurResolutionIndex == -1)
			{
				CurResolutionIndex = resolutions.IndexOf(resolutions.Find((Vector2 res) => res.x == (float)CurResolutionWidth && res.y == (float)CurResolutionHeight));
			}
			if (ManagerBase<SettingsManager>.Instance.defaultResolutionHeight == 0f)
			{
				ManagerBase<SettingsManager>.Instance.defaultResolutionHeight = Screen.currentResolution.height;
				ManagerBase<SettingsManager>.Instance.defaultResolutionWidth = Screen.currentResolution.width;
			}
			Instance = this;
			Avelog.Input.SetResolutionEvent += SetResolution;
			SaveManager.LoadEndEvent += UpdateResolution;
			SetResolution(new Vector2(CurResolutionWidth, CurResolutionHeight));
			IsInited = true;
		}
	}

	private void OnDestroy()
	{
		Avelog.Input.SetResolutionEvent -= SetResolution;
		SaveManager.LoadEndEvent -= UpdateResolution;
	}

	private void UpdateResolution()
	{
		SetResolution(new Vector2(CurResolutionWidth, CurResolutionHeight));
	}

	private void SetResolution(Vector2 resolution)
	{
		CurResolutionWidth = (int)resolution.x;
		CurResolutionHeight = (int)resolution.y;
		Screen.SetResolution(CurResolutionWidth, CurResolutionHeight, fullscreen: true);
		CurResolutionIndex = resolutions.IndexOf(resolutions.Find((Vector2 res) => res.x == (float)CurResolutionWidth && res.y == (float)CurResolutionHeight));
		ManagerBase<SettingsManager>.Instance.curPPIMulty = 1f;
		if ((float)CurResolutionHeight < ManagerBase<SettingsManager>.Instance.defaultResolutionHeight)
		{
			ManagerBase<SettingsManager>.Instance.curPPIMulty += 1f - Mathf.Pow((float)CurResolutionHeight / ManagerBase<SettingsManager>.Instance.defaultResolutionHeight, 2f);
		}
		else
		{
			ManagerBase<SettingsManager>.Instance.curPPIMulty -= 1f - Mathf.Pow(ManagerBase<SettingsManager>.Instance.defaultResolutionHeight / (float)CurResolutionHeight, 2f);
		}
	}
}
