using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Unity.Entities;
using BbxCommon.Ui;
using Debug = UnityEngine.Debug;

namespace BbxCommon
{
    public static class EcsApi
    {
        #region Common
        public static Entity CreateEntity(string group = "", EntityID entityID = new EntityID())
        {
            return EcsEntityManager.CreateEntity(entityID, group);
            //EcsEntityManager.GetEntityByID(EcsEntityManager.CreateEntity(), out var entity);
            //return entity;
            // var entity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity();
            // var ecsDataGroup = ObjectPool<EcsDataGroup>.Alloc();
            // ecsDataGroup.Init(entity);
            // EcsDataManager.CreateEcsDataGroup(entity, ecsDataGroup);
            // return entity;
            
        }
        
        public static void DestroyEntity(Entity entity)
        {
            EcsEntityManager.DestroyEntity(entity);
            // entity.ClearHud();
            // entity.GetGameObject().Destroy();
            // EcsDataManager.DestroyEntity(entity);
            // World.DefaultGameObjectInjectionWorld?.EntityManager.DestroyEntity(entity);
        }
        
        public static bool GetEntityByID(EntityID uniqueId, out Entity entity)
        {
            return EcsEntityManager.GetEntityByID(uniqueId, out entity);
        }
        
        public static void DestroyEntity(EntityID entityID)
        {
            EcsEntityManager.DestroyEntity(entityID);
        }


        public static void BindEntityWithGameObject(Entity entity, GameObject gameObject)
        {
            var goComp = EcsDataManager.AddRawComponent<GameObjectRawComponent>(entity);
            goComp.GameObject = gameObject;
        }

        /// <summary>
        /// Add a <see cref="EcsSingletonRawComponent"/> to a free access <see cref="Entity"/>.
        /// </summary>
        public static T AddSingletonRawComponent<T>() where T : EcsSingletonRawComponent, new()
        {
            return EcsDataManager.AddSingletonRawComponent<T>();
        }

        public static T GetSingletonRawComponent<T>() where T : EcsSingletonRawComponent
        {
            return EcsDataManager.GetSingletonRawComponent<T>();
        }

        /// <summary>
        /// Once getting the <see cref="EcsSingletonRawComponent"/>, remove it from its host.
        /// </summary>
        public static void RemoveSingletonRawComponent<T>() where T : EcsSingletonRawComponent
        {
            EcsDataManager.RemoveSingletonRawComponent<T>();
        }
        #endregion

        #region Entity Extend

        #region Component
        public static void AddComponent<T>(this Entity entity) where T : unmanaged, IComponentData
        {
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<T>(entity);
        }

        public static void AddComponent<T>(this Entity entity, T componentData) where T : unmanaged, IComponentData
        {
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<T>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(entity, componentData);
        }

        public static bool HasComponent<T>(this Entity entity) where T : unmanaged, IComponentData
        {
            return World.DefaultGameObjectInjectionWorld.EntityManager.HasComponent<T>(entity);
        }

        public static void RemoveComponent<T>(this Entity entity) where T : unmanaged, IComponentData
        {
            World.DefaultGameObjectInjectionWorld.EntityManager.RemoveComponent<T>(entity);
        }
        #endregion

        #region RawComponent
        public static T AddRawComponent<T>(this Entity entity) where T : EcsRawComponent, new()
        {
            return EcsDataManager.AddRawComponent<T>(entity);
        }

        public static bool HasRawComponent<T>(this Entity entity) where T : EcsRawComponent
        {
            return EcsDataManager.HasRawComponent<T>(entity);
        }

        public static T GetRawComponent<T>(this Entity entity) where T : EcsRawComponent
        {
            return EcsDataManager.GetRawComponent<T>(entity);
        }

        public static void RemoveRawComponent<T>(this Entity entity) where T : EcsRawComponent
        {
            EcsDataManager.RemoveRawComponent<T>(entity);
        }

        public static void ActivateRawComponent<T>(this Entity entity) where T : EcsRawComponent
        {
            var comp = EcsDataManager.GetRawComponent<T>(entity);
            EcsDataManager.ActivateEcsData(comp);
        }

        public static void DeactiveRawComponent<T>(this Entity entity) where T : EcsRawComponent
        {
            var comp = EcsDataManager.GetRawComponent<T>(entity);
            EcsDataManager.DeactivateEcsData(comp);
        }

        public static IEnumerable<T> GetEnumerator<T>() where T : EcsData
        {
            return EcsDataList<T>.GetEnumerator();
        }
        #endregion

        #region RawAspect
        public static T CreateRawAspect<T>(this Entity entity) where T : EcsRawAspect, new()
        {
            return EcsDataManager.CreateRawAspect<T>(entity);
        }

        public static void RemoveRawAspect<T>(this Entity entity) where T : EcsRawAspect
        {
            EcsDataManager.RemoveRawAspect<T>(entity);
        }

        public static void BindGameObject(this Entity entity, GameObject gameObject)
        {
            BindEntityWithGameObject(entity, gameObject);
        }

        public static void ActivateRawAspect<T>(this Entity entity) where T : EcsRawAspect
        {
            var aspect = EcsDataManager.GetRawAspect<T>(entity);
            EcsDataManager.ActivateEcsData(aspect);
        }

        public static void DeactiveRawAspect<T>(this Entity entity) where T : EcsRawAspect
        {
            var aspect = EcsDataManager.GetRawAspect<T>(entity);
            EcsDataManager.DeactivateEcsData(aspect);
        }
        #endregion

        #region Common
        public static GameObject GetGameObject(this Entity entity)
        {
            var goComp = entity.GetRawComponent<GameObjectRawComponent>();
            if (goComp == null)
                return null;
            return goComp.GameObject;
        }

        public static EntityID GetUniqueId(this Entity entity)
        {
            var entityIDComp = entity.GetRawComponent<EntityIDComponent>();
            if (entityIDComp != null)
            {
                return entityIDComp.EntityUniqueID;
            }
            
            return EntityID.INVALID;
        }

        public static Entity GetEntity(this EntityID entityID)
        {
            if (EcsEntityManager.GetEntityByID(entityID, out var entity))
            {
                return entity;
            }
            return Entity.Null;
        }

        public static void ResetEntitiesByGroup(string group)
        {
            EcsEntityManager.ResetEntitiesByGroup(group);
        }
        
        public static Dictionary<EntityID, Entity> GetEntitiesByGroup(string group)
        {
            return EcsEntityManager.GetEntitiesByGroup(group);
        }


        public static void Destroy(this Entity entity)
        {
            DestroyEntity(entity);
        }
        #endregion

        #endregion

        #region EcsData Extend
        public static void Activate<T>(this T data) where T : EcsData
        {
            EcsDataManager.ActivateEcsData(data);
        }

        public static void Deactivate<T>(this T data) where T : EcsData
        {
            EcsDataManager.DeactivateEcsData(data);
        }
        #endregion
    }
}
