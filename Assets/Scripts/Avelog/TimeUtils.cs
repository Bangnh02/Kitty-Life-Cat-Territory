using System;
using UnityEngine;

namespace Avelog
{
	public class TimeUtils
	{
		public enum TimeType
		{
			System,
			FromBoot
		}

		private static TimeUtils instance;

		private AndroidJavaObject jo;

		private static TimeUtils Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new TimeUtils();
				}
				return instance;
			}
		}

		private TimeUtils()
		{
		}

		public static long GetDeviceTime(TimeType timeType = TimeType.FromBoot)
		{
			switch (timeType)
			{
			case TimeType.System:
				return DateTime.Now.Ticks / 10000000;
			case TimeType.FromBoot:
				return DateTime.Now.Ticks / 10000000;
			default:
				return 0L;
			}
		}

		public static long GetDeviceTimeTicks(TimeType timeType = TimeType.FromBoot)
		{
			return GetDeviceTime(timeType) * 10000;
		}

		public static void SecondsToHMS(long secondsInput, out int hours, out int minutes, out int seconds)
		{
			minutes = (int)(secondsInput / 60);
			seconds = (int)(secondsInput % 60);
			hours = minutes / 60;
			minutes %= 60;
		}

		public static void SecondsToMS(long secondsInput, out int minutes, out int seconds)
		{
			minutes = (int)(secondsInput / 60);
			seconds = (int)(secondsInput % 60);
			minutes %= 60;
		}

		public static void SecondsToMS(float secondsInput, out int minutes, out int seconds)
		{
			minutes = (int)(secondsInput / 60f);
			seconds = (int)(secondsInput % 60f);
			minutes %= 60;
		}
	}
}
