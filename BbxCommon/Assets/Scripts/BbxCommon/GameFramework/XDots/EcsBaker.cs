using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace BbxCommon.Framework
{
    public class EcsBaker : MonoBehaviour
    {
        public Entity Entity;
        [Tooltip("If true, EcsBaker will destroy the entity when itself is destroyed.")]
        public bool DestroyEntity = true;

        // ensure creating aspects after components
        private List<EcsRawAspect> m_Aspects;

        private void Awake()
        {
            Entity = EcsApi.CreateEntity();
            m_Aspects = SimplePool<List<EcsRawAspect>>.Alloc();
            InternalBake();
            InternalCreateAspect();
            m_Aspects.CollectToPool();
        }

        private void OnDestroy()
        {
            if (DestroyEntity)
                Entity.Destroy();
        }

        protected virtual void Bake() { }

        private void InternalBake()
        {
            Entity.AttachToGameObject(gameObject);
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
            var aspect = AddRawComponent<T>();
            m_Aspects.Add(aspect);
        }

        private void InternalCreateAspect()
        {
            foreach (var aspect in m_Aspects)
            {
                aspect.Create();
            }
        }
    }
}
