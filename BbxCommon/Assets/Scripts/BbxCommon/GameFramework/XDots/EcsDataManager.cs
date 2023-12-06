using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Entities;
using System.Collections;
using Codice.Client.BaseCommands.BranchExplorer;
using System;

namespace BbxCommon
{
    /// <summary>
    /// <para>
    /// Offering internal interfaces of operating <see cref="Entity"/> but doesn't implement them directly, keeping interfaces clear and unchanged.
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

        /// <summary>
        /// Destroy <see cref="Entity"/> and remove its <see cref="EcsData"/>s.
        /// For how to remove <see cref="EcsData"/>s, see <see cref="EcsDataList{T}.RemoveDeletedDatas"/>.
        /// </summary>
        internal static void DestroyEntity(Entity entity)
        {
            var group = GetAndRefreshGroup(entity);
            for (int i = 0; i < group.RawComponents.Count; i++)
            {
                if (group.RawComponents[i] != null)
                {
                    group.RemoveRawComponent(i, out var comp);
                    comp.CollectToPool();
                }
            }
            for (int i = group.RawAspects.Count - 1; i >= 0; i--)
            {
                group.RemoveRawAspect(i, out var aspect);
                aspect.CollectToPool();
            }
            group.CollectToPool();
            m_EcsDataGroups[entity.Index] = null;
        }

        /// <summary>
        /// Set the <see cref="EcsData"/> active. Only active <see cref="EcsData"/>s can be
        /// visited via <see cref="EcsMixSystemBase.GetEnumerator{T}"/>.
        /// </summary>
        internal static void ActivateEcsData<T>(T data) where T : EcsData
        {
            if (data == null || data.Active)
                return;
            EcsDataList<T>.AddEcsData(data);
            data.Active = true;
            data.RequestDeactive = false;
        }

        /// <summary>
        /// Set the <see cref="EcsData"/> deactive. Deactive <see cref="EcsData"/>s will be
        /// ignored by <see cref="EcsMixSystemBase.GetEnumerator{T}"/>.
        /// </summary>
        internal static void DeactivateEcsData<T>(T data) where T : EcsData
        {
            data.RequestDeactive = true;
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
            if (group == null)
                return null;
            return group.GetRawComponent<T>();
        }

        internal static bool HasRawComponent<T>(Entity entity) where T : EcsRawComponent
        {
            var group = GetAndRefreshGroup(entity);
            if (group == null)
                return false;
            return group.HasRawComponent<T>();
        }

        internal static void RemoveRawComponent<T>(Entity entity) where T : EcsRawComponent
        {
            var group = GetAndRefreshGroup(entity);
            if (group == null)
                return;
            group.RemoveRawComponent<T>(out var comp);
            comp.CollectToPool();
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

        internal static T GetRawAspect<T>(Entity entity) where T : EcsRawAspect
        {
            var group = GetAndRefreshGroup(entity);
            if (group == null)
                return null;
            return group.GetRawAspect<T>();
        }

        internal static void RemoveRawAspect<T>(Entity entity) where T : EcsRawAspect
        {
            var group = GetAndRefreshGroup(entity);
            group.RemoveRawAspect<T>(out var aspect);
            aspect.CollectToPool();
        }
        #endregion

        #region private
        private static EcsDataGroup GetAndRefreshGroup(Entity entity)
        {
            if (m_EcsDataGroups.Count > entity.Index)  // there is some overhead when branch prediction misses
                return m_EcsDataGroups[entity.Index];
            else
            {
                m_EcsDataGroups.ModifyCount(entity.Index + 1);
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
        private static List<ObjRef<T>> m_EcsDatas = new();  // active datas
        private static List<int> m_DeletedDatas = new();    // data's index

        internal static void AddEcsData(T data)
        {
            if (data is IEcsSingletonData && m_EcsDatas.Count > 0)
            {
                if (m_EcsDatas[0].Obj == null)
                {
                    RemoveData(0);
                }
                else
                {
                    Debug.LogError("You are creating a duplicated EcsSingletonRawComponent " + typeof(T).FullName + "! The operation is invalid!");
                    return;
                }
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

        public static IEnumerable<T> GetEnumerator()
        {
            for (int i = 0; i < m_EcsDatas.Count; i++)
            {
                var data = m_EcsDatas[i].Obj;
                if (data == null || data.RequestDeactive)
                    m_DeletedDatas.Add(i);
                else
                    yield return data;
            }
            RemoveDeletedDatas();
        }
        #endregion

        #region private
        private static void RemoveData(int index)
        {
            var data = m_EcsDatas[index].Obj;
            if (data != null)
            {
                data.Active = false;
                data.RequestDeactive = false;
            }

            m_EcsDatas.UnorderedRemoveAt(index);
            if (m_EcsDatas.Count > 0 && index < m_EcsDatas.Count && m_EcsDatas[index].IsNotNull())
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
            for (int i = m_DeletedDatas.Count - 1; i >= 0; i--)
            {
                RemoveData(m_DeletedDatas[i]);
            }
            m_DeletedDatas.Clear();
        }
        #endregion
    }
}
