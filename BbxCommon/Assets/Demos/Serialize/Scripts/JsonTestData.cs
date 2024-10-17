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

        public override string ToString()
        {
            return "BoolValue = " + BoolValue +
                "\nIntValue = " + IntValue +
                "\nFloatValue = " + FloatValue +
                "\nInternalClass.Value = " + Class.Value +
                "\nAbsulutePath = " + AbsolutePath;
        }

        [Button]
        private void Serialize()
        {
            JsonApi.Serialize(this, AbsolutePath);
        }

        [Button]
        private void Deserialize()
        {
            var data = JsonApi.Deserialize<JsonTestData>(AbsolutePath);
            DebugApi.Log(data);
        }
    }
}
