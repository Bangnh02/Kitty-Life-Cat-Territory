using System;
using System.Collections.Generic;
using System.Text;
using CleanSourceTool.Editor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace CleanSourceTool.FilePropertyClean
{
    [HideReferenceObjectPicker, HideLabel]
    public class FileReNameWindow
    {
        public SearchId SearchId;
        
        public bool IgnoreResourcesFolder;
        public bool UseRandom;
        public bool SaveOldName;
        public int RandomTextCount = 10;
        
        public string Prefix;
        public string Suffix;
        
        [Button("ReNameAllSource")]
        private void ReMameAllSource()
        {
            StringBuilder searchMatch = new StringBuilder();
            foreach (SearchId search in System.Enum.GetValues(typeof(SearchId)))
                if(SearchId.IsMatch(search)) searchMatch.Append($"t:{search.ToString()} ");
            
            string[] guids = AssetDatabase.FindAssets(searchMatch.ToString(), new string[] { CleanSourceToolWindow.AssetsFolder });
            for (var i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (IgnoreResourcesFolder && path.Contains(CleanSourceToolWindow.ResourcesFolder)) continue;
                Object obj =  AssetDatabase.LoadAssetAtPath<Object>(path);

                string name = obj.name;
                if (UseRandom)
                {
                    if(SaveOldName)
                        name = $"{name}_{CreateNameRandom()}";
                    else
                        name = CreateNameRandom();
                }
                    
                if (!name.StartsWith(Prefix))
                    name = $"{Prefix}_{name}";
                if (!name.EndsWith(Suffix))
                    name = $"{name}_{Suffix}";
                
                EditorUtility.DisplayProgressBar($"File Rename", $"File Rename {obj.name} to {name}", (float)i / (float)guids.Length);
                AssetDatabase.RenameAsset(path, name);
                AssetDatabase.SaveAssets();
            }
            EditorUtility.ClearProgressBar();
        }

        private string CreateNameRandom()
        {
            string[] Alphabet = new string[26] {"A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"};
            string newName = "";
            for (int i = 0; i < RandomTextCount; i++)
            {
                string randomText = Alphabet[Random.Range(0, Alphabet.Length)];
                if (Random.Range(0, 2) == 0)
                    randomText = randomText.ToLower();
                newName += randomText;
            }
            return newName;
        }
    }
}