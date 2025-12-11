using UnityEngine;

public class DG_Mesh : MonoBehaviour
{
	[SerializeField]
	private Color color = new Color(0f, 0f, 0f, 0.5f);

	[SerializeField]
	private Mesh mesh;

	[SerializeField]
	private bool isWire = true;

	[SerializeField]
	private bool isOpaque = true;

	[SerializeField]
	private float scale = 1f;

	private void OnDrawGizmos()
	{
		if (!(mesh == null))
		{
			base.transform.localScale = Vector3.one * scale;
			Gizmos.color = color;
			if (isOpaque)
			{
				Gizmos.DrawMesh(mesh, base.transform.position, base.transform.rotation, Vector3.one * scale);
			}
			if (isWire)
			{
				Gizmos.DrawWireMesh(mesh, base.transform.position, base.transform.rotation, Vector3.one * scale);
			}
		}
	}
}
