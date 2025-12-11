using Avelog;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickScript : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private RectTransform joystickBase;

	public RectTransform PointerTransform;

	private RectTransform joystickRect;

	[SerializeField]
	private float inputMultiplier = 1.45f;

	[Range(0f, 1f)]
	[SerializeField]
	private float minHorInput = 0.1f;

	private bool isPointerDown;

	private Vector3 joystickStartPos;

	private Vector2 targetInput;

	private Vector2 curInput;

	private RectTransform BaseRect => joystickBase;

	public void OnInitializeUI()
	{
		joystickRect = GetComponent<RectTransform>();
		joystickStartPos = joystickRect.position;
	}

	private void OnDisable()
	{
		if (isPointerDown)
		{
			PointerUp(null);
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause && isPointerDown)
		{
			PointerUp(null);
		}
	}

	public void OnDrag(BaseEventData event_data)
	{
		Vector2 vector = (event_data as PointerEventData).position - new Vector2(base.transform.position.x, base.transform.position.y);
		if (isPointerDown)
		{
			vector = Vector2.ClampMagnitude(vector, BaseRect.rect.xMax);
			PointerTransform.localPosition = vector;
			vector.y = Mathf.Clamp(vector.y * inputMultiplier, 0f - BaseRect.rect.xMax, BaseRect.rect.xMax);
			float num = vector.x / BaseRect.rect.xMax;
			Mathf.Sign(num);
			if (num > 0f)
			{
				num = Mathf.Clamp(num - minHorInput, 0f, num);
			}
			else if (num < 0f)
			{
				num = Mathf.Clamp(num + minHorInput, num, 0f);
			}
			num /= 1f - minHorInput;
			if (targetInput == Vector2.zero)
			{
				Avelog.Input.AnimationAxisInput = Vector2.zero;
			}
			targetInput.Set(vector.y / BaseRect.rect.yMax, num);
		}
	}

	public void PointerDown(BaseEventData event_data)
	{
		PointerEventData pointerEventData = event_data as PointerEventData;
		if (ManagerBase<SettingsManager>.Instance.curJoystickType == JoystickType.Mobile)
		{
			joystickRect.position = pointerEventData.position;
		}
		Vector2 vector = pointerEventData.position - new Vector2(base.transform.position.x, base.transform.position.y);
		vector = Vector2.ClampMagnitude(vector, BaseRect.rect.xMax);
		PointerTransform.localPosition = new Vector3(vector.x, vector.y);
		vector.y = Mathf.Clamp(vector.y * inputMultiplier, 0f - BaseRect.rect.xMax, BaseRect.rect.xMax);
		if (targetInput == Vector2.zero)
		{
			Avelog.Input.AnimationAxisInput = Vector2.zero;
		}
		targetInput.Set(vector.y / BaseRect.rect.yMax, vector.x / BaseRect.rect.xMax);
		isPointerDown = true;
	}

	public void PointerUp(BaseEventData event_data)
	{
		PointerTransform.localPosition = new Vector3(0f, 0f);
		targetInput = Vector2.zero;
		isPointerDown = false;
		if (ManagerBase<SettingsManager>.Instance.curJoystickType == JoystickType.Mobile)
		{
			joystickRect.position = joystickStartPos;
		}
	}

	private void Update()
	{
		_003CUpdate_003Eg__RecalculateInput_007C17_0(Time.deltaTime, ref curInput.x, targetInput.x, cutoffOppisiteInput: true);
		_003CUpdate_003Eg__RecalculateInput_007C17_0(Time.deltaTime, ref curInput.y, targetInput.y, cutoffOppisiteInput: true);
		Avelog.Input.VertAxis = curInput.x;
		Avelog.Input.HorAxis = curInput.y;
	}

	[CompilerGenerated]
	private static void _003CUpdate_003Eg__RecalculateInput_007C17_0(float deltaTime, ref float curInputValue, float targetInputValue, bool cutoffOppisiteInput)
	{
		if (curInputValue != targetInputValue && deltaTime != 0f)
		{
			if (cutoffOppisiteInput && Mathf.Sign(curInputValue) != Mathf.Sign(targetInputValue))
			{
				curInputValue = 0f;
			}
			float num = Mathf.Sign(targetInputValue - curInputValue);
			curInputValue = Mathf.Clamp(curInputValue + 6f * num * deltaTime, Mathf.Min(curInputValue, targetInputValue), Mathf.Max(curInputValue, targetInputValue));
		}
	}
}
