using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Entities;

namespace BbxCommon
{
    /// <summary>
    /// <para>
    /// Internal interfaces of operating <see cref="Entity"/>. Other public classes call functions in <see cref="EcsDataManager"/> but doesn't implement
    /// them directly, to ensure interfaces' name and parameters remain unchanged.
    /// </para><para>
    /// <see cref="EcsRawComponent"/>s and <see cref="EcsRawAspect"/>s of <see cref="Entity"/>s are all stored here. For <see cref="Entity"/> is just a
    /// struct data which cannot be derrived and directly extended, we create a <see cref="List{T}"/> to  manage its <see cref="EcsRawComponent"/> and
    /// <see cref="EcsRawAspect"/> datas through <see cref="EcsDataGroup"/>.
    /// </para><para>
    /// However, <see cref="EcsDataManager"/> only operates <see cref="EcsRawComponent"/>s and <see cref="EcsRawAspect"/>s of the <see cref="Entity"/>s.
    /// To interact with DOTS <see cref="World"/>, see <see cref="EcsApi"/> functions.
    /// </para>
    /// </summary>
    internal static class EcsDataManager
    {
        #region Common
        /// <summary>
        /// <see cref="Entity"/>s with <see cref="EcsRawComponent"/>s added to it.
        /// </summary>
        private static List<EcsDataGroup> m_EcsDataGroups = new List<EcsDataGroup>(8);
        private static Entity m_SingletonRawComponentEntity;

        internal static void CreateEcsDataGroup(Entity entity, EcsDataGroup dataGroup)
        {
            GetAndRefreshGroup(entity);
            m_EcsDataGroups[entity.Index] = dataGroup;
        }

        internal static void DestroyEntity(Entity entity)
        {
            var group = GetAndRefreshGroup(entity);
            foreach (var comp in group.RawComponents)
            {
                comp.CollectToPool();
            }
            foreach (var aspect in group.RawAspects)
            {
                aspect.CollectToPool();
            }
            group.CollectToPool();
            m_EcsDataGroups[entity.Index] = null;
        }
        #endregion

        #region RawComponent
        internal static T AddRawComponent<T>(Entity entity) where T : EcsRawComponent, new()
        {
            var comp = ObjectPool<T>.Alloc();
            var group = GetAndRefreshGroup(entity);

            group.AddRawComponent(comp);
            comp.Entity = entity;
            EcsDataList<T>.AddEcsData(comp);
            return comp;
        }

        internal static T GetRawComponent<T>(Entity entity) where T : EcsRawComponent
        {
            var group = GetAndRefreshGroup(entity);
            return group.GetRawComponent<T>();
        }

        internal static bool HasRawComponent<T>(Entity entity) where T : EcsRawComponent
        {
            var group = GetAndRefreshGroup(entity);
            return group.HasRawComponent<T>();
        }

        internal static void RemoveRawComponent<T>(Entity entity) where T : EcsRawComponent
        {
            var group = GetAndRefreshGroup(entity);
            group.RemoveRawComponent<T>(out var comp);
            comp.CollectToPool();
        }

        internal static void ForeachRawComponent<T>(UnityAction<T> action) where T : EcsRawComponent
        {
            EcsDataList<T>.ForeachEcsData(action);
        }
        #endregion

        #region SingletonRawComponent
        internal static T GetSingletonRawComponent<T>() where T : EcsSingletonRawComponent
        {
            return EcsDataList<T>.GetSingletonEcsData<T>();
        }

        internal static T AddSingletonRawComponent<T>() where T : EcsSingletonRawComponent, new()
        {
            return AddRawComponent<T>(m_SingletonRawComponentEntity);
        }

        internal static void RemoveSingletonRawComponent<T>() where T : EcsSingletonRawComponent
        {
            var comp = EcsDataList<T>.GetSingletonEcsData<T>();
            RemoveRawComponent<T>(comp.GetEntity());
        }

        internal static void SetSingletonRawComponentEntity(Entity entity)
        {
            if (m_SingletonRawComponentEntity == Entity.Null)
                m_SingletonRawComponentEntity = entity;
            else
                Debug.LogError("SingletonRawComponentEntity can be only set once!");
        }
        #endregion

        #region RawAspect
        internal static T CreateRawAspect<T>(Entity entity) where T : EcsRawAspect, new()
        {
            var group = GetAndRefreshGroup(entity);
            var aspect = ObjectPool<T>.Alloc();
            aspect.Entity = entity;
            group.AddRawAspect(aspect);
            aspect.Create();
            EcsDataList<T>.AddEcsData(aspect);
            return aspect;
        }

        internal static void RemoveRawAspect<T>(Entity entity) where T : EcsRawAspect
        {
            var group = GetAndRefreshGroup(entity);
            group.RemoveRawAspect<T>(out var aspect);
            aspect.CollectToPool();
        }

        internal static void ForeachRawAspect<T>(UnityAction<T> action) where T : EcsRawAspect
        {
            EcsDataList<T>.ForeachEcsData(action);
        }
        #endregion

        #region private
        private static EcsDataGroup GetAndRefreshGroup(Entity entity)
        {
            if (m_EcsDataGroups.Count > entity.Index)  // there is some overhead when branch prediction misses
                return m_EcsDataGroups[entity.Index];
            else
            {
                while (m_EcsDataGroups.Capacity <= entity.Index)
                    m_EcsDataGroups.Capacity = (int)(m_EcsDataGroups.Capacity * 1.5f);
                for (int i = m_EcsDataGroups.Count; i < m_EcsDataGroups.Capacity; i++)
                    m_EcsDataGroups.Add(null);
                return m_EcsDataGroups[entity.Index];
            }
        }
        #endregion
    }

    /// <summary>
    /// <para>
    /// Store <see cref="EcsRawComponent"/>s separately in List to improve iterator performance.
    /// For example using in <see cref="ForeachEcsData{T}(UnityAction{T})"/>.
    /// </para><para>
    /// Using a generic type to reduce one-time reflection overhead and improve performance.
    /// </para>
    /// </summary>
    internal static class EcsDataList<T> where T : EcsData
    {
        #region Common
        private static List<ObjRef<T>> m_EcsDatas = new();
        private static List<int> m_DeletedDatas = new();    // data's index

        internal static void AddEcsData(T data)
        {
            if (data is IEcsSingletonData && m_EcsDatas.Count > 0)
            {
                Debug.LogError("You are creating a duplicated EcsSingletonRawComponent " + typeof(T).FullName + "! The operation is invalid!");
                return;
            }
            m_EcsDatas.Add(data.AsObjRef());
            data.Index = m_EcsDatas.Count - 1;
        }

        internal static TSingleton GetSingletonEcsData<TSingleton>() where TSingleton : EcsData, IEcsSingletonData
        {
            if (m_EcsDatas.Count > 0)
                return m_EcsDatas[0].Obj as TSingleton;
            return null;
        }

        internal static void ForeachEcsData(UnityAction<T> action)
        {
            for (int i = 0; i < m_EcsDatas.Count; i++)
            {
                var data = m_EcsDatas[i];
                if (data.Obj != null)
                    action(data.Obj);
                else
                    m_DeletedDatas.Add(i);
            }
            RemoveDeletedDatas();
        }
        #endregion

        #region private
        private static void RemoveEcsData(int index)
        {
            m_EcsDatas.UnorderedRemove(index);
            if (m_EcsDatas.Count > 0 && index < m_EcsDatas.Count)
                m_EcsDatas[index].Obj.Index = index;     // swap the last one to the removed slot, then set its index as new
        }

        /// <summary>
        /// <para>
        /// The full steps of removing a <see cref="EcsData"/> are as follow:
        /// </para><para>
        /// 1. Sign a <see cref="EcsData"/> as deleted via either <see cref="EcsDataManager.RemoveRawComponent{T}(Entity)"/> or <see cref="EcsDataManager.DestroyEntity(Entity)"/>.
        /// </para><para>
        /// 2. The <see cref="EcsData"/> then will be removed in <see cref="EcsDataGroup"/>, and be collected to <see cref="ObjectPool{T}"/>.
        /// </para><para>
        /// 3. For <see cref="EcsDataList{T}"/> holds <see cref="ObjRef{T}"/>, once it detects null reference, it removes it from the list.
        /// </para>
        /// </summary>
        private static void RemoveDeletedDatas()
        {
            m_DeletedDatas.Sort();
            for (int i = m_DeletedDatas.Count - 1; i >= 0; i--)
            {
                RemoveEcsData(i);
            }
            m_DeletedDatas.Clear();
        }
        #endregion
    }
}
