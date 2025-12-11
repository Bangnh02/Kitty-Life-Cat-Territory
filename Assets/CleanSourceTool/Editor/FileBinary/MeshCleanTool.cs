/*
using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace EcoMine.SourceCleanTool.MeshCleanTool
{
    public class MeshCleanTool : OdinEditorWindow
    {
        private string _parentFolder = "Assets/CleanSourceBakerPrefabs";
        private string _tempFolder = "Temp";
        private string _resultFolder = "Result";
        public GameObject[] ParentMeshs;

        
        [MenuItem("EcoMine/Tools/Mesh clean tool")]
        private static void OpenWindow()
        {
            MeshCleanTool meshCleanTool = GetWindow<MeshCleanTool>();
            meshCleanTool.Show();
        }

        [Button("Delete DeActive Object")]
        private void DeleteDeActiveObject()
        {
            int deleteCount = 0;
            for (var i = 0; i < ParentMeshs.Length; i++)
            {
                GameObject gameObject = ParentMeshs[i];
                for (int j = 0; j < gameObject.transform.childCount; j++)
                {
                    if (!gameObject.transform.GetChild(j).gameObject.activeSelf)
                    {
                        deleteCount++;
                        DestroyImmediate(gameObject.transform.GetChild(j).gameObject);
                    }
                }
            }
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            EditorUtility.DisplayDialog("SYSTEM", $"Delete {deleteCount} object Deactive", "OK");
        }
        
        [Button("Delete Suffix Object Duplication")]
        private void DeleteSuffixObjectDuplication()
        {
            int deleteCount = 0;
            for (var i = 0; i < ParentMeshs.Length; i++)
            {
                GameObject gameObject = ParentMeshs[i];
                for (int j = 0; j < gameObject.transform.childCount; j++)
                {
                    GameObject child = gameObject.transform.GetChild(j).gameObject;
                    string objectName = child.name;
                    if (objectName.EndsWith(")") && objectName.Contains("("))
                    {
                        objectName = objectName.Substring(0, objectName.IndexOf('(') - 1);
                        child.name = objectName;
                        deleteCount++;
                    }
                }
            }
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            EditorUtility.DisplayDialog("SYSTEM", $"Delete Suffix {deleteCount} object", "OK");
        }

        [Button("Make To Prefab")]
        private void ReimportMeshToPrefab()
        {
            string path = Path.Combine(_parentFolder, _tempFolder);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.CreateFolder(_parentFolder, _tempFolder);
            
            MB3_MeshBaker mb3MeshBaker = FindObjectOfType<MB3_MeshBaker>();
            if (mb3MeshBaker == null)
            {
                GameObject gameObject = (GameObject)PrefabUtility.InstantiatePrefab(
                    AssetDatabase.LoadAssetAtPath<GameObject>(
                        "Assets/CleanSourceTool/Prefabs/MeshBakerToPrefab.prefab"));
                mb3MeshBaker = gameObject.GetComponent<MB3_MeshBaker>();
            }
            int ReimportCount = 0;

            List<GameObject> destroyGameObjects = new List<GameObject>();
            
            for (var i = 0; i < ParentMeshs.Length; i++)
            {
                GameObject gameObject = ParentMeshs[i];
                int childCount = gameObject.transform.childCount;
                for (int j = 0; j < childCount; j++)
                {
                    GameObject child = gameObject.transform.GetChild(j).gameObject;
                    if(!HasMesh(child)) continue;
                    if (PrefabUtility.GetPrefabInstanceStatus(child) != PrefabInstanceStatus.NotAPrefab)
                        PrefabUtility.UnpackPrefabInstance(child, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                    
                    string prefabPath = path + $"/{child.name}.prefab";
                    destroyGameObjects.Add(child);
                    if (File.Exists(prefabPath))
                    {
                        GameObject prefabLoad = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                        if (PrefabEquals(prefabLoad, child))
                        {
                            MakeObjectToPrefab(gameObject.transform, child);
                            continue;
                        }
                        else
                        {
                            child.name = $"{child.name}_{Random.Range(0, 999999999)}";
                            prefabPath = path + $"/{child.name}.prefab";
                        }
                    }
                    
                    PrefabUtility.SaveAsPrefabAsset(child, prefabPath);
                    MakeObjectToPrefab(gameObject.transform, child);
                    ReimportCount++;
                }
            }
            
            for (var i = 0; i < destroyGameObjects.Count; i++)
                DestroyImmediate(destroyGameObjects[i]);
            DestroyImmediate(mb3MeshBaker.gameObject);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("SYSTEM", $"Reimport {ReimportCount} object", "OK");
        }

        [Button("TÉt")]
        public void Test()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            byte[] data = File.ReadAllBytes(path);
            byte[] padding = new byte[1024];

            // Tạo padding với các byte ngẫu nhiên
            System.Random rand = new System.Random();
            rand.NextBytes(padding);

            using (FileStream fs = new FileStream($"{_parentFolder}/{Path.GetFileName(path)}", FileMode.Create))
            {
                fs.Write(data, 0, data.Length); // Ghi dữ liệu gốc
                fs.Write(padding, 0, padding.Length); // Ghi padding
            }
            AssetDatabase.Refresh();
            Debug.Log($"Are files different? {AreFilesDifferent(path, $"{_parentFolder}/{Path.GetFileName(path)}")}");
        }
        
        public static bool AreFilesDifferent(string filePath1, string filePath2)
        {
            byte[] file1Bytes = File.ReadAllBytes(filePath1);
            byte[] file2Bytes = File.ReadAllBytes(filePath2);

            // So sánh kích thước file trước
            if (file1Bytes.Length != file2Bytes.Length)
            {
                return true; // Kích thước khác nhau => chắc chắn khác
            }

            // So sánh từng byte
            for (int i = 0; i < file1Bytes.Length; i++)
            {
                if (file1Bytes[i] != file2Bytes[i])
                {
                    return true; // Khác nhau tại vị trí nào đó
                }
            }
            return false; // File giống nhau hoàn toàn
        }

        [Button("Batch Material")]
        private void BatchMaterial()
        {
            string path = Path.Combine(_parentFolder, _resultFolder);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.CreateFolder(_parentFolder, _resultFolder);
            
            MB3_TextureBaker mb3TextureBaker = FindObjectOfType<MB3_TextureBaker>();
            MB3_BatchPrefabBaker mb3BatchPrefabBaker = FindObjectOfType<MB3_BatchPrefabBaker>();
            if (mb3TextureBaker == null)
            {
                GameObject gameObject = (GameObject)PrefabUtility.InstantiatePrefab(
                    AssetDatabase.LoadAssetAtPath<GameObject>(
                        "Assets/CleanSourceTool/Prefabs/BatchPrefabBaker.prefab"));
                mb3TextureBaker = gameObject.GetComponent<MB3_TextureBaker>();
                mb3BatchPrefabBaker = gameObject.GetComponent<MB3_BatchPrefabBaker>();
            }
            
            mb3TextureBaker.objsToMesh.Clear();
            
            for (var i = 0; i < ParentMeshs.Length; i++)
            {
                GameObject gameObject = ParentMeshs[i];
                MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>(true);
                for (var i1 = 0; i1 < meshFilters.Length; i1++)
                {
                    MeshFilter meshFilter = meshFilters[i1];
                    if(!meshFilter.TryGetComponent(out MeshRenderer meshRenderer)) continue;
                    if(meshFilter.sharedMesh != null)
                        mb3TextureBaker.objsToMesh.Add(meshFilter.gameObject);
                }
            }
            MB3_BatchPrefabBakerEditor.PopulatePrefabRowsFromTextureBaker(mb3BatchPrefabBaker);
            MB_BatchPrefabBakerEditorFunctions.CreateEmptyOutputPrefabs(path, mb3BatchPrefabBaker);
            mb3TextureBaker.CreateAtlases(MB3_TextureBakerEditorInternal.updateProgressBar, true, new MB3_EditorMethods());
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("SYSTEM", $"Batch Material Complete", "OK");
        }

        [Button("Batch Mesh")]
        private void BatchMesh()
        {
            MB3_BatchPrefabBaker mb3BatchPrefabBaker = FindObjectOfType<MB3_BatchPrefabBaker>();
            if (mb3BatchPrefabBaker == null)
            {
                GameObject gameObject = (GameObject)PrefabUtility.InstantiatePrefab(
                    AssetDatabase.LoadAssetAtPath<GameObject>(
                        "Assets/CleanSourceTool/Prefabs/BatchPrefabBaker.prefab"));
                mb3BatchPrefabBaker = gameObject.GetComponent<MB3_BatchPrefabBaker>();
            }
            MB_BatchPrefabBakerEditorFunctions.BakePrefabs(mb3BatchPrefabBaker, true);
            EditorUtility.DisplayDialog("SYSTEM", $"Batch Mesh Complete", "OK");
            DestroyImmediate(mb3BatchPrefabBaker.gameObject);
        }

        private void FixMeshPositionAndCollider(string path)
        {
            GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            GameObject child = gameObject.transform.GetChild(0).gameObject;
            child.transform.position = Vector3.zero;

            if (child.TryGetComponent(out MeshFilter meshFilter) && child.TryGetComponent(out MeshRenderer meshRenderer))
            {
                MeshCollider collider = child.AddComponent<MeshCollider>();
                collider.sharedMesh = meshFilter.sharedMesh;
                
                Bounds bounds = meshRenderer.bounds;
                float bottomY = bounds.min.y;
                float offsetY = bottomY;
                child.transform.position -= new Vector3(0, offsetY, 0);
            }
        }

        private void MakeObjectToPrefab(Transform parent, GameObject gameObject)
        {
            string prefabPath = Path.Combine(_parentFolder, _tempFolder) + $"/{gameObject.name}.prefab";
            GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath));
            
            newObject.transform.position = gameObject.transform.position;
            newObject.transform.localScale = gameObject.transform.localScale;
            newObject.transform.localRotation = gameObject.transform.localRotation;
            newObject.transform.SetParent(parent);
        }

        private bool PrefabEquals(GameObject prefab, GameObject gameObject)
        {
            MeshFilter[] meshFilterPrefabs = prefab.GetComponentsInChildren<MeshFilter>(true);
            MeshFilter[] meshFilterObjs = gameObject.GetComponentsInChildren<MeshFilter>(true);
            for (var i = 0; i < meshFilterPrefabs.Length; i++)
            {
                MeshFilter meshFilter = meshFilterPrefabs[i];
                bool isEquals = false;
                for (var i1 = 0; i1 < meshFilterObjs.Length; i1++)
                {
                    MeshFilter meshFilterObj = meshFilterObjs[i1];
                    if (meshFilterObj.sharedMesh == meshFilter.sharedMesh) 
                        isEquals = true;
                }
                if (!isEquals) return false;
            }
            
            MeshRenderer[] meshRendererPrefabs = prefab.GetComponentsInChildren<MeshRenderer>(true);
            MeshRenderer[] meshRendererObjs = gameObject.GetComponentsInChildren<MeshRenderer>(true);
            
            for (var i = 0; i < meshRendererPrefabs.Length; i++)
            {
                MeshRenderer meshRenderer = meshRendererPrefabs[i];
                bool isEquals = false;
                for (var i1 = 0; i1 < meshRendererObjs.Length; i1++)
                {
                    MeshRenderer meshRendererObj = meshRendererObjs[i1];
                    if (meshRendererObj.sharedMaterials.Length == meshRenderer.sharedMaterials.Length)
                    {
                        bool materialEquals = true;
                        for (var i2 = 0; i2 < meshRendererObj.sharedMaterials.Length; i2++)
                        {
                            if (meshRendererObj.sharedMaterials[i2] != meshRenderer.sharedMaterials[i2])
                                materialEquals = false;
                        }
                        if (materialEquals) isEquals = true;
                    }

                }
                if (!isEquals) return false;
            }
            return true;
        }

        private bool HasMesh(GameObject gameObject)
        {
            MeshFilter meshFilter = gameObject.GetComponentInChildren<MeshFilter>();
            if (meshFilter == null) return false;
            return meshFilter.sharedMesh != null;
        }
    }
}
*/
