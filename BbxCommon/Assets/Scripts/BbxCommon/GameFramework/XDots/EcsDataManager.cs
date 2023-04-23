using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Entities;

namespace BbxCommon
{
    /// <summary>
    /// <para>
    /// <see cref="EcsRawComponent"/>s and <see cref="EcsRawAspect"/> of <see cref="Entity"/>s are all stored here. For <see cref="Entity"/> is just a
    /// struct data which cannot be directly extended, we create a <see cref="Dictionary{TKey, TValue}"/> to  manage its <see cref="EcsRawComponent"/>
    /// and <see cref="EcsRawAspect"/> datas.
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
        internal static Dictionary<Entity, EcsDataGroup> EntityRawComponentGroup = new();

        private static Entity m_SingletonRawComponentEntity;

        public static void DestroyEntity(Entity entity)
        {
            var group = GetGroupAndRefreshHot(entity);
            foreach (var compPair in group.EcsDatas)
            {
                compPair.Value.Entity = Entity.Null;
            }
            group.CollectToPool();
        }
        #endregion

        #region RawComponent
        public static T AddRawComponent<T>(Entity entity) where T : EcsRawComponent, new()
        {
            var comp = ObjectPool<T>.Alloc();
            var group = GetGroupAndRefreshHot(entity);

            group.AddRawComponent(comp);
            comp.Entity = entity;
            EcsDataList<T>.AddEcsData(comp);
            return comp;
        }

        public static T GetRawComponent<T>(Entity entity) where T : EcsRawComponent
        {
            var group = GetGroupAndRefreshHot(entity);
            return group.GetRawComponent<T>();
        }

        public static bool HasRawComponent<T>(Entity entity) where T : EcsRawComponent
        {
            var group = GetGroupAndRefreshHot(entity);
            return group.HasRawComponent<T>();
        }

        public static void RemoveRawComponent<T>(Entity entity) where T : EcsRawComponent
        {
            var group = GetGroupAndRefreshHot(entity);
            group.RemoveRawComponent<T>(out var comp);
            EcsDataList<T>.RemoveEcsData(comp);
            comp.CollectToPool();
        }

        public static void ForeachRawComponent<T>(UnityAction<T> action) where T : EcsRawComponent
        {
            EcsDataList<T>.ForeachEcsData(action);
        }
        #endregion

        #region SingletonRawComponent
        public static T GetSingletonRawComponent<T>() where T : EcsSingletonRawComponent
        {
            return EcsDataList<T>.GetSingletonEcsData<T>();
        }

        public static T AddSingletonRawComponent<T>() where T : EcsSingletonRawComponent, new()
        {
            return AddRawComponent<T>(m_SingletonRawComponentEntity);
        }

        public static void RemoveSingletonRawComponent<T>() where T : EcsSingletonRawComponent
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
        public static T CreateRawAspect<T>(Entity entity) where T : EcsRawAspect, new()
        {
            var aspect = AddRawComponent<T>(entity);
            aspect.Create();
            return aspect;
        }

        public static void RemoveRawAspect<T>(Entity entity) where T : EcsRawAspect
        {
            RemoveRawComponent<T>(entity);
        }
        #endregion

        #region private
        // Generally, user may operate a single entity several times. If so, storing a hot data can reduce one time hash calculation.
        private static EcsDataGroup m_HotGroup = new EcsDataGroup();
        private static EcsDataGroup GetGroupAndRefreshHot(Entity entity)
        {
            if (m_HotGroup.Entity != entity)
                m_HotGroup = EntityRawComponentGroup[entity];
            return m_HotGroup;
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
        private static List<EcsData> m_EcsDatas = new();
        private static List<EcsData> m_DeletedDatas = new();

        internal static void AddEcsData(T data)
        {
            if (data is IEcsSingletonData && m_EcsDatas.Count > 0)
            {
                Debug.LogError("You are creating a duplicated EcsSingletonRawComponent " + typeof(T).FullName + "! The operation is invalid!");
                return;
            }
            m_EcsDatas.Add(data);
            data.Index = m_EcsDatas.Count - 1;
        }

        internal static void RemoveEcsData(T data)
        {
            m_EcsDatas.UnorderedRemove(data.Index);
            if (m_EcsDatas.Count > 0)
                m_EcsDatas[data.Index].Index = data.Index;     // swap the last one to the removed slot, then set its index as new
        }

        internal static TSingleton GetSingletonEcsData<TSingleton>() where TSingleton : EcsData, IEcsSingletonData
        {
            return (TSingleton)m_EcsDatas[0];
        }

        internal static void ForeachEcsData(UnityAction<T> action)
        {
            foreach (var data in m_EcsDatas)
            {
                if (data.Entity != Entity.Null)
                    action((T)data);
                else
                    m_DeletedDatas.Add(data);
            }
            RemoveDeletedDatas();
        }
        #endregion

        #region private
        /// <summary>
        /// <para>
        /// The full steps of removing a <see cref="EcsData"/> are as follow:
        /// </para><para>
        /// 1. Sign a <see cref="EcsData"/> as deleted via either <see cref="EcsDataManager.RemoveRawComponent{T}(Entity)"/> or <see cref="EcsDataManager.DestroyEntity(Entity)"/>.
        /// </para><para>
        /// 2. The <see cref="EcsData"/> then has been removed in <see cref="EcsDataGroup"/>, and its reference, <see cref="EcsData.Entity"/> has been set as <see cref="Entity.Null"/>.
        /// </para><para>
        /// 3. Those <see cref="EcsData"/>s has been signed as deleted will be found out when you call <see cref="ForeachEcsData(UnityAction{T})"/>, then references of them in
        /// <see cref="EcsDataList{T}"/> will be released, and they will be collected by <see cref="ObjectPool{T}"/> at last.
        /// </para>
        /// </summary>
        private static void RemoveDeletedDatas()
        {
            foreach (var data in m_DeletedDatas)
            {
                RemoveEcsData((T)data);
                data.CollectToPool();
            }
            m_DeletedDatas.Clear();
        }
        #endregion
    }
}
