using System;
using System.Collections.Generic;
using Unity.Entities;

namespace BbxCommon.Framework
{
    public abstract class EcsRawComponent : PooledObject
    {
        internal Entity Entity;
        /// <summary>
        /// Index of current component in the <see cref="EcsWorld.RawComponentLists"/>.
        /// </summary>
        internal int Index;

        internal void InitRawComponent(Entity entity, int index)
        {
            Entity = entity;
            Index = index;
        }

        public Entity GetEntity()
        {
            return Entity;
        }
    }

    public abstract class EcsSingletonRawComponent : EcsRawComponent
    {
        
    }

    internal class RawComponentGroup : PooledObject
    {
        public Entity Entity { get; private set; }
        private Dictionary<Type, EcsRawComponent> m_RawComponents = new Dictionary<Type, EcsRawComponent>();

        public void Init(Entity entity)
        {
            Entity = entity;
        }

        public T AddRawComponent<T>(T comp) where T : EcsRawComponent, new()
        {
            m_RawComponents.Add(typeof(T), comp);
            return comp;
        }

        public T GetRawComponent<T>() where T : EcsRawComponent
        {
            m_RawComponents.TryGetValue(typeof(T), out var comp);
            return (T)comp;
        }

        public bool HasRawComponent<T>() where T : EcsRawComponent
        {
            return m_RawComponents.ContainsKey(typeof(T));
        }

        public void RemoveRawComponent<T>(out T comp) where T : EcsRawComponent
        {
            m_RawComponents.Remove(typeof(T), out var removed);
            comp = (T)removed;
        }

        public override void OnCollect()
        {
            base.OnCollect();
            Entity = Entity.Null;
            m_RawComponents.Clear();
        }
    }
}
