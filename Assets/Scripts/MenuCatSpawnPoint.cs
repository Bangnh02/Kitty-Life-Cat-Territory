using UnityEngine;

public class MenuCatSpawnPoint : MonoBehaviour
{
	public enum AnimationType
	{
		Sitting,
		Resting
	}

	[ReadonlyInspector]
	public bool isBusy;

	[SerializeField]
	private AnimationType animationType;

	public AnimationType CurAnimationType => animationType;
}
