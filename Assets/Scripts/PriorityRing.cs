using System.Collections;
using UnityEngine;

public class PriorityRing : MonoBehaviour
{
	[SerializeField]
	private float smallButtonRectIndent = 5f;

	[SerializeField]
	private float bigButtonRectIndent = 10f;

	[SerializeField]
	private float rotationSpeed = 180f;

	private RectTransform _rectTransform;

	public float SmallButtonRectIndent => smallButtonRectIndent;

	public float BigButtonRectIndent => bigButtonRectIndent;

	public RectTransform RectTransform
	{
		get
		{
			if (_rectTransform == null)
			{
				_rectTransform = GetComponent<RectTransform>();
			}
			return _rectTransform;
		}
	}

	private void OnEnable()
	{
		StartCoroutine(Rotate());
	}

	private void OnDisable()
	{
		RectTransform.localRotation = default(Quaternion);
		StopCoroutine(Rotate());
	}

	private IEnumerator Rotate()
	{
		while (true)
		{
			RectTransform.Rotate(new Vector3(0f, 0f, 1f), rotationSpeed * Time.deltaTime);
			yield return null;
		}
	}
}
