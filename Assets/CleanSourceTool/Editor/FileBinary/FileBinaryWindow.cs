using System.IO;
using System.Text;
using BG.Library.Tools;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace CleanSourceTool.Editor.FileBinary
{
    [HideReferenceObjectPicker, HideLabel]
    public class FileBinaryWindow
    {
        public SearchId SearchId;
        
        [Button("Binary & Metadata Encryption")]
        public void Encryption()
        {
            StringBuilder searchMatch = new StringBuilder();
            foreach (SearchId search in System.Enum.GetValues(typeof(SearchId)))
                if(SearchId.IsMatch(search)) searchMatch.Append($"t:{search.ToString()} ");

            int modifyFileCount = 0;
            int ignoreModifyFileCount = 0;
            string[] guids = AssetDatabase.FindAssets(searchMatch.ToString(), new []{ CleanSourceToolWindow.AssetsFolder });
            for (var i = 0; i < guids.Length; i++)
            {
                string filePath = AssetDatabase.GUIDToAssetPath(guids[i]);
                AdvancedFileModifier.ModifyFileCompletely(filePath);
            }
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("SYSTEM", $"File Binary Metadata changed {modifyFileCount} file, ignore Resources folder {ignoreModifyFileCount} file", "OK");
        }
    }
}