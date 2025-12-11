using UnityEngine;

public abstract class XORButton : MonoBehaviour
{
	[SerializeField]
	private int priority;

	[SerializeField]
	private XORButtonsController controller;

	public virtual int Priority => priority;

	public abstract bool WantToEnable();

	protected void UpdateButtonState()
	{
		controller.UpdateButtonsState();
	}
}
