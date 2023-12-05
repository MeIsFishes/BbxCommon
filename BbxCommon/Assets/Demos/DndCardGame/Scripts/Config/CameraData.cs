using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Sirenix.OdinInspector;
using BbxCommon;

namespace Dcg
{
    [CreateAssetMenu(fileName = "CameraData", menuName = "Demos/Dcg/CameraData")]
    public class CameraData : BbxScriptableObject
    {
        public GameObject CameraPrefab;

        [FoldoutGroup("Dungeon")]
        public Vector3 DungeonOffset;

        protected override void OnLoad()
        {
            DataApi.SetData(this);
        }
    }
}
