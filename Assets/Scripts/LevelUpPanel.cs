using I2.Loc;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpPanel : ExtraPanel, IInitializableUI
{
	[Header("LevelUpPanel")]
	[SerializeField]
	private LocalizationParamsManager levelParamLoc;

	private const string levelNumberParamName = "levelNumber";

	public static LevelUpPanel Instance
	{
		get;
		private set;
	}

	public static event Action startProcessingEvent;

	public void OnInitializeUI()
	{
		Instance = this;
		if (PlayerSpawner.IsPlayerSpawned)
		{
			OnSpawnPlayer();
		}
		else
		{
			PlayerSpawner.spawnPlayerEvent += OnSpawnPlayer;
		}
		canvasRenderers = new List<CanvasRenderer>(GetComponentsInChildren<CanvasRenderer>(includeInactive: true));
		curState = State.Disabled;
	}

	private void OnSpawnPlayer()
	{
		PlayerManager.levelChangeEvent += OnLevelChange;
		SuperBonusPanel.startProcessingEvent += OnSuperBonusPanelStartProcessing;
	}

	private void OnDestroy()
	{
		PlayerSpawner.spawnPlayerEvent -= OnSpawnPlayer;
		PlayerManager.levelChangeEvent -= OnLevelChange;
		SuperBonusPanel.startProcessingEvent -= OnSuperBonusPanelStartProcessing;
	}

	private void OnLevelChange()
	{
		if (SuperBonusPanel.Instance.CurState == State.Disabled)
		{
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
			levelParamLoc.SetParameterValue("levelNumber", ManagerBase<PlayerManager>.Instance.Level.ToString());
			canvasRenderers.ForEach(delegate(CanvasRenderer x)
			{
				x.SetAlpha(1f);
			});
			base.transform.localScale = Vector3.zero;
			if (coroutine != null)
			{
				StopCoroutine(coroutine);
			}
			if (base.gameObject.activeInHierarchy)
			{
				coroutine = StartCoroutine(ChangeScaleCoroutine());
			}
			else
			{
				curState = State.ChangeScale;
			}
			LevelUpPanel.startProcessingEvent?.Invoke();
		}
	}

	private void OnSuperBonusPanelStartProcessing()
	{
		canvasRenderers.ForEach(delegate(CanvasRenderer x)
		{
			x.SetAlpha(0f);
		});
		curState = State.Disabled;
		if (coroutine != null)
		{
			StopCoroutine(coroutine);
		}
	}
}
