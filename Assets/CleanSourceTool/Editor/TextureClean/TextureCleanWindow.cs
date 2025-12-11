using System.Collections.Generic;
using System.IO;
using System.Text;
using CleanSourceTool.Editor;
using CleanSourceTool.Editor.FileBinary;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace BG.Library.Tools
{
    [HideReferenceObjectPicker, HideLabel]
    internal sealed class TextureCleanWindow
    {
        public List<TextureCleanData> TextureCleanDatas = new List<TextureCleanData>();

        [Button("Reimport and changed"), PropertyOrder(0)]
        private void LoadAllTexture()
        {
            TextureCleanDatas.Clear();
            string[] guids = AssetDatabase.FindAssets("t:Texture", new[] { CleanSourceToolWindow.AssetsFolder });
            for (var i = 0; i < guids.Length; i++)
            {
                string filePath = AssetDatabase.GUIDToAssetPath(guids[i]);
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);
                if (texture != null && IsTexture(Path.GetFileName(filePath)))
                {
                    EditorUtility.DisplayProgressBar($"Texture Clean", $"Texture Clean {texture.name}",
                        (float)i / (float)guids.Length);
                    TextureCleanData textureCleanData = new TextureCleanData();
                    textureCleanData.BaseTexture = texture;
                    textureCleanData.ReImportTextureAndClean();
                    TextureCleanDatas.Add(textureCleanData);
                }
            }

            EditorUtility.ClearProgressBar();
        }

        private bool IsTexture(string fileName)
        {
            string id = fileName.Split('.')[^1];
            return id.Equals("png") || id.Equals("jpg") || id.Equals("exr") || id.Equals("tga");
        }
    }
}