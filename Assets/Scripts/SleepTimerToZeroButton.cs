using UnityEngine;

public class SleepTimerToZeroButton : MonoBehaviour
{
	public void SleepTimerToZero()
	{
		ManagerBase<PlayerManager>.Instance.sleepTimer = 0f;
	}
}
