using UnityEngine;
using BbxCommon;

namespace Dcg
{
    public class DungeonRoomSingletonRawComponent : EcsSingletonRawComponent
    {
        public bool RequestSpawnRoom;
        public GameObject CurRoom;

        public readonly Vector3 CharacterOffset;
        public readonly Vector3 MonsterOffset;

        public override void OnCollect()
        {
            RequestSpawnRoom = false;
            CurRoom = null;
        }
    }
}