using System;
using System.Runtime.InteropServices;
using Unity.RemoteConfig;
using UnityEngine;

public class GameConfigManager : ManagerBase<GameConfigManager>
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct UserAttributes
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct AppAttributes
	{
	}

	[Save]
	[HideInInspector]
	public bool isFirstPlay = true;

	[Save]
	[HideInInspector]
	public bool isVisitedSkinsWindow;

	[Save]
	[ReadonlyInspector]
	public bool isGenderChoosed;

	[SerializeField]
	private string gameVersionNumber;

	public string GameVersionNumber => gameVersionNumber;

	public static event Action remoteConfigsLoadedEvent;

	protected override void OnInit()
	{
		ConfigManager.FetchCompleted += OnFetchCompleted;
		ConfigManager.FetchConfigs(default(AppAttributes), default(UserAttributes));
	}

	private void OnDestroy()
	{
		ConfigManager.FetchCompleted -= OnFetchCompleted;
	}

	private void OnFetchCompleted(ConfigResponse configResponse)
	{
		if (ConfigManager.appConfig != null && ConfigManager.appConfig.HasKey("skinsLevelLimits"))
		{
			ManagerBase<SkinManager>.Instance.haveLevelLimits = ConfigManager.appConfig.GetBool("skinsLevelLimits");
		}
	}
}
