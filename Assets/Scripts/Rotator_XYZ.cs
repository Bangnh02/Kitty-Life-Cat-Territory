using UnityEngine;

public class Rotator_XYZ : MonoBehaviour
{
	public bool worldRotation;

	public float XAngularSpeed = 360f;

	public bool XRotation;

	public float YAngularSpeed = 360f;

	public bool YRotation;

	public float ZAngularSpeed = 360f;

	public bool ZRotation = true;

	private void Awake()
	{
	}

	private void Update()
	{
		float xAngle = 0f;
		float yAngle = 0f;
		float zAngle = 0f;
		if (XRotation)
		{
			xAngle = XAngularSpeed * Time.deltaTime;
		}
		if (YRotation)
		{
			yAngle = YAngularSpeed * Time.deltaTime;
		}
		if (ZRotation)
		{
			zAngle = ZAngularSpeed * Time.deltaTime;
		}
		if (worldRotation)
		{
			base.gameObject.transform.Rotate(xAngle, yAngle, zAngle, Space.World);
		}
		else
		{
			base.gameObject.transform.Rotate(xAngle, yAngle, zAngle, Space.Self);
		}
	}
}
