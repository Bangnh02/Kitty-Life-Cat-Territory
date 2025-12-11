using System.Collections;
using UnityEngine;

public class MenuPriorityRing : MonoBehaviour
{
	[SerializeField]
	private float rectIndent = 36f;

	[SerializeField]
	private float rotationSpeed = 180f;

	private RectTransform _rectTransform;

	public float RectIndent => rectIndent;

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
			RectTransform.Rotate(new Vector3(0f, 0f, 1f), rotationSpeed * Time.unscaledDeltaTime);
			yield return null;
		}
	}
}
