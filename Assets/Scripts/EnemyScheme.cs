using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyScheme : MonoBehaviour
{
	public enum SchemeType
	{
		Simple,
		MiniBoss,
		Boss,
		BossAssistant
	}

	public enum CowardiceMod
	{
		Yes,
		No,
		FromArchetype
	}

	[SerializeField]
	private SchemeType type;

	[SerializeField]
	[HideInInspector]
	private EnemyArchetype archetype;

	public float health;

	public float hitPower;

	[SerializeField]
	private bool coward;

	[SerializeField]
	private int coins;

	public int coinsByBonus;

	public float experience;

	[SerializeField]
	private bool edible;

	[Postfix(PostfixAttribute.Id.Percents)]
	public float modelScale;

	[SerializeField]
	private int minimumLevel;

	public List<Material> materials;

	public SchemeType Type => type;

	public virtual EnemyArchetype Archetype
	{
		get
		{
			if (archetype == null)
			{
				archetype = GetComponentInParent<EnemyArchetype>();
			}
			return archetype;
		}
	}

	public bool Coward
	{
		get
		{
			if (type == SchemeType.Simple)
			{
				return coward;
			}
			return false;
		}
	}

	public int Coins
	{
		get
		{
			if (type == SchemeType.Simple)
			{
				return 0;
			}
			return coins;
		}
	}

	public bool Edible
	{
		get
		{
			if (type == SchemeType.Simple || type == SchemeType.MiniBoss)
			{
				return edible;
			}
			return false;
		}
	}

	public int MinimumLevel
	{
		get
		{
			if (type == SchemeType.MiniBoss)
			{
				return minimumLevel;
			}
			return 1;
		}
	}
}
