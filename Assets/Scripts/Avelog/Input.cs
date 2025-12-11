using System;
using UnityEngine;

namespace Avelog
{
	public static class Input
	{
		public delegate void TouchRotateHandler(Vector2 touchDelta);

		public delegate void pressJoystickTypeChangeToggleHandler(JoystickType joystickType);

		public delegate void pressVSyncChangeToggleHandler(int vSyncCount);

		public delegate void makeChildHandler(string childName);

		public delegate void MakeSpouseHandler(string spouseName);

		private static float horAxis;

		private static float vertAxis;

		private static Vector2 animationAxisInput;

		public static float HorAxis
		{
			get
			{
				float axis = UnityEngine.Input.GetAxis("Horizontal");
				if (axis != 0f)
				{
					return axis;
				}
				return horAxis;
			}
			set
			{
				horAxis = value;
			}
		}

		public static float VertAxis
		{
			get
			{
				float axis = UnityEngine.Input.GetAxis("Vertical");
				if (axis != 0f)
				{
					return axis;
				}
				return vertAxis;
			}
			set
			{
				vertAxis = value;
			}
		}

		public static Vector2 AnimationAxisInput
		{
			get
			{
				return animationAxisInput;
			}
			set
			{
				animationAxisInput = value;
			}
		}

		public static event TouchRotateHandler touchRotateEvent;

		public static event Action jumpPressedEvent;

		public static event Action attackPressedEvent;

		public static event Action invisibilityPressedEvent;

		public static event Action eatPressedEvent;

		public static event Action drinkPressedEvent;

		public static event Action pickPressedEvent;

		public static event Action dropPressedEvent;

		public static event Action familyEatPressedEvent;

		public static event Action familyDrinkPressedEvent;

		public static event Action plantPressedEvent;

		public static event Action satisfyFarmResidentPressedEvent;

		public static event Action pausePressedEvent;

		public static event Action<Vector2> SetResolutionEvent;

		public static event Action<bool> pressAntiAliasingToggleEvent;

		public static event pressJoystickTypeChangeToggleHandler pressJoystickTypeChangeToggleEvent;

		public static event pressVSyncChangeToggleHandler pressVSyncChangeToggleEvent;

		public static event makeChildHandler makeChildPressedEvent;

		public static event MakeSpouseHandler makeSpousePressedEvent;

		public static event Action startGamePressedEvent;

		public static event Action toMainMenuPressedEvent;

		public static event Action questHintPressedEvent;

		public static event Action changeGenderPressedEvent;

		public static event Action sleepButtonPressedEvent;

		public static void FireTouchRotate(Vector2 touchDelta)
		{
			Input.touchRotateEvent?.Invoke(touchDelta);
		}

		public static void FireJumpPressed()
		{
			Input.jumpPressedEvent?.Invoke();
		}

		public static void FireAttackPressed()
		{
			Input.attackPressedEvent?.Invoke();
		}

		public static void FireInvisibilityPressed()
		{
			Input.invisibilityPressedEvent?.Invoke();
		}

		public static void FireEatPressed()
		{
			Input.eatPressedEvent?.Invoke();
		}

		public static void FireDrinkPressed()
		{
			Input.drinkPressedEvent?.Invoke();
		}

		public static void FirePickPressed()
		{
			Input.pickPressedEvent?.Invoke();
		}

		public static void FireDropPressed()
		{
			Input.dropPressedEvent?.Invoke();
		}

		public static void FireFamilyEatPressed()
		{
			Input.familyEatPressedEvent?.Invoke();
		}

		public static void FireFamilyDrinkPressed()
		{
			Input.familyDrinkPressedEvent?.Invoke();
		}

		public static void FirePlantPressed()
		{
			Input.plantPressedEvent?.Invoke();
		}

		public static void FireSatisfyFarmResidentPressed()
		{
			Input.satisfyFarmResidentPressedEvent?.Invoke();
		}

		public static void FirePausePressed()
		{
			Input.pausePressedEvent?.Invoke();
		}

		public static void FireSetResolutionEvent(Vector2 resolution)
		{
			Input.SetResolutionEvent?.Invoke(resolution);
		}

		public static void FirePressAntiAliasingToggleEvent(bool state)
		{
			Input.pressAntiAliasingToggleEvent?.Invoke(state);
		}

		public static void FirePressJoystickTypeChangeToggleEvent(JoystickType joystickType)
		{
			Input.pressJoystickTypeChangeToggleEvent?.Invoke(joystickType);
		}

		public static void FirePressVSyncTypeChangeToggleEvent(int vSyncCount)
		{
			Input.pressVSyncChangeToggleEvent?.Invoke(vSyncCount);
		}

		public static void FireMakeChildPressed(string childName)
		{
			Input.makeChildPressedEvent?.Invoke(childName);
		}

		public static void FireMakeSpousePressed(string spouseName)
		{
			Input.makeSpousePressedEvent?.Invoke(spouseName);
		}

		public static void FireStartGamePressed()
		{
			Input.startGamePressedEvent?.Invoke();
		}

		public static void FireToMainMenuPressed()
		{
			Input.toMainMenuPressedEvent?.Invoke();
		}

		public static void FireQuestHintPressed()
		{
			Input.questHintPressedEvent?.Invoke();
		}

		public static void FireChangeGenderPressed()
		{
			Input.changeGenderPressedEvent?.Invoke();
		}

		public static void SleepButtonPressed()
		{
			Input.sleepButtonPressedEvent?.Invoke();
		}
	}
}
