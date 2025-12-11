using System;
using UnityEngine;

[Serializable]
public class PostfixAttribute : PropertyAttribute
{
	public enum Id
	{
		Percents,
		Seconds,
		Minutes,
		Hours,
		PBI,
		PBS
	}

	public Id id;

	public PostfixAttribute(Id id)
	{
		this.id = id;
	}
}
