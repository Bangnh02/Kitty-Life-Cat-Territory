using System.Collections;
using UnityEngine;

public class EnemyFood : Food
{
	[SerializeField]
	[Header("EnemyFood: Графика")]
	private MeshFilter meshFilter;

	[SerializeField]
	private MeshRenderer meshRenderer;

	[SerializeField]
	private MeshFilter shadowsMeshFilter;

	[SerializeField]
	private MeshRenderer shadowsMeshRenderer;

	private float timeWithoutProcessing;

	private bool edible;

	public MeshFilter MeshFilter => meshFilter;

	public new MeshRenderer MeshRenderer => meshRenderer;

	public MeshFilter ShadowsMeshFilter => shadowsMeshFilter;

	public MeshRenderer ShadowsMeshRenderer => shadowsMeshRenderer;

	public EnemyArchetype EnemyArchetype
	{
		get;
		private set;
	}

	public override bool Eatable => edible;

	public override bool Pickable => edible;

	public override string Name => EnemyArchetype.name;

	public void Setup(EnemyArchetype enemyArchetype, bool edible, float scaleMod)
	{
		EnemyArchetype = enemyArchetype;
		foodEffect = EnemyArchetype.foodEffect;
		this.edible = edible;
		pickOffset = EnemyArchetype.pickOffset;
		dropOffset = EnemyArchetype.dropOffset;
		pickScale = EnemyArchetype.pickScale / scaleMod;
		usePickRotOverride = EnemyArchetype.usePickRotOverride;
		pickRotOverride = EnemyArchetype.pickRotOverride;
		useDropRotOverride = EnemyArchetype.useDropRotOverride;
		dropRotOverride = EnemyArchetype.dropRotOverride;
	}

	private void OnEnable()
	{
		base.CurCountFoodUnits = 1;
		base.ProcessingSwitch?.UpdateProcessingState(forceUpdate: true);
		timeWithoutProcessing = 0f;
	}

	protected override IEnumerator ResetThrownState()
	{
		yield break;
	}

	private void Update()
	{
		if (!base.ProcessingSwitch.OnProcessingDistance)
		{
			timeWithoutProcessing += Time.deltaTime;
			if (timeWithoutProcessing >= thrownLifetime && base.CountOccupiedFoodUnits == 0)
			{
				Unspawn();
			}
		}
		else if (timeWithoutProcessing >= 0f)
		{
			timeWithoutProcessing = 0f;
		}
	}

	public override bool CanBePicked(ItemUser itemUser)
	{
		if (base.CanBePicked(itemUser))
		{
			return Eatable;
		}
		return false;
	}
}
