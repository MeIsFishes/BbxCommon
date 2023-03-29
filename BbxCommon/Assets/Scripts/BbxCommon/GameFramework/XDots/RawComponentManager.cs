using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Entities;

namespace BbxCommon.Framework
{
    /// <summary>
    /// <para>
    /// <see cref="EcsRawComponent"/>s and <see cref="EcsRawAspect"/> of <see cref="Entity"/>s are all stored here. For <see cref="Entity"/> is just a
    /// struct data which cannot be directly extended, we create a <see cref="Dictionary{TKey, TValue}"/> to  manage its <see cref="EcsRawComponent"/>
    /// and <see cref="EcsRawAspect"/> datas.
    /// </para><para>
    /// However, <see cref="RawComponentManager"/> only operates <see cref="EcsRawComponent"/>s and <see cref="EcsRawAspect"/>s of the <see cref="Entity"/>s,
    /// to interact with DOTS <see cref="World"/>, see <see cref="EcsApi"/> functions.
    /// </para>
    /// </summary>
    internal static class RawComponentManager
    {
        /// <summary>
        /// <see cref="Entity"/>s with <see cref="EcsRawComponent"/>s added to it.
        /// </summary>
        internal static Dictionary<Entity, RawComponentGroup> EntityRawComponentGroup = new Dictionary<Entity, RawComponentGroup>();
        /// <summary>
        /// Store <see cref="EcsRawComponent"/>s separately in List to improve iterator performance.
        /// For example using in <see cref="ForeachRawComponent{T}(UnityAction{T})"/>.
        /// </summary>
        internal static Dictionary<Type, List<EcsRawComponent>> RawComponentLists = new Dictionary<Type, List<EcsRawComponent>>();

        public static T AddRawComponent<T>(Entity entity) where T : EcsRawComponent, new()
        {
            var type = typeof(T);
            var comp = ObjectPool<T>.Alloc();
            var group = GetGroupAndRefreshHot(entity);
            if (RawComponentLists.TryGetValue(type, out var list) == false)
            {
                list = SimplePool<List<EcsRawComponent>>.Alloc();
                RawComponentLists[type] = list;
            }
            if (comp is EcsSingletonRawComponent && list.Count > 0)
            {
                Debug.LogError("You are creating a duplicated EcsSingletonRawComponent " + type.FullName + "! It's invalid!");
                return (T)list[0];
            }

            group.AddRawComponent(comp);
            list.Add(comp);
            comp.InitRawComponent(entity, list.Count - 1);
            return comp;
        }

        public static T GetRawComponent<T>(Entity entity) where T : EcsRawComponent
        {
            var group = GetGroupAndRefreshHot(entity);
            return group.GetRawComponent<T>();
        }

        public static T GetSingletonRawComponent<T>() where T : EcsSingletonRawComponent
        {
            if (RawComponentLists.TryGetValue(typeof(T), out var list) && list.Count > 0)
                return (T)list[0];
            return null;
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
            RemoveRawComponentFromList(comp);
            comp.CollectToPool();
        }

        public static void ForeachRawComponent<T>(UnityAction<T> action) where T : EcsRawComponent
        {
            if (RawComponentLists.TryGetValue(typeof(T), out var list))
            {
                foreach (var component in list)
                {
                    action((T)component);
                }
            }
        }

        public static void DestroyEntity(Entity entity)
        {
            var group = GetGroupAndRefreshHot(entity);
            foreach (var compPair in group.RawComponents)
            {
                RemoveRawComponentFromList(compPair.Key, compPair.Value);
            }
            group.CollectToPool();
        }

        internal static void RemoveRawComponentFromList<T>(T comp) where T : EcsRawComponent
        {
            var list = RawComponentLists[typeof(T)];
            list.UnorderedRemove(comp.Index);
            list[comp.Index].Index = comp.Index;    // swap removing, then set its index as new
        }

        internal static void RemoveRawComponentFromList(Type type, EcsRawComponent comp)
        {
            var list = RawComponentLists[type];
            list.UnorderedRemove(comp.Index);
            list[comp.Index].Index = comp.Index;    // swap removing, then set its index as new
        }

        // Generally, user may operate a single entity several times. If so, storing a hot data can reduce one time hash calculation.
        private static RawComponentGroup m_HotGroup = new RawComponentGroup();
        internal static RawComponentGroup GetGroupAndRefreshHot(Entity entity)
        {
            if (m_HotGroup.Entity != entity)
                m_HotGroup = EntityRawComponentGroup[entity];
            return m_HotGroup;
        }

    }
}
