using UnityEngine;

public class ClewSpawnPoint : MonoBehaviour
{
	[SerializeField]
	private GameObject clewPrefab;

	private ClewManager.Data _clewData;

	public GameObject ClewPrefab => clewPrefab;

	public ClewManager.Data ClewData
	{
		get
		{
			if (_clewData == null)
			{
				_clewData = ManagerBase<ClewManager>.Instance.GetClewData(Id);
			}
			return _clewData;
		}
	}

	public int Id => base.transform.GetSiblingIndex();
}
