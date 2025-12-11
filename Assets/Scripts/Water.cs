using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
	public class DrinkPoint
	{
		public Transform point;

		public FamilyMemberController Owner
		{
			get;
			private set;
		}

		public bool Hold(FamilyMemberController newOwner)
		{
			if (Owner != null)
			{
				return false;
			}
			Owner = newOwner;
			return true;
		}

		public void Unhold()
		{
			Owner = null;
		}
	}

	public delegate void SpawnHandler(Water water);

	private Collider collider;

	private List<DrinkPoint> drinkPoints;

	public static SpawnHandler spawnEvent;

	public Collider Collider
	{
		get
		{
			if (collider == null)
			{
				collider = GetComponent<Collider>();
			}
			return collider;
		}
	}

	public List<DrinkPoint> DrinkPoints
	{
		get
		{
			if (drinkPoints == null)
			{
				drinkPoints = new List<DrinkPoint>();
				for (int i = 0; i < base.transform.childCount; i++)
				{
					drinkPoints.Add(new DrinkPoint
					{
						point = base.transform.GetChild(i)
					});
				}
			}
			return drinkPoints;
		}
	}

	public bool HaveDrinkPoints
	{
		get
		{
			if (DrinkPoints != null)
			{
				return DrinkPoints.Count > 0;
			}
			return false;
		}
	}

	private void Start()
	{
		spawnEvent?.Invoke(this);
	}
}
