using UnityEngine;
using Unity.Entities;

namespace BbxCommon
{
    public static class EcsApi
    {
        #region Common
        public static Entity CreateEntity()
        {
            var entity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity();
            var rawComponentGroup = ObjectPool<EcsDataGroup>.Alloc();
            rawComponentGroup.Init(entity);
            EcsDataManager.EntityRawComponentGroup[entity] = rawComponentGroup;
            return entity;
        }

        public static void DestroyEntity(Entity entity)
        {
            EcsDataManager.DestroyEntity(entity);
            World.DefaultGameObjectInjectionWorld?.EntityManager.DestroyEntity(entity);
        }

        public static void AttachEntityToGameObject(Entity entity, GameObject gameObject)
        {
            var goComp = EcsDataManager.AddEcsData<GameObjectRawComponent>(entity);
            goComp.GameObject = gameObject;
        }

        public static T AddSingletonRawComponent<T>() where T : EcsSingletonRawComponent, new()
        {
            return EcsDataManager.AddSingletonRawComponent<T>();
        }

        public static T GetSingletonRawComponent<T>() where T : EcsSingletonRawComponent
        {
            return EcsDataManager.GetSingletonRawComponent<T>();
        }

        public static void RemoveSingletonRawComponent<T>() where T : EcsSingletonRawComponent
        {
            EcsDataManager.RemoveSingletonRawComponent<T>();
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

        internal static T AddEcsData<T>(this Entity entity) where T : EcsData, new()
        {
            return EcsDataManager.AddEcsData<T>(entity);
        }

        public static T AddRawComponent<T>(this Entity entity) where T : EcsRawComponent, new()
        {
            return EcsDataManager.AddEcsData<T>(entity);
        }

        public static bool HasRawComponent<T>(this Entity entity) where T : EcsRawComponent
        {
            return EcsDataManager.HasEcsData<T>(entity);
        }

        public static T GetRawComponent<T>(this Entity entity) where T : EcsRawComponent
        {
            return EcsDataManager.GetEcsData<T>(entity);
        }

        public static void RemoveRawComponent<T>(this Entity entity) where T : EcsRawComponent
        {
            EcsDataManager.RemoveEcsData<T>(entity);
        }

        public static T CreateRawAspect<T>(this Entity entity) where T : EcsRawAspect, new()
        {
            return EcsDataManager.CreateRawAspect<T>(entity);
        }

        public static void RemoveRawAspect<T>(this Entity entity) where T : EcsRawAspect
        {
            EcsDataManager.RemoveRawAspect<T>(entity);
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
