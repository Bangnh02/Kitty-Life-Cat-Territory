using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Avelog
{
	public class MeshUtils
	{
		[StructLayout(LayoutKind.Auto)]
		[CompilerGenerated]
		private struct _003C_003Ec__DisplayClass2_0
		{
			public int[] triangles;

			public int vertexIndex;

			public Vector3[] vertices;
		}

		public static Mesh CreateSector(float angle, float radius, int arcPointsCount)
		{
			if (arcPointsCount < 2)
			{
				arcPointsCount = 2;
			}
			Mesh mesh = new Mesh();
			int num = arcPointsCount + 1;
			Vector3[] array = new Vector3[num];
			Vector2[] uv = new Vector2[num];
			int[] array2 = new int[num * 3];
			float num2 = (0f - angle) / 2f;
			float num3 = angle / (float)(arcPointsCount - 1);
			array[0] = Vector3.zero;
			int num4 = 0;
			for (int i = 0; i < arcPointsCount; i++)
			{
				Vector3 vector = VectorUtils.GetVectorFromAngle(num2) * radius;
				int num5 = i + 1;
				array[num5] = VectorUtils.GetVectorFromAngle(num2) * radius;
				if (i > 0)
				{
					array2[num4] = 0;
					array2[num4 + 1] = num5 - 1;
					array2[num4 + 2] = num5;
					num4 += 3;
				}
				num2 += num3;
			}
			mesh.vertices = array;
			mesh.uv = uv;
			mesh.triangles = array2;
			return mesh;
		}

		public static Mesh CreateSectorVolumeByAngle(float angle, float startRadius, float endRadius, int arcPointsCount, float thicknessAngle = 0f)
		{
			float largerArcThickness = endRadius * Mathf.Tan(thicknessAngle / 2f * ((float)Math.PI / 180f)) * 2f;
			return CreateSectorVolume(angle, startRadius, endRadius, arcPointsCount, largerArcThickness);
		}

		public static Mesh CreateSectorVolume(float angle, float startRadius, float endRadius, int arcPointsCount, float largerArcThickness = 0f)
		{
			if (largerArcThickness == 0f)
			{
				return CreateSector(angle, endRadius, arcPointsCount);
			}
			float d = startRadius / endRadius * largerArcThickness;
			if (arcPointsCount < 2)
			{
				arcPointsCount = 2;
			}
			Mesh mesh = new Mesh();
			int num = arcPointsCount * 4;
			_003C_003Ec__DisplayClass2_0 _003C_003Ec__DisplayClass2_ = default(_003C_003Ec__DisplayClass2_0);
			_003C_003Ec__DisplayClass2_.vertices = new Vector3[num];
			_003C_003Ec__DisplayClass2_.vertexIndex = -1;
			Vector2[] uv = new Vector2[num];
			int num2 = ((arcPointsCount - 1) * 6 + 4) * 3;
			_003C_003Ec__DisplayClass2_.triangles = new int[num2];
			float num3 = (0f - angle) / 2f;
			float num4 = angle / (float)(arcPointsCount - 1);
			_003C_003Ec__DisplayClass2_.vertices[0] = Vector3.zero;
			_003C_003Ec__DisplayClass2_.vertices[1] = Vector3.zero;
			int curTriangleIndex = 0;
			for (int i = 0; i < arcPointsCount; i++)
			{
				Vector3 vectorFromAngle = VectorUtils.GetVectorFromAngle(num3);
				Vector3 a = vectorFromAngle * startRadius;
				_003CCreateSectorVolume_003Eg__AddVertex_007C2_1(a - Vector3.up * d, ref _003C_003Ec__DisplayClass2_);
				_003CCreateSectorVolume_003Eg__AddVertex_007C2_1(a + Vector3.up * d, ref _003C_003Ec__DisplayClass2_);
				Vector3 a2 = vectorFromAngle * endRadius;
				_003CCreateSectorVolume_003Eg__AddVertex_007C2_1(a2 - Vector3.up * largerArcThickness, ref _003C_003Ec__DisplayClass2_);
				_003CCreateSectorVolume_003Eg__AddVertex_007C2_1(a2 + Vector3.up * largerArcThickness, ref _003C_003Ec__DisplayClass2_);
				if (i == 0)
				{
					_003CCreateSectorVolume_003Eg__CreateTriangle_007C2_0(ref curTriangleIndex, _003C_003Ec__DisplayClass2_.vertexIndex - 2, _003C_003Ec__DisplayClass2_.vertexIndex - 3, _003C_003Ec__DisplayClass2_.vertexIndex - 1, ref _003C_003Ec__DisplayClass2_);
					_003CCreateSectorVolume_003Eg__CreateTriangle_007C2_0(ref curTriangleIndex, _003C_003Ec__DisplayClass2_.vertexIndex - 2, _003C_003Ec__DisplayClass2_.vertexIndex - 1, _003C_003Ec__DisplayClass2_.vertexIndex, ref _003C_003Ec__DisplayClass2_);
				}
				else
				{
					_003CCreateSectorVolume_003Eg__CreateTriangle_007C2_0(ref curTriangleIndex, _003C_003Ec__DisplayClass2_.vertexIndex - 6, _003C_003Ec__DisplayClass2_.vertexIndex - 4, _003C_003Ec__DisplayClass2_.vertexIndex, ref _003C_003Ec__DisplayClass2_);
					_003CCreateSectorVolume_003Eg__CreateTriangle_007C2_0(ref curTriangleIndex, _003C_003Ec__DisplayClass2_.vertexIndex - 6, _003C_003Ec__DisplayClass2_.vertexIndex, _003C_003Ec__DisplayClass2_.vertexIndex - 2, ref _003C_003Ec__DisplayClass2_);
					_003CCreateSectorVolume_003Eg__CreateTriangle_007C2_0(ref curTriangleIndex, _003C_003Ec__DisplayClass2_.vertexIndex - 2, _003C_003Ec__DisplayClass2_.vertexIndex - 3, _003C_003Ec__DisplayClass2_.vertexIndex - 7, ref _003C_003Ec__DisplayClass2_);
					_003CCreateSectorVolume_003Eg__CreateTriangle_007C2_0(ref curTriangleIndex, _003C_003Ec__DisplayClass2_.vertexIndex - 2, _003C_003Ec__DisplayClass2_.vertexIndex - 7, _003C_003Ec__DisplayClass2_.vertexIndex - 6, ref _003C_003Ec__DisplayClass2_);
					_003CCreateSectorVolume_003Eg__CreateTriangle_007C2_0(ref curTriangleIndex, _003C_003Ec__DisplayClass2_.vertexIndex - 4, _003C_003Ec__DisplayClass2_.vertexIndex - 5, _003C_003Ec__DisplayClass2_.vertexIndex - 1, ref _003C_003Ec__DisplayClass2_);
					_003CCreateSectorVolume_003Eg__CreateTriangle_007C2_0(ref curTriangleIndex, _003C_003Ec__DisplayClass2_.vertexIndex - 4, _003C_003Ec__DisplayClass2_.vertexIndex - 1, _003C_003Ec__DisplayClass2_.vertexIndex, ref _003C_003Ec__DisplayClass2_);
				}
				num3 += num4;
			}
			_003CCreateSectorVolume_003Eg__CreateTriangle_007C2_0(ref curTriangleIndex, _003C_003Ec__DisplayClass2_.vertexIndex, _003C_003Ec__DisplayClass2_.vertexIndex - 1, _003C_003Ec__DisplayClass2_.vertexIndex - 3, ref _003C_003Ec__DisplayClass2_);
			_003CCreateSectorVolume_003Eg__CreateTriangle_007C2_0(ref curTriangleIndex, _003C_003Ec__DisplayClass2_.vertexIndex, _003C_003Ec__DisplayClass2_.vertexIndex - 3, _003C_003Ec__DisplayClass2_.vertexIndex - 2, ref _003C_003Ec__DisplayClass2_);
			mesh.vertices = _003C_003Ec__DisplayClass2_.vertices;
			mesh.uv = uv;
			mesh.triangles = _003C_003Ec__DisplayClass2_.triangles;
			return mesh;
		}

		[CompilerGenerated]
		private static void _003CCreateSectorVolume_003Eg__CreateTriangle_007C2_0(ref int curTriangleIndex, int vert1, int vert2, int vert3, ref _003C_003Ec__DisplayClass2_0 P_4)
		{
			P_4.triangles[curTriangleIndex] = vert1;
			P_4.triangles[curTriangleIndex + 1] = vert2;
			P_4.triangles[curTriangleIndex + 2] = vert3;
			curTriangleIndex += 3;
		}

		[CompilerGenerated]
		private static void _003CCreateSectorVolume_003Eg__AddVertex_007C2_1(Vector3 vertex, ref _003C_003Ec__DisplayClass2_0 P_1)
		{
			int num = ++P_1.vertexIndex;
			P_1.vertices[P_1.vertexIndex] = vertex;
		}
	}
}
