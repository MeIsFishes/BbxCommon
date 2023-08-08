using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    public class DungeonRoomSingletonRawComponent : EcsSingletonRawComponent
    {
        public bool RequestSpawnRoom;
        public Entity CurRoom
        {
            get
            {
                if (m_Rooms.Count > 0)
                    return m_Rooms[m_Rooms.Count - 1];
                return Entity.Null;
            }
        }

        private List<Entity> m_Rooms = new();

        public void AddRoom(Entity room)
        {
            m_Rooms.Add(room);
        }

        public void DestroyAllRooms()
        {
            foreach (var room in m_Rooms)
            {
                room.Destroy();
            }
        }

        public override void OnCollect()
        {
            RequestSpawnRoom = false;
        }
    }
}