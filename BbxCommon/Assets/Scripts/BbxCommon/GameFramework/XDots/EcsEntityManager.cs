using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BbxCommon.Ui;
using UnityEngine;
using UnityEngine.Events;
using Unity.Entities;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace BbxCommon
{
    internal static class EcsEntityManager
    {
        
        private static Dictionary<string, UniqueIdGenerator> m_IdGenerators = new Dictionary<string, UniqueIdGenerator>();
        private static Dictionary<string, Dictionary<EntityID, Entity>> m_EntityByDistrict = new Dictionary<string, Dictionary<EntityID, Entity>>();
        
        internal static Entity CreateEntity(EntityID entityID, string district)
        {
            UniqueIdGenerator idGenerator;
            if(!m_IdGenerators.TryGetValue(district,out idGenerator))
            {
                idGenerator = new UniqueIdGenerator();
                m_IdGenerators[district] = idGenerator;
            }

            Dictionary<EntityID, Entity> districtEntities;
            if (!m_EntityByDistrict.TryGetValue(district, out districtEntities))
            {
                districtEntities = new Dictionary<EntityID, Entity>();
                m_EntityByDistrict[district] = districtEntities;
            }
            
            if (districtEntities.TryGetValue(entityID, out var entity))
            {
                DebugApi.LogError("entityID conflict!!!");
                return Entity.Null;
            }
            
            entity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity();
            if (entityID == EntityID.INVALID)
            {
                entityID = new EntityID(idGenerator.GenerateId(), district);
            }
            
            var ecsDataGroup = ObjectPool<EcsDataGroup>.Alloc();
            ecsDataGroup.Init(entity);
            EcsDataManager.CreateEcsDataGroup(entity, ecsDataGroup);
            var entityIDComp = entity.AddRawComponent<EntityIDComponent>();
            entityIDComp.EntityUniqueID = entityID;
            
            districtEntities[entityID] = entity;
            return entity;
        }

        internal static bool GetEntityByID(EntityID uniqueId, out Entity entity)
        {
            if (m_EntityByDistrict.TryGetValue(uniqueId.GetDistrict(), out var districtDic))
            {
                return districtDic.TryGetValue(uniqueId, out entity);
            }

            entity = Entity.Null;
            return false;
        }
        
        internal static void DestroyEntity(EntityID entityID)
        {
            if (entityID == EntityID.INVALID)
            {
                return;
            }

            if (m_EntityByDistrict.TryGetValue(entityID.GetDistrict(), out var districtDic))
            {
                if (districtDic.TryGetValue(entityID, out var entity))
                {
                    entity.ClearHud();
                    entity.GetGameObject().Destroy();
                    EcsDataManager.DestroyEntity(entityID);
                    World.DefaultGameObjectInjectionWorld?.EntityManager.DestroyEntity(entity);
                    districtDic.Remove(entityID);
                }
               
            }
        }
        
        internal static void DestroyEntity(Entity entity)
        {
            var entityID = entity.GetUniqueId();
            entity.ClearHud();
            entity.GetGameObject().Destroy();
            if (entityID != EntityID.INVALID)
            {
                EcsDataManager.DestroyEntity(entityID);
            }
            World.DefaultGameObjectInjectionWorld?.EntityManager.DestroyEntity(entity);
            
            if (m_EntityByDistrict.TryGetValue(entityID.GetDistrict(), out var districtDic))
            {
                districtDic.Remove(entityID);
            }
        }

        internal static void ResetEntitiesByDistrict(string district)
        {
            if (m_EntityByDistrict.TryGetValue(district, out var districtDic))
            {
                foreach (var entity in districtDic.Values)
                {
                    DestroyEntity(entity);
                }
            }

            if (m_IdGenerators.TryGetValue(district, out var idGenerator))
            {
                idGenerator.ResetCounter();
            }
            
        }

        internal static Dictionary<EntityID, Entity> GetEntitiesByDistrict(string district)
        {
            return m_EntityByDistrict.GetValueOrDefault(district);
        }

        internal static int CheckLeftEntityCount(string district)
        {
            if (m_EntityByDistrict.TryGetValue(district, out var districtDic))
            {
                return districtDic.Count;
            }

            return 0;
        }
        
    }
}