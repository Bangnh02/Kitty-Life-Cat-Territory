using System;
using UnityEngine;

public static class Vector2Ext
{
	public static Vector2 Rotate(this Vector2 source, float angle)
	{
		return new Vector2(source.x * Mathf.Cos((float)Math.PI / 180f * angle) - source.y * Mathf.Sin((float)Math.PI / 180f * angle), source.x * Mathf.Sin((float)Math.PI / 180f * angle) + source.y * Mathf.Cos((float)Math.PI / 180f * angle));
	}
}
