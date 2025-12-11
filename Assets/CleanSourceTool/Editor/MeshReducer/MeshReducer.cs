using System.Collections.Generic;
using UnityEngine;

public static class MeshReducer
{
    public static Mesh ReduceMesh(Mesh originalMesh, float reductionFactor = 0.5f)
    {
        // Dữ liệu của lưới gốc
        Vector3[] originalVertices = originalMesh.vertices;
        Vector2[] originalUVs = originalMesh.uv;
        Vector3[] originalNormals = originalMesh.normals;
        
        // Số lượng đỉnh mục tiêu sau khi giảm
        int targetVertexCount = Mathf.FloorToInt(originalVertices.Length * reductionFactor);
        
        // Cấu trúc dữ liệu mới
        List<Vector3> newVertices = new List<Vector3>();
        List<Vector2> newUVs = new List<Vector2>();
        List<Vector3> newNormals = new List<Vector3>();
        Dictionary<int, int> vertexMap = new Dictionary<int, int>();

        // Giảm số lượng đỉnh: chỉ lấy 'targetVertexCount' đỉnh đầu tiên
        for (int i = 0; i < targetVertexCount && i < originalVertices.Length; i++)
        {
            newVertices.Add(originalVertices[i]);
            if (originalUVs.Length > i) newUVs.Add(originalUVs[i]);
            if (originalNormals.Length > i) newNormals.Add(originalNormals[i]);
            vertexMap[i] = i;
        }

        // Danh sách để lưu tam giác của các submesh đã giảm
        List<int[]> newSubmeshTriangles = new List<int[]>();

        // Xử lý từng submesh
        for (int submeshIndex = 0; submeshIndex < originalMesh.subMeshCount; submeshIndex++)
        {
            int[] originalSubmeshTriangles = originalMesh.GetTriangles(submeshIndex);
            List<int> newSubmeshTrianglesList = new List<int>();

            // Xử lý các tam giác trong submesh
            for (int i = 0; i < originalSubmeshTriangles.Length; i += 3)
            {
                int v1 = originalSubmeshTriangles[i];
                int v2 = originalSubmeshTriangles[i + 1];
                int v3 = originalSubmeshTriangles[i + 2];

                // Nếu đỉnh có trong danh sách đỉnh đã giảm, thêm vào danh sách tam giác submesh mới
                if (vertexMap.ContainsKey(v1) && vertexMap.ContainsKey(v2) && vertexMap.ContainsKey(v3))
                {
                    newSubmeshTrianglesList.Add(vertexMap[v1]);
                    newSubmeshTrianglesList.Add(vertexMap[v2]);
                    newSubmeshTrianglesList.Add(vertexMap[v3]);
                }
            }

            // Thêm danh sách tam giác đã giảm của submesh này vào danh sách tổng
            newSubmeshTriangles.Add(newSubmeshTrianglesList.ToArray());
        }

        // Tạo lưới mới với các đỉnh và submesh đã giảm
        Mesh newMesh = new Mesh();
        newMesh.vertices = newVertices.ToArray();
        newMesh.uv = newUVs.ToArray();
        newMesh.normals = newNormals.ToArray();

        // Gán tam giác cho từng submesh trong lưới mới
        newMesh.subMeshCount = newSubmeshTriangles.Count;
        for (int i = 0; i < newSubmeshTriangles.Count; i++)
        {
            newMesh.SetTriangles(newSubmeshTriangles[i], i);
        }

        // Tính lại ranh giới và pháp tuyến cho lưới mới
        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();

        return newMesh;
    }
}
