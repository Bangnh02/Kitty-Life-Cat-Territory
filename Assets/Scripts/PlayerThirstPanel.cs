using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerThirstPanel : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private Image paramStateImage;

	private List<ThirstObject> thirsts = new List<ThirstObject>();

	private const int notDefinedThirstParts = -1;

	private int prevThirstParts = -1;

	public static PlayerThirstPanel Instance
	{
		get;
		private set;
	}

	private PlayerEating PlayerEating => PlayerSpawner.PlayerInstance.PlayerEating;

	private int ThirstEnabledObjsCount
	{
		get
		{
			if (ManagerBase<PlayerManager>.Instance.thirstCurrent != 0f)
			{
				return Mathf.CeilToInt(ManagerBase<PlayerManager>.Instance.thirstCurrent / ManagerBase<PlayerManager>.Instance.ThirstEffect);
			}
			return 0;
		}
	}

	public int ThirstObjsCount
	{
		get
		{
			if (thirsts == null)
			{
				return 1;
			}
			return thirsts.Count;
		}
	}

	public void OnInitializeUI()
	{
		if (Instance == null)
		{
			Instance = this;
			thirsts = new List<ThirstObject>(GetComponentsInChildren<ThirstObject>(includeInactive: true));
			if (ThirstObjsCount == 0)
			{
				UnityEngine.Debug.LogError("Не добавлены объекты гидрации в панель Жажды игрока");
			}
			thirsts.Sort((ThirstObject o1, ThirstObject o2) => o2.transform.parent.GetSiblingIndex().CompareTo(o1.transform.parent.GetSiblingIndex()));
			if (PlayerSpawner.IsPlayerSpawned)
			{
				OnSpawnPlayer();
			}
			else
			{
				PlayerSpawner.spawnPlayerEvent += OnSpawnPlayer;
			}
		}
	}

	private void OnDestroy()
	{
		PlayerEating.changeThirstEvent -= OnChangeThirst;
		PlayerSpawner.spawnPlayerEvent -= OnSpawnPlayer;
	}

	private void OnSpawnPlayer()
	{
		PlayerEating.changeThirstEvent += OnChangeThirst;
		UpdateThirstBar();
	}

	private void OnChangeThirst()
	{
		UpdateThirstBar();
	}

	private void UpdateThirstBar()
	{
		if (ThirstEnabledObjsCount != prevThirstParts)
		{
			prevThirstParts = ThirstEnabledObjsCount;
			for (int i = 0; i < thirsts.Count; i++)
			{
				thirsts[i].thirstBackground.gameObject.SetActive(i < ThirstEnabledObjsCount);
			}
		}
		if (ManagerBase<PlayerManager>.Instance.thirstCurrent == 0f && !paramStateImage.enabled)
		{
			paramStateImage.enabled = true;
		}
		else if (ManagerBase<PlayerManager>.Instance.thirstCurrent != 0f && paramStateImage.enabled)
		{
			paramStateImage.enabled = false;
		}
	}
}
