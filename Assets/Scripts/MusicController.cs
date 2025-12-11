using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class MusicController : MonoBehaviour
{
	private class StoredTheme
	{
		public AudioClipVolume theme;

		public float time;
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003C_003Ec__DisplayClass30_0
	{
		public float changeVolumeDuration;
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Predicate<EnemyController> _003C_003E9__30_1;

		public static Func<EnemyController, float> _003C_003E9__30_2;

		public static Predicate<AudioClipVolume> _003C_003E9__31_1;

		public static Func<AudioClipVolume, bool> _003C_003E9__31_2;

		public static Action<AudioClipVolume> _003C_003E9__31_3;

		internal bool _003CUpdate_003Eb__30_1(EnemyController x)
		{
			if (x.CurState != EnemyController.State.Die)
			{
				return x.CurState != EnemyController.State.None;
			}
			return false;
		}

		internal float _003CUpdate_003Eb__30_2(EnemyController x)
		{
			return (x.ModelTransform.position - PlayerSpawner.PlayerInstance.transform.position).sqrMagnitude;
		}

		internal bool _003CSelectNextGameTheme_003Eb__31_1(AudioClipVolume x)
		{
			return !x.isUsed;
		}

		internal bool _003CSelectNextGameTheme_003Eb__31_2(AudioClipVolume x)
		{
			return x.isUsed;
		}

		internal void _003CSelectNextGameTheme_003Eb__31_3(AudioClipVolume x)
		{
			x.isUsed = false;
		}
	}

	[Header("Основные настройки")]
	[Postfix(PostfixAttribute.Id.Seconds)]
	[SerializeField]
	private float changeThemeDuriation = 1f;

	[Postfix(PostfixAttribute.Id.Seconds)]
	[SerializeField]
	private float changeMenuThemeDuration = 0.5f;

	[Postfix(PostfixAttribute.Id.Seconds)]
	[SerializeField]
	private float forestThemeMinDuration = 150f;

	[SerializeField]
	private float forestThemeMaxDuration = 240f;

	private float forestThemeTimer;

	[SerializeField]
	private AudioClipVolume menuTheme;

	[SerializeField]
	private AudioClipVolume forestTheme;

	[SerializeField]
	private AudioClipVolume mainTheme1;

	[SerializeField]
	private AudioClipVolume mainTheme2;

	[SerializeField]
	private AudioClipVolume stealthTheme;

	[Header("Настройки боя")]
	[SerializeField]
	private bool allowMainThemeInCombat = true;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private float mainThemeCombatVolumeMod = 50f;

	[Header("Настройки стелса")]
	[SerializeField]
	private float stealthThemeEnableDistance = 15f;

	[SerializeField]
	private float stealthThemeDisableDistance = 24f;

	[Header("Ссылки")]
	[SerializeField]
	private AudioSource gameAudioSource;

	[SerializeField]
	private AudioSource stealthAudioSource;

	[SerializeField]
	private AudioSource menuAudioSource;

	private List<AudioClipVolume> mainGameThemes = new List<AudioClipVolume>();

	private AudioClipVolume curGameTheme;

	private AudioClipVolume nextGameTheme;

	private StoredTheme beforeCombatTheme;

	private bool IsMenuSceneActive => SceneController.Instance.CurActiveScene == SceneController.SceneType.Menu;

	private bool IsPlayerInCombat
	{
		get
		{
			if (PlayerSpawner.PlayerInstance != null)
			{
				return PlayerSpawner.PlayerInstance.PlayerCombat.InCombat;
			}
			return false;
		}
	}

	private bool IsPlayerInvisible => ManagerBase<PlayerManager>.Instance.Invisibility.IsActive;

	private void Start()
	{
		gameAudioSource.volume = 0f;
		stealthAudioSource.volume = 0f;
		menuAudioSource.volume = 0f;
		stealthAudioSource.clip = stealthTheme.clip;
		menuAudioSource.clip = menuTheme.clip;
		gameAudioSource.ignoreListenerPause = true;
		stealthAudioSource.ignoreListenerPause = true;
		menuAudioSource.ignoreListenerPause = true;
		mainGameThemes.Add(mainTheme1);
		mainGameThemes.Add(mainTheme2);
		SelectNextGameTheme();
	}

	private void OnDestroy()
	{
	}

	private void Update()
	{
		if (nextGameTheme == null && beforeCombatTheme == null)
		{
			if (curGameTheme == forestTheme && gameAudioSource.isPlaying)
			{
				forestThemeTimer -= Time.unscaledDeltaTime;
				if (forestThemeTimer <= 0f)
				{
					SelectNextGameTheme();
				}
			}
			else if (curGameTheme != forestTheme && gameAudioSource.time >= curGameTheme.clip.length - changeThemeDuriation / 2f)
			{
				SelectNextGameTheme();
			}
		}
		if (!gameAudioSource.isPlaying && nextGameTheme != null)
		{
			curGameTheme = nextGameTheme;
			gameAudioSource.clip = curGameTheme.clip;
			gameAudioSource.time = 0f;
			nextGameTheme = null;
			if (curGameTheme == forestTheme)
			{
				forestThemeTimer = UnityEngine.Random.Range(forestThemeMinDuration, forestThemeMaxDuration);
			}
			gameAudioSource.loop = (curGameTheme == forestTheme);
		}
		_003C_003Ec__DisplayClass30_0 _003C_003Ec__DisplayClass30_ = default(_003C_003Ec__DisplayClass30_0);
		_003C_003Ec__DisplayClass30_.changeVolumeDuration = 0f;
		if (IsMenuSceneActive || menuAudioSource.isPlaying)
		{
			_003C_003Ec__DisplayClass30_.changeVolumeDuration = changeMenuThemeDuration / 2f;
		}
		else
		{
			_003C_003Ec__DisplayClass30_.changeVolumeDuration = changeThemeDuriation / 2f;
		}
		if (IsMenuSceneActive)
		{
			_003CUpdate_003Eg__UpdateVolume_007C30_0(gameAudioSource, 0f, 0f, curGameTheme.Volume, ref _003C_003Ec__DisplayClass30_);
			_003CUpdate_003Eg__UpdateVolume_007C30_0(stealthAudioSource, 0f, 0f, stealthTheme.Volume, ref _003C_003Ec__DisplayClass30_);
			if (!gameAudioSource.isPlaying && !stealthAudioSource.isPlaying)
			{
				_003CUpdate_003Eg__UpdateVolume_007C30_0(menuAudioSource, menuTheme.Volume, 0f, menuTheme.Volume, ref _003C_003Ec__DisplayClass30_);
			}
			return;
		}
		if (IsPlayerInCombat)
		{
			_003CUpdate_003Eg__UpdateVolume_007C30_0(menuAudioSource, 0f, 0f, menuTheme.Volume, ref _003C_003Ec__DisplayClass30_);
			_003CUpdate_003Eg__UpdateVolume_007C30_0(stealthAudioSource, 0f, 0f, stealthTheme.Volume, ref _003C_003Ec__DisplayClass30_);
			if (menuAudioSource.isPlaying || stealthAudioSource.isPlaying)
			{
				return;
			}
			if (gameAudioSource.clip != forestTheme.clip)
			{
				if (allowMainThemeInCombat)
				{
					float targetVolume = curGameTheme.Volume * mainThemeCombatVolumeMod / 100f;
					_003CUpdate_003Eg__UpdateVolume_007C30_0(gameAudioSource, targetVolume, 0f, curGameTheme.Volume, ref _003C_003Ec__DisplayClass30_);
					return;
				}
				_003CUpdate_003Eg__UpdateVolume_007C30_0(gameAudioSource, 0f, 0f, curGameTheme.Volume, ref _003C_003Ec__DisplayClass30_);
				if (!gameAudioSource.isPlaying)
				{
					beforeCombatTheme = new StoredTheme
					{
						theme = curGameTheme,
						time = gameAudioSource.time
					};
					curGameTheme = forestTheme;
					gameAudioSource.clip = curGameTheme.clip;
					gameAudioSource.loop = (curGameTheme == forestTheme);
				}
			}
			else
			{
				_003CUpdate_003Eg__UpdateVolume_007C30_0(gameAudioSource, curGameTheme.Volume, 0f, curGameTheme.Volume, ref _003C_003Ec__DisplayClass30_);
			}
			return;
		}
		if (IsPlayerInvisible)
		{
			_003CUpdate_003Eg__UpdateVolume_007C30_0(menuAudioSource, 0f, 0f, menuTheme.Volume, ref _003C_003Ec__DisplayClass30_);
			float num = EnemyController.Instances.FindAll((EnemyController x) => x.CurState != EnemyController.State.Die && x.CurState != EnemyController.State.None).Min((EnemyController x) => (x.ModelTransform.position - PlayerSpawner.PlayerInstance.transform.position).sqrMagnitude);
			if ((stealthAudioSource.isPlaying && num < Mathf.Pow(stealthThemeDisableDistance, 2f)) || (!stealthAudioSource.isPlaying && num < Mathf.Pow(stealthThemeEnableDistance, 2f)))
			{
				_003CUpdate_003Eg__UpdateVolume_007C30_0(gameAudioSource, 0f, 0f, curGameTheme.Volume, ref _003C_003Ec__DisplayClass30_);
				if (!gameAudioSource.isPlaying)
				{
					_003CUpdate_003Eg__UpdateVolume_007C30_0(stealthAudioSource, stealthTheme.Volume, 0f, stealthTheme.Volume, ref _003C_003Ec__DisplayClass30_);
				}
			}
			else
			{
				_003CUpdate_003Eg__UpdateVolume_007C30_0(stealthAudioSource, 0f, 0f, stealthTheme.Volume, ref _003C_003Ec__DisplayClass30_);
				if (!stealthAudioSource.isPlaying)
				{
					_003CUpdate_003Eg__UpdateVolume_007C30_0(gameAudioSource, curGameTheme.Volume, 0f, curGameTheme.Volume, ref _003C_003Ec__DisplayClass30_);
				}
			}
			return;
		}
		_003CUpdate_003Eg__UpdateVolume_007C30_0(menuAudioSource, 0f, 0f, menuTheme.Volume, ref _003C_003Ec__DisplayClass30_);
		_003CUpdate_003Eg__UpdateVolume_007C30_0(stealthAudioSource, 0f, 0f, stealthTheme.Volume, ref _003C_003Ec__DisplayClass30_);
		if (beforeCombatTheme != null)
		{
			_003CUpdate_003Eg__UpdateVolume_007C30_0(gameAudioSource, 0f, 0f, curGameTheme.Volume, ref _003C_003Ec__DisplayClass30_);
			if (!gameAudioSource.isPlaying)
			{
				gameAudioSource.clip = beforeCombatTheme.theme.clip;
				gameAudioSource.time = beforeCombatTheme.time;
				gameAudioSource.loop = (curGameTheme == forestTheme);
				curGameTheme = beforeCombatTheme.theme;
				beforeCombatTheme = null;
			}
		}
		else if (nextGameTheme != null)
		{
			_003CUpdate_003Eg__UpdateVolume_007C30_0(gameAudioSource, 0f, 0f, curGameTheme.Volume, ref _003C_003Ec__DisplayClass30_);
		}
		else if (!stealthAudioSource.isPlaying && !menuAudioSource.isPlaying)
		{
			_003CUpdate_003Eg__UpdateVolume_007C30_0(gameAudioSource, curGameTheme.Volume, 0f, curGameTheme.Volume, ref _003C_003Ec__DisplayClass30_);
		}
	}

	private void SelectNextGameTheme()
	{
		if (curGameTheme == null)
		{
			if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
			{
				nextGameTheme = forestTheme;
			}
			else
			{
				_003CSelectNextGameTheme_003Eg__SelectRandomMainTheme_007C31_0();
			}
		}
		else if (curGameTheme == forestTheme)
		{
			_003CSelectNextGameTheme_003Eg__SelectRandomMainTheme_007C31_0();
		}
		else
		{
			nextGameTheme = forestTheme;
		}
	}

	[CompilerGenerated]
	private static void _003CUpdate_003Eg__UpdateVolume_007C30_0(AudioSource audioSource, float targetVolume, float minVolume, float maxVolume, ref _003C_003Ec__DisplayClass30_0 P_4)
	{
		if (targetVolume == 0f)
		{
			if (audioSource.volume == 0f)
			{
				if (audioSource.isPlaying)
				{
					audioSource.Pause();
				}
				return;
			}
		}
		else if (!audioSource.isPlaying)
		{
			if (audioSource.time != 0f)
			{
				audioSource.UnPause();
			}
			else
			{
				audioSource.Play();
			}
		}
		float num = maxVolume / P_4.changeVolumeDuration;
		targetVolume *= ManagerBase<SettingsManager>.Instance.musicVolume;
		maxVolume = Mathf.Max(audioSource.volume, maxVolume * ManagerBase<SettingsManager>.Instance.musicVolume, targetVolume * ManagerBase<SettingsManager>.Instance.musicVolume);
		float num2 = Mathf.Sign(targetVolume - audioSource.volume);
		float value = num2 * num * Time.unscaledDeltaTime;
		value = ((!(num2 > 0f)) ? Mathf.Clamp(value, targetVolume - audioSource.volume, maxVolume) : Mathf.Clamp(value, minVolume, targetVolume - audioSource.volume));
		audioSource.volume += value;
	}

	[CompilerGenerated]
	private void _003CSelectNextGameTheme_003Eg__SelectRandomMainTheme_007C31_0()
	{
		AudioClipVolume audioClipVolume = mainGameThemes.FindAll((AudioClipVolume x) => !x.isUsed).Random();
		audioClipVolume.isUsed = true;
		if (mainGameThemes.All((AudioClipVolume x) => x.isUsed))
		{
			mainGameThemes.ForEach(delegate(AudioClipVolume x)
			{
				x.isUsed = false;
			});
		}
		nextGameTheme = audioClipVolume;
	}
}
