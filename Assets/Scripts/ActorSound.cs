using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActorSound : MonoBehaviour
{
	[Serializable]
	public class SoundEffect
	{
		public AudioClip audioClip;

		[HideInInspector]
		public bool isUsed;
	}

	[Serializable]
	public class SoundEffectsVolume
	{
		[Range(0f, 100f)]
		[SerializeField]
		[Postfix(PostfixAttribute.Id.Percents)]
		private int volume = 100;

		public List<SoundEffect> soundEffects;

		public float Volume => (float)volume / 100f;
	}

	[Serializable]
	public class SoundEffectVolume
	{
		[Range(0f, 100f)]
		[SerializeField]
		[Postfix(PostfixAttribute.Id.Percents)]
		private int volume = 100;

		public SoundEffect soundEffect;

		public float Volume => (float)volume / 100f;
	}

	protected AudioSource audioSource;

	public void PlayRandomSound(List<SoundEffect> soundEffects, float volume = 1f)
	{
		PlayRandomSound(soundEffects, audioSource, volume);
	}

	public void PlayRandomSound(List<SoundEffect> soundEffects, AudioSource audioSource, float volume = 1f)
	{
		if (soundEffects.Count != 0 && !(audioSource == null))
		{
			if (audioSource.clip != null)
			{
				audioSource.clip = null;
				audioSource.volume = 1f;
			}
			SoundEffect soundEffect = soundEffects.FindAll((SoundEffect x) => !x.isUsed).Random();
			soundEffect.isUsed = true;
			if (soundEffects.Count((SoundEffect x) => x.isUsed) == soundEffects.Count)
			{
				soundEffects.ForEach(delegate(SoundEffect x)
				{
					x.isUsed = false;
				});
			}
			audioSource.PlayOneShot(soundEffect.audioClip, volume * ManagerBase<SettingsManager>.Instance.effectsVolume);
		}
	}

	public void PlayDelayedRandomSound(List<SoundEffect> soundEffects, float delay, float volume = 1f)
	{
		PlayDelayedRandomSound(soundEffects, audioSource, delay, volume);
	}

	public void PlayDelayedRandomSound(List<SoundEffect> soundEffects, AudioSource audioSource, float delay, float volume = 1f)
	{
		if (soundEffects.Count != 0 && !(audioSource == null))
		{
			SoundEffect soundEffect = soundEffects.FindAll((SoundEffect x) => !x.isUsed).Random();
			soundEffect.isUsed = true;
			if (soundEffects.Count((SoundEffect x) => x.isUsed) == soundEffects.Count)
			{
				soundEffects.ForEach(delegate(SoundEffect x)
				{
					x.isUsed = false;
				});
			}
			audioSource.clip = soundEffect.audioClip;
			audioSource.volume = volume * ManagerBase<SettingsManager>.Instance.effectsVolume;
			audioSource.PlayDelayed(delay);
		}
	}

	public void PlaySound(AudioClip sound, float volume = 1f)
	{
		PlaySound(sound, audioSource, volume);
	}

	public void PlaySound(AudioClip sound, AudioSource audioSource, float volume = 1f)
	{
		if (!(sound == null) && !(audioSource == null))
		{
			if (audioSource.clip != null)
			{
				audioSource.clip = null;
				audioSource.volume = 1f;
			}
			audioSource.PlayOneShot(sound, volume * ManagerBase<SettingsManager>.Instance.effectsVolume);
		}
	}

	public SoundEffect GetRandomSound(List<SoundEffect> soundEffects)
	{
		if (soundEffects.Count == 0 || audioSource == null)
		{
			return null;
		}
		SoundEffect soundEffect = soundEffects.FindAll((SoundEffect x) => !x.isUsed).Random();
		soundEffect.isUsed = true;
		if (soundEffects.Count((SoundEffect x) => x.isUsed) == soundEffects.Count)
		{
			soundEffects.ForEach(delegate(SoundEffect x)
			{
				x.isUsed = false;
			});
		}
		return soundEffect;
	}
}
