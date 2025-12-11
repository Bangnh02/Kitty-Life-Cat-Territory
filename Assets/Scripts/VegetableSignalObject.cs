using Avelog;
using System.Collections;
using UnityEngine;

public class VegetableSignalObject : MonoBehaviour
{
	[SerializeField]
	private float groundOffset;

	[SerializeField]
	private bool useRotating = true;

	private Quaternion Rotation
	{
		get
		{
			return base.transform.rotation;
		}
		set
		{
			base.transform.rotation = value;
		}
	}

	public VegetableBehaviour CurBehaviour
	{
		get;
		private set;
	}

	public bool IsFree => CurBehaviour == null;

	private void Start()
	{
	}

	public void Spawn(VegetableBehaviour vegetable)
	{
		CurBehaviour = vegetable;
		if (CurBehaviour.IsPlayerInTrigger)
		{
			base.gameObject.SetActive(value: false);
		}
		else
		{
			base.gameObject.SetActive(value: true);
		}
		base.transform.position = vegetable.transform.position;
		if (Physics.Raycast(base.transform.position, Vector3.down, out RaycastHit hitInfo, 100f, 1 << Layers.ColliderLayer))
		{
			base.transform.position = new Vector3(base.transform.position.x, hitInfo.point.y + groundOffset, base.transform.position.z);
		}
		else if (Physics.Raycast(base.transform.position, Vector3.up, out hitInfo, 100f, 1 << Layers.ColliderLayer))
		{
			base.transform.position = new Vector3(base.transform.position.x, hitInfo.point.y + groundOffset, base.transform.position.z);
		}
		else
		{
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + groundOffset, base.transform.position.z);
		}
	}

	public void Unspawn()
	{
		CurBehaviour = null;
		base.gameObject.SetActive(value: false);
	}

	private void OnEnable()
	{
		if (useRotating)
		{
			StartCoroutine(Rotating());
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private IEnumerator Rotating()
	{
		while (true)
		{
			if (useRotating)
			{
				Rotation = ManagerBase<VegetableManager>.Instance.PlantSignalRotation;
			}
			yield return null;
		}
	}
}
