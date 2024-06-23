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
        private static UniqueIdGenerator m_IdGenerator = new UniqueIdGenerator();
        private static Dictionary<EntityID, Entity> m_DictionaryEntitys = new Dictionary<EntityID, Entity>();
        
        internal static EntityID CreateEntity(out Entity entity)
        {
            entity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity();
            var uniqueId =  m_IdGenerator.GenerateId();
            var ecsDataGroup = ObjectPool<EcsDataGroup>.Alloc();
            ecsDataGroup.Init(entity);
            EcsDataManager.CreateEcsDataGroup(entity, ecsDataGroup);
            m_DictionaryEntitys[uniqueId] = entity;
            
            var entityIDComp = entity.AddRawComponent<EntityIDComponent>();
            entityIDComp.EntityUniqueID = uniqueId;
            return uniqueId;
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
            var entityID = entity.GetUniqueID();
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