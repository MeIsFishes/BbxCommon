using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    [DisableAutoCreation]
    public class SpawnDungeonRoomSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {
            var roomComp = EcsApi.GetSingletonRawComponent<DungeonRoomSingletonRawComponent>();
            if (roomComp.RequestSpawnRoom == false)
                return;

            var roomData = DataApi.GetData<RoomData>();
            var taskMoveTo = TaskManager<TaskMoveTo>.Instance.CreateTask();
            var newRoom = Object.Instantiate(roomData.RoomPrefab);
            var finalPos = roomComp.CurRoom.transform.position.AddX(roomData.RoomSize.x);
            // 初始化在稍稍偏下的位置
            newRoom.transform.position = finalPos.AddY(-1);
            // 慢慢拼上去
            taskMoveTo.Init(newRoom, finalPos, 2f, -1f);
            roomComp.AddRoom(newRoom);
        }
    }
}
