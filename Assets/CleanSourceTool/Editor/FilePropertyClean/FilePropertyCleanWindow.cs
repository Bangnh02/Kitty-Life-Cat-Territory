using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using BG.Library.Tools;
using Microsoft.Win32;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CleanSourceTool.FilePropertyClean
{
    
    public class FilePropertyCleanWindow : OdinEditorWindow
    {
        public List<PropertyConvert> PropertyConverts = new List<PropertyConvert>();
        
        /*[MenuItem("BG/Tools/FilePropertyClean")]
        private static void OpenTextureCleanWindow()
        {
            FilePropertyCleanWindow filePropertyCleanWindow = GetWindow<FilePropertyCleanWindow>();
            filePropertyCleanWindow.Show();
        }*/

        [Button("Load all file property")]
        public void LoadAllFileProperty()
        {
            PropertyConverts.Clear();
            Process proc = new Process();
            proc.StartInfo = new ProcessStartInfo
            {
                FileName = $"{Application.dataPath}\\CleanSourceTool\\Library\\MetadataClean.exe",
                Arguments = $"Export {Application.dataPath}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string metadataExportData = proc.StandardOutput.ReadLine();
                MetadataCleanData metadataCleanData = JsonConvert.DeserializeObject<MetadataCleanData>(metadataExportData);
                for (var _fileData = 0; _fileData < metadataCleanData.FileDatas.Count; _fileData++)
                {
                    FileData fileData = metadataCleanData.FileDatas[_fileData];
                    for (var i_propertyData = 0; i_propertyData < fileData.PropertyDatas.Count; i_propertyData++)
                    {
                        PropertyData propertyData = fileData.PropertyDatas[i_propertyData];
                        PropertyConvert propertyConvert = GetPropertyConvert(propertyData.propertyId);
                        if(!propertyConvert.propertyValues.Contains(propertyData.propertyValue))
                            propertyConvert.propertyValues.Add(propertyData.propertyValue);
                    }
                }
                Debug.Log("Load all file property successfully");
            }
        }
        
        /*
        [Button("Call clean property")]
        public void CallCleanFileProperty()
        {
            ReplaceConvert = new ReplaceConvert();
            for (var i = 0; i < PropertyConverts.Count; i++)
            {
                PropertyConvert propertyConvert = PropertyConverts[i];
                if(string.IsNullOrEmpty(propertyConvert.replaceFrom) || string.IsNullOrEmpty(propertyConvert.replaceTo))
                    continue;
                
                ReplaceData replaceData = GetReplaceData(propertyConvert.propertyId);
                replaceData.replaceFrom = propertyConvert.replaceFrom;
                replaceData.replaceTo = propertyConvert.replaceTo;
            }
            Debug.Log($"{Application.dataPath} {Base64Encode(JsonConvert.SerializeObject(ReplaceConvert))}");

            /*Process proc = new Process();
            proc.StartInfo = new ProcessStartInfo
            {
                FileName = @"D:\BGLibraryTool\MetadataClean\MetadataClean\bin\Debug\MetadataClean.exe",
                Arguments = $"Import {Application.dataPath} {Base64Encode(JsonConvert.SerializeObject(ReplaceConvert))}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string result = proc.StandardOutput.ReadLine();
                Debug.Log($"Call clean file result: " + result);
            }#1#
        }
        */
        
        /*
        public string Base64Encode(string plainText) 
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        
        public ReplaceData GetReplaceData(string id)
        {
            ReplaceData replaceData = ReplaceConvert.ReplaceDatas.Find(replace => replace.propertyId.Equals(id));
            if (replaceData == null)
            {
                replaceData = new ReplaceData();
                replaceData.propertyId = id;
                ReplaceConvert.ReplaceDatas.Add(replaceData);
            }
            return replaceData;
        }*/
        
        public PropertyConvert GetPropertyConvert(string id)
        {
            PropertyConvert propertyConvert = PropertyConverts.Find(property => property.propertyId.Equals(id));
            if (propertyConvert == null)
            {
                propertyConvert = new PropertyConvert();
                propertyConvert.propertyId = id;
                PropertyConverts.Add(propertyConvert);
            }
            return propertyConvert;
        }
    }
}