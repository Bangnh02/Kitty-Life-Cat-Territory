using Avelog;
using System.Collections.Generic;
using UnityEngine;

public class SuperBonusSpawner : Singleton<SuperBonusSpawner>
{
	[SerializeField]
	private List<SuperBonus.Params> superBonusesParams;

	[Header("Параметры вылета при спавне")]
	[SerializeField]
	private float dropForceVertAngle = 50f;

	[SerializeField]
	private float dropForcePower = 15f;

	[SerializeField]
	private Transform spawnedObjsParent;

	protected override void OnInit()
	{
	}

	public void Spawn(SuperBonus.Id id, Vector3 dropPosition, Vector3 dropDirection)
	{
		SuperBonus.Params @params = superBonusesParams.Find((SuperBonus.Params x) => x.id == id);
		List<Vector3> dropForces = SpawnUtils.GetDropForces(dropDirection, 1, dropForceVertAngle, dropForcePower);
		List<Vector3> list = dropForces;
		list[0] *= dropForcePower;
		Object.Instantiate(@params.prefab, dropPosition, Quaternion.identity, spawnedObjsParent).GetComponent<SuperBonus>().Spawn(id, dropForces[0]);
	}

	private void Update()
	{
		ManagerBase<FarmResidentManager>.Instance.SuperBonusesData.ForEach(delegate(FarmResidentManager.SuperBonusData x)
		{
			if (!x.IsPermanent && x.Timer > 0f)
			{
				x.Timer -= Time.deltaTime;
			}
		});
	}
}
