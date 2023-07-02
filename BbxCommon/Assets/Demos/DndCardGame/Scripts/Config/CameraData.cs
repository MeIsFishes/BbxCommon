using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Sirenix.OdinInspector;
using BbxCommon;

namespace Dcg
{
    [CreateAssetMenu(fileName = "CameraData", menuName = "Demos/Dcg/CameraData")]
    public class CameraData : ScriptableObject
    {
        public GameObject CameraPrefab;

        [FoldoutGroup("Dungeon")]
        public Vector3 DungeonOffset;
    }
}
