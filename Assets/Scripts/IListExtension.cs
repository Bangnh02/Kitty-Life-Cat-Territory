using System.Collections.Generic;

public static class IListExtension
{
	public static bool IsIndexValid<T>(this IList<T> collection, int index)
	{
		if (collection == null || collection.Count == 0)
		{
			return false;
		}
		if (index >= 0)
		{
			return index < collection.Count;
		}
		return false;
	}
}
