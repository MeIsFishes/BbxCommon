using UnityEngine;
using Unity.Entities;

namespace BbxCommon.Framework
{
    public class EcsBaker : MonoBehaviour
    {
        public Entity Entity;

        private void Awake()
        {
            Entity = EcsApi.CreateEntity();
            InternalBake();
            Bake();
        }

        protected virtual void Bake() { }

        private void InternalBake()
        {
            var goComp = AddRawComponent<GameObjectRawComponent>();
            goComp.GameObject = gameObject;
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
    }
}
