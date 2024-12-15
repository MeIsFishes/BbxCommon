using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon
{
    [CreateAssetMenu(fileName = "LoadingTimeData", menuName = "Demos/LoadingTimeData")]
    public class LoadingTimeData: ScriptableObject
    {
        public SerializableDic <string, float> dataDictionary = new SerializableDic<string, float>();

    }
    
}