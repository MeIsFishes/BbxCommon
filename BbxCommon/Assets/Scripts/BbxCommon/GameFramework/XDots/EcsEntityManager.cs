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
        //本地的entityID从0xFFFFFFFF处开始，主机传过来的entityID从1开始
        private static UniqueIdGenerator m_IdGenerator = new UniqueIdGenerator(0xFFFFFFFF);
        private static Dictionary<EntityID, Entity> m_DictionaryEntitys = new Dictionary<EntityID, Entity>();
        
        internal static Entity CreateEntity(EntityID entityID)
        {
            if (m_DictionaryEntitys.TryGetValue(entityID, out var entity))
            {
                Debug.LogError("entityID conflict!!!");
                return Entity.Null;
            }
            
            entity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity();
            if (entityID == EntityID.INVALID)
            {
                entityID = m_IdGenerator.GenerateId();
            }
            
            var ecsDataGroup = ObjectPool<EcsDataGroup>.Alloc();
            ecsDataGroup.Init(entity);
            EcsDataManager.CreateEcsDataGroup(entity, ecsDataGroup);
            m_DictionaryEntitys[entityID] = entity;
            
            var entityIDComp = entity.AddRawComponent<EntityIDComponent>();
            entityIDComp.EntityUniqueID = entityID;
            return entity;
        }

        internal static bool GetEntityByID(EntityID uniqueId, out Entity entity)
        {
            return m_DictionaryEntitys.TryGetValue(uniqueId, out entity);
        }
        
        internal static void DestroyEntity(EntityID entityID)
        {
            if (entityID == EntityID.INVALID)
            {
                return;
            }
            
            if (GetEntityByID(entityID, out var entity))
            {
                entity.ClearHud();
                entity.GetGameObject().Destroy();
                EcsDataManager.DestroyEntity(entityID);
                World.DefaultGameObjectInjectionWorld?.EntityManager.DestroyEntity(entity);
                m_DictionaryEntitys.Remove(entityID);
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
            m_DictionaryEntitys.Remove(entityID);
        }
        
    }
}