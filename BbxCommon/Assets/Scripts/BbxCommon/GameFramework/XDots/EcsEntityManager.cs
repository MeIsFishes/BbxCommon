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
        private static Dictionary<string, Dictionary<EntityID, Entity>> m_EntityByGroup = new Dictionary<string, Dictionary<EntityID, Entity>>();
        
        internal static Entity CreateEntity(EntityID entityID, string group)
        {
            UniqueIdGenerator idGenerator;
            if(!m_IdGenerators.TryGetValue(group,out idGenerator))
            {
                idGenerator = new UniqueIdGenerator();
                m_IdGenerators[group] = idGenerator;
            }

            Dictionary<EntityID, Entity> groupEntities;
            if (!m_EntityByGroup.TryGetValue(group, out groupEntities))
            {
                groupEntities = new Dictionary<EntityID, Entity>();
                m_EntityByGroup[group] = groupEntities;
            }
            
            if (groupEntities.TryGetValue(entityID, out var entity))
            {
                DebugApi.LogError("entityID conflict!!!");
                return Entity.Null;
            }
            
            entity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity();
            if (entityID == EntityID.INVALID)
            {
                entityID = new EntityID(idGenerator.GenerateId(), group);
            }
            
            var ecsDataGroup = ObjectPool<EcsDataGroup>.Alloc();
            ecsDataGroup.Init(entity);
            EcsDataManager.CreateEcsDataGroup(entity, ecsDataGroup);
            var entityIDComp = entity.AddRawComponent<EntityIDComponent>();
            entityIDComp.EntityUniqueID = entityID;
            
            groupEntities[entityID] = entity;
            return entity;
        }

        internal static bool GetEntityByID(EntityID uniqueId, out Entity entity)
        {
            if (m_EntityByGroup.TryGetValue(uniqueId.GetGroup(), out var groupDic))
            {
                return groupDic.TryGetValue(uniqueId, out entity);
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

            if (m_EntityByGroup.TryGetValue(entityID.GetGroup(), out var groupDic))
            {
                if (groupDic.TryGetValue(entityID, out var entity))
                {
                    entity.ClearHud();
                    entity.GetGameObject().Destroy();
                    EcsDataManager.DestroyEntity(entityID);
                    World.DefaultGameObjectInjectionWorld?.EntityManager.DestroyEntity(entity);
                    groupDic.Remove(entityID);
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
            
            if (m_EntityByGroup.TryGetValue(entityID.GetGroup(), out var groupDic))
            {
                groupDic.Remove(entityID);
            }
        }

        internal static void ResetEntitiesByGroup(string group)
        {
            if (m_EntityByGroup.TryGetValue(group, out var groupDic))
            {
                foreach (var entity in groupDic.Values)
                {
                    DestroyEntity(entity);
                }
            }

            if (m_IdGenerators.TryGetValue(group, out var idGenerator))
            {
                idGenerator.ResetCounter();
            }
            
        }

        internal static Dictionary<EntityID, Entity> GetEntitiesByGroup(string group)
        {
            return m_EntityByGroup.GetValueOrDefault(group);
        }

        internal static int CheckLeftEntityCount(string group)
        {
            if (m_EntityByGroup.TryGetValue(group, out var gruopDic))
            {
                return gruopDic.Count;
            }

            return 0;
        }
        
    }
}