using Avelog;

public class MakeSpouseButton : XORButton, IInitializableUI
{
	private PotentialSpouseController PotentialSpouse => Singleton<SpouseSpawner>.Instance.PotentialSpouse;

	public void OnInitializeUI()
	{
		PotentialSpouseController.switchPlayerNearEvent += OnSwitchPlayerNear;
		PlayerFamilyController.addFamilyMemberEvent += OnAddFamilyMember;
		PlayerSleepController.sleepStartEvent += base.UpdateButtonState;
		PlayerSleepController.awakeEndEvent += base.UpdateButtonState;
		UpdateButtonState();
	}

	private void OnDestroy()
	{
		PotentialSpouseController.switchPlayerNearEvent -= OnSwitchPlayerNear;
		PlayerFamilyController.addFamilyMemberEvent -= OnAddFamilyMember;
		PlayerSleepController.sleepStartEvent -= base.UpdateButtonState;
		PlayerSleepController.awakeEndEvent -= base.UpdateButtonState;
	}

	private void OnAddFamilyMember(FamilyManager.FamilyMemberData familyMember)
	{
		if (familyMember.role == FamilyManager.FamilyMemberRole.Spouse)
		{
			UpdateButtonState();
		}
	}

	private void OnSwitchPlayerNear(bool isPlayerNear)
	{
		UpdateButtonState();
	}

	public override bool WantToEnable()
	{
		if (PlayerSpawner.PlayerInstance != null)
		{
			return PlayerSpawner.PlayerInstance.PlayerFamilyController.CanMakeSpouse();
		}
		return false;
	}

	public void TryMakeSpouse()
	{
		Input.FireMakeSpousePressed("Spouse");
		PotentialSpouse.Unspawn();
	}
}
