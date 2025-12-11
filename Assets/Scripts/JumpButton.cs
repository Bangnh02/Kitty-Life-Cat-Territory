using Avelog;
using UnityEngine;

public class JumpButton : MonoBehaviour
{
	public void Jump()
	{
		Avelog.Input.FireJumpPressed();
	}
}
