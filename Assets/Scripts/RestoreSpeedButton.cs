using UnityEngine;

public class RestoreSpeedButton : MonoBehaviour
{
	private float startMediumSpeed;

	private void Start()
	{
		startMediumSpeed = ManagerBase<PlayerManager>.Instance.GetOriginalSpeedMedium();
	}

	public void RestoreSpeed()
	{
		ManagerBase<PlayerManager>.Instance.SetCheatMediumSpeed(startMediumSpeed);
	}
}
