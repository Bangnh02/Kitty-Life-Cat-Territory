using I2.Loc;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownWindow : WindowSingleton<DropdownWindow>
{
	public enum DropdownType
	{
		Generic,
		Wide
	}

	[Serializable]
	public class DropdownOption
	{
		private string text;

		private List<LocalizationParamsManager.ParamValue> localizeParamValues;

		public object Option
		{
			get;
			private set;
		}

		public bool UseLocalization => Term != null;

		public string Term
		{
			get;
			private set;
		}

		public DropdownOption(object option)
		{
			Option = option;
		}

		public DropdownOption(object option, string text)
		{
			Option = option;
			this.text = text;
		}

		public DropdownOption(object option, string term, params LocalizationParamsManager.ParamValue[] localizeParamValues)
		{
			Option = option;
			Term = term;
			if (localizeParamValues != null)
			{
				this.localizeParamValues = new List<LocalizationParamsManager.ParamValue>(localizeParamValues);
			}
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return Option.ToString();
		}

		public void ApplyLocalizeParams(LocalizationParamsManager localizationParamsManager)
		{
			if (localizeParamValues != null)
			{
				localizeParamValues.ForEach(delegate(LocalizationParamsManager.ParamValue x)
				{
					localizationParamsManager.SetParameterValue(x.Name, x.Value);
				});
			}
		}

		public void SetLocalizeParamValues(params LocalizationParamsManager.ParamValue[] localizeParamValues)
		{
			if (localizeParamValues != null)
			{
				this.localizeParamValues = new List<LocalizationParamsManager.ParamValue>(localizeParamValues);
			}
		}
	}

	[SerializeField]
	private GameObject dropdownGenericPanel;

	[SerializeField]
	private GameObject dropdownLevelPanel;

	[SerializeField]
	private GameObject dropdownGenericTable;

	[SerializeField]
	private GameObject dropdownWideTable;

	[SerializeField]
	private GameObject dropdownElemPrefab;

	[SerializeField]
	private Localize titleText;

	[SerializeField]
	private ScrollRect dropdownGenericScrollrect;

	[SerializeField]
	private ScrollRect dropdownWideScrollrect;

	private List<DropdownElem> dropdownElems = new List<DropdownElem>();

	private Action<DropdownOption> ChooseElemCallback;

	private DropdownOption curOption;

	private bool isOptionChoosed = true;

	private Window prevWindow;

	protected override void OnInitialize()
	{
	}

	public void OpenGeneric(string titleTerm, DropdownOption curOption, List<DropdownOption> options, Action<DropdownOption> EndCallback)
	{
		DropdownType dropdownType = DropdownType.Generic;
		if (titleTerm != null)
		{
			titleText.SetTerm(titleTerm);
		}
		Open(curOption, options, EndCallback, dropdownType);
	}

	public void OpenWide(DropdownOption curOption, List<DropdownOption> options, Action<DropdownOption> EndCallback)
	{
		DropdownType dropdownType = DropdownType.Wide;
		Open(curOption, options, EndCallback, dropdownType);
	}

	private void Open(DropdownOption curOption, List<DropdownOption> options, Action<DropdownOption> EndCallback, DropdownType dropdownType = DropdownType.Generic)
	{
		this.curOption = curOption;
		ChooseElemCallback = EndCallback;
		GameObject gameObject = null;
		gameObject = ((dropdownType != 0) ? dropdownWideTable : dropdownGenericTable);
		dropdownGenericPanel.SetActive(dropdownType == DropdownType.Generic);
		dropdownLevelPanel.SetActive(dropdownType == DropdownType.Wide);
		int num = 0;
		for (int i = 0; i < options.Count; i++)
		{
			if (dropdownElems.Count <= i)
			{
				DropdownElem component = UnityEngine.Object.Instantiate(dropdownElemPrefab, gameObject.transform).GetComponent<DropdownElem>();
				dropdownElems.Add(component);
			}
			bool flag = options[i] == curOption;
			if (flag)
			{
				num = ((i < dropdownElems.Count / 2) ? i : (i + 1));
			}
			dropdownElems[i].Setup(options[i], flag);
			dropdownElems[i].gameObject.SetActive(value: true);
			dropdownElems[i].transform.SetParent(gameObject.transform);
			dropdownElems[i].transform.SetSiblingIndex(i);
		}
		float num2 = 0f;
		if (num != 0)
		{
			num2 = (float)num / (float)dropdownElems.Count;
		}
		for (int j = options.Count; j < dropdownElems.Count; j++)
		{
			dropdownElems[j].gameObject.SetActive(value: false);
		}
		prevWindow = Window.CurWindow;
		Open();
		isOptionChoosed = false;
		if (dropdownType == DropdownType.Generic)
		{
			dropdownGenericScrollrect.verticalNormalizedPosition = 1f - num2;
		}
	}

	public void OnElemClick(DropdownOption option)
	{
		isOptionChoosed = true;
		prevWindow.Open();
		if (ChooseElemCallback != null)
		{
			ChooseElemCallback(option);
		}
	}

	public void ClosePanel()
	{
		OnElemClick(curOption);
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		{
			ClosePanel();
		}
	}
}
