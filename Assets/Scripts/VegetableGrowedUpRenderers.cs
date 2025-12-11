using UnityEngine;

public class VegetableGrowedUpRenderers : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer topMeshRenderer;

	[SerializeField]
	private MeshRenderer vegetableMeshRenderer;

	public MeshRenderer TopMeshRenderer => topMeshRenderer;

	public MeshRenderer VegetableMeshRenderer => vegetableMeshRenderer;
}
