using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class IEnumerableExt
{
	public static T Random<T>(this IEnumerable<T> collection, Func<T, float> getElementChanceMethod)
	{
		if (collection == null)
		{
			return default(T);
		}
		if (collection.Count() == 0)
		{
			return default(T);
		}
		List<(T, float)> list = new List<(T, float)>();
		float num = 0f;
		foreach (T item in collection)
		{
			float num2 = getElementChanceMethod(item);
			num += num2;
			list.Add((item, num2));
		}
		float num3 = UnityEngine.Random.Range(0f, num);
		float num4 = 0f;
		float num5 = 0f;
		for (int i = 0; i < list.Count; i++)
		{
			num4 = num5;
			num5 += list[i].Item2;
			if (num3 >= num4 && num3 <= num5)
			{
				return list[i].Item1;
			}
		}
		return default(T);
	}

	public static T Random<T>(this IEnumerable<T> collection)
	{
		if (collection == null)
		{
			return default(T);
		}
		int num = collection.Count();
		if (num == 0)
		{
			return default(T);
		}
		int index = UnityEngine.Random.Range(0, num);
		return collection.ElementAt(index);
	}
}
