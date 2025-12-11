using UnityEngine;

public class HelpFarmResidentQuest : Quest
{
	[SerializeField]
	private FarmResidentId farmResidentId;

	private FarmResident _farmResident;

	public FarmResidentId FarmResidentId => farmResidentId;

	private FarmResident FarmResident
	{
		get
		{
			if (_farmResident == null)
			{
				_farmResident = Singleton<FarmResidentSpawner>.Instance.SpawnedFarmResidents.Find((FarmResident x) => x.FarmResidentData.farmResidentId == farmResidentId);
			}
			return _farmResident;
		}
	}

	public override bool CanStart()
	{
		if (Singleton<FarmResidentSpawner>.Instance != null && FarmResident != null)
		{
			return FarmResident.HaveValidNeed();
		}
		return false;
	}

	protected override float CalculateMaxProgress()
	{
		return FarmResident.FarmResidentData.needProgressMaximum;
	}

	protected override void OnStart()
	{
		FarmResident.updateNeedProgressEvent += OnUpdateNeedProgress;
		base.CurProgress = FarmResident.FarmResidentData.needProgressCurrent;
	}

	protected override void OnEnd()
	{
		FarmResident.updateNeedProgressEvent -= OnUpdateNeedProgress;
	}

	private void OnUpdateNeedProgress(FarmResident farmResident)
	{
		if (farmResident == FarmResident)
		{
			base.CurProgress = farmResident.FarmResidentData.needProgressCurrent;
		}
	}
}
