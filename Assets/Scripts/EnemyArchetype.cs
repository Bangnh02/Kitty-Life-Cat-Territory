using UnityEngine;

public class EnemyArchetype : MonoBehaviour
{
	public new string name = "Enemy";

	public int killsForMinibossSpawn = 5;

	public float speedMinimum;

	public float speedMedium;

	public FloatPBI speedMaximum;

	public float foodEffect;

	[Postfix(PostfixAttribute.Id.Seconds)]
	public float hitFrequency = 1f;

	[Postfix(PostfixAttribute.Id.Percents)]
	[SerializeField]
	private float hitRegistrationAnimPerc = 30f;

	public GameObject modelPrefab;

	private EnemyScheme simpleScheme;

	private EnemyScheme miniBossScheme;

	private EnemyScheme bossScheme;

	private EnemyScheme bossAssistantScheme;

	[Header("Параметры пика/дропа еды")]
	public Vector3 pickOffset = Vector3.zero;

	public Vector3 dropOffset = Vector3.zero;

	public Vector3 pickScale = Vector3.one;

	public bool usePickRotOverride;

	public Vector3 pickRotOverride = Vector3.zero;

	public bool useDropRotOverride;

	public Vector3 dropRotOverride = Vector3.zero;

	public float HitRegistrationAnimNormalizedTime => hitRegistrationAnimPerc / 100f;

	public EnemyScheme SimpleScheme
	{
		get
		{
			if (simpleScheme == null)
			{
				InitSchemes();
			}
			return simpleScheme;
		}
	}

	public EnemyScheme MiniBossScheme
	{
		get
		{
			if (miniBossScheme == null)
			{
				InitSchemes();
			}
			return miniBossScheme;
		}
	}

	public EnemyScheme BossScheme
	{
		get
		{
			if (bossScheme == null)
			{
				InitSchemes();
			}
			return bossScheme;
		}
	}

	public EnemyScheme BossAssistantScheme
	{
		get
		{
			if (bossAssistantScheme == null)
			{
				InitSchemes();
			}
			return bossAssistantScheme;
		}
	}

	private void InitSchemes()
	{
		if (simpleScheme != null)
		{
			return;
		}
		EnemyScheme[] componentsInChildren = GetComponentsInChildren<EnemyScheme>();
		foreach (EnemyScheme enemyScheme in componentsInChildren)
		{
			if (enemyScheme.Type == EnemyScheme.SchemeType.Simple)
			{
				simpleScheme = enemyScheme;
			}
			else if (enemyScheme.Type == EnemyScheme.SchemeType.MiniBoss)
			{
				miniBossScheme = enemyScheme;
			}
			else if (enemyScheme.Type == EnemyScheme.SchemeType.Boss)
			{
				bossScheme = enemyScheme;
			}
			else if (enemyScheme.Type == EnemyScheme.SchemeType.BossAssistant)
			{
				bossAssistantScheme = enemyScheme;
			}
		}
	}

	private void Awake()
	{
		InitSchemes();
	}
}
