using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelUpEffectController : MonoBehaviour, IInitializablePlayerComponent
{
	private List<ParticleSystem> levelUpParticleSystems;

	public void Initialize()
	{
		PlayerManager.levelChangeEvent += OnLevelChange;
		levelUpParticleSystems = new List<ParticleSystem>(GetComponentsInChildren<ParticleSystem>());
	}

	private void OnDestroy()
	{
		PlayerManager.levelChangeEvent -= OnLevelChange;
	}

	private void OnLevelChange()
	{
		base.gameObject.SetActive(value: true);
	}

	private void Update()
	{
		if (base.gameObject.activeSelf && levelUpParticleSystems.TrueForAll((ParticleSystem x) => !x.isPlaying))
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
