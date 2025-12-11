using UnityEngine;

public class GetExperienceButton : MonoBehaviour
{
	[SerializeField]
	private float experience = 500f;

	public void GetExperience()
	{
		ManagerBase<PlayerManager>.Instance.AddExperience(experience);
	}
}
