using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace BG.Library.Tools
{
    [HideReferenceObjectPicker]
    [System.Serializable]
    public class MetadataCleanData
    {
        public List<FileData> FileDatas = new List<FileData>();
    }
}