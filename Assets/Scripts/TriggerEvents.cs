using System.Collections.Generic;
using UnityEngine;

public class TriggerEvents : MonoBehaviour
{
	public delegate void TriggerEventHandler(Collider other);

	private List<Collider> collidersInsideTrigger = new List<Collider>();

	public event TriggerEventHandler triggerEnterEvent;

	public event TriggerEventHandler triggerStayEvent;

	public event TriggerEventHandler triggerExitEvent;

	private void OnDisable()
	{
		if (collidersInsideTrigger.Count > 0)
		{
			collidersInsideTrigger.ForEach(delegate(Collider x)
			{
				this.triggerExitEvent?.Invoke(x);
			});
			collidersInsideTrigger.Clear();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		this.triggerEnterEvent?.Invoke(other);
		if (!collidersInsideTrigger.Contains(other))
		{
			collidersInsideTrigger.Add(other);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		this.triggerStayEvent?.Invoke(other);
	}

	private void OnTriggerExit(Collider other)
	{
		this.triggerExitEvent?.Invoke(other);
		if (collidersInsideTrigger.Contains(other))
		{
			collidersInsideTrigger.Remove(other);
		}
	}
}
