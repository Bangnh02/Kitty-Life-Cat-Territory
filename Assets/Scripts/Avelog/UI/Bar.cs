using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Avelog.UI
{
	public class Bar : MonoBehaviour, IInitializableUI
	{
		private enum State
		{
			None,
			SetValueInstantly,
			IncreaseValueSlowly,
			DecreaseValueSlowly,
			Paused
		}

		private enum ChangeBarType
		{
			Speed,
			Time
		}

		[Tooltip("Скорость заполнения бара в долях в секунду (fillSpeed = 1 означает полный бар за секунду)")]
		public float fillSpeed = 1f;

		[Tooltip("Использование реального времени для плавного заполнения бара. Делает бар независимым от Time.timeScale (и при 0 таймскейле бар так же обновляется)")]
		[SerializeField]
		private bool useRealtime;

		[Tooltip("Продолжение обновления бара пока он или его родительские объекты (например окно) выключены")]
		[SerializeField]
		private bool continueUpdateWhileDisabled;

		private const bool increaseWithBackgroundFill = false;

		private const bool decreaseWithBackgroundFill = true;

		[Header("Ссылки")]
		[SerializeField]
		private Image mainFillImage;

		[Tooltip("Подложка заполнения. Необязательный параметр")]
		[SerializeField]
		private Image backgroundFillImage;

		[Header("Debug")]
		[ReadonlyInspector]
		[SerializeField]
		private State prevState;

		[ReadonlyInspector]
		[SerializeField]
		private State _curState;

		private Vector2 startAnchorMax = Vector2.zero;

		private float targetBarValue;

		private Action curEndCallback;

		private Coroutine settingValueCor;

		private float coroutinePrevTime;

		private List<Image> slowFillImages = new List<Image>();

		private bool isInitialized;

		public Image MainFillImage => mainFillImage;

		public Image BackgroundFillImage => backgroundFillImage;

		private State CurState
		{
			get
			{
				return _curState;
			}
			set
			{
				prevState = _curState;
				_curState = value;
			}
		}

		private bool IsBackgroundFillUsed
		{
			get
			{
				if (_003Cget_IsBackgroundFillUsed_003Eg__IsBackgroundChangeNeeded_007C19_0())
				{
					return IsBackgroundReadyForChange();
				}
				return false;
			}
		}

		public float CurBarValue
		{
			get;
			private set;
		} = 1f;


		public bool IsProcessing => CurState != State.None;

		public void OnInitializeUI()
		{
			Initialize();
		}

		private void Initialize()
		{
			if (!isInitialized)
			{
				startAnchorMax = mainFillImage.rectTransform.anchorMax;
				isInitialized = true;
			}
		}

		private void OnEnable()
		{
			if (CurState != State.Paused)
			{
				return;
			}
			if (settingValueCor != null)
			{
				if (backgroundFillImage != null)
				{
					backgroundFillImage.enabled = false;
				}
				StopCoroutine(settingValueCor);
			}
			CurState = prevState;
			settingValueCor = StartCoroutine(SettingValue());
		}

		private void OnDisable()
		{
			if (settingValueCor != null)
			{
				CurState = State.Paused;
			}
		}

		private void SetFillAmount(Image image, float fillAmount)
		{
			Initialize();
			if (image.type == Image.Type.Filled)
			{
				image.fillAmount = fillAmount;
			}
			else if (image.type == Image.Type.Tiled)
			{
				image.rectTransform.anchorMax = new Vector2(Mathf.Lerp(image.rectTransform.anchorMin.x, startAnchorMax.x, fillAmount), image.rectTransform.anchorMax.y);
			}
		}

		public bool IsBackgroundReadyForChange()
		{
			if (backgroundFillImage != null)
			{
				return backgroundFillImage.gameObject.activeSelf;
			}
			return false;
		}

		public void SetValue(float valuePart, Action endCallback = null)
		{
			targetBarValue = valuePart;
			State state = State.None;
			state = ((fillSpeed <= 0f || CurBarValue == valuePart) ? State.SetValueInstantly : ((!(CurBarValue > valuePart)) ? State.IncreaseValueSlowly : State.DecreaseValueSlowly));
			if (CurState == state)
			{
				curEndCallback?.Invoke();
				curEndCallback = null;
				curEndCallback = endCallback;
				if (state != State.SetValueInstantly)
				{
					return;
				}
			}
			if (settingValueCor != null)
			{
				if (backgroundFillImage != null)
				{
					backgroundFillImage.enabled = false;
				}
				StopCoroutine(settingValueCor);
			}
			settingValueCor = null;
			curEndCallback?.Invoke();
			curEndCallback = null;
			if (state == State.SetValueInstantly)
			{
				SetValueInstantly(valuePart);
				endCallback?.Invoke();
				return;
			}
			CurState = state;
			curEndCallback = endCallback;
			if (!base.gameObject.activeInHierarchy)
			{
				coroutinePrevTime = Time.time;
				CurState = State.Paused;
			}
			else
			{
				settingValueCor = StartCoroutine(SettingValue());
			}
		}

		public void SetValueInstantly(float valuePart)
		{
			curEndCallback?.Invoke();
			curEndCallback = null;
			CurState = State.SetValueInstantly;
			targetBarValue = valuePart;
			CurBarValue = valuePart;
			SetFillAmount(mainFillImage, valuePart);
			if (IsBackgroundReadyForChange())
			{
				SetFillAmount(backgroundFillImage, valuePart);
			}
			CurState = State.None;
		}

        private IEnumerator SettingValue()
        {
            slowFillImages.Clear();
            Image instantFillImage = null;

            if (IsBackgroundFillUsed)
                backgroundFillImage.enabled = true;

            // Chọn ảnh đi chậm / ảnh nhảy tức thì theo state
            if (CurState == State.DecreaseValueSlowly)
            {
                if (IsBackgroundFillUsed)
                {
                    instantFillImage = mainFillImage;
                    slowFillImages.Add(backgroundFillImage);
                }
                else
                {
                    slowFillImages.Add(mainFillImage);
                }
            }
            else if (CurState == State.IncreaseValueSlowly)
            {
                if (IsBackgroundFillUsed)
                    instantFillImage = backgroundFillImage;

                slowFillImages.Add(mainFillImage);
            }

            // Local helpers bám sát decompile
            float StartValue() => this.CurBarValue;
            float EndValue() => this.targetBarValue;
            float GetCurTime() => this.useRealtime ? Time.realtimeSinceStartup : Time.time;

            void FillBar(float dt)
            {
                float dir = Mathf.Sign(EndValue() - StartValue());
                float delta = this.fillSpeed * dt * dir;

                // clamp theo min/max giữa "giá trị hiện tại" và "đích"
                float lo = Mathf.Min(StartValue(), EndValue());
                float hi = Mathf.Max(StartValue(), EndValue());
                this.CurBarValue = Mathf.Clamp(this.CurBarValue + delta, lo, hi);

                // cập nhật các ảnh đi chậm
                slowFillImages.ForEach(slowImg => SetFillAmount(slowImg, this.CurBarValue));

                // cập nhật ảnh nhảy tức thì (nếu có) tới đích
                if (instantFillImage != null)
                    SetFillAmount(instantFillImage, EndValue());
            }

            // Set ảnh ban đầu
            if (instantFillImage != null)
                SetFillAmount(instantFillImage, EndValue());

            slowFillImages.ForEach(img => SetFillAmount(img, StartValue()));

            var realTimeWait = new WaitForSecondsRealtime(0f);

            // Nếu vừa resume từ Paused và cho phép cập nhật bù
            if (prevState == State.Paused && continueUpdateWhileDisabled)
            {
                float timeDelta = GetCurTime() - coroutinePrevTime;
                FillBar(timeDelta);
            }

            // Vòng cập nhật mượt cho tới khi đạt đích
            while (!CurBarValue.NearlyEqual(EndValue()))
            {
                coroutinePrevTime = GetCurTime();

                if (useRealtime) yield return realTimeWait;
                else yield return null;

                float dt = GetCurTime() - coroutinePrevTime;
                FillBar(dt);
            }

            if (backgroundFillImage != null)
                backgroundFillImage.enabled = false;

            CurState = State.None;
            settingValueCor = null;

            var cb = curEndCallback;
            curEndCallback = null;
            cb?.Invoke();
        }

        [CompilerGenerated]
		private bool _003Cget_IsBackgroundFillUsed_003Eg__IsBackgroundChangeNeeded_007C19_0()
		{
			if (CurState != State.DecreaseValueSlowly)
			{
				State curState = CurState;
				return CurState == State.SetValueInstantly;
			}
			return true;
		}
	}
}
