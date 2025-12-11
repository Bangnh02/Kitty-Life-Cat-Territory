using System;
using UnityEngine;

[Serializable]
public class SeparatorAttribute : PropertyAttribute
{
	public enum Colors
	{
		NONE,
		GREEN,
		BLUE,
		RED,
		GRAY,
		WHITE,
		BLACK,
		YELLOW
	}

	public readonly string title;

	public readonly float height;

	public readonly bool isReadOnly;

	public Color color
	{
		get;
		set;
	}

	public SeparatorAttribute()
	{
		title = "";
		height = 20f;
		color = new Color(0f, 0f, 0f, 0f);
		isReadOnly = false;
	}

	public SeparatorAttribute(string _title, Colors _color = Colors.WHITE, float _height = 20f, bool _isReadOnly = false)
	{
		title = _title;
		height = _height;
		isReadOnly = _isReadOnly;
		switch (_color)
		{
		case Colors.GREEN:
			color = new Color(0f, 1f, 0f, 0.5f);
			break;
		case Colors.BLACK:
			color = new Color(0f, 0f, 0f, 0.8f);
			break;
		case Colors.BLUE:
			color = new Color(0f, 0f, 1f, 0.3f);
			break;
		case Colors.RED:
			color = new Color(1f, 0f, 0f, 0.5f);
			break;
		case Colors.WHITE:
			color = new Color(1f, 1f, 1f, 0.5f);
			break;
		case Colors.YELLOW:
			color = new Color(1f, 0.92f, 0.016f, 0.5f);
			break;
		case Colors.GRAY:
			color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
			break;
		case Colors.NONE:
			color = new Color(0f, 0f, 0f, 0f);
			break;
		}
	}
}
