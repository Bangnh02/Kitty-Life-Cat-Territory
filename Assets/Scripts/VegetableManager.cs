using System;
using System.Collections.Generic;
using UnityEngine;

public class VegetableManager : ManagerBase<VegetableManager>
{
	[Serializable]
	public class VegetableParams
	{
		public VegetableType type;

		public float growthTime = 10f;

		public int cost = 10;

		public float growingModelMinScale = 0.3f;
	}

	[Serializable]
	public class VegetableData
	{
		[Save]
		public VegetableParams vegetableParams;

		[Save]
		public VegetableType type;

		[Save]
		public int vegetableId;

		[Save]
		public int gardenId;

		[Save]
		public bool isPlanted;

		[Save]
		public float curGrowthTime;

		public bool IsBonusActive => isPlanted;

		public bool IsGrowedUp => curGrowthTime >= vegetableParams.growthTime;

		public float GrowthProgressPart => Mathf.Clamp(curGrowthTime / vegetableParams.growthTime, 0f, 1f);
	}

	[Serializable]
	public class VegetableMaterials
	{
		public Material topMaterial;

		public Material vegetableMaterial;
	}

	public enum VegetableType
	{
		Beet,
		Carrot,
		Pumpkin,
		Turnip
	}

	[SerializeField]
	private int vegetablesCount = 80;

	public List<VegetableParams> vegetablesParams;

	[Postfix(PostfixAttribute.Id.Percents)]
	[SerializeField]
	private float carrotMoveSpeedBonus = 3f;

	[Postfix(PostfixAttribute.Id.Percents)]
	[SerializeField]
	private float pumpkinHealthBonus = 3f;

	[Postfix(PostfixAttribute.Id.Percents)]
	[SerializeField]
	private float turnipThirstBonus = 3f;

	[Postfix(PostfixAttribute.Id.Percents)]
	[SerializeField]
	private float beetHitPowerBonus = 3f;

	[SerializeField]
	private VegetableMaterials previewMaterial;

	[SerializeField]
	private VegetableMaterials plantedMaterial;

	[SerializeField]
	private float plantSignalRotationSpeed = 100f;

	[SerializeField]
	private float processingDistance = 150f;

	[Header("Отладка")]
	[Save]
	public List<VegetableData> vegetablesData = new List<VegetableData>();

	public int VegetablesCount => vegetablesCount;

	public float CarrotMoveSpeedBonus => carrotMoveSpeedBonus;

	public float PumpkinHealthBonus => pumpkinHealthBonus;

	public float TurnipThirstBonus => turnipThirstBonus;

	public float BeetHitPowerBonus => beetHitPowerBonus;

	public VegetableMaterials PreviewMaterial => previewMaterial;

	public VegetableMaterials PlantedMaterial => plantedMaterial;

	public float ProcessingDistance => processingDistance;

	public Quaternion PlantSignalRotation
	{
		get;
		private set;
	} = new Quaternion(0f, 0f, 0f, 1f);


	protected override void OnInit()
	{
	}

	private void Update()
	{
		if (Time.deltaTime != 0f)
		{
			PlantSignalRotation *= Quaternion.Euler(0f, plantSignalRotationSpeed * Time.deltaTime, 0f);
		}
	}
}
