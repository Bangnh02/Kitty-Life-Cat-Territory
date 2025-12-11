using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Sirenix.OdinInspector;

namespace CleanSourceTool.FilePropertyClean
{
    [HideReferenceObjectPicker]
    [System.Serializable]
    public class PropertyConvert
    {
        public string propertyId;
        /*[HorizontalGroup("Replace")] public string replaceFrom;
        [HorizontalGroup("Replace")] public string replaceTo;*/
        public List<string> propertyValues = new List<string>();
    }
}