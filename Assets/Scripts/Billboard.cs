using Avelog;
using UnityEngine;

public class Billboard : MonoBehaviour
{
	private void Update()
	{
		base.transform.rotation = Quaternion.LookRotation(base.transform.position - CameraUtils.PlayerCamera.transform.position);
	}
}
