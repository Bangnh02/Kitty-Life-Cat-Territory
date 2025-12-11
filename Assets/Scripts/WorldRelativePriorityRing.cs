using UnityEngine;

public class WorldRelativePriorityRing : MonoBehaviour
{
	private RectTransform _rectTransform;

	private CanvasRenderer _canvasRenderer;

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

	private CanvasRenderer CanvasRenderer
	{
		get
		{
			if (_canvasRenderer == null)
			{
				_canvasRenderer = GetComponent<CanvasRenderer>();
			}
			return _canvasRenderer;
		}
	}

	private void Update()
	{
		RectTransform.rotation = WorldRelativePanelPriorityController.Instance.PriorityRingRotation;
	}
}
