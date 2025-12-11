using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Avelog.Spawn
{
	public class SpawnDistributor<SpawnPointType> where SpawnPointType : ISpawnPoint
	{
		[Serializable]
		private class SpawnPointDistance
		{
			public SpawnPointType spawnPoint;

			public float sqrDistance;
		}

		[Serializable]
		private class SpawnPointRelativeDistances
		{
			public SpawnPointType spawnPoint;

			public List<SpawnPointDistance> nearSpawnPoints;

			public float NearestBusyPointSqrDistance
			{
				get;
				private set;
			} = float.MaxValue;


			public void RecalculateDistances()
			{
				NearestBusyPointSqrDistance = float.MaxValue;
				foreach (SpawnPointDistance nearSpawnPoint in nearSpawnPoints)
				{
					if (nearSpawnPoint.spawnPoint.IsBusy && NearestBusyPointSqrDistance > nearSpawnPoint.sqrDistance)
					{
						NearestBusyPointSqrDistance = nearSpawnPoint.sqrDistance;
					}
				}
			}

			public bool Contains(SpawnPointType spawnPoint)
			{
				return nearSpawnPoints.Any((SpawnPointDistance x) => (object)x.spawnPoint == (object)spawnPoint);
			}
		}

		public delegate void EventHandler(SpawnPointType spawnPoint);

		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Comparison<SpawnPointDistance> _003C_003E9__13_0;

			public static Func<SpawnPointRelativeDistances, float> _003C_003E9__15_0;

			public static Func<(SpawnPointType spawnPoint, float priority), float> _003C_003E9__15_1;

			internal int _003CRecalcRelativeDistances_003Eb__13_0(SpawnPointDistance x, SpawnPointDistance y)
			{
				return x.sqrDistance.CompareTo(y.sqrDistance);
			}

			internal float _003CSelectSpawnPoint_003Eb__15_0(SpawnPointRelativeDistances x)
			{
				return x.spawnPoint.FreeTime;
			}

			internal float _003CSelectSpawnPoint_003Eb__15_1((SpawnPointType spawnPoint, float priority) x)
			{
				return x.priority;
			}
		}

		private float spawnDistanceToPlayerMinimum = 15f;

		private int nearSpawnPointsCount = 5;

		private float timeWeight = 1f;

		private float occupancyWeight = 1f;

		private float startTimeWeight = 1f;

		private GameObject playerGO;

		private List<SpawnPointType> spawnPoints;

		private List<SpawnPointRelativeDistances> spawnPointsRelativeDistances;

		private List<SpawnPointRelativeDistances> bestSpawnPointsDistances = new List<SpawnPointRelativeDistances>();

		public SpawnDistributor(List<SpawnPointType> spawnPoints, GameObject playerGO, ref EventHandler OccupyPointEvent, ref EventHandler FreePointEvent, int nearSpawnPointsCount = 5, float spawnDistanceToPlayerMinimum = 50f, float timeWeight = 1f, float occupancyWeight = 1f, float startTimeWeight = 1f)
		{
			this.spawnPoints = spawnPoints;
			this.playerGO = playerGO;
			this.nearSpawnPointsCount = nearSpawnPointsCount;
			this.spawnDistanceToPlayerMinimum = spawnDistanceToPlayerMinimum;
			this.timeWeight = timeWeight;
			this.startTimeWeight = startTimeWeight;
			this.occupancyWeight = occupancyWeight;
			OccupyPointEvent = (EventHandler)Delegate.Combine(OccupyPointEvent, new EventHandler(RecalcPointsOccupancy));
			FreePointEvent = (EventHandler)Delegate.Combine(FreePointEvent, new EventHandler(RecalcPointsOccupancy));
			RecalcRelativeDistances();
		}

		private void RecalcRelativeDistances()
		{
			spawnPointsRelativeDistances = new List<SpawnPointRelativeDistances>();
			foreach (SpawnPointType spawnPoint in spawnPoints)
			{
				List<SpawnPointDistance> list = new List<SpawnPointDistance>();
				spawnPointsRelativeDistances.Add(new SpawnPointRelativeDistances
				{
					spawnPoint = spawnPoint,
					nearSpawnPoints = list
				});
				foreach (SpawnPointType spawnPoint2 in spawnPoints)
				{
					if ((object)spawnPoint2 != (object)spawnPoint)
					{
						float sqrMagnitude = (spawnPoint2.GetPosition() - spawnPoint.GetPosition()).sqrMagnitude;
						list.Add(new SpawnPointDistance
						{
							spawnPoint = spawnPoint2,
							sqrDistance = sqrMagnitude
						});
					}
				}
				list.Sort((SpawnPointDistance x, SpawnPointDistance y) => x.sqrDistance.CompareTo(y.sqrDistance));
				while (list.Count > nearSpawnPointsCount)
				{
					SpawnPointDistance item = list[list.Count - 1];
					list.Remove(item);
				}
			}
		}

		private void RecalcPointsOccupancy(SpawnPointType updatedSpawnPoint)
		{
			foreach (SpawnPointRelativeDistances spawnPointsRelativeDistance in spawnPointsRelativeDistances)
			{
				if (spawnPointsRelativeDistance.Contains(updatedSpawnPoint))
				{
					spawnPointsRelativeDistance.RecalculateDistances();
				}
			}
		}

		public SpawnPointType SelectSpawnPoint(Predicate<SpawnPointType> predicate = null)
		{
			bestSpawnPointsDistances.Clear();
			List<(SpawnPointType, float)> list = new List<(SpawnPointType, float)>();
			float num = 0f;
			foreach (SpawnPointRelativeDistances spawnPointsRelativeDistance in spawnPointsRelativeDistances)
			{
				if (spawnPointsRelativeDistance.NearestBusyPointSqrDistance != float.MaxValue && spawnPointsRelativeDistance.NearestBusyPointSqrDistance > num)
				{
					num = spawnPointsRelativeDistance.NearestBusyPointSqrDistance;
				}
			}
			if (num == 0f)
			{
				num = float.MaxValue;
			}
			float num2 = spawnPointsRelativeDistances.Max((SpawnPointRelativeDistances x) => x.spawnPoint.FreeTime);
			foreach (SpawnPointRelativeDistances spawnPointsRelativeDistance2 in spawnPointsRelativeDistances)
			{
				if (!spawnPointsRelativeDistance2.spawnPoint.IsBusy && _003CSelectSpawnPoint_003Eg__IsFarFromPlayer_007C15_2(spawnPointsRelativeDistance2.spawnPoint.GetPosition()) && (predicate == null || predicate(spawnPointsRelativeDistance2.spawnPoint)))
				{
					float num3 = 0f;
					if (num != 0f)
					{
						num3 = ((spawnPointsRelativeDistance2.NearestBusyPointSqrDistance != float.MaxValue) ? (num3 + spawnPointsRelativeDistance2.NearestBusyPointSqrDistance / num * occupancyWeight) : (num3 + occupancyWeight));
					}
					list.Add(new ValueTuple<SpawnPointType, float>(item2: (num2 == 0f) ? (num3 + startTimeWeight) : (num3 + (1f - spawnPointsRelativeDistance2.spawnPoint.FreeTime / num2) * timeWeight), item1: spawnPointsRelativeDistance2.spawnPoint));
				}
			}
			if (list.Count == 0)
			{
				return default(SpawnPointType);
			}
			return ((IEnumerable<(SpawnPointType, float)>)list).Random((Func<(SpawnPointType, float), float>)(((SpawnPointType spawnPoint, float priority) x) => x.priority)).Item1;
		}

		[CompilerGenerated]
		private bool _003CSelectSpawnPoint_003Eg__IsFarFromPlayer_007C15_2(Vector3 pos)
		{
			return (pos - playerGO.transform.position).IsLonger(spawnDistanceToPlayerMinimum);
		}
	}
}
