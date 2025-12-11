using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraPanel : MonoBehaviour
{
	public enum State
	{
		Disabled,
		ChangeScale,
		View,
		ChangeAlpha
	}

	[Header("ExtraPanel")]
	[SerializeField]
	protected float changeScaleTime = 0.2f;

	[SerializeField]
	protected float viewPanelTime = 2f;

	[SerializeField]
	protected float firstViewPanleTimeMulty = 2.5f;

	[SerializeField]
	protected float changeAlphaTime = 1f;

	protected State curState;

	protected float curTime;

	protected Coroutine coroutine;

	protected List<CanvasRenderer> canvasRenderers;

	[NonSerialized]
	protected bool isFirstEnablePanel;

	public State CurState => curState;

	private void OnDisable()
	{
		if (coroutine != null)
		{
			StopCoroutine(coroutine);
		}
	}

	private void OnEnable()
	{
		if (curState == State.ChangeScale)
		{
			coroutine = StartCoroutine(ChangeScaleCoroutine(resetTime: false));
		}
		else if (curState == State.View)
		{
			coroutine = StartCoroutine(ViewCoroutine(resetTime: false));
		}
		else if (curState == State.ChangeAlpha)
		{
			coroutine = StartCoroutine(ChangeAlphaCoroutine(resetTime: false));
		}
		else if (curState == State.Disabled)
		{
			canvasRenderers.ForEach(delegate(CanvasRenderer x)
			{
				x.SetAlpha(0f);
			});
		}
	}

	protected IEnumerator ChangeScaleCoroutine(bool resetTime = true)
	{
		if (resetTime)
		{
			curTime = 0f;
		}
		curState = State.ChangeScale;
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
		coroutine = StartCoroutine(ViewCoroutine());
	}

	private IEnumerator ViewCoroutine(bool resetTime = true)
	{
		if (resetTime)
		{
			curTime = 0f;
		}
		float curViewPanelTime = viewPanelTime;
		if (isFirstEnablePanel)
		{
			curViewPanelTime *= firstViewPanleTimeMulty;
		}
		curState = State.View;
		while (true)
		{
			curTime = Mathf.Clamp(curTime + Time.deltaTime, 0f, curViewPanelTime);
			if (curTime == curViewPanelTime)
			{
				break;
			}
			yield return null;
		}
		coroutine = StartCoroutine(ChangeAlphaCoroutine());
	}

	private IEnumerator ChangeAlphaCoroutine(bool resetTime = true)
	{
		if (resetTime)
		{
			curTime = 0f;
		}
		curState = State.ChangeAlpha;
		while (true)
		{
			curTime = Mathf.Clamp(curTime + Time.deltaTime, 0f, changeAlphaTime);
			float num = curTime / changeAlphaTime;
			float curAlpha = Mathf.Lerp(1f, 0f, num);
			canvasRenderers.ForEach(delegate(CanvasRenderer x)
			{
				x.SetAlpha(curAlpha);
			});
			if (num == 1f)
			{
				break;
			}
			yield return null;
		}
		curState = State.Disabled;
	}

	private void OnValidate()
	{
	}
}
