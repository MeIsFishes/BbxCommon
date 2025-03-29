using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg
{
    public class MapRoomRawComponent : EcsRawComponent
    {
        public int Layer;
        public int Index;
        public List<int> NextRoomLayers;
        public List<int> NextRoomIndices;
        public List<int> PreviousRoomLayers;
        public List<int> PreviousRoomIndices;
        public Vector2 Position;
        public EMapRoomType Type;
    }
}
