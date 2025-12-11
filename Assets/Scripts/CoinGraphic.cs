using UnityEngine;

public class CoinGraphic : MonoBehaviour
{
	[SerializeField]
	private float lerp = 10f;

	[SerializeField]
	private Transform physic;

	private void Start()
	{
	}

	private void OnEnable()
	{
		if (physic != null)
		{
			base.transform.position = physic.transform.position;
		}
	}

	private void Update()
	{
		if (physic != null)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, physic.transform.position, lerp * Time.deltaTime);
		}
	}
}
