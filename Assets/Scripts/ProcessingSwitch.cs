using Avelog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProcessingSwitch : MonoBehaviour
{
	public enum SwitchState
	{
		Enabling,
		Enabled,
		Disabling,
		Disabled,
		Pause
	}

	public delegate void SwitchHandler(ProcessingSwitch processingSwitch);

	private const int notInitializedIndex = -1;

	private int instanceIndex = -1;

	[HideInInspector]
	public bool manualControl;

	[SerializeField]
	public float maxProcessingDistance;

	[SerializeField]
	private float updateFrequency;

	[SerializeField]
	private bool useScaling = true;

	[SerializeField]
	private float scaleDuration = 0.3f;

	[Tooltip("Объект, дистанция между которым и камерой используется для просчета необходимости обработки")]
	[SerializeField]
	private GameObject checkDistanceGO;

	[Tooltip("Необязательные переключаемые геймобжекты")]
	[SerializeField]
	private List<GameObject> switchableObjs;

	[Tooltip("Необязательные переключаемые монобехи")]
	[SerializeField]
	private List<MonoBehaviour> switchableBehs;

	[SerializeField]
	private GameObject scalableGO;

	private Coroutine switchCor;

	private float normalScale = 1f;

	private Action switchCallback;

	private bool isInited;

	public static List<ProcessingSwitch> Instances
	{
		get;
		private set;
	} = new List<ProcessingSwitch>();


	public float UpdateFrequency => updateFrequency;

	public GameObject CheckDistanceGO
	{
		get
		{
			return checkDistanceGO;
		}
		set
		{
			checkDistanceGO = value;
		}
	}

	private bool HaveSwitchableObjs
	{
		get
		{
			if (switchableObjs != null)
			{
				return switchableObjs.Count > 0;
			}
			return false;
		}
	}

	public SwitchState CurSwitchState
	{
		get;
		private set;
	} = SwitchState.Enabled;


	public bool OnProcessingDistance
	{
		get;
		private set;
	}

	public float SqrDistanceToTarget
	{
		get;
		private set;
	}

	public static event SwitchHandler switchEvent;

	private void Start()
	{
		Init();
	}

	private void OnEnable()
	{
		if (!manualControl)
		{
			UpdateProcessingState();
		}
	}

	private void Init()
	{
		if (isInited)
		{
			return;
		}
		if (HaveSwitchableObjs)
		{
			if (switchableObjs.Any((GameObject x) => x.activeSelf))
			{
				CurSwitchState = SwitchState.Enabled;
			}
			else
			{
				CurSwitchState = SwitchState.Disabled;
			}
			switchableObjs.ForEach(delegate(GameObject x)
			{
				x.SetActive(CurSwitchState == SwitchState.Enabled);
			});
		}
		switchableBehs?.ForEach(delegate(MonoBehaviour x)
		{
			x.enabled = (CurSwitchState == SwitchState.Enabled);
		});
		PlayerSpawner.spawnPlayerEvent += OnSpawnPlayer;
		PlayerSpawner.respawnPlayerEvent += OnRespawnPlayer;
		instanceIndex = Instances.Count;
		Instances.Add(this);
		isInited = true;
	}

	private void OnDestroy()
	{
		if (Instances.Contains(this))
		{
			Instances.Remove(this);
		}
		PlayerSpawner.spawnPlayerEvent -= OnSpawnPlayer;
		PlayerSpawner.respawnPlayerEvent -= OnRespawnPlayer;
	}

	private void OnRespawnPlayer()
	{
		if (base.gameObject.activeSelf)
		{
			UpdateProcessingState(forceUpdate: true, instantEnabling: true);
		}
	}

	private void OnSpawnPlayer()
	{
		if (base.gameObject.activeSelf)
		{
			UpdateProcessingState(forceUpdate: true, instantEnabling: true);
		}
	}

	private void OnDisable()
	{
		if (switchCor != null)
		{
			StopCoroutine(switchCor);
			CurSwitchState = SwitchState.Pause;
		}
		switchCor = null;
		Action action = switchCallback;
		switchCallback = null;
		action?.Invoke();
	}

	public virtual void UpdateByTime()
	{
		if (!manualControl && NeedUpdateProcessingStateByTimer())
		{
			UpdateProcessingState();
		}
	}

	public void UpdateProcessingState(bool forceUpdate = false, bool instantEnabling = false, bool instantDisabling = false, Action endCallback = null)
	{
		Init();
		if (!(CameraUtils.PlayerCamera == null) && !(CheckDistanceGO == null))
		{
			MeasureDistance();
			Switch(OnProcessingDistance, forceUpdate, instantEnabling, instantDisabling, endCallback);
		}
	}

	public void MeasureDistance()
	{
		(bool, float) valueTuple = MeasureDistance(checkDistanceGO.transform.position);
		OnProcessingDistance = valueTuple.Item1;
		SqrDistanceToTarget = valueTuple.Item2;
	}

	public (bool onProcessingDistance, float sqrDistance) MeasureDistance(Vector3 targetPosition)
	{
		float num = (CameraUtils.PlayerCamera != null) ? (CameraUtils.PlayerCamera.gameObject.transform.position - targetPosition).sqrMagnitude : float.MaxValue;
		return (CameraUtils.PlayerCamera != null && num < maxProcessingDistance * maxProcessingDistance, num);
	}

	public void Switch(bool isEnabled, bool forceUpdate = false, bool instantEnabling = false, bool instantDisabling = false, Action endCallback = null)
	{
		if (!base.enabled || !base.gameObject.activeInHierarchy)
		{
			if (forceUpdate && ((isEnabled && instantEnabling) || (!isEnabled && instantDisabling)))
			{
				InstantSwitch(isEnabled ? SwitchState.Enabled : SwitchState.Disabled);
			}
			endCallback?.Invoke();
			return;
		}
		switchCallback = endCallback;
		if (isEnabled)
		{
			if (NeedSwitch(SwitchState.Enabling) | forceUpdate)
			{
				if (!base.gameObject.activeSelf)
				{
					base.gameObject.SetActive(value: true);
				}
				if (switchCor != null)
				{
					StopCoroutine(switchCor);
				}
				switchCor = StartCoroutine(Switch(SwitchState.Enabling, forceUpdate, instantEnabling, instantDisabling, endCallback));
			}
		}
		else if (NeedSwitch(SwitchState.Disabling) | forceUpdate)
		{
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
			if (switchCor != null)
			{
				StopCoroutine(switchCor);
			}
			switchCor = StartCoroutine(Switch(SwitchState.Disabling, forceUpdate, instantEnabling, instantDisabling, endCallback));
		}
	}

	private bool NeedSwitch(SwitchState newSwitchState)
	{
		if (CurSwitchState != SwitchState.Pause)
		{
			if (newSwitchState != 0 || CurSwitchState == SwitchState.Enabled || CurSwitchState == SwitchState.Enabling)
			{
				if (newSwitchState == SwitchState.Disabling && CurSwitchState != SwitchState.Disabled)
				{
					return CurSwitchState != SwitchState.Disabling;
				}
				return false;
			}
			return true;
		}
		return true;
	}

	private IEnumerator Switch(SwitchState newSwitchState, bool forceUpdate, bool instantEnabling, bool instantDisabling, Action endCallback)
	{
		CurSwitchState = newSwitchState;
		if (newSwitchState == SwitchState.Enabling)
		{
			switchableObjs?.ForEach(delegate(GameObject x)
			{
				x.SetActive(value: true);
			});
			switchableBehs?.ForEach(delegate(MonoBehaviour x)
			{
				x.enabled = true;
			});
		}
		if (useScaling)
		{
			float startScale = (newSwitchState == SwitchState.Enabling) ? 0f : 1f;
			float endScale = (newSwitchState == SwitchState.Disabling) ? 0f : 1f;
			if ((newSwitchState == SwitchState.Enabling && instantEnabling) || (newSwitchState == SwitchState.Disabling && instantDisabling))
			{
				scalableGO.transform.localScale = endScale * Vector3.one;
			}
			else
			{
				float scaleTime = 0f;
				if (!forceUpdate)
				{
					scaleTime = ((newSwitchState != 0) ? ((1f - scalableGO.transform.localScale.x / startScale) * scaleDuration) : (scalableGO.transform.localScale.x / endScale * scaleDuration));
				}
				while (true)
				{
					float t = scaleTime / scaleDuration;
					float d = Mathf.Lerp(startScale, endScale, t);
					scalableGO.transform.localScale = d * Vector3.one;
					if (scaleTime >= scaleDuration)
					{
						break;
					}
					scaleTime = Mathf.Clamp(scaleTime + Time.deltaTime, 0f, scaleDuration);
					yield return null;
				}
			}
		}
		if (newSwitchState == SwitchState.Disabling)
		{
			switchableObjs?.ForEach(delegate(GameObject x)
			{
				x.SetActive(value: false);
			});
			switchableBehs?.ForEach(delegate(MonoBehaviour x)
			{
				x.enabled = false;
			});
		}
		switch (newSwitchState)
		{
		case SwitchState.Enabling:
			CurSwitchState = SwitchState.Enabled;
			break;
		case SwitchState.Disabling:
			CurSwitchState = SwitchState.Disabled;
			break;
		}
		switchCor = null;
		ProcessingSwitch.switchEvent?.Invoke(this);
		endCallback?.Invoke();
	}

	private void InstantSwitch(SwitchState newSwitchState)
	{
		if (switchCor != null)
		{
			StopCoroutine(switchCor);
		}
		if (useScaling)
		{
			float d = (newSwitchState == SwitchState.Disabled || newSwitchState == SwitchState.Disabling) ? 0f : 1f;
			scalableGO.transform.localScale = d * Vector3.one;
		}
		bool enabling = newSwitchState == SwitchState.Enabled;
		switchableObjs?.ForEach(delegate(GameObject x)
		{
			x.SetActive(enabling);
		});
		switchableBehs?.ForEach(delegate(MonoBehaviour x)
		{
			x.enabled = enabling;
		});
		CurSwitchState = newSwitchState;
		ProcessingSwitch.switchEvent?.Invoke(this);
	}

	protected virtual bool NeedUpdateProcessingStateByTimer()
	{
		return true;
	}
}
