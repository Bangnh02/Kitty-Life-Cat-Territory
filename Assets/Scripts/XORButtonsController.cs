using System.Collections.Generic;
using UnityEngine;

public class XORButtonsController : MonoBehaviour, IInitializableUI
{
	private bool isInited;

	public List<XORButton> XORButtons
	{
		get;
		private set;
	}

	public void OnInitializeUI()
	{
		XORButtons = new List<XORButton>(GetComponentsInChildren<XORButton>(includeInactive: true));
		isInited = true;
		UpdateButtonsState();
	}

	public void UpdateButtonsState()
	{
		if (isInited)
		{
			List<XORButton> list = XORButtons.FindAll((XORButton x) => x.WantToEnable());
			if (list.Count > 0)
			{
				list.Sort((XORButton x, XORButton y) => x.Priority.CompareTo(y.Priority));
				XORButton xORButton = list[0];
				xORButton.gameObject.SetActive(value: true);
				foreach (XORButton xORButton2 in XORButtons)
				{
					if (xORButton2 != xORButton)
					{
						xORButton2.gameObject.SetActive(value: false);
					}
				}
			}
			else
			{
				XORButtons.ForEach(delegate(XORButton x)
				{
					x.gameObject.SetActive(value: false);
				});
			}
		}
	}
}
