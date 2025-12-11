using UnityEngine;

public class FarmResidentSound : ActorSound
{
	[SerializeField]
	private SoundEffectsVolume satisfyNeedSounds;

	[SerializeField]
	private SoundEffectsVolume walkingSounds;

	[SerializeField]
	private float minTimeToPlayWalkSound = 0.5f;

	[SerializeField]
	private float maxTimeToPlayWalkSound = 4f;

	[SerializeField]
	[ReadonlyInspector]
	private float timer;

	private FarmResident farmResident;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		farmResident = GetComponent<FarmResident>();
		timer = Random.Range(minTimeToPlayWalkSound, maxTimeToPlayWalkSound);
		FarmResident.updateNeedProgressEvent += OnUpdateNeedProgress;
	}

	private void OnDestroy()
	{
		FarmResident.updateNeedProgressEvent -= OnUpdateNeedProgress;
	}

	private void OnUpdateNeedProgress(FarmResident farmResident)
	{
		if (!(this.farmResident != farmResident))
		{
			PlayRandomSound(satisfyNeedSounds.soundEffects, satisfyNeedSounds.Volume);
		}
	}

	private void Update()
	{
		if (timer == 0f)
		{
			if (!audioSource.isPlaying)
			{
				PlayRandomSound(satisfyNeedSounds.soundEffects, satisfyNeedSounds.Volume);
			}
			timer = Random.Range(minTimeToPlayWalkSound, maxTimeToPlayWalkSound);
		}
		else
		{
			timer = Mathf.Clamp(timer - Time.deltaTime, 0f, maxTimeToPlayWalkSound);
		}
	}
}
