using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CollectablePanel<T> : MonoBehaviour, IInitializableUI where T : CollectablePanel<T>
{
	public enum PanelState
	{
		Enabling,
		Enabled,
		Disabling,
		Disabled
	}

	private static T instance;

	[Header("CollectablePanel")]
	[SerializeField]
	private float changeScaleTime = 0.2f;

	[SerializeField]
	private float changeAlphaTime = 1f;

	[SerializeField]
	protected Text countText;

	[SerializeField]
	protected CanvasGroup canvasGroup;

	private float curTime;

	protected Coroutine myCoroutine;

	public static T Instance
	{
		get
		{
			if ((UnityEngine.Object)instance == (UnityEngine.Object)null)
			{
				instance = UnityEngine.Object.FindObjectOfType<T>();
			}
			return instance;
		}
	}

	public PanelState СurState
	{
		get;
		protected set;
	} = PanelState.Disabled;


	public event Action startProcessingEvent;

	public virtual void OnInitializeUI()
	{
		СurState = PanelState.Disabled;
		canvasGroup.alpha = 0f;
		if ((UnityEngine.Object)instance == (UnityEngine.Object)null)
		{
			instance = (this as T);
		}
	}

	protected void StartProcessingPanel()
	{
		canvasGroup.alpha = 1f;
		if (myCoroutine != null)
		{
			StopCoroutine(myCoroutine);
		}
		this.startProcessingEvent?.Invoke();
		myCoroutine = StartCoroutine(ChangeScaleCoroutine());
	}

	protected IEnumerator ChangeScaleCoroutine(bool resetTime = true)
	{
		if (resetTime)
		{
			curTime = 0f;
		}
		СurState = PanelState.Enabling;
		while (true)
		{
			curTime = Mathf.Clamp(curTime + Time.deltaTime, 0f, changeScaleTime);
			float num = curTime / changeScaleTime;
			Vector3 localScale = Vector3.Lerp(Vector3.zero, Vector3.one, num);
			base.transform.localScale = localScale;
			if (num == 1f)
			{
				break;
			}
			yield return null;
		}
		СurState = PanelState.Enabled;
	}

	protected IEnumerator ChangeAlphaCoroutine(bool resetTime = true)
	{
		if (resetTime)
		{
			curTime = 0f;
		}
		СurState = PanelState.Disabling;
		while (true)
		{
			curTime = Mathf.Clamp(curTime + Time.deltaTime, 0f, changeAlphaTime);
			float num = curTime / changeAlphaTime;
			float alpha = Mathf.Lerp(1f, 0f, num);
			canvasGroup.alpha = alpha;
			if (num == 1f)
			{
				break;
			}
			yield return null;
		}
		СurState = PanelState.Disabled;
	}

	private void OnEnable()
	{
		if (СurState == PanelState.Enabling)
		{
			myCoroutine = StartCoroutine(ChangeScaleCoroutine(resetTime: false));
		}
		else if (СurState == PanelState.Disabling)
		{
			myCoroutine = StartCoroutine(ChangeAlphaCoroutine(resetTime: false));
		}
	}

	private void OnDisable()
	{
		if (myCoroutine != null)
		{
			StopCoroutine(myCoroutine);
		}
	}
}
