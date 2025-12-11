using Avelog;
using UnityEngine;

public class EnemyFieldOfView : MonoBehaviour
{
	[SerializeField]
	private int arcPointCount = 10;

	[SerializeField]
	private float startRadius = 5f;

	[SerializeField]
	private float sectorThicknessAngle = 15f;

	[SerializeField]
	private MeshFilter meshFilter;

	[SerializeField]
	private EnemyModel enemyModel;

	private float prevViewAngle;

	private float prevViewDistance;

	private float ViewAngle
	{
		get
		{
			if (!(enemyModel != null) || !(enemyModel.EnemyController != null))
			{
				return 0f;
			}
			return enemyModel.EnemyController.ViewAngle;
		}
	}

	private float ViewDistance
	{
		get
		{
			if (!(enemyModel != null) || !(enemyModel.EnemyController != null))
			{
				return 0f;
			}
			return enemyModel.EnemyController.ViewDistance;
		}
	}

	private void Awake()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Switch(bool value)
	{
		if (!base.gameObject.activeSelf && value)
		{
			UpdateFOW();
		}
		base.gameObject.SetActive(value);
	}

	private void Update()
	{
		if (prevViewAngle != ViewAngle || prevViewDistance != ViewDistance)
		{
			prevViewAngle = ViewAngle;
			prevViewDistance = ViewDistance;
			UpdateFOW();
		}
	}

	private void UpdateFOW()
	{
		meshFilter.sharedMesh = MeshUtils.CreateSectorVolumeByAngle(ViewAngle, startRadius, ViewDistance, arcPointCount, sectorThicknessAngle);
	}
}
