using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(AudioListener), typeof(Camera))]
public class MenuCamera : MonoBehaviour
{
	[SerializeField]
	private AudioListener audioListener;

	[SerializeField]
	private Camera camera;

	private void Start()
	{
		audioListener = GetComponent<AudioListener>();
		camera = GetComponent<Camera>();
		SceneController.changeActiveSceneEvent += OnChangeActiveScene;
		UpdateState();
	}

	private void OnDestroy()
	{
		SceneController.changeActiveSceneEvent -= OnChangeActiveScene;
	}

	private void OnChangeActiveScene(SceneController.SceneType newActiveScene)
	{
		UpdateState();
	}

	private void UpdateState()
	{
		_003CUpdateState_003Eg__SwitchState_007C5_0(SceneController.Instance.CurActiveScene == SceneController.SceneType.Menu);
	}

	[CompilerGenerated]
	private void _003CUpdateState_003Eg__SwitchState_007C5_0(bool enabled)
	{
		if (base.enabled != enabled)
		{
			base.enabled = enabled;
			camera.enabled = enabled;
			audioListener.enabled = enabled;
		}
	}
}
