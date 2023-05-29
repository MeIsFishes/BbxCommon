//#define ASPECT_DUPLICATE_CHECK

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

    /// <summary>
    /// A collection to store <see cref="EcsRawComponent"/>s and <see cref="EcsRawAspect"/>s of an <see cref="Unity.Entities.Entity"/>.
    /// </summary>
    internal class EcsDataGroup : PooledObject
    {
        #region Common
        internal Entity Entity { get; private set; }
        internal List<EcsRawComponent> RawComponents = new List<EcsRawComponent>(8);
        internal List<EcsRawAspect> RawAspects = new(); // generally, aspects are less to be added, removed and got

        internal void Init(Entity entity)
        {
            Entity = entity;
        }

        public override void OnCollect()
        {
            base.OnCollect();
            Entity = Entity.Null;
            RawComponents.Clear();
            RawAspects.Clear();
        }
        #endregion

        #region RawComponent
        internal T AddRawComponent<T>(T comp) where T : EcsRawComponent, new()
        {
            RefreshCapacity();
            RawComponents[EcsRawComponentId<T>.Id] = comp;
            return comp;
        }

        internal T GetRawComponent<T>() where T : EcsRawComponent
        {
            RefreshCapacity();
            return (T)RawComponents[EcsRawComponentId<T>.Id];
        }

        internal bool HasRawComponent<T>() where T : EcsRawComponent
        {
            RefreshCapacity();
            return RawComponents[EcsRawComponentId<T>.Id] != null;
        }

        internal void RemoveRawComponent<T>(out T comp) where T : EcsRawComponent
        {
            RefreshCapacity();
            var removed = RawComponents[EcsRawComponentId<T>.Id];
            RawComponents[EcsRawComponentId<T>.Id] = null;
            comp = (T)removed;
        }

        private void RefreshCapacity()
        {
            if (RawComponents.Count > EcsRawComponentId.CurId)
                return;
            RawComponents.Capacity = EcsRawComponentId.CurId + 1;
            for (int i = RawComponents.Count; i < RawComponents.Capacity; i++)
                RawComponents.Add(null);
        }
        #endregion

        #region RawAspect
        internal T AddRawAspect<T>(T aspect) where T : EcsRawAspect, new()
        {
#if ASPECT_DUPLICATE_CHECK
            if (HasRawAspect<T>())
            {
                UnityEngine.Debug.LogWarning("There has been a duplicate EcsRawAspect " + typeof(T).FullName + " in Entity " + Entity + "!");
                return aspect;
            }
#endif
            RawAspects.Add(aspect);
            return aspect;
        }

        internal T GetRawAspect<T>() where T : EcsRawAspect
        {
            foreach (var element in RawAspects)
            {
                if (element is T)
                    return (T)element;
            }
            return null;
        }

        internal bool HasRawAspect<T>() where T : EcsRawAspect
        {
            foreach (var element in RawAspects)
            {
                if (element is T)
                    return true;
            }
            return false;
        }

        internal void RemoveRawAspect<T>(out T aspect) where T : EcsRawAspect
        {
            for (int i = 0; i < RawAspects.Count; i++)
            {
                if (RawAspects[i] is T)
                {
                    aspect = (T)RawAspects[i];
                    RawAspects.UnorderedRemoveAt(i);
                    return;
                }
            }
            aspect = null;
        }
        #endregion
    }
}
