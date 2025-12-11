using UnityEngine;

public class BoundsTest : MonoBehaviour
{
	private RectTransform myRect;

	private void Start()
	{
		myRect = GetComponent<RectTransform>();
	}

	private void Update()
	{
		UnityEngine.Debug.Log(IsFullyVisibleFrom(myRect));
	}

	private int CountCornersVisibleFrom(RectTransform rectTransform)
	{
		Camera main = Camera.main;
		Rect rect = new Rect(0f, 0f, Screen.width, Screen.height);
		Vector3[] array = new Vector3[4];
		rectTransform.GetWorldCorners(array);
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 point = main.WorldToScreenPoint(array[i]);
			if (rect.Contains(point))
			{
				num++;
			}
		}
		return num;
	}

	public bool IsFullyVisibleFrom(RectTransform rectTransform)
	{
		return CountCornersVisibleFrom(rectTransform) == 4;
	}

	public bool IsVisibleFrom(RectTransform rectTransform)
	{
		return CountCornersVisibleFrom(rectTransform) > 0;
	}
}
