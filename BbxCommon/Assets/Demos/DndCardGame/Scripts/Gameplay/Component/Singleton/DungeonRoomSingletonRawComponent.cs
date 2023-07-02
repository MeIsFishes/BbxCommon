using System.Collections.Generic;
using UnityEngine;
using BbxCommon;

namespace Dcg
{
    public class DungeonRoomSingletonRawComponent : EcsSingletonRawComponent
    {
        public bool RequestSpawnRoom;
        public GameObject CurRoom
        {
            get
            {
                if (m_Rooms.Count > 0)
                    return m_Rooms[m_Rooms.Count - 1];
                return null;
            }
        }

        private List<GameObject> m_Rooms = new();

        public void AddRoom(GameObject room)
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