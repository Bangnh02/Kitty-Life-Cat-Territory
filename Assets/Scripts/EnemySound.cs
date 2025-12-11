using UnityEngine;

public class EnemySound : ActorSound
{
	[SerializeField]
	private SoundEffectsVolume attackSounds;

	private EnemyCombat enemyCombat;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		enemyCombat = GetComponent<EnemyCombat>();
		enemyCombat.hitEvent += OnHit;
	}

	private void OnDestroy()
	{
		enemyCombat.hitEvent -= OnHit;
	}

	private void OnHit(ActorCombat attacker, ActorCombat target)
	{
		PlayRandomSound(attackSounds.soundEffects, attackSounds.Volume);
	}
}
