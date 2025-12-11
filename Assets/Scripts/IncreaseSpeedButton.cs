using UnityEngine;

public class IncreaseSpeedButton : MonoBehaviour
{
	[SerializeField]
	private float newMediumSpeed = 100f;

	public void IncreaseSpeed()
	{
		ManagerBase<PlayerManager>.Instance.SetCheatMediumSpeed(newMediumSpeed);
	}
}
