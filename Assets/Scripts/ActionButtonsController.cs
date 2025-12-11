using System.Collections.Generic;
using UnityEngine;

public class ActionButtonsController : MonoBehaviour, IInitializableUI
{
	private List<ActionButton> actionButtons;

	private bool isInited;

	public void OnInitializeUI()
	{
		actionButtons = new List<ActionButton>(GetComponentsInChildren<ActionButton>(includeInactive: true));
		isInited = true;
		UpdateButtonsState();
	}

	public void UpdateButtonsState()
	{
		if (isInited)
		{
			List<ActionButton> list = actionButtons.FindAll((ActionButton x) => x.WantToEnable());
			if (list.Count > 0)
			{
				list.Sort((ActionButton x, ActionButton y) => x.Priority.CompareTo(y.Priority));
				ActionButton actionButton = list[0];
				actionButton.gameObject.SetActive(value: true);
				foreach (ActionButton actionButton2 in actionButtons)
				{
					if (actionButton2 != actionButton)
					{
						actionButton2.gameObject.SetActive(value: false);
					}
				}
			}
			else
			{
				actionButtons.ForEach(delegate(ActionButton x)
				{
					x.gameObject.SetActive(value: false);
				});
			}
		}
	}
}
