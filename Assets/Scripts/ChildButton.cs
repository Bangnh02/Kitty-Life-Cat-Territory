using UnityEngine;

public class ChildButton : MonoBehaviour
{
	public void MakeChild()
	{
		PlayerSpawner.PlayerInstance.PlayerFamilyController.TestMakeChild();
	}
}
