using Avelog;
using UnityEngine;
using UnityEngine.UI;

public class MakeChildButton : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private Button makeChildButton;

	[SerializeField]
	private CanvasGroup makeChildButtonCanvasGroup;

	[SerializeField]
	private float disableButtonAlpha = 0.5f;

	[SerializeField]
	private Text costText;

	public void OnInitializeUI()
	{
		PlayerManager.coinsChangeEvent += OnCoinsChangeEvent;
		PlayerManager.levelChangeEvent += UpdateButtonState;
		FamilyManager.stageUpEvent += OnFamilyMemberStageUp;
		PlayerFamilyController.addFamilyMemberEvent += OnAddFamilyMember;
		SaveManager.LoadEndEvent += UpdateButtonState;
		PlayerSpawner.spawnPlayerEvent += UpdateButtonState;
		PlayerSleepController.sleepStartEvent += UpdateButtonState;
		PlayerSleepController.awakeEndEvent += UpdateButtonState;
		UpdateButtonState();
	}

	private void OnDestroy()
	{
		PlayerFamilyController.addFamilyMemberEvent -= OnAddFamilyMember;
		PlayerManager.coinsChangeEvent -= OnCoinsChangeEvent;
		PlayerManager.levelChangeEvent -= UpdateButtonState;
		FamilyManager.stageUpEvent -= OnFamilyMemberStageUp;
		SaveManager.LoadEndEvent -= UpdateButtonState;
		PlayerSpawner.spawnPlayerEvent -= UpdateButtonState;
		PlayerSleepController.sleepStartEvent -= UpdateButtonState;
		PlayerSleepController.awakeEndEvent -= UpdateButtonState;
	}

	private void OnEnable()
	{
		costText.text = ManagerBase<FamilyManager>.Instance.ChildCost.ToString();
	}

	private void OnCoinsChangeEvent(int coinsInc)
	{
		UpdateButtonState();
	}

	private void OnAddFamilyMember(FamilyManager.FamilyMemberData familyMember)
	{
		UpdateButtonState();
	}

	private void OnFamilyMemberStageUp(FamilyManager.FamilyMemberData familyMemberData)
	{
		UpdateButtonState();
	}

	private void UpdateButtonState()
	{
		bool flag = PlayerSpawner.PlayerInstance != null && PlayerSpawner.PlayerInstance.PlayerFamilyController.EnoughCoinsForChild();
		makeChildButton.interactable = flag;
		makeChildButtonCanvasGroup.alpha = (flag ? 1f : disableButtonAlpha);
		base.gameObject.SetActive(PlayerSpawner.PlayerInstance != null && PlayerSpawner.PlayerInstance.PlayerFamilyController.CanMakeChild());
	}

	public void MakeChild()
	{
		if (ManagerBase<PlayerManager>.Instance.CurCoins >= ManagerBase<FamilyManager>.Instance.ChildCost)
		{
			Avelog.Input.FireMakeChildPressed("Child");
		}
	}
}
