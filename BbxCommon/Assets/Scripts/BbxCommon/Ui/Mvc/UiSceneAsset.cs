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
            [NonSerialized]
            public UiControllerBase CreatedController;
            public string PrefabPath;
            public Type UiControllerType;
            public int UiGroup;
            public bool DefaultShow;
            public Vector3 Position;
            public Vector3 Scale;
            public Vector2 Pivot;
        }

        public List<UiObjectData> UiObjectDatas = new List<UiObjectData>();
    }
}
