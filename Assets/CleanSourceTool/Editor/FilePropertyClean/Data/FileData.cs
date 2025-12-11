using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace BG.Library.Tools
{
    [HideReferenceObjectPicker]
    [System.Serializable]
    public class FileData
    {
        public string filePath;
        public List<PropertyData> PropertyDatas = new List<PropertyData>();
    }
}