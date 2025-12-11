using I2.Loc;
using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class ErrorWindow : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private Text errorText;

	[SerializeField]
	private LocalizationParamsManager gameVersionParam;

	private string gameVersionNumberParam = "version";

	private string buildPlatformParam = "platform";

	public void OnInitializeUI()
	{
	}

	private void OnDestroy()
	{
		Application.logMessageReceived -= OnLogMessageReceived;
	}

	private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
	{
		if (type == LogType.Exception)
		{
			DateTime now = DateTime.Now;
			gameVersionParam.SetParameterValue(gameVersionNumberParam, ManagerBase<GameConfigManager>.Instance.GameVersionNumber);
			switch (Application.platform)
			{
			case RuntimePlatform.Android:
				gameVersionParam.SetParameterValue(buildPlatformParam, "Android");
				break;
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
				gameVersionParam.SetParameterValue(buildPlatformParam, "Editor");
				break;
			default:
				gameVersionParam.SetParameterValue(buildPlatformParam, "iOS");
				break;
			}
			errorText.text = now.ToString(new CultureInfo("ru-RU")) + " " + condition + "\n" + stackTrace;
			base.gameObject.SetActive(value: true);
		}
	}
}
