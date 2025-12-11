using UnityEngine;

namespace Avelog.Spawn
{
	public interface ISpawnPoint
	{
		bool IsBusy
		{
			get;
		}

		float FreeTime
		{
			get;
		}

		Vector3 GetPosition();
	}
}
