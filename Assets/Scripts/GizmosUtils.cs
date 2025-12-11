using System.Collections.Generic;
using UnityEngine;

public class GizmosUtils
{
	public static Mesh GetTriangleMesh(ref Mesh mesh, float zoneAngle, Vector3 zoneDir)
	{
		if (mesh == null)
		{
			mesh = new Mesh();
		}
		List<Vector3> list = new List<Vector3>();
		list.Add(Vector3.zero);
		Vector3 item = Quaternion.Euler(0f, zoneAngle / 2f, 0f) * zoneDir;
		list.Add(item);
		Vector3 item2 = Quaternion.Euler(0f, (0f - zoneAngle) / 2f, 0f) * zoneDir;
		list.Add(item2);
		mesh.vertices = list.ToArray();
		List<Vector3> list2 = new List<Vector3>();
		list2.Add(Vector3.up);
		list2.Add(Vector3.up);
		list2.Add(Vector3.up);
		mesh.normals = list2.ToArray();
		List<int> list3 = new List<int>();
		list3.Add(2);
		list3.Add(1);
		list3.Add(0);
		mesh.triangles = list3.ToArray();
		List<Vector2> list4 = new List<Vector2>();
		list4.Add(new Vector2(0f, 0f));
		list4.Add(new Vector2(1f, 0f));
		list4.Add(new Vector2(0f, 1f));
		mesh.uv = list4.ToArray();
		mesh.Optimize();
		mesh.MarkDynamic();
		return mesh;
	}
}
