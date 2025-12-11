using UnityEngine;

public class InvisibilityIcon : MonoBehaviour
{
	private GameObject iconGO;

	private PlayerCombat PlayerCombat => PlayerSpawner.PlayerInstance.PlayerCombat;

	private void Start()
	{
		iconGO = base.transform.GetChild(0).gameObject;
	}

	private void Update()
	{
		if (!(PlayerCombat == null) && iconGO.activeSelf != PlayerCombat.IsInvisibilityActive())
		{
			iconGO.SetActive(PlayerCombat.IsInvisibilityActive());
		}
	}
}
