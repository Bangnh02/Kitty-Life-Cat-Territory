using System;
using System.Collections.Generic;

namespace Avelog
{
	public static class DictionaryExt
	{
		public static bool FindKey<Key, Value>(this Dictionary<Key, Value> dictionary, Predicate<KeyValuePair<Key, Value>> match, out Key key)
		{
			foreach (KeyValuePair<Key, Value> item in dictionary)
			{
				if (match(item))
				{
					key = item.Key;
					return true;
				}
			}
			key = default(Key);
			return false;
		}

		public static bool FindKeys<Key, Value>(this Dictionary<Key, Value> dictionary, Predicate<KeyValuePair<Key, Value>> match, out List<Key> keys)
		{
			keys = new List<Key>();
			foreach (KeyValuePair<Key, Value> item in dictionary)
			{
				if (match(item))
				{
					keys.Add(item.Key);
					return true;
				}
			}
			return false;
		}
	}
}
