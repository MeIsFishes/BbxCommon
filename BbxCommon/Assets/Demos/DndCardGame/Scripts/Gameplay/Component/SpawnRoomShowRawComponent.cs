using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg
{
    public class SpawnRoomShowRawComponent : EcsRawComponent
    {
        public bool IsSpawning;
        public float ElapsedTime;
        /// <summary>
        /// 被创建房间应在的位置
        /// </summary>
        public Vector3 OriginalPos;
    }
}
