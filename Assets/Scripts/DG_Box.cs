using UnityEngine;

public class DG_Box : MonoBehaviour
{
	[SerializeField]
	private bool isEnabled = true;

	[SerializeField]
	private Color color = new Color(0f, 0f, 0f, 0.5f);

	[SerializeField]
	private bool isWire;

	private void OnDrawGizmos()
	{
		if (isEnabled)
		{
			Matrix4x4 matrix4x = Matrix4x4.TRS(base.transform.position, base.transform.rotation, base.transform.localScale);
			Matrix4x4 matrix = Gizmos.matrix;
			Gizmos.matrix *= matrix4x;
			Gizmos.color = color;
			if (!isWire)
			{
				Gizmos.DrawCube(Vector3.zero, Vector3.one);
			}
			else
			{
				Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
			}
			Gizmos.matrix = matrix;
		}
	}
}
