using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Sirenix.OdinInspector;
using BbxCommon;

namespace Ser
{
    [CreateAssetMenu(fileName = "JsonTestData", menuName = "Demos/Ser/JsonTestData")]
    public class JsonTestData : BbxScriptableObject
    {
        public bool BoolValue;
        public int IntValue;
        public float FloatValue;
        public InternalClass Class;
        public string AbsolutePath;

        [Serializable]
        public class InternalClass
        {
            public string Value;
        }

        protected override void OnLoad()
        {
            DataApi.SetData(this);
        }

        [Button]
        private void Serialize()
        {
            JsonApi.Serialize(this, AbsolutePath);
        }
    }
}
