using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace CleanSourceTool.FilePropertyClean
{
    [HideReferenceObjectPicker, HideLabel]
    public class FileReimportWindow
    {
        public List<Object> FolderAssets = new List<Object>();
        private int _fileUpdateCount;
        
        [Button("ReimportAllFile")]
        private void ReimportAllFile()
        {
            _fileUpdateCount = 0;
            string[] paths = new string[FolderAssets.Count];
            for (var i = 0; i < FolderAssets.Count; i++)
            {
                string path = AssetDatabase.GetAssetPath(FolderAssets[i]);
                paths[i] = path;
            }
  
            for (var i = 0; i < paths.Length; i++)
                LoadAllFileInDirectory(paths[i]);
            EditorUtility.ClearProgressBar();
        }
        
        private void LoadAllFileInDirectory(string targetDirectory)
        {
            if (File.Exists(targetDirectory))
            {
                EditorUtility.DisplayProgressBar($"Reimport file", $"Reimport file {Path.GetFileName(targetDirectory)}", 1f);
                ReimportFile(targetDirectory);
                AssetDatabase.Refresh();
            }
            else
            {
                string [] fileEntries = Directory.GetFiles(targetDirectory);
                for (var i = 0; i < fileEntries.Length; i++)
                {
                    EditorUtility.DisplayProgressBar($"Reimport file", $"Reimport file {Path.GetFileName(fileEntries[i])}", (float)i / (float)fileEntries.Length);
                    ReimportFile(fileEntries[i]);
                    AssetDatabase.Refresh();
                }
                
                string [] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                foreach(string subdirectory in subdirectoryEntries)
                    LoadAllFileInDirectory(subdirectory);
            }
        }

        private void ReimportFile(string filePath)
        {
            string persistentDataPath = Application.persistentDataPath;
            
            byte[] bytes = File.ReadAllBytes(filePath);
            string fileName = Path.GetFileName(filePath);
            string savePath = Path.Combine(persistentDataPath, fileName);
            
            FileStream file = File.Create(savePath);
            file.Write(bytes, 0, bytes.Length);
            file.Close();
            
            File.Delete(filePath);
            File.Move(savePath, filePath);
            _fileUpdateCount++;
        }
    }
}