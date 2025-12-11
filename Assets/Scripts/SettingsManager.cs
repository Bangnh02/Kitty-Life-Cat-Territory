using UnityEngine;

public class SettingsManager : ManagerBase<SettingsManager>
{
	[Header("Gameplay")]
	[Save]
	public JoystickType curJoystickType = JoystickType.Mobile;

	[Header("Sounds")]
	[Save]
	public float musicVolume = 1f;

	[Save]
	public float effectsVolume = 1f;

	[Header("Resolution")]
	[Save]
	[ReadonlyInspector]
	public float defaultResolutionHeight;

	[Save]
	[ReadonlyInspector]
	public float defaultResolutionWidth;

	[Save]
	[ReadonlyInspector]
	public int curResolutionWidth;

	[Save]
	[ReadonlyInspector]
	public int curResolutionHeight;

	[Save]
	[ReadonlyInspector]
	public int curResolutionIndex = -1;

	public const int notSelectedResolutionIndex = -1;

	[Header("UI scale")]
	[SerializeField]
	private Vector2 minUIResolution = new Vector2(1920f, 1080f);

	[SerializeField]
	private Vector2 maxUIResolution = new Vector2(1360f, 768f);

	[Save]
	[SerializeField]
	public float curUIScale;

	[Save]
	[ReadonlyInspector]
	public float curPPIMulty = 1f;

	[Header("Sensitivity")]
	[Save]
	public float sensitivity = 1f;

	[Header("Other")]
	[Save]
	public bool antiAliasing;

	[Save]
	[ReadonlyInspector]
	public bool settingsAntiAliasing;

	[Save]
	public bool invertRotation = true;

	[Save]
	public string language;

	[Save]
	public bool hotKeysHint = true;

	[Save]
	public bool hotKeysHintShowedOnce;

	public Vector2 UIScaleResolution => Vector2.Lerp(minUIResolution, maxUIResolution, curUIScale);

	protected override void OnInit()
	{
	}
}
