using UnityEngine;

public class ResetThirstButton : MonoBehaviour
{
	public void ResetThirst()
	{
		if (PlayerSpawner.PlayerInstance != null)
		{
			PlayerSpawner.PlayerInstance?.PlayerEating?.ChangeThirst(0f - ManagerBase<PlayerManager>.Instance.thirstCurrent);
			PlayerSpawner.PlayerInstance?.PlayerFamilyController?.FamilyMembersControllers?.ForEach(delegate(FamilyMemberController x)
			{
				x.ChangeThirst(0f - x.familyMemberData.thirst);
			});
		}
		else
		{
			ManagerBase<PlayerManager>.Instance.thirstCurrent = 0f;
			ManagerBase<FamilyManager>.Instance.family.ForEach(delegate(FamilyManager.FamilyMemberData x)
			{
				x.thirst = 0f;
			});
		}
	}
}
