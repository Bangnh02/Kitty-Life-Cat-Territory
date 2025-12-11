using System.Collections.Generic;
using UnityEngine;

public class DG_Raycast : MonoBehaviour
{
	[SerializeField]
	private Color color = new Color(0f, 0f, 0f, 0.5f);

	[SerializeField]
	private List<string> hitMask;

	[SerializeField]
	private float maxDistance = 100f;

	[SerializeField]
	private bool logHitDistance;

	private void OnDrawGizmos()
	{
		int hitLayerMask = 0;
		hitMask.ForEach(delegate(string x)
		{
			hitLayerMask |= 1 << LayerMask.NameToLayer(x);
		});
		RaycastHit hitInfo;
		bool num = Physics.Raycast(base.transform.position, base.transform.forward, out hitInfo, maxDistance, hitLayerMask, QueryTriggerInteraction.Ignore);
		Gizmos.color = color;
		if (num)
		{
			Gizmos.DrawLine(base.transform.position, hitInfo.point);
		}
		else
		{
			Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.forward * maxDistance);
		}
		if (num && logHitDistance)
		{
			UnityEngine.Debug.Log($"{base.gameObject.name} raycast hit distance = {hitInfo.distance}");
		}
	}
}
