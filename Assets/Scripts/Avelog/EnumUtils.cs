using System;
using System.Collections.Generic;
using System.Linq;

namespace Avelog
{
	public static class EnumUtils
	{
		public static IEnumerable<T> GetValues<T>()
		{
			return (T[])Enum.GetValues(typeof(T));
		}

		public static List<T> ToList<T>()
		{
			return GetValues<T>().ToList();
		}
	}
}
