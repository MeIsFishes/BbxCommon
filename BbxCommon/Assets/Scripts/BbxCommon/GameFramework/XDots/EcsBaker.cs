using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace BbxCommon
{
    public abstract class EcsBaker : MonoBehaviour
    {
        public Entity Entity;
        [Tooltip("If true, EcsBaker will destroy the entity when itself is destroyed.")]
        public bool DestroyEntity = true;

        private void Awake()
        {
            EcsApi.CreateEntity(out var uniqueId, out Entity);
            //Entity = EcsApi.CreateEntity();
            InternalBake();
        }

        private void OnDestroy()
        {
            if (DestroyEntity)
                Entity.Destroy();
        }

        protected abstract void Bake();

        private void InternalBake()
        {
            Entity.BindGameObject(gameObject);
            Bake();
        }

        protected void AddComponent<T>() where T : unmanaged, IComponentData
        {
            Entity.AddComponent<T>();
        }

        protected void AddComponent<T>(T componentData) where T : unmanaged, IComponentData
        {
            Entity.AddComponent(componentData);
        }

        protected T AddRawComponent<T>() where T : EcsRawComponent, new()
        {
            return Entity.AddRawComponent<T>();
        }

        protected void CreateRawAspect<T>() where T : EcsRawAspect, new()
        {
            Entity.CreateRawAspect<T>();
        }
    }
}
