using UnityEngine;

public static class RectTransformExt
{
	public static bool IsCenterVisibleOnScreen(this RectTransform rectTransform)
	{
		return new Rect(0f, 0f, Screen.width, Screen.height).Contains(rectTransform.position);
	}
}
