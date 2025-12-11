using UnityEngine;

namespace Avelog
{
	public static class AnimationHashes
	{
		public class AnimationHash
		{
			public int Value
			{
				get;
				private set;
			}

			public AnimationHash(string animationName)
			{
				Value = Animator.StringToHash(animationName);
			}
		}

		public static readonly AnimationHash idle = new AnimationHash("Idle");

		public static readonly AnimationHash extraIdle = new AnimationHash("ExtraIdle");

		public static readonly AnimationHash eat = new AnimationHash("Eat");

		public static readonly AnimationHash drink = new AnimationHash("Drink");

		public static readonly AnimationHash meetingPlayer = new AnimationHash("MeetingPlayer");

		public static readonly AnimationHash takeDamage = new AnimationHash("TakeDamage");

		public static readonly AnimationHash eatEnter = new AnimationHash("EatEnter");

		public static readonly AnimationHash eatCycle = new AnimationHash("EatCycle");

		public static readonly AnimationHash eatExit = new AnimationHash("EatExit");

		public static readonly AnimationHash drinkEnter = new AnimationHash("DrinkEnter");

		public static readonly AnimationHash drinkCycle = new AnimationHash("DrinkCycle");

		public static readonly AnimationHash drinkExit = new AnimationHash("DrinkExit");

		public static readonly AnimationHash sleep = new AnimationHash("Sleep");
	}
}
