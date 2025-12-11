using UnityEngine;

public class DigZone : Zone
{
	public bool HaveFarmerDigPoints()
	{
		return base.transform.childCount > 0;
	}

	public Transform GetRandomFarmerDigPoint()
	{
		int index = Random.Range(0, base.transform.childCount);
		return base.transform.GetChild(index);
	}
}
