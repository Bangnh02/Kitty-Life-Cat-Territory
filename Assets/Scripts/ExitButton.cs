using UnityEngine;

public class ExitButton : MonoBehaviour
{
	public void Exit()
	{
		ManagerBase<SaveManager>.Instance.SaveToLocal();
		Application.Quit();
	}
}
