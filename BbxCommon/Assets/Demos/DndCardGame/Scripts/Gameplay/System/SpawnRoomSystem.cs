using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    [DisableAutoCreation]
    public partial class SpawnRoomSystem : EcsMixSystemBase
    {
        protected override void OnSystemUpdate()
        {
            var dungeonRoomComp = EcsApi.GetSingletonRawComponent<DungeonRoomSingletonRawComponent>();
            if (dungeonRoomComp == null)
                return;
            var roomData = DataApi.GetData<RoomData>();
            if (roomData == null)
                return;

            if (dungeonRoomComp.RequestSpawnRoom)
            {
                var curRoomPos = dungeonRoomComp.CurRoom.GetGameObject().transform.position;
                dungeonRoomComp.AddRoom(EntityCreator.CreateRoomEntity(curRoomPos.AddZ(roomData.RoomSize.y)));
                dungeonRoomComp.RequestSpawnRoom = false;
            }
        }
    }
}
