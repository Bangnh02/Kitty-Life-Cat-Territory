using Avelog;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldRelativePanel : MonoBehaviour
{
	private const int notInitializedIndex = -1;

	private int instanceIndex = -1;

	[Header("WorldRelativePanel")]
	private bool changeAlphaOnBackside = true;

	[SerializeField]
	private bool useAngleChangeAlpha = true;

	[SerializeField]
	private float minAlphaAngle = 80f;

	[SerializeField]
	private float maxAlphaAngle = 100f;

	[SerializeField]
	private bool useRaycast;

	[SerializeField]
	private float raycastAlphaMulty = 0.3f;

	private bool raycastHittedCollider;

	[SerializeField]
	private float alphaSwitchSpeed = 2f;

	private float curViewAngle;

	[SerializeField]
	private WorldRelativePriorityRing iconPriorityRing;

	protected bool alphaManualControl;

	private float targetAlpha;

	private List<CanvasRenderer> _canvasRenderers;

	private List<CanvasGroup> _canvasGroups;

	private float _curAlpha = 1f;

	public static List<WorldRelativePanel> Instances
	{
		get;
		private set;
	} = new List<WorldRelativePanel>();


	public WorldRelativePriorityRing IconPriorityRing => iconPriorityRing;

	protected List<CanvasRenderer> CanvasRenderers
	{
		get
		{
			if (_canvasRenderers == null)
			{
				_canvasRenderers = new List<CanvasRenderer>(GetComponentsInChildren<CanvasRenderer>(includeInactive: true));
			}
			return _canvasRenderers;
		}
	}

	protected List<CanvasGroup> CanvasGroups
	{
		get
		{
			if (_canvasGroups == null)
			{
				_canvasGroups = new List<CanvasGroup>(GetComponentsInChildren<CanvasGroup>(includeInactive: true));
			}
			return _canvasGroups;
		}
	}

	public float CurAlpha
	{
		get
		{
			return _curAlpha;
		}
		protected set
		{
			_curAlpha = value;
			CanvasRenderers?.ForEach(delegate(CanvasRenderer x)
			{
				x.SetAlpha(value);
			});
			CanvasGroups?.ForEach(delegate(CanvasGroup x)
			{
				x.alpha = value;
			});
		}
	}

	protected abstract bool HaveTarget();

	protected abstract Vector3 GetTargetPosition();

	protected abstract RectTransform GetEnabledRectTransform();

	protected void Start()
	{
		iconPriorityRing?.gameObject.SetActive(value: false);
		UpdatePanel();
		instanceIndex = Instances.Count;
		Instances.Add(this);
		CanvasRenderers.ForEach(delegate(CanvasRenderer x)
		{
			x.cullTransparentMesh = true;
		});
	}

	protected virtual void OnDestroy()
	{
		Instances.Remove(this);
	}

	protected void OnEnable()
	{
		CurAlpha = CurAlpha;
	}

	protected virtual void LateUpdate()
	{
		UpdatePanel();
	}

	protected void UpdatePanel()
	{
		if (!HaveTarget() || CameraUtils.PlayerCamera == null || Time.deltaTime == 0f)
		{
			return;
		}
		if (useAngleChangeAlpha || changeAlphaOnBackside)
		{
			curViewAngle = GetViewAngle(GetTargetPosition());
		}
		base.transform.position = GetPanelPosition(GetTargetPosition());
		if (GetEnabledRectTransform().IsCenterVisibleOnScreen())
		{
			if (changeAlphaOnBackside && IsOnBackside())
			{
				targetAlpha = 0f;
				CurAlpha = 0f;
			}
			else if (!alphaManualControl && (useAngleChangeAlpha || useRaycast || changeAlphaOnBackside))
			{
				float num = 1f;
				if (useAngleChangeAlpha)
				{
					num = GetAlphaByAngle();
				}
				if (useRaycast && num != 0f)
				{
					num *= GetAlphaByRaycast();
				}
				SetTargetAlpha(num);
			}
		}
		else
		{
			SetTargetAlpha(0f);
		}
		if (CurAlpha != targetAlpha)
		{
			float num2 = Mathf.Sign(targetAlpha - CurAlpha);
			float num3 = alphaSwitchSpeed * Time.deltaTime * num2;
			float min;
			float max;
			if (num2 > 0f)
			{
				min = 0f;
				max = targetAlpha;
			}
			else
			{
				min = targetAlpha;
				max = 1f;
			}
			float num5 = CurAlpha = Mathf.Clamp(CurAlpha + num3, min, max);
		}
	}

	protected abstract Vector3 GetWorldPositionOffset();

	protected float GetViewAngle(Vector3 targetPosition)
	{
		Vector3 from = Vector3.ProjectOnPlane(CameraUtils.PlayerCamera.transform.forward, Vector3.up);
		Vector3 to = Vector3.ProjectOnPlane(targetPosition - CameraUtils.PlayerCamera.transform.position, Vector3.up);
		curViewAngle = Vector3.Angle(from, to);
		return curViewAngle;
	}

	private Vector3 GetPanelPosition(Vector3 targetPosition)
	{
		return CameraUtils.PlayerCamera.WorldToScreenPoint(targetPosition + GetWorldPositionOffset());
	}

	protected bool IsOnBackside()
	{
		return IsOnBackside(curViewAngle);
	}

	protected bool IsOnBackside(float angle)
	{
		return angle >= 90f;
	}

	protected float GetAlphaByAngle()
	{
		return GetAlphaByAngle(curViewAngle);
	}

	protected float GetAlphaByAngle(float viewAngle)
	{
		return 1f - Mathf.InverseLerp(minAlphaAngle, maxAlphaAngle, viewAngle);
	}

	protected float GetAlphaByRaycast()
	{
		if (LoadBalancingUtils.ByFrames(instanceIndex, 0.5f))
		{
			Vector3 direction = GetTargetPosition() + GetWorldPositionOffset() / 2f - CameraUtils.PlayerCamera.transform.position;
			raycastHittedCollider = Physics.Raycast(CameraUtils.PlayerCamera.transform.position, direction, direction.magnitude, 1 << Layers.ColliderLayer);
		}
		if (raycastHittedCollider)
		{
			return raycastAlphaMulty;
		}
		return 1f;
	}

	protected void SetTargetAlpha(float targetAlpha)
	{
		this.targetAlpha = targetAlpha;
	}

	public bool IsVisible(Vector3 targetPosition)
	{
		Vector3 panelPosition = GetPanelPosition(targetPosition);
		if (new Rect(0f, 0f, Screen.width, Screen.height).Contains(panelPosition))
		{
			return false;
		}
		if (useAngleChangeAlpha || changeAlphaOnBackside)
		{
			float viewAngle = GetViewAngle(GetTargetPosition());
			if (changeAlphaOnBackside && IsOnBackside(viewAngle))
			{
				return false;
			}
			if (useAngleChangeAlpha && GetAlphaByAngle(viewAngle) == 0f)
			{
				return false;
			}
		}
		return true;
	}
}
