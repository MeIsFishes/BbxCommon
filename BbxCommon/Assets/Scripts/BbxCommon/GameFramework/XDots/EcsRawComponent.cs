using System;
using System.Collections.Generic;
using Unity.Entities;

namespace BbxCommon
{
    public abstract class EcsData : PooledObject
    {
        internal Entity Entity;
        /// <summary>
        /// Index of current component in the <see cref="EcsDataList{T}"/>.
        /// </summary>
        internal int Index;

        public Entity GetEntity()
        {
            return Entity;
        }
    }

    internal interface IEcsSingletonData { }

    public abstract class EcsRawComponent : EcsData
    {
        
    }

    public abstract class EcsSingletonRawComponent : EcsRawComponent, IEcsSingletonData
    {
        
    }

    internal class EcsDataGroup : PooledObject
    {
        public Entity Entity { get; private set; }
        internal Dictionary<Type, EcsData> EcsDatas = new Dictionary<Type, EcsData>();

        public void Init(Entity entity)
        {
            Entity = entity;
        }

        public T AddRawComponent<T>(T comp) where T : EcsRawComponent, new()
        {
            EcsDatas.Add(typeof(T), comp);
            return comp;
        }

        public T GetRawComponent<T>() where T : EcsRawComponent
        {
            EcsDatas.TryGetValue(typeof(T), out var comp);
            return (T)comp;
        }

        public bool HasRawComponent<T>() where T : EcsRawComponent
        {
            return EcsDatas.ContainsKey(typeof(T));
        }

        public void RemoveRawComponent<T>(out T comp) where T : EcsRawComponent
        {
            EcsDatas.Remove(typeof(T), out var removed);
            comp = (T)removed;
        }

        public override void OnCollect()
        {
            base.OnCollect();
            Entity = Entity.Null;
            EcsDatas.Clear();
        }
    }
}
