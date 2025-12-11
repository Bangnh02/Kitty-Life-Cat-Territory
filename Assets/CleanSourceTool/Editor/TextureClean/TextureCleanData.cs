using System;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace BG.Library.Tools
{
    [HideReferenceObjectPicker]
    public class TextureCleanData
    {
        [Title("@TextureName")]
        [PreviewField(ObjectFieldAlignment.Left, Height = 25), LabelText("New TextureChanged")]
        public Texture2D BaseTexture;
        public byte[] OldBinary;
        public byte[] NewBinary;

        public string TextureName => $"{AssetDatabase.GetAssetPath(BaseTexture)} (IsBinaryChanged >> {!OldBinary.Equals(NewBinary)})";

        public void ReImportTextureAndClean()
        {
            string savePath = AssetDatabase.GetAssetPath(BaseTexture);
            try
            {
                OldBinary = File.ReadAllBytes(savePath);
                LimitBinary(100, ref OldBinary);

                Texture2D tempTexture = new Texture2D(BaseTexture.width, BaseTexture.height);
                CopyTextureDataAndChanged(BaseTexture, tempTexture);


                SaveTexture(savePath, tempTexture);
                BaseTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(savePath);
                NewBinary = File.ReadAllBytes(savePath);
                LimitBinary(100, ref NewBinary);
            }
            catch (Exception e)
            {
                Debug.Log($"Error: {e}, {savePath}, {BaseTexture.name}");
            }

        }

        public void LimitBinary(int maxData, ref byte[] binary)
        {
            if (maxData >= binary.Length) maxData = binary.Length;
            byte[] newLimitBinary = new byte[maxData];
            for (var i = 0; i < newLimitBinary.Length; i++)
                if (i < binary.Length) newLimitBinary[i] = binary[i];
            binary = newLimitBinary;
        }
        
        private void SaveTexture(string path, Texture2D texture)
        {
            byte[] bytes = new byte[] { };
            string id = path.Split('.')[^1];
            switch (id)
            {
                case "png":
                    bytes = texture.EncodeToPNG();
                    break;
                case "jpg":
                    bytes = texture.EncodeToJPG();
                    break;
                case "exr":
                    bytes = texture.EncodeToEXR();
                    break;
                case "tga":
                    bytes = texture.EncodeToTGA();
                    break;
            }
            File.WriteAllBytes(path, bytes);
            AssetDatabase.Refresh();
        }

        private void CopyTextureDataAndChanged(Texture2D copyTexture, Texture2D texture)
        {
            Color[] changedPixels = GetPixelTexture(copyTexture);
            float change = 0.1f / 255f;
            for (var i = 0; i < changedPixels.Length; i++)
            {
                Color color = changedPixels[i];
                if(color.r > 0) color.r -= change;
                else color.r += change;
                
                if(color.g > 0) color.g -= change;
                else color.g += change;
                
                if(color.b > 0) color.b -= change;
                else color.b += change;
                
                if(color.a > 0) color.a -= change;
                else color.a += change;
                
                changedPixels[i] = color;
            }
            SetPixelTexture(texture, changedPixels);
        }
        
        private void SetPixelTexture(Texture2D texture2D, Color[] pixels)
        {
            bool isReadable = texture2D.isReadable;
            TextureImporter textureImporter = null;
            try
            {
                if (!isReadable)
                {
                    string texturePath = AssetDatabase.GetAssetPath(texture2D);
                    textureImporter = (TextureImporter)AssetImporter.GetAtPath(texturePath);
                    textureImporter.isReadable = true;
                    textureImporter.SaveAndReimport();
                }
                texture2D.SetPixels(pixels);
                texture2D.Apply();
            }
            finally
            {
                if (!isReadable && textureImporter != null)
                {
                    textureImporter.isReadable = false;
                    textureImporter.SaveAndReimport();
                }
            }
        }

        private Color[] GetPixelTexture(Texture2D texture2D)
        {
            bool isReadable = texture2D.isReadable;
            TextureImporter textureImporter = null;
            try
            {
                if (!isReadable)
                {
                    var origTexPath = AssetDatabase.GetAssetPath(texture2D);
                    textureImporter = (TextureImporter)AssetImporter.GetAtPath(origTexPath);
                    textureImporter.isReadable = true;
                    textureImporter.SaveAndReimport();
                }
                Color[] pixelData = texture2D.GetPixels();
                return pixelData;
            }
            finally
            {
                if (!isReadable && textureImporter != null)
                {
                    textureImporter.isReadable = false;
                    textureImporter.SaveAndReimport();
                }
            }
        }
    }
}