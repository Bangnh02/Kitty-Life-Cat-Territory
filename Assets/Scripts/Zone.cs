using UnityEngine;

[RequireComponent(typeof(DG_Box))]
public class Zone : MonoBehaviour
{
	public Vector3 GetRandomPoint()
	{
		float x = UnityEngine.Random.Range((0f - base.transform.localScale.x) / 2f, base.transform.localScale.x / 2f);
		float y = UnityEngine.Random.Range((0f - base.transform.localScale.y) / 2f, base.transform.localScale.y / 2f);
		float z = UnityEngine.Random.Range((0f - base.transform.localScale.z) / 2f, base.transform.localScale.z / 2f);
		Vector3 point = new Vector3(x, y, z);
		point = base.transform.rotation * point;
		return point + base.transform.position;
	}
}
