using UnityEngine;

public class ClearSaveButton : MonoBehaviour
{
	public void ClearSave()
	{
		ManagerBase<SaveManager>.Instance.ClearSave(null);
	}
}
