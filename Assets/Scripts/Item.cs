using Avelog;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;

public class Item : MonoBehaviour
{
	public enum PickState
	{
		OnStartPosition,
		Picked,
		Thrown
	}

	public delegate void ItemEventHandler(Item item);

	[SerializeField]
	private ItemId id;

	[SerializeField]
	protected Vector3 pickOffset = Vector3.zero;

	[SerializeField]
	protected Vector3 pickScale = Vector3.one;

	[SerializeField]
	protected bool usePickRotOverride;

	[SerializeField]
	protected Vector3 pickRotOverride = Vector3.zero;

	[SerializeField]
	protected Vector3 dropOffset = Vector3.zero;

	[SerializeField]
	protected bool useDropRotOverride;

	[SerializeField]
	protected Vector3 dropRotOverride = Vector3.zero;

	[SerializeField]
	protected float thrownLifetime = 15f;

	protected Coroutine thrownResetCor;

	protected GameObject graphicObject;

	[Header("Ссылки")]
	[SerializeField]
	private ParentConstraint parentConstraint;

	private MeshRenderer _meshRenderer;

	[SerializeField]
	[Header("Отладка")]
	private PickState curPickState;

	private Collider collider;

	protected Transform startParent;

	protected Vector3 startScale;

	protected Vector3 startPos;

	protected Quaternion startRot;

	public ItemId Id
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
		}
	}

	public virtual bool Pickable => true;

	public MeshRenderer MeshRenderer
	{
		get
		{
			if (_meshRenderer == null)
			{
				_meshRenderer = GetComponentInChildren<MeshRenderer>(includeInactive: true);
			}
			return _meshRenderer;
		}
	}

	public ItemUser CurItemUser
	{
		get;
		protected set;
	}

	public ProcessingSwitch ProcessingSwitch
	{
		get;
		protected set;
	}

	public PickState CurPickState
	{
		get
		{
			return curPickState;
		}
		protected set
		{
			curPickState = value;
			FireUpdatePickableStateEvent();
		}
	}

	public Collider Collider
	{
		get
		{
			if (collider == null)
			{
				collider = GetComponent<Collider>();
			}
			return collider;
		}
	}

	public static event ItemEventHandler updatePickableStateEvent;

	protected void FireUpdatePickableStateEvent()
	{
		Item.updatePickableStateEvent?.Invoke(this);
	}

	protected virtual void Start()
	{
		graphicObject = base.transform.GetChild(0).gameObject;
		startParent = base.transform.parent;
		startScale = graphicObject.transform.localScale;
		startPos = base.transform.position;
		startRot = base.transform.rotation;
	}

	public void Pick(Transform pickSlot)
	{
		if (thrownResetCor != null)
		{
			StopCoroutine(thrownResetCor);
			thrownResetCor = null;
		}
		if (usePickRotOverride)
		{
			graphicObject.transform.localEulerAngles = pickRotOverride;
		}
		graphicObject.transform.localScale = pickScale;
		graphicObject.transform.localPosition = pickOffset;
		EnableParentContraint(pickSlot);
		CurPickState = PickState.Picked;
	}

	public void Drop(Vector3 dropPosition, Quaternion dropRotation)
	{
		if (CurPickState == PickState.Picked)
		{
			DisableParentConstraint();
			graphicObject.transform.localPosition = dropOffset;
			graphicObject.transform.localEulerAngles = dropRotOverride;
			base.transform.position = dropPosition;
			base.transform.rotation = dropRotation;
			graphicObject.transform.localScale = startScale;
			thrownResetCor = StartCoroutine(ResetThrownState());
			CurPickState = PickState.Thrown;
		}
	}

	private void EnableParentContraint(Transform parent)
	{
		ClearParentConstraintSources();
		ConstraintSource constraintSource = default(ConstraintSource);
		constraintSource.sourceTransform = parent;
		constraintSource.weight = 1f;
		ConstraintSource source = constraintSource;
		parentConstraint.AddSource(source);
		parentConstraint.constraintActive = true;
	}

	private void DisableParentConstraint()
	{
		ClearParentConstraintSources();
		parentConstraint.constraintActive = false;
	}

	private void ClearParentConstraintSources()
	{
		for (int i = 0; i < parentConstraint.sourceCount; i++)
		{
			parentConstraint.RemoveSource(i);
		}
	}

	protected void ResetTransform()
	{
		DisableParentConstraint();
		graphicObject.transform.localEulerAngles = Vector3.zero;
		graphicObject.transform.localPosition = Vector3.zero;
		base.transform.localEulerAngles = Vector3.zero;
		base.transform.localScale = startScale;
		base.transform.position = startPos;
		base.transform.rotation = startRot;
	}

	protected virtual IEnumerator ResetThrownState()
	{
		WaitForSeconds waitOnProcessing = new WaitForSeconds(1f);
		WaitForSeconds waitOnNotProcessing = new WaitForSeconds(3f);
		float thrownTimer = thrownLifetime;
		while (true)
		{
			if (!ProcessingSwitch.OnProcessingDistance)
			{
				if (thrownTimer == 0f)
				{
					break;
				}
				thrownTimer = Mathf.Clamp(thrownTimer - 3f, 0f, thrownLifetime);
				yield return waitOnNotProcessing;
			}
			else
			{
				if (thrownTimer != thrownLifetime)
				{
					thrownTimer = thrownLifetime;
				}
				yield return waitOnProcessing;
			}
		}
		ResetTransform();
		CurPickState = PickState.OnStartPosition;
	}

	private Vector3 CalcDropPosition()
	{
		if (Physics.Raycast(new Ray(base.transform.position, Vector3.down), out RaycastHit hitInfo, 100f, 1 << Layers.ColliderLayer))
		{
			return hitInfo.point + dropOffset;
		}
		return Vector3.positiveInfinity;
	}

	public virtual bool CanBePicked(ItemUser itemUser)
	{
		if (CurPickState != PickState.Picked && (CurItemUser == null || CurItemUser == itemUser))
		{
			return Pickable;
		}
		return false;
	}
}
