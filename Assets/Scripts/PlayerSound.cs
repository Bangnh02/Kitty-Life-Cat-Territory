using Avelog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerSound : ActorSound, IInitializablePlayerComponent
{
	[Serializable]
	public class CustomSoundDuration
	{
		public AudioClip soundEffect;

		[Range(0f, 100f)]
		[SerializeField]
		[Postfix(PostfixAttribute.Id.Percents)]
		private int volume = 100;

		[SerializeField]
		private float minDelay = 2f;

		[SerializeField]
		private float maxDelay = 5f;

		[ReadonlyInspector]
		public float curDelay;

		[SerializeField]
		private float minDuration = 4f;

		[SerializeField]
		private float maxDuration = 16f;

		[ReadonlyInspector]
		public float curDuration;

		public float changeVolumeDuration = 0.15f;

		public float Volume => (float)volume / 100f;

		public void InitDelay()
		{
			curDelay = UnityEngine.Random.Range(minDelay, maxDelay);
		}

		public void InitDuration()
		{
			curDuration = UnityEngine.Random.Range(minDuration, maxDuration);
		}
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Func<AudioSource, bool> _003C_003E9__83_0;

		public static Action<AudioSource> _003C_003E9__83_1;

		public static Func<AudioSource, bool> _003C_003E9__84_0;

		public static Action<AudioSource> _003C_003E9__84_1;

		public static Func<AudioSource, bool> _003C_003E9__84_9;

		public static Func<AudioSource, bool> _003C_003E9__84_2;

		public static Func<AudioSource, bool> _003C_003E9__84_4;

		internal bool _003CStopMovingAudioSources_003Eb__83_0(AudioSource x)
		{
			return x.isPlaying;
		}

		internal void _003CStopMovingAudioSources_003Eb__83_1(AudioSource x)
		{
			x.Stop();
		}

		internal bool _003CUpdate_003Eb__84_0(AudioSource x)
		{
			return x.isPlaying;
		}

		internal void _003CUpdate_003Eb__84_1(AudioSource x)
		{
			x.Stop();
		}

		internal bool _003CUpdate_003Eb__84_9(AudioSource x)
		{
			return !x.isPlaying;
		}

		internal bool _003CUpdate_003Eb__84_2(AudioSource x)
		{
			return !x.isPlaying;
		}

		internal bool _003CUpdate_003Eb__84_4(AudioSource x)
		{
			return !x.isPlaying;
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003C_003Ec__DisplayClass84_0
	{
		public double soundDelay;
	}

	[Header("Звуки атаки")]
	[SerializeField]
	private SoundEffectsVolume hitAttackSounds;

	[SerializeField]
	private SoundEffectsVolume stealthAttackSounds;

	[SerializeField]
	private SoundEffectsVolume missAttackSounds;

	[Header("Питание")]
	[SerializeField]
	private SoundEffectsVolume drinkingSounds;

	[SerializeField]
	private SoundEffectsVolume eatingSounds;

	[Header("Движение")]
	[SerializeField]
	private float speedWhichSoundPlay = 9f;

	[SerializeField]
	private SoundEffectsVolume walkingSounds;

	[SerializeField]
	private bool walkPlayWithDelay = true;

	[SerializeField]
	private double walkSoundDelay = 0.1;

	[SerializeField]
	private SoundEffectsVolume runningSounds;

	[SerializeField]
	private bool runPlayWithDelay = true;

	[SerializeField]
	private double runSoundDelay = 0.1;

	[SerializeField]
	private SoundEffectsVolume walkingOnWaterSounds;

	[SerializeField]
	private bool walkOnWaterPlayWithDelay = true;

	[SerializeField]
	private double walkOnWaterSoundDelay = 0.1;

	[SerializeField]
	private SoundEffectsVolume landingSounds;

	[SerializeField]
	private SoundEffectsVolume jumpingSounds;

	[Header("Прочие звуки")]
	[SerializeField]
	private CustomSoundDuration purrSound;

	[SerializeField]
	private SoundEffectsVolume pickCoinSounds;

	[SerializeField]
	private SoundEffectsVolume pickClewSounds;

	[SerializeField]
	private SoundEffectVolume pickFoodSound;

	[SerializeField]
	private SoundEffectVolume dropFoodSound;

	[SerializeField]
	private SoundEffectVolume satisfyFarmResidentSound;

	[SerializeField]
	private SoundEffectsVolume plantVegetableSounds;

	[SerializeField]
	private SoundEffectVolume questStartSound;

	[SerializeField]
	private SoundEffectVolume questCompleateSound;

	[SerializeField]
	private SoundEffectVolume levelUpSound;

	[SerializeField]
	private SoundEffectVolume pickSuperBonusSound;

	[SerializeField]
	private SoundEffectVolume weddingSound;

	[SerializeField]
	private SoundEffectVolume childBornSound;

	[Header("Ссылки")]
	[SerializeField]
	private List<AudioSource> movingAudioSources;

	[SerializeField]
	private AudioSource purrAudioSource;

	[SerializeField]
	private AudioSource levelUpAudioSource;

	[SerializeField]
	private AudioSource questAudioSource;

	[SerializeField]
	private AudioSource pickCollectableAudioSource;

	[SerializeField]
	private AudioSource superBonusAudioSource;

	[SerializeField]
	private AudioSource eatAudioSource;

	[SerializeField]
	private AudioSource drinkAudioSource;

	[SerializeField]
	private AudioSource familyEventAudioSource;

	private Coroutine playSoundCoroutine;

	private bool isPlayingPurrSound;

	private bool isFalling;

	private int nextMoveAudioSource;

	private double nextMoveStartTime;

	private float timer;

	private const double minTimeToSetNextMovementSound = 0.2;

	private AudioSource MainAudioSource => audioSource;

	private double WalkSoundDelay
	{
		get
		{
			if (!walkPlayWithDelay)
			{
				return 0.0;
			}
			if (PlayerMovement.CurMoveSpeed == ManagerBase<PlayerManager>.Instance.OriginalSpeedMedium)
			{
				return walkSoundDelay;
			}
			if (PlayerMovement.CurMoveSpeed < ManagerBase<PlayerManager>.Instance.OriginalSpeedMedium)
			{
				float value = Convert.ToSingle(walkSoundDelay) - CalculationsHelpUtils.CalculateProp(Convert.ToSingle(walkSoundDelay), PlayerMovement.CurMoveSpeed, ManagerBase<PlayerManager>.Instance.OriginalSpeedMedium);
				return walkSoundDelay + Convert.ToDouble(value) * (double)(1f / PlayerWalkingAnimation.animationSpeed);
			}
			float value2 = CalculationsHelpUtils.CalculateProp(Convert.ToSingle(walkSoundDelay), PlayerMovement.CurMoveSpeed, ManagerBase<PlayerManager>.Instance.OriginalSpeedMedium) - Convert.ToSingle(walkSoundDelay);
			return walkSoundDelay - Convert.ToDouble(value2);
		}
	}

	private double RunSoundDelay
	{
		get
		{
			if (!runPlayWithDelay)
			{
				return 0.0;
			}
			if (PlayerMovement.CurMoveSpeed == ManagerBase<PlayerManager>.Instance.OriginalSpeedMaximum)
			{
				return runSoundDelay;
			}
			if (PlayerMovement.CurMoveSpeed < ManagerBase<PlayerManager>.Instance.OriginalSpeedMaximum)
			{
				float value = Convert.ToSingle(runSoundDelay) - CalculationsHelpUtils.CalculateProp(Convert.ToSingle(runSoundDelay), PlayerMovement.CurMoveSpeed, ManagerBase<PlayerManager>.Instance.OriginalSpeedMaximum);
				return runSoundDelay + Convert.ToDouble(value);
			}
			float value2 = CalculationsHelpUtils.CalculateProp(Convert.ToSingle(runSoundDelay), PlayerMovement.CurMoveSpeed, ManagerBase<PlayerManager>.Instance.OriginalSpeedMaximum) - Convert.ToSingle(runSoundDelay);
			return runSoundDelay - Convert.ToDouble(value2);
		}
	}

	private double WalkOnWaterSoundDelay
	{
		get
		{
			if (!walkPlayWithDelay)
			{
				return 0.0;
			}
			if (PlayerMovement.CurMoveSpeed == ManagerBase<PlayerManager>.Instance.OriginalSpeedMedium)
			{
				return walkOnWaterSoundDelay;
			}
			if (PlayerMovement.CurMoveSpeed < ManagerBase<PlayerManager>.Instance.OriginalSpeedMedium)
			{
				float value = Convert.ToSingle(walkOnWaterSoundDelay) - CalculationsHelpUtils.CalculateProp(Convert.ToSingle(walkOnWaterSoundDelay), PlayerMovement.CurMoveSpeed, ManagerBase<PlayerManager>.Instance.OriginalSpeedMedium);
				return walkOnWaterSoundDelay + Convert.ToDouble(value) * (double)(1f / PlayerWalkingAnimation.animationSpeed);
			}
			float value2 = CalculationsHelpUtils.CalculateProp(Convert.ToSingle(walkOnWaterSoundDelay), PlayerMovement.CurMoveSpeed, ManagerBase<PlayerManager>.Instance.OriginalSpeedMedium) - Convert.ToSingle(walkOnWaterSoundDelay);
			return walkOnWaterSoundDelay - Convert.ToDouble(value2);
		}
	}

	private PlayerCombat PlayerCombat => PlayerSpawner.PlayerInstance.PlayerCombat;

	private PlayerMovement PlayerMovement => PlayerSpawner.PlayerInstance.PlayerMovement;

	private PlayerIdleController PlayerIdleController => PlayerSpawner.PlayerInstance.PlayerIdleController;

	private ActorPicker PlayerPicker => PlayerSpawner.PlayerInstance.PlayerPicker;

	private PlayerBrain PlayerBrain => PlayerSpawner.PlayerInstance;

	private CommandBase PlayerCommand => PlayerSpawner.PlayerInstance.CurCommand;

	public void Initialize()
	{
		audioSource = GetComponentInChildren<AudioSource>();
		familyEventAudioSource.ignoreListenerPause = true;
		pickCollectableAudioSource.ignoreListenerPause = true;
		if (PlayerSpawner.IsPlayerSpawned)
		{
			OnSpawnPlayer();
		}
		else
		{
			PlayerSpawner.spawnPlayerEvent += OnSpawnPlayer;
		}
	}

	private void OnSpawnPlayer()
	{
		PlayerCombat.attackEvent += OnAttack;
		PlayerCombat.hitEvent += OnHit;
		PlayerMovement.jumpEvent += OnJump;
		PlayerPicker.pickEvent += OnPick;
		PlayerPicker.dropEvent += OnDrop;
		Quest.startEvent += OnQuestStart;
		Quest.completeEvent += OnQuestComplete;
		VegetableBehaviour.plantEvent += OnVegetablePlant;
		Coin.pickEvent += OnPickCoin;
		Clew.pickEvent += OnPickClew;
		PlayerManager.levelChangeEvent += OnLevelChange;
		SuperBonus.pickEvent += OnSuperBonusPick;
		PlayerFamilyController.addFamilyMemberEvent += OnAddFamilyMember;
	}

	private void OnDestroy()
	{
		PlayerSpawner.spawnPlayerEvent -= OnSpawnPlayer;
		Quest.startEvent -= OnQuestStart;
		Quest.completeEvent -= OnQuestComplete;
		VegetableBehaviour.plantEvent -= OnVegetablePlant;
		Coin.pickEvent -= OnPickCoin;
		Clew.pickEvent -= OnPickClew;
		PlayerManager.levelChangeEvent -= OnLevelChange;
		SuperBonus.pickEvent -= OnSuperBonusPick;
		PlayerFamilyController.addFamilyMemberEvent -= OnAddFamilyMember;
		if (PlayerSpawner.IsPlayerSpawned)
		{
			PlayerCombat.attackEvent -= OnAttack;
			PlayerCombat.hitEvent -= OnHit;
			PlayerMovement.jumpEvent -= OnJump;
			PlayerPicker.pickEvent -= OnPick;
			PlayerPicker.dropEvent -= OnDrop;
		}
	}

	private void OnAttack()
	{
		if (PlayerCombat.IsInvisibilityActive())
		{
			PlayRandomSound(stealthAttackSounds.soundEffects, stealthAttackSounds.Volume);
		}
		else if (!PlayerCombat.HaveEnemyInAttackBox)
		{
			PlayRandomSound(missAttackSounds.soundEffects, missAttackSounds.Volume);
		}
		StopMovingAudioSources();
	}

	private void OnHit(ActorCombat attacker, ActorCombat target)
	{
		if (PlayerCombat.IsInvisibilityActive())
		{
			PlayRandomSound(stealthAttackSounds.soundEffects, stealthAttackSounds.Volume);
		}
		else
		{
			PlayRandomSound(hitAttackSounds.soundEffects, hitAttackSounds.Volume);
		}
		StopMovingAudioSources();
	}

	private void OnJump()
	{
		if (MainAudioSource.isPlaying)
		{
			MainAudioSource.Stop();
		}
		PlayRandomSound(jumpingSounds.soundEffects, jumpingSounds.Volume);
	}

	private void OnPickCoin(bool isSpawnedCoin)
	{
		PlayRandomSound(pickCoinSounds.soundEffects, pickCollectableAudioSource, pickCoinSounds.Volume);
	}

	private void OnPickClew()
	{
		PlayRandomSound(pickClewSounds.soundEffects, pickCollectableAudioSource, pickClewSounds.Volume);
	}

	private void OnPick(Item pickedItem)
	{
		PlaySound(pickFoodSound.soundEffect.audioClip, pickFoodSound.Volume);
	}

	private void OnDrop(Item droppedItem, bool isSatisfyFarmResident)
	{
		if (isSatisfyFarmResident)
		{
			PlaySound(satisfyFarmResidentSound.soundEffect.audioClip, satisfyFarmResidentSound.Volume);
		}
		else
		{
			PlaySound(dropFoodSound.soundEffect.audioClip, dropFoodSound.Volume);
		}
	}

	private void OnVegetablePlant(VegetableBehaviour vegetable)
	{
		PlayRandomSound(plantVegetableSounds.soundEffects, plantVegetableSounds.Volume);
	}

	private void OnQuestComplete(Quest quest)
	{
		PlaySound(questStartSound.soundEffect.audioClip, questAudioSource, questStartSound.Volume);
	}

	private void OnQuestStart(Quest quest)
	{
		PlaySound(questCompleateSound.soundEffect.audioClip, questAudioSource, questCompleateSound.Volume);
	}

	private void OnLevelChange()
	{
		PlaySound(levelUpSound.soundEffect.audioClip, levelUpAudioSource, levelUpSound.Volume);
	}

	private void OnSuperBonusPick(FarmResidentManager.SuperBonusData superBonusData)
	{
		PlaySound(pickSuperBonusSound.soundEffect.audioClip, superBonusAudioSource, pickSuperBonusSound.Volume);
	}

	private void OnAddFamilyMember(FamilyManager.FamilyMemberData familyMember)
	{
		if (familyMember.role == FamilyManager.FamilyMemberRole.Spouse)
		{
			PlaySound(weddingSound.soundEffect.audioClip, familyEventAudioSource, weddingSound.Volume);
		}
		else if (familyMember.role == FamilyManager.FamilyMemberRole.FirstStageChild)
		{
			PlaySound(childBornSound.soundEffect.audioClip, familyEventAudioSource, childBornSound.Volume);
		}
	}

	private void StopMovingAudioSources()
	{
		if (movingAudioSources.Any((AudioSource x) => x.isPlaying))
		{
			movingAudioSources.ForEach(delegate(AudioSource x)
			{
				x.Stop();
			});
		}
	}

	private void Update()
	{
		if ((PlayerMovement.CurMoveSpeed == 0f || PlayerMovement.IsFalling || PlayerCombat.IsInvisibilityActive()) && movingAudioSources.Any((AudioSource x) => x.isPlaying))
		{
			movingAudioSources.ForEach(delegate(AudioSource x)
			{
				x.Stop();
			});
		}
		bool flag = PlayerBrain.IsCommandExecuting() && PlayerBrain.CurCommand.CommandId == CommandId.Drink && PlayerCommand.GetExecutedTimePart() > 0f && PlayerCommand.GetExecutedTimePart() < 1f;
		bool flag2 = !flag && PlayerBrain.IsCommandExecuting() && PlayerBrain.CurCommand.CommandId == CommandId.Eat && PlayerCommand.GetExecutedTimePart() > 0f && PlayerCommand.GetExecutedTimePart() < 1f;
		if (!eatAudioSource.isPlaying && flag2)
		{
			PlayRandomSound(eatingSounds.soundEffects, eatAudioSource, eatingSounds.Volume);
		}
		else if (eatAudioSource.isPlaying && !flag2)
		{
			eatAudioSource.Stop();
		}
		if (!drinkAudioSource.isPlaying && flag)
		{
			PlayRandomSound(drinkingSounds.soundEffects, drinkAudioSource, drinkingSounds.Volume);
		}
		else if (drinkAudioSource.isPlaying && !flag)
		{
			drinkAudioSource.Stop();
		}
		if (PlayerIdleController.CurIdleType == PlayerIdleController.IdleAnimationId.Rest || PlayerIdleController.CurIdleType == PlayerIdleController.IdleAnimationId.Sleep)
		{
			if (!purrAudioSource.isPlaying)
			{
				if (purrSound.curDuration != 0f)
				{
					PlaySound(purrSound.soundEffect, purrAudioSource);
				}
				else if (purrSound.curDelay == 0f)
				{
					timer = 0f;
					purrSound.InitDelay();
				}
				else if (timer >= purrSound.curDelay)
				{
					timer = 0f;
					purrSound.curDelay = 0f;
					purrAudioSource.volume = 0f;
					PlaySound(purrSound.soundEffect, purrAudioSource);
					isPlayingPurrSound = true;
				}
			}
			else
			{
				if (timer <= purrSound.changeVolumeDuration)
				{
					_003CUpdate_003Eg__IncreaseVolume_007C84_7(purrAudioSource, purrSound.changeVolumeDuration, purrSound.Volume * ManagerBase<SettingsManager>.Instance.effectsVolume);
				}
				else if (purrSound.curDuration - timer <= purrSound.changeVolumeDuration)
				{
					_003CUpdate_003Eg__DecreaseVolume_007C84_8(purrAudioSource, purrSound.changeVolumeDuration, purrSound.Volume * ManagerBase<SettingsManager>.Instance.effectsVolume);
				}
				if (purrSound.curDuration == 0f)
				{
					timer = 0f;
					purrSound.InitDuration();
				}
				else if (timer >= purrSound.curDuration)
				{
					timer = 0f;
					purrSound.curDuration = 0f;
					purrAudioSource.Stop();
					purrAudioSource.volume = 1f;
					isPlayingPurrSound = false;
				}
			}
			timer += Time.deltaTime;
		}
		else if (purrAudioSource.isPlaying && isPlayingPurrSound)
		{
			timer = Mathf.Clamp(timer - Time.deltaTime, 0f, purrSound.changeVolumeDuration);
			if (timer == 0f)
			{
				purrSound.curDuration = 0f;
				purrAudioSource.Stop();
				purrAudioSource.volume = 1f;
				isPlayingPurrSound = false;
			}
			else
			{
				_003CUpdate_003Eg__DecreaseVolume_007C84_8(purrAudioSource, purrSound.changeVolumeDuration, purrSound.Volume * ManagerBase<SettingsManager>.Instance.effectsVolume);
			}
		}
		if (PlayerMovement.IsFalling && !isFalling)
		{
			isFalling = true;
		}
		else if (!PlayerMovement.IsFalling && isFalling)
		{
			if (!MainAudioSource.isPlaying)
			{
				PlayRandomSound(landingSounds.soundEffects, landingSounds.Volume);
			}
			isFalling = false;
		}
		else if (PlayerMovement.CurMoveSpeed >= speedWhichSoundPlay && !PlayerMovement.IsFalling && !PlayerCombat.IsInvisibilityActive() && PlayerMovement.IsMovingOnWater && !MainAudioSource.isPlaying)
		{
			_003C_003Ec__DisplayClass84_0 _003C_003Ec__DisplayClass84_ = default(_003C_003Ec__DisplayClass84_0);
			if (PlayerMovement.IsWalkingAnimationPlaying)
			{
				_003C_003Ec__DisplayClass84_.soundDelay = WalkSoundDelay;
			}
			else
			{
				_003C_003Ec__DisplayClass84_.soundDelay = RunSoundDelay;
			}
			if (movingAudioSources.All((AudioSource x) => !x.isPlaying))
			{
				AudioClip audioClip = GetRandomSound(walkingOnWaterSounds.soundEffects).audioClip;
				movingAudioSources[nextMoveAudioSource].clip = audioClip;
				movingAudioSources[nextMoveAudioSource].volume = walkingOnWaterSounds.Volume * ManagerBase<SettingsManager>.Instance.effectsVolume;
				movingAudioSources[nextMoveAudioSource].PlayScheduled(AudioSettings.dspTime + _003C_003Ec__DisplayClass84_.soundDelay);
				double num = (double)audioClip.samples / (double)audioClip.frequency;
				nextMoveStartTime = AudioSettings.dspTime + num + 2.0 * _003C_003Ec__DisplayClass84_.soundDelay;
				nextMoveAudioSource = 1 - nextMoveAudioSource;
				_003CUpdate_003Eg__SetNextWalkSound_007C84_10(ref _003C_003Ec__DisplayClass84_);
			}
			else if (AudioSettings.dspTime > nextMoveStartTime - (0.2 + _003C_003Ec__DisplayClass84_.soundDelay))
			{
				_003CUpdate_003Eg__SetNextWalkSound_007C84_10(ref _003C_003Ec__DisplayClass84_);
			}
		}
		else if (PlayerMovement.CurMoveSpeed >= speedWhichSoundPlay && PlayerMovement.IsWalkingAnimationPlaying && !PlayerMovement.IsFalling && !PlayerCombat.IsInvisibilityActive() && !PlayerMovement.IsMovingOnWater && !MainAudioSource.isPlaying)
		{
			if (movingAudioSources.All((AudioSource x) => !x.isPlaying))
			{
				AudioClip audioClip2 = GetRandomSound(walkingSounds.soundEffects).audioClip;
				movingAudioSources[nextMoveAudioSource].clip = audioClip2;
				movingAudioSources[nextMoveAudioSource].volume = walkingSounds.Volume * ManagerBase<SettingsManager>.Instance.effectsVolume;
				movingAudioSources[nextMoveAudioSource].PlayScheduled(AudioSettings.dspTime + WalkSoundDelay);
				double num2 = (double)audioClip2.samples / (double)audioClip2.frequency;
				nextMoveStartTime = AudioSettings.dspTime + num2 + 2.0 * WalkSoundDelay;
				nextMoveAudioSource = 1 - nextMoveAudioSource;
				_003CUpdate_003Eg__SetNextWalkSound_007C84_3();
			}
			else if (AudioSettings.dspTime > nextMoveStartTime - (0.2 + WalkSoundDelay))
			{
				_003CUpdate_003Eg__SetNextWalkSound_007C84_3();
			}
		}
		else if (PlayerMovement.CurMoveSpeed >= speedWhichSoundPlay && PlayerMovement.IsRunningAnimationPlaying && !PlayerMovement.IsFalling && !PlayerCombat.IsInvisibilityActive() && !PlayerMovement.IsMovingOnWater && !MainAudioSource.isPlaying)
		{
			if (movingAudioSources.All((AudioSource x) => !x.isPlaying))
			{
				AudioClip audioClip3 = GetRandomSound(runningSounds.soundEffects).audioClip;
				movingAudioSources[nextMoveAudioSource].clip = audioClip3;
				movingAudioSources[nextMoveAudioSource].volume = runningSounds.Volume * ManagerBase<SettingsManager>.Instance.effectsVolume;
				movingAudioSources[nextMoveAudioSource].PlayScheduled(AudioSettings.dspTime + RunSoundDelay);
				double num3 = (double)audioClip3.samples / (double)audioClip3.frequency;
				nextMoveStartTime = AudioSettings.dspTime + num3 + 2.0 * RunSoundDelay;
				nextMoveAudioSource = 1 - nextMoveAudioSource;
				_003CUpdate_003Eg__SetNextRunSound_007C84_5();
			}
			else if (AudioSettings.dspTime > nextMoveStartTime - (0.2 + RunSoundDelay))
			{
				_003CUpdate_003Eg__SetNextRunSound_007C84_5();
			}
		}
	}

	[CompilerGenerated]
	private void _003CUpdate_003Eg__SetNextWalkSound_007C84_10(ref _003C_003Ec__DisplayClass84_0 P_0)
	{
		AudioClip audioClip = GetRandomSound(walkingOnWaterSounds.soundEffects).audioClip;
		movingAudioSources[nextMoveAudioSource].clip = audioClip;
		movingAudioSources[nextMoveAudioSource].volume = walkingOnWaterSounds.Volume * ManagerBase<SettingsManager>.Instance.effectsVolume;
		movingAudioSources[nextMoveAudioSource].PlayScheduled(nextMoveStartTime);
		double num = (double)audioClip.samples / (double)audioClip.frequency;
		nextMoveStartTime = nextMoveStartTime + num + P_0.soundDelay;
		nextMoveAudioSource = 1 - nextMoveAudioSource;
	}

	[CompilerGenerated]
	private void _003CUpdate_003Eg__SetNextWalkSound_007C84_3()
	{
		AudioClip audioClip = GetRandomSound(walkingSounds.soundEffects).audioClip;
		movingAudioSources[nextMoveAudioSource].clip = audioClip;
		movingAudioSources[nextMoveAudioSource].volume = walkingSounds.Volume * ManagerBase<SettingsManager>.Instance.effectsVolume;
		movingAudioSources[nextMoveAudioSource].PlayScheduled(nextMoveStartTime);
		double num = (double)audioClip.samples / (double)audioClip.frequency;
		nextMoveStartTime = nextMoveStartTime + num + WalkSoundDelay;
		nextMoveAudioSource = 1 - nextMoveAudioSource;
	}

	[CompilerGenerated]
	private void _003CUpdate_003Eg__SetNextRunSound_007C84_5()
	{
		AudioClip audioClip = GetRandomSound(runningSounds.soundEffects).audioClip;
		movingAudioSources[nextMoveAudioSource].clip = audioClip;
		movingAudioSources[nextMoveAudioSource].volume = runningSounds.Volume * ManagerBase<SettingsManager>.Instance.effectsVolume;
		movingAudioSources[nextMoveAudioSource].PlayScheduled(nextMoveStartTime);
		double num = (double)audioClip.samples / (double)audioClip.frequency;
		nextMoveStartTime = nextMoveStartTime + num + RunSoundDelay;
		nextMoveAudioSource = 1 - nextMoveAudioSource;
	}

	[CompilerGenerated]
	private static void _003CUpdate_003Eg__UpdateVolume_007C84_6(AudioSource audioSource, float changeVolumeSpeed, float maxVolume)
	{
		audioSource.volume = Mathf.Clamp(audioSource.volume + changeVolumeSpeed * Time.deltaTime, 0f, maxVolume);
	}

	[CompilerGenerated]
	private static void _003CUpdate_003Eg__IncreaseVolume_007C84_7(AudioSource audioSource, float changeVolumeDuration, float maxVolume)
	{
		float changeVolumeSpeed = maxVolume / changeVolumeDuration;
		_003CUpdate_003Eg__UpdateVolume_007C84_6(audioSource, changeVolumeSpeed, maxVolume);
	}

	[CompilerGenerated]
	private static void _003CUpdate_003Eg__DecreaseVolume_007C84_8(AudioSource audioSource, float changeVolumeDuration, float maxVolume)
	{
		float f = maxVolume / changeVolumeDuration;
		_003CUpdate_003Eg__UpdateVolume_007C84_6(audioSource, 0f - Mathf.Abs(f), maxVolume);
	}
}
