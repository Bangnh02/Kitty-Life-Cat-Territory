using UnityEngine;

public abstract class ActionButton : MonoBehaviour
{
	[SerializeField]
	private int priority;

	[SerializeField]
	private ActionButtonsController controller;

	public virtual int Priority => priority;

	public abstract bool WantToEnable();

	protected void UpdateButtonState()
	{
		controller.UpdateButtonsState();
	}
}
