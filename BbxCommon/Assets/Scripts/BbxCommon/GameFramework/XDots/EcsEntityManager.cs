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
        private static Dictionary<string, Entity> m_DictionaryEntitys = new Dictionary<string, Entity>();
        
        internal static string CreateEntity(out Entity entity, string methodName)
        {
            entity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity();
            var uniqueId =  methodName +  m_IdGenerator.GenerateId();
            var ecsDataGroup = ObjectPool<EcsDataGroup>.Alloc();
            ecsDataGroup.Init(entity);
            EcsDataManager.CreateEcsDataGroup(entity, ecsDataGroup);
            m_DictionaryEntitys[uniqueId] = entity;
            
            var entityIDComp = entity.AddRawComponent<EntityIDComponent>();
            entityIDComp.EntityUniqueID = uniqueId;
            return uniqueId;
        }

        internal static bool GetEntityByID(string uniqueId, out Entity entity)
        {
            return m_DictionaryEntitys.TryGetValue(uniqueId, out entity);
        }
        
        internal static void DestroyEntity(string entityID)
        {
            if (GetEntityByID(entityID, out var entity))
            {
                entity.ClearHud();
                entity.GetGameObject().Destroy();
                EcsDataManager.DestroyEntity(entityID);
                World.DefaultGameObjectInjectionWorld?.EntityManager.DestroyEntity(entity);
                m_DictionaryEntitys.Remove(entityID);
            }
        }
        
    }
}