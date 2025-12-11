using UnityEngine;
using UnityEngine.UI;

public class UIScaleSettingTest : MonoBehaviour
{
	[SerializeField]
	private Slider slider;

	[SerializeField]
	private Canvas canvas;

	private CanvasScaler canvasScaler;

	private float curUIScale = 1f;

	[SerializeField]
	private Vector2 minUIResolution = new Vector2(1920f, 1080f);

	[SerializeField]
	private Vector2 maxUIResolution = new Vector2(1360f, 768f);

	private Vector2 UIScaleResolution => Vector2.Lerp(minUIResolution, maxUIResolution, curUIScale);

	private void Start()
	{
		canvasScaler = UnityEngine.Object.FindObjectOfType<CanvasScaler>();
	}

	public void OnSliderMove()
	{
		canvas.gameObject.SetActive(value: false);
		curUIScale = slider.value;
		canvasScaler.referenceResolution = UIScaleResolution;
		canvas.gameObject.SetActive(value: true);
	}
}
