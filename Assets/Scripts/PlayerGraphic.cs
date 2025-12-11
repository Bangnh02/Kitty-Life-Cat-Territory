using UnityEngine;

public class PlayerGraphic : MonoBehaviour, IInitializablePlayerComponent
{
	[Header("Ссылки")]
	[SerializeField]
	private Transform playerPhysic;

	[SerializeField]
	private PlayerMovement playerMovement;

	public void Initialize()
	{
		base.transform.parent = Singleton<PlayerSpawner>.Instance.SpawnedObjsParent;
		base.transform.rotation = playerMovement.transform.rotation;
	}

	private void Update()
	{
		UpdateGraphicTransform();
	}

	public void UpdateGraphicTransform()
	{
		base.transform.position = playerPhysic.position;
		Vector3 vector = Vector3.ProjectOnPlane(base.transform.forward, Vector3.up);
		Vector3 b = Vector3.ProjectOnPlane(playerMovement.transform.forward, Vector3.up);
		Vector3 toDirection = Vector3.Lerp(vector, b, ManagerBase<PlayerManager>.Instance.GraphicRotLerp * Time.deltaTime);
		Quaternion rhs = Quaternion.FromToRotation(vector, toDirection);
		Vector3 b2 = Vector3.ProjectOnPlane(playerMovement.AlignGroundNormal, playerMovement.transform.right);
		Vector3 toDirection2 = Vector3.Lerp(base.transform.up, b2, ManagerBase<PlayerManager>.Instance.GraphicAlignLerp * Time.deltaTime);
		Quaternion lhs = Quaternion.FromToRotation(base.transform.up, toDirection2);
		base.transform.rotation = lhs * (base.transform.rotation * rhs);

	}
}
