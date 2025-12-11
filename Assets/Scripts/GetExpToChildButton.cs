using UnityEngine;

public class GetExpToChildButton : MonoBehaviour
{
	[SerializeField]
	private float experience = 500f;

	public void AddExperienceToChild()
	{
		if (ManagerBase<FamilyManager>.Instance.HaveGrowingChild)
		{
			ManagerBase<FamilyManager>.Instance.AddExperience(experience, ManagerBase<FamilyManager>.Instance.GrowingChild);
		}
	}
}
