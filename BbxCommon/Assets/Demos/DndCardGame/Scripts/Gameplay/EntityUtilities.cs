using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    public static class EntityUtilities
    {
        #region Room
        public static void SpawnRoomStart(Entity entity, Vector3 originalPos)
        {
            var spawnRoomShowComp = entity.GetRawComponent<SpawnRoomShowRawComponent>();
            spawnRoomShowComp.IsSpawning = true;
            spawnRoomShowComp.ElapsedTime = 0;
            spawnRoomShowComp.OriginalPos = originalPos;
            entity.ActivateRawAspect<SpawnRoomShowRawAspect>();
        }

        public static void SpawnRoomEnd(Entity entity)
        {
            entity.GetRawComponent<SpawnRoomShowRawComponent>().IsSpawning = false;
            entity.DeactiveRawComponent<SpawnRoomShowRawComponent>();
            entity.DeactiveRawAspect<SpawnRoomShowRawAspect>();
        }
        #endregion
    }
}
