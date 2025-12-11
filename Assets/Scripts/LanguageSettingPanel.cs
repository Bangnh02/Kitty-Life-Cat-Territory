using I2.Loc;
using System.Collections.Generic;
using UnityEngine;

public class LanguageSettingPanel : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private Localize languageTextLoc;

	private const string termGroupName = "Languages/";

	private List<string> languages;

	private int curLanguageIndex;

	private string CurLanguage
	{
		get
		{
			return ManagerBase<SettingsManager>.Instance.language;
		}
		set
		{
			ManagerBase<SettingsManager>.Instance.language = value;
		}
	}

	public void OnInitializeUI()
	{
		languages = LocalizationManager.GetAllLanguages();
		if (string.IsNullOrEmpty(CurLanguage))
		{
			DetectLanguage();
		}
		curLanguageIndex = languages.IndexOf(CurLanguage);
		SelectLanguage();
		SaveManager.LoadEndEvent += OnLoadEnd;
	}

	private void OnDestroy()
	{
		SaveManager.LoadEndEvent -= OnLoadEnd;
	}

	private void DetectLanguage()
	{
		if (Application.systemLanguage == SystemLanguage.Russian || Application.systemLanguage == SystemLanguage.Ukrainian || Application.systemLanguage == SystemLanguage.Belarusian)
		{
			CurLanguage = "Russian";
		}
		else if (Application.systemLanguage == SystemLanguage.German)
		{
			CurLanguage = "German";
		}
		else if (Application.systemLanguage == SystemLanguage.French)
		{
			CurLanguage = "French";
		}
		else if (Application.systemLanguage == SystemLanguage.Spanish)
		{
			CurLanguage = "Spanish";
		}
		else if (Application.systemLanguage == SystemLanguage.Italian)
		{
			CurLanguage = "Italian";
		}
		else
		{
			CurLanguage = "English";
		}
	}

	public void SelectNextLanguage(bool nextProduct)
	{
		if (nextProduct)
		{
			curLanguageIndex++;
		}
		else
		{
			curLanguageIndex--;
		}
		if (curLanguageIndex < 0)
		{
			curLanguageIndex = languages.Count - 1;
		}
		else if (curLanguageIndex >= languages.Count)
		{
			curLanguageIndex = 0;
		}
		SelectLanguage();
	}

	private void SelectLanguage()
	{
		string str = LocalizationManager.CurrentLanguage = (CurLanguage = languages[curLanguageIndex]);
		languageTextLoc.SetTerm("Languages/" + str);
	}

	private void OnLoadEnd()
	{
		curLanguageIndex = languages.IndexOf(CurLanguage);
		SelectLanguage();
	}
}
