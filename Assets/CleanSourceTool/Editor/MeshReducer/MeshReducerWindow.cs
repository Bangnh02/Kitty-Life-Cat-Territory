using System.Collections.Generic;
using System.IO;
using System.Linq;
using CleanSourceTool.Editor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CleanSourceTool.MeshAPI
{
    [HideReferenceObjectPicker, HideLabel]
    public class MeshReducerWindow
    {
        public int MeshReducerCount = 1;
        
        [Button("MeshReducerAll")]
        private void MeshReducerAll()
        {
            string[] guidMeshs = AssetDatabase.FindAssets("t:Mesh", new string[] { CleanSourceToolWindow.AssetsFolder });
            
            EditorUtility.DisplayProgressBar($"Mesh delete", $"Mesh delete reducer", 0f);
            AssetDatabase.DeleteAsset("Assets/CleanMeshs");
            AssetDatabase.CreateFolder("Assets", "CleanMeshs");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Dictionary<string, string> meshReplaces = new Dictionary<string, string>();
            for (var i = 0; i < guidMeshs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guidMeshs[i]);
                Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);

                if (obj is Mesh mesh)
                {
                    EditorUtility.DisplayProgressBar($"Mesh Reducer", $"Mesh Reducer {mesh.name}", (float)i / (float)guidMeshs.Length);
                    CreateMesh(guidMeshs[i], meshReplaces, mesh);
                }
                else
                {
                    Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
                    foreach (Object asset in assets)
                    {
                        if (asset is Mesh meshL)
                        {
                            EditorUtility.DisplayProgressBar($"Mesh Reducer", $"Mesh Reducer {meshL.name}", (float)i / (float)guidMeshs.Length);
                            CreateMesh(guidMeshs[i], meshReplaces, meshL);
                        }
                    }
                }
            }
            
            string[] guidPrefabs = AssetDatabase.FindAssets("t:Prefab", new string[] { CleanSourceToolWindow.AssetsFolder });
            for (var i = 0; i < guidPrefabs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guidPrefabs[i]);
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>(true);
                for (var j = 0; j < meshFilters.Length; j++)
                {
                    Mesh mesh = meshFilters[j].sharedMesh;
                    if (mesh != null)
                    {
                        string assetPath = AssetDatabase.GetAssetPath(mesh);
                        if(assetPath.Contains("CleanMeshs")) continue;
                        string guid = AssetDatabase.AssetPathToGUID(assetPath);
                        string key = $"{guid}~{mesh.name}";
                        if(!meshReplaces.TryGetValue(key, out string replace)) continue;
                        
                        EditorUtility.DisplayProgressBar($"Mesh Replace", $"Replace Mesh Prefab {mesh.name} to {replace}", (float)i / (float)meshFilters.Length);
                        Mesh newMesh = AssetDatabase.LoadAssetAtPath<Mesh>(replace);
                        meshFilters[j].sharedMesh = newMesh;
                    }
                }
                
                EditorUtility.SetDirty(obj);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            for (var i = 0; i < scenes.Length; i++)
            {
                EditorBuildSettingsScene editorBuildSettingsScene = scenes[i];
                EditorUtility.DisplayProgressBar($"Mesh Replace", $"Open Scene {editorBuildSettingsScene.path}", (float)i / (float)scenes.Length);
                EditorSceneManager.OpenScene(editorBuildSettingsScene.path);
                MeshFilter[] meshFilters = Object.FindObjectsOfType<MeshFilter>(true);
                for (var j = 0; j < meshFilters.Length; j++)
                {
                    Mesh mesh = meshFilters[j].sharedMesh;
                    if (mesh != null)
                    {
                        string assetPath = AssetDatabase.GetAssetPath(mesh);
                        if(assetPath.Contains("CleanMeshs")) continue;
                        string guid = AssetDatabase.AssetPathToGUID(assetPath);
                        string key = $"{guid}~{mesh.name}";
                        if(!meshReplaces.TryGetValue(key, out string replace)) continue;
                        
                        EditorUtility.DisplayProgressBar($"Mesh Replace", $"Replace Mesh Scene {mesh.name} to {replace}", (float)i / (float)scenes.Length);
                        Mesh newMesh = AssetDatabase.LoadAssetAtPath<Mesh>(replace);
                        meshFilters[j].sharedMesh = newMesh;
                    }
                }
                
                EditorSceneManager.SaveScene(SceneManager.GetSceneByPath(editorBuildSettingsScene.path));
            }
            
            EditorUtility.ClearProgressBar();
        }

        private void CreateMesh(string guid, Dictionary<string, string> meshReplaces, Mesh mesh)
        {
            float reductionFactor = ((float)mesh.vertices.Length - MeshReducerCount) / ((float)mesh.vertices.Length);
            if (mesh.vertices.Length < 150) reductionFactor = 1f; 
            Mesh newMesh = MeshReducer.ReduceMesh(mesh, reductionFactor);
            string pathSave = $"Assets/CleanMeshs/{mesh.name}_MeshReducer.asset";
            if(File.Exists(pathSave))
                pathSave = $"Assets/CleanMeshs/{mesh.name}_MeshReducer{Random.Range(0, 99999999)}.asset";
                
            meshReplaces.Add($"{guid}~{mesh.name}", pathSave);
            AssetDatabase.CreateAsset(newMesh, pathSave);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}