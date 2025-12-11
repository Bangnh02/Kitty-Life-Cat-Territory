using UnityEngine;

public class DG_Sphere : MonoBehaviour
{
	[SerializeField]
	private Color color = new Color(0f, 0f, 0f, 0.5f);

	[SerializeField]
	private float radius = 1f;

	[SerializeField]
	private bool isWire;

	public float Radius => radius;

	private void OnDrawGizmos()
	{
		Gizmos.color = color;
		if (!isWire)
		{
			Gizmos.DrawSphere(base.transform.position, radius);
		}
		else
		{
			Gizmos.DrawWireSphere(base.transform.position, radius);
		}
	}
}
