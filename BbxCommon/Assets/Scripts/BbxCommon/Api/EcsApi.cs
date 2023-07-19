using System.Collections.Generic;
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
            var ecsDataGroup = ObjectPool<EcsDataGroup>.Alloc();
            ecsDataGroup.Init(entity);
            EcsDataManager.CreateEcsDataGroup(entity, ecsDataGroup);
            return entity;
        }

        public static void DestroyEntity(Entity entity)
        {
            EcsDataManager.DestroyEntity(entity);
            World.DefaultGameObjectInjectionWorld?.EntityManager.DestroyEntity(entity);
        }

        public static void AttachEntityToGameObject(Entity entity, GameObject gameObject)
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

        /// <summary>
        /// Set the <see cref="EcsRawComponent"/> active. Only active <see cref="EcsRawComponent"/>s can be
        /// visited via <see cref="EcsMixSystemBase.GetEnumerator{T}"/>.
        /// </summary>
        public static void ActivateRawComponent<T>(T comp) where T : EcsRawComponent
        {
            if (comp.Active)
                return;
            EcsDataList<T>.AddEcsData(comp);
            comp.Active = true;
            comp.RequestDeactive = false;
        }

        /// <summary>
        /// Set the <see cref="EcsRawComponent"/> deactive. Deactive <see cref="EcsRawComponent"/>s will be
        /// ignored by <see cref="EcsMixSystemBase.GetEnumerator{T}"/>.
        /// </summary>
        public static void DeactivateRawComponent<T>(T comp) where T : EcsRawComponent
        {
            comp.RequestDeactive = true;
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

        #region RawComponent Extend
        /// <summary>
        /// Set the <see cref="EcsRawComponent"/> active. Only active <see cref="EcsRawComponent"/>s can be
        /// visited via <see cref="EcsMixSystemBase.GetEnumerator{T}"/>.
        /// </summary>
        public static void Activate<T>(this T comp) where T : EcsRawComponent
        {
            ActivateRawComponent(comp);
        }

        /// <summary>
        /// Set the <see cref="EcsRawComponent"/> deactive. Deactive <see cref="EcsRawComponent"/>s will be
        /// ignored by <see cref="EcsMixSystemBase.GetEnumerator{T}"/>.
        /// </summary>
        public static void Deactivate<T>(this T comp) where T : EcsRawComponent
        {
            DeactivateRawComponent(comp);
        }
        #endregion
    }
}
