using UnityEngine;

public class SimpleRotate : MonoBehaviour
{
	public float Speed = 1f;

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.RotateAroundLocal(Vector3.up, Speed * Time.deltaTime);
	}
}
