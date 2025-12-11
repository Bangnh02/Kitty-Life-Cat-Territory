using System.Collections.Generic;
using UnityEngine;

public class FoodInfosPanel : MonoBehaviour
{
	private List<FoodInfoPanel> spawnedInfoPanels = new List<FoodInfoPanel>();

	[SerializeField]
	private GameObject infoPanelPrefab;
}
