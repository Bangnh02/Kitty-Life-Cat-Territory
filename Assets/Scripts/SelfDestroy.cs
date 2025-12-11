using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
	private enum DestroyTime
	{
		OnEditorPlay,
		OnBuildPlay,
		Everyting
	}

	[SerializeField]
	private DestroyTime destroyTime = DestroyTime.Everyting;

	private void Start()
	{
		if (destroyTime == DestroyTime.Everyting || destroyTime == DestroyTime.OnBuildPlay)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
