using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TakeDamagePanel : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private float animationDuration = 1f;

	[SerializeField]
	[Postfix(PostfixAttribute.Id.Percents)]
	private float startProcessingHealth = 30f;

	[SerializeField]
	private float minAlphaProcessing = 0.3f;

	[SerializeField]
	private float maxAlphaProcessing = 0.7f;

	private Image damageImage;

	private Coroutine animationCoroutine;

	private float curAlpha;

	private float StartProcessingHealthPart => startProcessingHealth / 100f;

	public void OnInitializeUI()
	{
		damageImage = GetComponent<Image>();
		if (PlayerSpawner.IsPlayerSpawned)
		{
			OnSpawnPlayer();
		}
		PlayerSpawner.spawnPlayerEvent += OnSpawnPlayer;
	}

	private void OnDestroy()
	{
		PlayerSpawner.spawnPlayerEvent -= OnSpawnPlayer;
		if (PlayerSpawner.IsPlayerSpawned)
		{
			PlayerSpawner.PlayerInstance.PlayerCombat.healEvent += OnPlayerHeal;
			PlayerSpawner.PlayerInstance.PlayerCombat.takeDamageEvent -= OnPlayerTakeDamage;
		}
	}

	private void OnSpawnPlayer()
	{
		PlayerSpawner.PlayerInstance.PlayerCombat.healEvent += OnPlayerHeal;
		PlayerSpawner.PlayerInstance.PlayerCombat.takeDamageEvent += OnPlayerTakeDamage;
	}

	private void OnPlayerHeal()
	{
		ChangeProcessingAlpha();
	}

	private void OnPlayerTakeDamage(ActorCombat.TakeDamageType takeDamageType, float damage, ActorCombat attacker)
	{
		ChangeProcessingAlpha();
		if (takeDamageType == ActorCombat.TakeDamageType.AttackedByEnemy)
		{
			PlayAnimation();
		}
	}

	private void ChangeProcessingAlpha()
	{
		float num = ManagerBase<PlayerManager>.Instance.healthCurrent / ManagerBase<PlayerManager>.Instance.HealthMaximum;
		if (num <= StartProcessingHealthPart)
		{
			float t = num / StartProcessingHealthPart;
			curAlpha = Mathf.Lerp(maxAlphaProcessing, minAlphaProcessing, t);
			damageImage.color = new Color(damageImage.color.r, damageImage.color.g, damageImage.color.b, curAlpha);
		}
		else if (curAlpha != 0f)
		{
			curAlpha = 0f;
			damageImage.color = new Color(damageImage.color.r, damageImage.color.g, damageImage.color.b, curAlpha);
		}
	}

	private void OnDisable()
	{
		if (animationCoroutine != null)
		{
			StopCoroutine(animationCoroutine);
		}
	}

	private void OnEnable()
	{
		if (curAlpha == 0f && damageImage != null && damageImage.color.a != 0f)
		{
			animationCoroutine = StartCoroutine(Animation(damageImage.color.a));
		}
	}

	private void PlayAnimation()
	{
		if (animationCoroutine != null)
		{
			StopCoroutine(animationCoroutine);
		}
		if (base.gameObject.activeInHierarchy)
		{
			animationCoroutine = StartCoroutine(Animation());
		}
	}

	private IEnumerator Animation(float startAlpha = 1f)
	{
		float changeAlphaTime = 0f;
		while (true)
		{
			float t = changeAlphaTime / animationDuration;
			float a = Mathf.Lerp(1f, 0f, t);
			damageImage.color = new Color(damageImage.color.r, damageImage.color.g, damageImage.color.b, a);
			if (changeAlphaTime >= animationDuration)
			{
				break;
			}
			changeAlphaTime = Mathf.Clamp(changeAlphaTime + Time.deltaTime, 0f, animationDuration);
			yield return null;
		}
	}
}
