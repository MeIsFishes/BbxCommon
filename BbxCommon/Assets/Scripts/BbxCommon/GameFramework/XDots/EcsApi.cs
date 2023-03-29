using UnityEngine;
using Unity.Entities;

namespace BbxCommon.Framework
{
    public static class EcsApi
    {
        #region Common
        public static Entity CreateEntity()
        {
            var entity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity();
            var rawComponentGroup = ObjectPool<RawComponentGroup>.Alloc();
            rawComponentGroup.Init(entity);
            RawComponentManager.EntityRawComponentGroup[entity] = rawComponentGroup;
            return entity;
        }

        public static void DestroyEntity(Entity entity)
        {
            RawComponentManager.DestroyEntity(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(entity);
        }

        public static void AttachEntityToGameObject(Entity entity, GameObject gameObject)
        {
            var goComp = RawComponentManager.AddRawComponent<GameObjectRawComponent>(entity);
            goComp.GameObject = gameObject;
        }

        public static T GetSingletonRawComponent<T>() where T : EcsSingletonRawComponent
        {
            return RawComponentManager.GetSingletonRawComponent<T>();
        }
        #endregion

        #region Entity Extend
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

        public static T AddRawComponent<T>(this Entity entity) where T : EcsRawComponent, new()
        {
            return RawComponentManager.AddRawComponent<T>(entity);
        }

        public static bool HasRawComponent<T>(this Entity entity) where T : EcsRawComponent
        {
            return RawComponentManager.HasRawComponent<T>(entity);
        }

        public static T GetRawComponent<T>(this Entity entity) where T : EcsRawComponent
        {
            return RawComponentManager.GetRawComponent<T>(entity);
        }

        public static void RemoveRawComponent<T>(this Entity entity) where T : EcsRawComponent
        {
            RawComponentManager.RemoveRawComponent<T>(entity);
        }

        public static void AttachToGameObject(this Entity entity, GameObject gameObject)
        {
            AttachEntityToGameObject(entity, gameObject);
        }

        public static GameObject GetGameObject(this Entity entity)
        {
            var goComp = entity.GetRawComponent<GameObjectRawComponent>();
            if (goComp == null)
                return null;
            return goComp.GameObject;
        }

        public static void Destroy(this Entity entity)
        {
            DestroyEntity(entity);
        }
        #endregion
    }
}
