using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon
{
    public class ResourcesFileData : ScriptableObject
    {
        public Dictionary<string, List<string>> FileDic = new Dictionary<string, List<string>>();

        public void AddFile(string path)
        {
            
        }
    }
}
