using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CleanSourceTool.Editor.FilterUnusedResources
{
    
    [HideReferenceObjectPicker, HideLabel]
    public class FilterUnusedResourceWindow
    {
        [Button("Filter Unused Resource")]
        public void FilterUnusedResource()
        {
            List<string> filterDatas = new List<string>();

            string[] guids = AssetDatabase.FindAssets($"t:Script", new[] { CleanSourceToolWindow.AssetsFolder });
            for (var i = 0; i < guids.Length; i++)
            {
                string input = File.ReadAllText(AssetDatabase.GUIDToAssetPath(guids[i]));
                string pattern = @"Resources\.Load(All)?<(?<type>\w+)>\((?<path>[^)]+)\)";
                Regex regex = new Regex(pattern);

                MatchCollection matches = regex.Matches(input);
                foreach (Match match in matches)
                {
                    string path = match.Groups["path"].Value;
                    filterDatas.Add(path.Replace("\"", ""));
                }
            }

            if (!AssetDatabase.IsValidFolder("Assets/CleanResourcesMove"))
                AssetDatabase.CreateFolder("Assets", "CleanResourcesMove");
            
            Object[] objects = Resources.LoadAll("");
            for (var i = 0; i < objects.Length; i++)
            {
                Object obj = objects[i];
                string[] path = GetFoldersFromPath(AssetDatabase.GetAssetPath(obj));
                if(path.Length > 2) continue;
                if (!filterDatas.Contains(obj.name))
                {
                    string assetPath = AssetDatabase.GetAssetPath(obj);
                    EditorUtility.DisplayProgressBar($"Filter Unused Resources", $"Move file {assetPath} to {Path.Combine("Assets/CleanResourcesMove", Path.GetFileName(assetPath))}", (float)i / (float)objects.Length);
                    AssetDatabase.MoveAsset(assetPath, Path.Combine("Assets/CleanResourcesMove", Path.GetFileName(assetPath)));
                }
            }
            EditorUtility.ClearProgressBar();
        }
        
        static string[] GetFoldersFromPath(string path)
        {
            string directoryPath = System.IO.Path.GetDirectoryName(path);
            return directoryPath.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }

    public class Folder
    {
        public string folderName;
        public string fileName;
        public bool IsFocus;
    }
}