using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour, IInitializableUI
{
	public void OnInitializeUI()
	{
		EventTrigger component = base.gameObject.GetComponent<EventTrigger>();
		if (component != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerDown;
			entry.callback.AddListener(delegate
			{
				ManagerBase<UIManager>.Instance.PlayUniversalButtonSound();
			});
			component.triggers.Add(entry);
		}
		else
		{
			Button component2 = base.gameObject.GetComponent<Button>();
			if (component2 != null)
			{
				component2.onClick.AddListener(ManagerBase<UIManager>.Instance.PlayUniversalButtonSound);
			}
		}
	}
}
