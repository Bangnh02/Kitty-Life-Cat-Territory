using System.Collections.Generic;
using BG.Library.Tools;
using CleanSourceTool.Backup;
using CleanSourceTool.Editor.FileBinary;
using CleanSourceTool.Editor.FilterUnusedResources;
using CleanSourceTool.FilePropertyClean;
using CleanSourceTool.MeshAPI;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace CleanSourceTool.Editor
{
    internal sealed class CleanSourceToolWindow : OdinEditorWindow
    {
        public const string ResourcesFolder = "/Resources/";
        public const string AssetsFolder = "Assets";
        
        [TabGroup("File Reimport")]
        public FileReimportWindow FileReimportWindow = new FileReimportWindow();
        
        [TabGroup("File Backup")] 
        public FileBackup FileBackup = new FileBackup();
        
        [TabGroup("Filter Unused Resource")]
        public FilterUnusedResourceWindow FilterUnusedResourceWindow = new FilterUnusedResourceWindow();
        
        [TabGroup ("Texture Clean")]
        public TextureCleanWindow TextureCleanWindow = new TextureCleanWindow();
        
        [TabGroup ("Mesh Reducer")]
        public MeshReducerWindow MeshReducerWindow = new MeshReducerWindow();
        
        [TabGroup("File ReName")]
        public FileReNameWindow FileReNameWindow = new FileReNameWindow();

        [TabGroup("File Binary Metadata")] 
        public FileBinaryWindow FileBinaryWindow = new FileBinaryWindow();
        
        [MenuItem("BG/Tools/Clean source tool")]
        private static void OpenWindow()
        {
            CleanSourceToolWindow cleanSourceToolWindow = GetWindow<CleanSourceToolWindow>();
            cleanSourceToolWindow.Show();
        }
    }

}