using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapWindow : WindowSingleton<MapWindow>
{
	[Serializable]
	private class FarmResidentUI
	{
		public FarmResidentId farmResidentId;

		public Image mapIcon;

		public Image arrowIcon;

		public Image needIcon;

		public Text needText;
	}

	[Serializable]
	public class NeedSprite
	{
		public string need;

		public Sprite sprite;
	}

	[SerializeField]
	private float openZoomDuration = 0.5f;

	[SerializeField]
	private float iconSize = 65f;

	[Header("Ссылки")]
	[SerializeField]
	private Image mapImage;

	[SerializeField]
	private ScrollRect mapScrollRect;

	[SerializeField]
	private Image playerIcon;

	[SerializeField]
	private List<Image> plantIcons;

	[SerializeField]
	private GameObject bossIconPrefab;

	[SerializeField]
	private GameObject miniBossIconPrefab;

	[SerializeField]
	private GameObject potentialSpouseIconPrefab;

	[SerializeField]
	private GameObject waterIconPrefab;

	[SerializeField]
	private GameObject spouseDescriptionPanel;

	[SerializeField]
	private List<FarmResidentUI> farmResidentUIs;

	private List<(Image icon, Vector3 worldPosition)> spawnedIcons = new List<(Image, Vector3)>();

	private Vector2 mapSize;

	private Coroutine smoothZoomMapCor;

	protected override void OnInitialize()
	{
		mapSize = mapImage.rectTransform.sizeDelta;
	}

	private void OnEnable()
	{
		if (Singleton<EnemySpawner>.Instance != null && Singleton<EnemySpawner>.Instance.SpawnedBoss != null)
		{
			SpawnIcon(Singleton<EnemySpawner>.Instance.SpawnedBoss.model.transform.position, bossIconPrefab);
		}
		foreach (EnemySpawner.EnemyInstance item in from x in Singleton<EnemySpawner>.Instance.SpawnedEnemies
			where x.logic.currentScheme.Scheme.Type == EnemyScheme.SchemeType.MiniBoss
			select x)
		{
			SpawnIcon(item.model.transform.position, miniBossIconPrefab);
		}
		Water[] array = UnityEngine.Object.FindObjectsOfType<Water>();
		foreach (Water water in array)
		{
			if (water.HaveDrinkPoints)
			{
				SpawnIcon(water.transform.position, waterIconPrefab);
			}
		}
		if (Singleton<SpouseSpawner>.Instance.PotentialSpouse != null)
		{
			SpawnIcon(Singleton<SpouseSpawner>.Instance.PotentialSpouse.transform.position, potentialSpouseIconPrefab);
		}
		spouseDescriptionPanel.SetActive(Singleton<SpouseSpawner>.Instance.PotentialSpouse != null);
		if (Singleton<FarmResidentSpawner>.Instance != null)
		{
			foreach (FarmResident farmResident in Singleton<FarmResidentSpawner>.Instance.SpawnedFarmResidents)
			{
				FarmResidentUI farmResidentUI2 = farmResidentUIs.Find((FarmResidentUI x) => x.farmResidentId == farmResident.FarmResidentData.farmResidentId);
				SetIconPos(farmResident.transform.position, farmResidentUI2.mapIcon);
				farmResidentUI2.mapIcon.transform.SetAsLastSibling();
			}
		}
		SetupPlayerIcon();
		SetupPlantIcons();
		CenterMapOnPlayer();
		if (Singleton<FarmResidentSpawner>.Instance != null)
		{
			foreach (FarmResidentUI farmResidentUI in farmResidentUIs)
			{
				FarmResidentManager.FarmResidentData farmResidentData = ManagerBase<FarmResidentManager>.Instance.FarmResidentsData.Find((FarmResidentManager.FarmResidentData x) => x.farmResidentId == farmResidentUI.farmResidentId);
				farmResidentUI.mapIcon.sprite = ManagerBase<UIManager>.Instance.GetFarmResidentSprite(farmResidentUI.farmResidentId);
				bool flag = !string.IsNullOrEmpty(farmResidentData.curNeed);
				farmResidentUI.needIcon.gameObject.SetActive(flag);
				farmResidentUI.needText.gameObject.SetActive(flag);
				farmResidentUI.arrowIcon.gameObject.SetActive(flag);
				if (flag)
				{
					farmResidentUI.needIcon.sprite = ManagerBase<UIManager>.Instance.GetFarmResidentNeedSprite(farmResidentData.curNeed);
					farmResidentUI.needText.text = $"{farmResidentData.needProgressCurrent}/{farmResidentData.needProgressMaximum}";
				}
			}
		}
		if (smoothZoomMapCor != null)
		{
			StopCoroutine(smoothZoomMapCor);
			smoothZoomMapCor = null;
		}
		smoothZoomMapCor = StartCoroutine(SmoothZoomMap(mapScrollRect.viewport.rect.size, mapSize));
	}

	private void SetupPlayerIcon()
	{
		Vector2 sizeDelta = new Vector2(iconSize, iconSize);
		playerIcon.rectTransform.anchoredPosition = WorldToMap(PlayerSpawner.PlayerInstance.transform.position);
		playerIcon.rectTransform.sizeDelta = sizeDelta;
		playerIcon.rectTransform.eulerAngles = new Vector3(0f, 0f, 0f - PlayerSpawner.PlayerInstance.transform.eulerAngles.y);
		playerIcon.transform.SetAsLastSibling();
	}

	private void SetupPlantIcons()
	{
		Vector2 size = new Vector2(iconSize, iconSize);
		plantIcons.ForEach(delegate(Image plantIcon)
		{
			plantIcon.rectTransform.sizeDelta = size;
		});
	}

	private void SpawnIcon(Vector3 worldPosition, GameObject iconPrefab)
	{
		Image component = UnityEngine.Object.Instantiate(iconPrefab, mapImage.transform).GetComponent<Image>();
		spawnedIcons.Add((component, worldPosition));
		SetIconPos(worldPosition, component);
	}

	private void SetIconPos(Vector3 worldPosition, Image icon)
	{
		Vector2 anchoredPosition = WorldToMap(worldPosition);
		Vector2 sizeDelta = new Vector2(iconSize, iconSize);
		icon.rectTransform.sizeDelta = sizeDelta;
		icon.rectTransform.anchoredPosition = anchoredPosition;
	}

	private void OnDisable()
	{
		foreach (var spawnedIcon in spawnedIcons)
		{
			UnityEngine.Object.Destroy(spawnedIcon.icon.gameObject);
		}
		spawnedIcons.Clear();
		if (smoothZoomMapCor != null)
		{
			StopCoroutine(smoothZoomMapCor);
			smoothZoomMapCor = null;
		}
	}

	private IEnumerator SmoothZoomMap(Vector2 startMapSize, Vector2 endMapSize)
	{
		float zoomTime = 0f;
		mapImage.raycastTarget = false;
		List<Vector2> plantIconsRatio = new List<Vector2>();
		foreach (Image plantIcon in plantIcons)
		{
			plantIconsRatio.Add(new Vector2(plantIcon.rectTransform.anchoredPosition.x / mapImage.rectTransform.sizeDelta.x, plantIcon.rectTransform.anchoredPosition.y / mapImage.rectTransform.sizeDelta.y));
		}
		while (zoomTime < openZoomDuration)
		{
			zoomTime = Mathf.Clamp(zoomTime + Time.unscaledDeltaTime, 0f, openZoomDuration);
			mapImage.rectTransform.sizeDelta = Vector2.Lerp(startMapSize, endMapSize, zoomTime / openZoomDuration);
			spawnedIcons.ForEach(delegate((Image icon, Vector3 worldPosition) x)
			{
				SetIconPos(x.worldPosition, x.icon);
			});
			if (Singleton<FarmResidentSpawner>.Instance != null)
			{
				foreach (FarmResident farmResident in Singleton<FarmResidentSpawner>.Instance.SpawnedFarmResidents)
				{
					FarmResidentUI farmResidentUI = farmResidentUIs.Find((FarmResidentUI x) => x.farmResidentId == farmResident.FarmResidentData.farmResidentId);
					SetIconPos(farmResident.transform.position, farmResidentUI.mapIcon);
				}
			}
			for (int i = 0; i < plantIcons.Count; i++)
			{
				plantIcons[i].rectTransform.anchoredPosition = new Vector2(plantIconsRatio[i].x * mapImage.rectTransform.sizeDelta.x, plantIconsRatio[i].y * mapImage.rectTransform.sizeDelta.y);
			}
			SetupPlayerIcon();
			CenterMapOnPlayer();
			yield return null;
		}
		mapImage.raycastTarget = true;
	}

	private Vector2 WorldToMap(Vector3 worldPos)
	{
		float num = Terrain.activeTerrain.terrainData.size.x / mapImage.rectTransform.sizeDelta.x;
		float num2 = Terrain.activeTerrain.terrainData.size.z / mapImage.rectTransform.sizeDelta.y;
		return new Vector2(worldPos.x / num, worldPos.z / num2);
	}

	private void CenterMapOnPlayer()
	{
		Vector2 vector = WorldToMap(PlayerSpawner.PlayerInstance.transform.position);
		float num = vector.x / mapImage.rectTransform.sizeDelta.x;
		float num2 = vector.y / mapImage.rectTransform.sizeDelta.y;
		float num3 = mapScrollRect.viewport.rect.width / 2f;
		float num4 = mapImage.rectTransform.rect.width - num3;
		num = Mathf.Clamp((vector.x - num3) / (num4 - num3), 0f, 1f);
		float num5 = mapScrollRect.viewport.rect.height / 2f;
		float num6 = mapImage.rectTransform.rect.height - num5;
		num2 = Mathf.Clamp((vector.y - num5) / (num6 - num5), 0f, 1f);
		mapScrollRect.horizontalNormalizedPosition = num;
		mapScrollRect.verticalNormalizedPosition = num2;
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.M) || UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		{
			Back();
		}
	}

	public void Back()
	{
		WindowSingleton<GameWindow>.Instance.Open();
	}
}
