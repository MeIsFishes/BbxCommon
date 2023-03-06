using System;
using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon.Ui
{
    public class UiSceneAsset : ScriptableObject
    {
        [Serializable]
        public class UiObjectData
        {
            public string PrefabPath;
            public int UiGroup;
            public bool DefaultOpen;
            public Vector3 Position;
            public Vector3 Scale;
            public Vector2 Pivot;
        }

        public List<UiObjectData> UiObjectDatas = new List<UiObjectData>();
    }
}
