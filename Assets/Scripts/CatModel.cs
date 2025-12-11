using System;
using UnityEngine;

public class CatModel : MonoBehaviour
{
	[SerializeField]
	private Transform pickSlot;

	[SerializeField]
	private Transform mouthDrinkPos;

	[SerializeField]
	private Animator animator;

	private Vector3 startModelScale;

	public Transform PickSlot => pickSlot;

	[SerializeField]
	public Transform MouthDrinkPos => mouthDrinkPos;

	public Animator Animator => animator;

	public Vector3 StartModelScale => startModelScale;

	private void Awake()
	{
		startModelScale = base.transform.localScale;
	}

	private void Update()
	{
		transform.localPosition = new Vector3(0,transform.localPosition.y,0);
	}
}
