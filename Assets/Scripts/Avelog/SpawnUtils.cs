using System;
using System.Collections.Generic;
using UnityEngine;

namespace Avelog
{
	public static class SpawnUtils
	{
		public static List<Vector3> GetDropForces(Vector3 dropDirection, int forcesCount, float verticalAngle, float forceSectorAngle = 180f, float prefferedAngleOffset = 30f)
		{
			List<float> list = new List<float>();
			for (int i = 0; i < forcesCount; i++)
			{
				float num = 0f;
				bool flag = false;
				bool flag2 = false;
				float num2 = 0f;
				float num3 = forceSectorAngle;
				float num4 = 0f;
				if (list.Count > 0)
				{
					list.Sort((float x, float y) => x.CompareTo(y));
					for (int j = 0; j < list.Count - 1; j++)
					{
						float num5 = list[j + 1] - list[j];
						if (num5 > num4)
						{
							num2 = list[j];
							num3 = list[j + 1];
							num4 = num5;
						}
					}
					if (list[0] > num4)
					{
						num2 = 0f;
						num3 = list[0];
						num4 = list[0];
						flag = true;
						flag2 = false;
					}
					if (forceSectorAngle - list[list.Count - 1] > num4)
					{
						num2 = list[list.Count - 1];
						num3 = forceSectorAngle;
						num4 = forceSectorAngle - list[list.Count - 1];
						flag = false;
						flag2 = true;
					}
				}
				else
				{
					num4 = forceSectorAngle;
					flag = true;
					flag2 = true;
				}
				if (flag && flag2)
				{
					num = UnityEngine.Random.Range(num2, num3);
				}
				else if (flag)
				{
					if (num4 > prefferedAngleOffset)
					{
						num3 -= prefferedAngleOffset;
						num = UnityEngine.Random.Range(num2, num3);
					}
					else
					{
						num = (num3 - num2) / 2f;
					}
				}
				else if (flag2)
				{
					if (num4 > prefferedAngleOffset)
					{
						num2 += prefferedAngleOffset;
						num = UnityEngine.Random.Range(num2, num3);
					}
					else
					{
						num = (num3 - num2) / 2f;
					}
				}
				else if (num4 > prefferedAngleOffset * 2f)
				{
					float num6 = (num3 + num2) / 2f;
					float num7 = num4 - prefferedAngleOffset * 2f;
					num2 = num6 + num7 / 2f;
					num3 = num6 - num7 / 2f;
					num = UnityEngine.Random.Range(num2, num3);
				}
				else
				{
					num = (num3 + num2) / 2f;
				}
				list.Add(num);
			}
			List<Vector3> list2 = new List<Vector3>();
			foreach (float item in list)
			{
				float num8 = 0f;
				num8 = ((!(item >= forceSectorAngle / 2f)) ? (0f - (forceSectorAngle / 2f - item)) : (item - forceSectorAngle / 2f));
				Vector2 source = new Vector2(dropDirection.normalized.x, dropDirection.normalized.z);
				source = source.Rotate(num8);
				Vector3 normalized = Vector3.RotateTowards(new Vector3(source.x, 0f, source.y).normalized, Vector3.up, (float)Math.PI / 180f * verticalAngle, 0f).normalized;
				list2.Add(normalized);
			}
			return list2;
		}
	}
}
