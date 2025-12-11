using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CleanSourceTool.Editor;
using CleanSourceTool.Editor.FileBinary;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CleanSourceTool.Backup
{
    [HideReferenceObjectPicker, HideLabel]
    public class FileBackup
    {
        [ShowIf("HasFolder")] public SearchId SearchId;

        [Button("BackupAll File"), HideIf("HasFolder")]
        private void Encryption()
        {
            try
            {
                string savePath = Application.dataPath.Replace("Assets", "CleanToolBackup");
                foreach (SearchId search in System.Enum.GetValues(typeof(SearchId)))
                {
                    string[] guids = AssetDatabase.FindAssets($"t:{search.ToString()}",
                        new[] { CleanSourceToolWindow.AssetsFolder });
                    for (var i = 0; i < guids.Length; i++)
                    {
                        string filePath = AssetDatabase.GUIDToAssetPath(guids[i]);
                        string fileName = Path.GetFileName(filePath).Split('.')[0];
                        string fileId = $"{search}~{guids[i]}~{fileName}";
                        EditorUtility.DisplayProgressBar($"Backup {search}",
                            $"Backup {filePath} to {savePath}/{fileId}", (float)i / (float)guids.Length);
                        BackupFile(savePath, filePath, fileId);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            EditorUtility.ClearProgressBar();
        }

        [Button("Restore All"), ShowIf("HasFolder")]
        private void RestoreAll()
        {
            string savePath = Application.dataPath.Replace("Assets", "CleanToolBackup");
            string[] files = Directory.GetFiles(savePath);
            for (int i = 0; i < files.Length; i++)
            {
                string filePath = files[i];
                string[] split = Path.GetFileName(filePath).Split('~');

                string guid = split[1];
                string backupFileName = split[2];
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileName(path).Split('.')[0];
                EditorUtility.DisplayProgressBar($"Restore", $"Restore File {fileName}",
                    (float)i / (float)files.Length);

                if (!AdvancedFileModifier.AreFilesBinaryIdentical(path, filePath))
                {
                    byte[] bytes = File.ReadAllBytes(filePath);
                    File.WriteAllBytes(path, bytes);
                }

                if (!fileName.Equals(backupFileName))
                {
                    AssetDatabase.RenameAsset(path, backupFileName);
                    AssetDatabase.SaveAssets();
                }

                AssetDatabase.Refresh();
            }

            EditorUtility.ClearProgressBar();
            Directory.Delete(savePath, true);
        }

        [Button("Restore Custom SearchID"), ShowIf("HasFolder")]
        private void RestoreCustomSearchID()
        {
            string savePath = Application.dataPath.Replace("Assets", "CleanToolBackup");
            string[] files = Directory.GetFiles(savePath);
            for (int i = 0; i < files.Length; i++)
            {
                string filePath = files[i];
                string[] split = Path.GetFileName(filePath).Split('~');
                SearchId searchId = (SearchId)Enum.Parse(typeof(SearchId), split[0]);
                if (SearchId.IsMatch(searchId))
                {
                    string guid = split[1];
                    string backupFileName = split[2];
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    string fileName = Path.GetFileName(path).Split('.')[0];
                    EditorUtility.DisplayProgressBar($"Restore", $"Restore File {fileName}",
                        (float)i / (float)files.Length);

                    if (!AdvancedFileModifier.AreFilesBinaryIdentical(path, filePath))
                    {
                        byte[] bytes = File.ReadAllBytes(filePath);
                        File.WriteAllBytes(path, bytes);
                    }

                    if (!fileName.Equals(backupFileName))
                    {
                        AssetDatabase.RenameAsset(path, backupFileName);
                        AssetDatabase.SaveAssets();
                    }

                    AssetDatabase.Refresh();
                }
            }

            EditorUtility.ClearProgressBar();
        }

        [Button("Restore FileName"), ShowIf("HasFolder")]
        private void RestoreFileName()
        {
            string savePath = Application.dataPath.Replace("Assets", "CleanToolBackup");
            string[] files = Directory.GetFiles(savePath);
            for (int i = 0; i < files.Length; i++)
            {
                string filePath = files[i];
                string[] split = Path.GetFileName(filePath).Split('~');
                string guid = split[1];
                string backupFileName = split[2];
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileName(path).Split('.')[0];
                EditorUtility.DisplayProgressBar($"Restore FileName",
                    $"Restore FileName {fileName} to {backupFileName}", (float)i / (float)files.Length);
                if (!fileName.Equals(backupFileName))
                {
                    AssetDatabase.RenameAsset(path, backupFileName);
                    AssetDatabase.SaveAssets();
                }
            }

            EditorUtility.ClearProgressBar();
        }

        private void BackupFile(string savePath, string filePath, string fileId)
        {
            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

            byte[] bytes = File.ReadAllBytes(filePath);
            using (FileStream fs = new FileStream(Path.Combine(savePath, fileId), FileMode.Create))
            {
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            }
        }

        private bool HasFolder()
        {
            string savePath = Application.dataPath.Replace("Assets", "CleanToolBackup");
            return Directory.Exists(savePath);
        }
    }
}