using UnityEngine;

public class MaximizeStatsButton : MonoBehaviour
{
	public void MaximizeStats()
	{
		ManagerBase<PlayerManager>.Instance.healthCurrent = ManagerBase<PlayerManager>.Instance.HealthMaximum;
		ManagerBase<PlayerManager>.Instance.satietyCurrent = ManagerBase<PlayerManager>.Instance.SatietyMaximum;
		ManagerBase<PlayerManager>.Instance.thirstCurrent = ManagerBase<PlayerManager>.Instance.ThirstMaximum;
		ManagerBase<FamilyManager>.Instance.familySatiety = ManagerBase<FamilyManager>.Instance.FamilySatietyMaximum;
		ManagerBase<FamilyManager>.Instance.family.ForEach(delegate(FamilyManager.FamilyMemberData x)
		{
			x.thirst = ManagerBase<FamilyManager>.Instance.ThirstMaximum;
		});
	}
}
