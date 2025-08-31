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

        /// <summary>
        /// Deactive instances will not be visited via <see cref="EcsMixSystemBase.GetEnumerator{T}"/>.
        /// </summary>
        internal bool Active;
        /// <summary>
        /// An <see cref="EcsData"/> requested to be deactivated will be removed from <see cref="EcsDataList{T}.m_EcsDatas"/>
        /// when the enumerator runs, but be added to <see cref="EcsDataList{T}.m_EcsDatas"/> at once be requested. All these
        /// offer better-performed activating and deactivating operations.
        /// </summary>
        internal bool RequestDeactive;

        public Entity GetEntity()
        {
            return Entity;
        }

        protected override void OnAllocate()
        {
            Active = true;
            RequestDeactive = false;
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

        protected override void OnCollect()
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
            RawComponents[ClassTypeId<EcsRawComponent, T>.Id] = comp;
            return comp;
        }

        internal T GetRawComponent<T>() where T : EcsRawComponent
        {
            RefreshCapacity();
            return (T)RawComponents[ClassTypeId<EcsRawComponent, T>.Id];
        }

        internal bool HasRawComponent<T>() where T : EcsRawComponent
        {
            RefreshCapacity();
            return RawComponents[ClassTypeId<EcsRawComponent, T>.Id] != null;
        }

        internal void RemoveRawComponent<T>(out T comp) where T : EcsRawComponent
        {
            RefreshCapacity();
            var removed = RawComponents[ClassTypeId<EcsRawComponent, T>.Id];
            RawComponents[ClassTypeId<EcsRawComponent, T>.Id] = null;
            comp = (T)removed;
        }

        internal void RemoveRawComponent(int typeId, out EcsRawComponent comp)
        {
            RefreshCapacity();
            var removed = RawComponents[typeId];
            RawComponents[typeId] = null;
            comp = removed;
        }

        private void RefreshCapacity()
        {
            if (RawComponents.Count > TypeIdCounter<EcsRawComponent>.CurId)
                return;
            RawComponents.Capacity = TypeIdCounter<EcsRawComponent>.CurId + 1;
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
                UnityEngine.DebugApi.LogWarning("There has been a duplicate EcsRawAspect " + typeof(T).FullName + " in Entity " + Entity + "!");
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

        internal void RemoveRawAspect(int index, out EcsRawAspect aspect)
        {
            aspect = RawAspects[index];
            RawAspects.UnorderedRemoveAt(index);
        }
        #endregion
    }
}
