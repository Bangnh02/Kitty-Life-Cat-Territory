using UnityEngine;

public class EnemyCombat : ActorCombat
{
	[Header("EnemyCombat")]
	[SerializeField]
	private int numberIdleAnimInCombat;

	[SerializeField]
	private Animator animator;

	private float hitFrequency;

	private float maxHealth = 100f;

	private float curHealth = 100f;

	private float attackPower = 10f;

	private EnemyModel enemyModel;

	public int NumberIdleAnimInCombat => numberIdleAnimInCombat;

	protected override float AttackFrequency => hitFrequency;

	public override float MaxHealth => maxHealth;

	public override float CurHealth
	{
		get
		{
			return curHealth;
		}
		protected set
		{
			curHealth = value;
		}
	}

	protected override float AttackPower => attackPower * hitFrequency;

	public EnemyModel EnemyModel
	{
		get
		{
			if (enemyModel == null)
			{
				enemyModel = GetComponent<EnemyModel>();
			}
			return enemyModel;
		}
	}

	protected override Animator GetAnimator()
	{
		return animator;
	}

	public void Configure(float maxHealth, float attackPower, float hitFrequency)
	{
		float num = this.maxHealth;
		this.maxHealth = maxHealth;
		this.attackPower = attackPower;
		this.hitFrequency = hitFrequency;
		float num2 = curHealth / num * maxHealth;
		Heal(num2 - curHealth);
	}
}
