using Avelog.UI;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSatietyPanel : MonoBehaviour
{
	[SerializeField]
	private Bar bar;

	[SerializeField]
	private Text satietyText;

	[SerializeField]
	private Image paramStateImage;

	private const int notDefinedSatietyPart = -1;

	private int curSatietyValue = -1;

	private PlayerEating PlayerEating => PlayerSpawner.PlayerInstance.PlayerEating;

	private void Start()
	{
		if (PlayerSpawner.IsPlayerSpawned)
		{
			OnSpawnPlayer();
		}
		PlayerSpawner.spawnPlayerEvent += OnSpawnPlayer;
	}

	private void OnSpawnPlayer()
	{
		bar.SetValueInstantly(ManagerBase<PlayerManager>.Instance.satietyCurrent / ManagerBase<PlayerManager>.Instance.SatietyMaximum);
		PlayerEating.changeSatietyEvent += OnChangeSatiety;
		UpdateSatietyBar();
		PlayerSpawner.PlayerInstance.PlayerCombat.respawnEvent += OnPlayerRespawn;
	}

	private void OnDestroy()
	{
		PlayerSpawner.spawnPlayerEvent -= OnSpawnPlayer;
		PlayerEating.changeSatietyEvent -= OnChangeSatiety;
		if (PlayerSpawner.PlayerInstance != null)
		{
			PlayerSpawner.PlayerInstance.PlayerCombat.respawnEvent += OnPlayerRespawn;
		}
	}

	private void OnPlayerRespawn()
	{
		bar.SetValueInstantly(ManagerBase<PlayerManager>.Instance.satietyCurrent / ManagerBase<PlayerManager>.Instance.SatietyMaximum);
		UpdateSatietyBar();
	}

	private void OnChangeSatiety()
	{
		UpdateSatietyBar();
	}

	private void UpdateSatietyBar()
	{
		float num = ManagerBase<PlayerManager>.Instance.satietyCurrent / ManagerBase<PlayerManager>.Instance.SatietyMaximum;
		bar.SetValue(num);
		int num2 = Mathf.CeilToInt(num * 100f);
		if (curSatietyValue != num2)
		{
			curSatietyValue = num2;
			satietyText.text = num2.ToString() + "%";
		}
		if (num == 0f)
		{
			if (paramStateImage.sprite != ManagerBase<UIManager>.Instance.ParamZeroSprite)
			{
				paramStateImage.sprite = ManagerBase<UIManager>.Instance.ParamZeroSprite;
			}
			if (!paramStateImage.enabled)
			{
				paramStateImage.enabled = true;
			}
		}
		else if (PlayerEating.IsLosingSatietyDueThirst)
		{
			if (paramStateImage.sprite != ManagerBase<UIManager>.Instance.ParamFallSprite)
			{
				paramStateImage.sprite = ManagerBase<UIManager>.Instance.ParamFallSprite;
			}
			if (!paramStateImage.enabled)
			{
				paramStateImage.enabled = true;
			}
		}
		else if (paramStateImage.enabled)
		{
			paramStateImage.enabled = false;
		}
	}
}
