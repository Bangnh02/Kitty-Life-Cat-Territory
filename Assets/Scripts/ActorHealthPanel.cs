using Avelog.UI;
using UnityEngine;
using UnityEngine.UI;

public class ActorHealthPanel : MonoBehaviour
{
	[SerializeField]
	private Bar bar;

	[SerializeField]
	private Text healthText;

	private ActorCombat myCombat;

	public void Setup(ActorCombat enemyCombat)
	{
		myCombat = enemyCombat;
		myCombat.takeDamageEvent += OnTakeDamage;
		myCombat.healEvent += OnHeal;
		myCombat.changeLifeStateEvent += OnChangeState;
		myCombat.respawnEvent += OnRespawn;
		SetupHealthBar();
	}

	private void OnDestroy()
	{
		if (myCombat != null)
		{
			myCombat.takeDamageEvent -= OnTakeDamage;
			myCombat.healEvent -= OnHeal;
			myCombat.changeLifeStateEvent -= OnChangeState;
			myCombat.respawnEvent -= OnRespawn;
		}
	}

	private void OnTakeDamage(ActorCombat.TakeDamageType takeDamageType, float damage, ActorCombat attacker)
	{
		UpdateHealthBar();
	}

	private void OnChangeState(ActorCombat.LifeState state)
	{
	}

	private void OnRespawn()
	{
		bar.SetValueInstantly(myCombat.CurHealth / myCombat.MaxHealth);
		UpdateHealthBar();
	}

	private void OnHeal()
	{
		UpdateHealthBar();
	}

	private void UpdateHealthBar()
	{
		bar.SetValue(myCombat.CurHealth / myCombat.MaxHealth);
		healthText.text = Mathf.CeilToInt(myCombat.CurHealth).ToString();
	}

	private void SetupHealthBar()
	{
		bar.SetValueInstantly(myCombat.CurHealth / myCombat.MaxHealth);
		healthText.text = Mathf.CeilToInt(myCombat.CurHealth).ToString();
	}
}
