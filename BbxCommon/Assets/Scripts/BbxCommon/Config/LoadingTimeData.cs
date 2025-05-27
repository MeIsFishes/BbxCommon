using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon
{
    public class LoadingTimeData: ScriptableObject
    {
        public SerializableDic <string, float> dataDictionary = new SerializableDic<string, float>();
    }
}