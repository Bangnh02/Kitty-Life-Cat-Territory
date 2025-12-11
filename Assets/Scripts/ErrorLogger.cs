using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class ErrorLogger : MonoBehaviour, IInitializableUI
{
	private int errorCount;

	private List<GameObject> errorMessages = new List<GameObject>();

	[SerializeField]
	private GameObject scrollViewGO;

	[SerializeField]
	private Text errorCountTXT;

	[SerializeField]
	private Button openPanelBTN;

	[SerializeField]
	private GameObject errorMessagePrefab;

	[SerializeField]
	private VerticalLayoutGroup errorPanelGroup;

	[SerializeField]
	private int countDisplayedMessages = 5;

	public void OnInitializeUI()
	{
	}

	private void OnDestroy()
	{
	}

	private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
	{
		if (type == LogType.Error || type == LogType.Exception)
		{
			DateTime now = DateTime.Now;
			if (errorCount == 0)
			{
				openPanelBTN.gameObject.SetActive(value: true);
			}
			Text text = null;
			if (errorMessages.Count == countDisplayedMessages)
			{
				errorMessages.Sort((GameObject hole1, GameObject hole2) => hole1.transform.GetSiblingIndex().CompareTo(hole2.transform.GetSiblingIndex()));
				text = errorMessages[0].GetComponentInChildren<Text>();
				errorMessages[0].transform.SetAsLastSibling();
			}
			else
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(errorMessagePrefab, errorPanelGroup.transform);
				text = gameObject.GetComponentInChildren<Text>();
				errorMessages.Add(gameObject);
			}
			string text3 = text.text = now.ToString(new CultureInfo("ru-RU")) + " " + condition + "\n" + stackTrace;
			errorCount++;
			errorCountTXT.text = "x" + errorCount.ToString();
		}
	}

	public void ShowErrorPanel()
	{
		scrollViewGO.SetActive(value: true);
		openPanelBTN.gameObject.SetActive(value: false);
	}

	public void CloseErrorPanel()
	{
		scrollViewGO.SetActive(value: false);
		openPanelBTN.gameObject.SetActive(value: true);
	}
}
