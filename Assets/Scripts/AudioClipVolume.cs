using System;
using UnityEngine;

[Serializable]
public class AudioClipVolume
{
	public AudioClip clip;

	[Range(0f, 100f)]
	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private int volume = 100;

	[HideInInspector]
	public bool isUsed;

	public float Volume => (float)volume / 100f;
}
