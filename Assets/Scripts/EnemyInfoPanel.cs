using Avelog;
using I2.Loc;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInfoPanel : WorldRelativePanel
{
	private EnemyController enemyController;

	[Header("EnemyInfoPanel")]
	[SerializeField]
	private float switchPanelsDistance = 30f;

	[SerializeField]
	private float maxAlpha = 0.8f;

	[Header("Ссылки")]
	[SerializeField]
	private GameObject fullInfoPanel;

	[SerializeField]
	private GameObject iconInfoPanel;

	[SerializeField]
	private List<GameObject> elitsimInfoPanels;

	[SerializeField]
	private Text levelText;

	[SerializeField]
	private Localize nameLoc;

	[SerializeField]
	private LocalizationParamsManager nameLocParams;

	[SerializeField]
	private Image iconImage;

	[SerializeField]
	private List<Image> elitsimIcons;

	[SerializeField]
	private ActorHealthPanel enemyHealthPanel;

	[SerializeField]
	private EnemyStaminaPanel enemyStaminaPanel;

	[SerializeField]
	private ProcessingSwitch processingSwitch;

	[SerializeField]
	private Sprite bossIcon;

	[SerializeField]
	private Sprite miniBossIcon;

	private RectTransform iconRectTransform;

	private RectTransform fullRectTransform;

	private const string enemyArchetypeTermCategory = "EnemyArchetypeNames/";

	private const string enemySchemeTermCategory = "EnemySchemeNames/";

	private const string miniBossTerm = "MiniBoss";

	private const string bossTerm = "Boss";

	private const string archetypeNameParam = "archetype";

	private const string schemeNameParam = "scheme";

	private EnemyCurrentScheme scheme;

	public EnemyController EnemyController
	{
		get
		{
			return enemyController;
		}
		private set
		{
			enemyController = value;
		}
	}

	private PlayerCombat PlayerCombat => PlayerSpawner.PlayerInstance.PlayerCombat;

	private bool IsPlayerInCombat
	{
		get
		{
			if (PlayerSpawner.PlayerInstance == null)
			{
				return false;
			}
			return PlayerSpawner.PlayerInstance.PlayerCombat.InCombat;
		}
	}

	public void Spawn(EnemyController enemyController)
	{
		EnemyController = enemyController;
		scheme = enemyController.CurrentScheme;
		EnemyController.MyCombat.changeLifeStateEvent += OnChangeLifeState;
		EnemyCurrentScheme enemyCurrentScheme = scheme;
		enemyCurrentScheme.correctLevelEvent = (Action)Delegate.Combine(enemyCurrentScheme.correctLevelEvent, new Action(OnCorrectLevel));
		enemyStaminaPanel.Setup(EnemyController);
		enemyHealthPanel.Setup(EnemyController.MyCombat);
		iconImage.sprite = ManagerBase<UIManager>.Instance.GetArchetypeSprite(EnemyController.CurrentScheme.Archetype);
		processingSwitch.CheckDistanceGO = EnemyController.EnemyModel.gameObject;
		processingSwitch.maxProcessingDistance = (EnemyController.EnemyProcessingSwitch.distanceToSwitchMoveMode + EnemyController.EnemyProcessingSwitch.maxProcessingDistance) / 2f;
		processingSwitch.MeasureDistance();
		if (processingSwitch.OnProcessingDistance)
		{
			processingSwitch.Switch(isEnabled: true, forceUpdate: true);
		}
		else
		{
			processingSwitch.Switch(isEnabled: false, forceUpdate: true, instantEnabling: true, instantDisabling: true);
		}
		UpdatePanel();
		UpdateTexts();
		fullInfoPanel.SetActive(value: false);
		iconInfoPanel.SetActive(value: true);
		base.gameObject.SetActive(value: true);
		if (EnemyController.CurrentScheme.Scheme.Type == EnemyScheme.SchemeType.Boss)
		{
			elitsimInfoPanels.ForEach(delegate(GameObject x)
			{
				x.SetActive(value: true);
			});
			elitsimIcons.ForEach(delegate(Image x)
			{
				x.sprite = bossIcon;
			});
		}
		else if (EnemyController.CurrentScheme.Scheme.Type == EnemyScheme.SchemeType.MiniBoss)
		{
			elitsimInfoPanels.ForEach(delegate(GameObject x)
			{
				x.SetActive(value: true);
			});
			elitsimIcons.ForEach(delegate(Image x)
			{
				x.sprite = miniBossIcon;
			});
		}
		else
		{
			elitsimInfoPanels.ForEach(delegate(GameObject x)
			{
				x.SetActive(value: false);
			});
		}
	}

	private void Awake()
	{
		fullRectTransform = fullInfoPanel.GetComponent<RectTransform>();
		iconRectTransform = iconInfoPanel.GetComponent<RectTransform>();
	}

	private void OnChangeLifeState(ActorCombat.LifeState state)
	{
		if (state == ActorCombat.LifeState.Dead)
		{
			processingSwitch.Switch(isEnabled: false, forceUpdate: true, instantEnabling: true, instantDisabling: true);
		}
	}

	private void OnCorrectLevel()
	{
		UpdateTexts();
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		if (EnemyController != null)
		{
			EnemyController.MyCombat.changeLifeStateEvent -= OnChangeLifeState;
		}
		if (scheme != null)
		{
			EnemyCurrentScheme enemyCurrentScheme = scheme;
			enemyCurrentScheme.correctLevelEvent = (Action)Delegate.Remove(enemyCurrentScheme.correctLevelEvent, new Action(OnCorrectLevel));
		}
		scheme = null;
	}

	private void OnDisable()
	{
		if (!base.gameObject.activeSelf)
		{
			if (EnemyController != null)
			{
				EnemyController.MyCombat.changeLifeStateEvent -= OnChangeLifeState;
			}
			EnemyController = null;
			if (scheme != null)
			{
				EnemyCurrentScheme enemyCurrentScheme = scheme;
				enemyCurrentScheme.correctLevelEvent = (Action)Delegate.Remove(enemyCurrentScheme.correctLevelEvent, new Action(OnCorrectLevel));
			}
			scheme = null;
		}
	}

	private new void OnEnable()
	{
		base.OnEnable();
		if (EnemyController != null)
		{
			UpdateTexts();
		}
	}

	private void UpdateTexts()
	{
		levelText.text = EnemyController.CurrentScheme.level.ToString();
		string translation = LocalizationManager.GetTranslation("EnemyArchetypeNames/" + EnemyController.CurrentScheme.Archetype.name);
		string paramValue = "";
		if (EnemyController.CurrentScheme.Scheme.Type == EnemyScheme.SchemeType.MiniBoss)
		{
			paramValue = "(" + LocalizationManager.GetTranslation("EnemySchemeNames/MiniBoss") + ")";
		}
		else if (EnemyController.CurrentScheme.Scheme.Type == EnemyScheme.SchemeType.Boss)
		{
			paramValue = "(" + LocalizationManager.GetTranslation("EnemySchemeNames/Boss") + ")";
		}
		nameLocParams.SetParameterValue("archetype", translation);
		nameLocParams.SetParameterValue("scheme", paramValue);
	}

	protected override void LateUpdate()
	{
		bool flag = EnemyController.CurState == EnemyController.State.Attack || EnemyController.CurState == EnemyController.State.Escape || EnemyController.CurState == EnemyController.State.Pursuit;
		bool flag2 = PlayerCombat.HavePursuitedEnemy && PlayerCombat.PursuitedEnemy == EnemyController;
		if (!flag && !PlayerCombat.InCombat)
		{
			alphaManualControl = false;
		}
		else
		{
			alphaManualControl = true;
			if (IsOnBackside())
			{
				SetTargetAlpha(0f);
			}
			else if (PlayerCombat.RecommendedTargetForAttack == EnemyController.MyCombat)
			{
				SetTargetAlpha(1f);
			}
			else
			{
				SetTargetAlpha(0.5f);
			}
		}
		UpdatePanel();
		if (CameraUtils.PlayerCamera == null)
		{
			return;
		}
		Vector3 vector = CameraUtils.PlayerCamera.transform.position - GetTargetPosition();
		if (vector.IsShorterOrEqual(switchPanelsDistance) | flag2 | flag)
		{
			iconInfoPanel.SetActive(value: false);
			fullInfoPanel.SetActive(value: true);
		}
		else if (vector.IsLonger(switchPanelsDistance))
		{
			if (IsPlayerInCombat)
			{
				iconInfoPanel.SetActive(value: false);
			}
			else
			{
				iconInfoPanel.SetActive(value: true);
			}
			fullInfoPanel.SetActive(value: false);
		}
	}

	protected override bool HaveTarget()
	{
		return EnemyController != null;
	}

	protected override Vector3 GetTargetPosition()
	{
		return EnemyController.EnemyModel.transform.position;
	}

	protected override Vector3 GetWorldPositionOffset()
	{
		return new Vector3(EnemyController.EnemyModel.WorldPanelOffset.x, EnemyController.EnemyModel.WorldPanelOffset.y + EnemyController.NavAgent.height, EnemyController.EnemyModel.WorldPanelOffset.z);
	}

	protected override RectTransform GetEnabledRectTransform()
	{
		if (iconInfoPanel.activeSelf)
		{
			return iconRectTransform;
		}
		return fullRectTransform;
	}

	public static (bool isVisible, float sqrDistance) GetProcessingData(EnemyController enemyController)
	{
		float sqrMagnitude = (CameraUtils.PlayerCamera.gameObject.transform.position - enemyController.ModelTransform.position).sqrMagnitude;
		float value = (enemyController.EnemyProcessingSwitch.distanceToSwitchMoveMode + enemyController.EnemyProcessingSwitch.maxProcessingDistance) / 2f;
		return (CameraUtils.PlayerCamera != null && enemyController.MyCombat.CurLifeState != ActorCombat.LifeState.Dead && (CameraUtils.PlayerCamera.gameObject.transform.position - enemyController.ModelTransform.position).IsShorter(value), sqrMagnitude);
	}
}
