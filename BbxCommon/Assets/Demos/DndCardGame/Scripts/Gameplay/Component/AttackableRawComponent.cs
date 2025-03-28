﻿using System.Collections.Generic;
using UnityEngine.Events;
using Unity.Entities;
using BbxCommon;
using UnityEngine;

namespace Dcg
{
    // 1.在CauseDamageSystem中处理造成伤害的增益等逻辑，然后将伤害请求写给target
    // 2.在TakeDamageSystem中处理受到伤害的相关逻辑，并与AttributesRawComponent交互
    // 3.buff、道具等相关的逻辑处理通过delegate挂到OnCauseDamage、OnTakeDamage上
    public class DamageRequest : PooledObject
    {
        public Entity Attacker;
        public Entity Target;
        public int Damage;
        public DamageType DamageType;
        public Vector3 DamagePos;
    }

    public enum DamageType
    {
        None = 0,
        Slash = 1,
        Piercing = 2,
        Bludgeoning = 3,
        Exploding = 4,
        Fire = 5,
        Cold = 6,
        Lightning = 7,
        Force = 8,
        Radiant = 9,
        Poison = 10,
        Psychic = 11,
        Healing = 12,
        UpperLimit = 99,
    }


    public enum ArmorTagType
    {
        Armor = 0,
        Property = 1,

    }

    public class AttackableRawComponent : EcsRawComponent, IListenable
    {
        public enum EEvent
        {
            DamageRequestProcessed,
            AttackMiss,
        }

        public List<DamageRequest> CauseDamageRequests;
        public List<DamageRequest> TakeDamageRequests;

        public UnityAction<DamageRequest> OnCauseDamage;
        public UnityAction<DamageRequest> OnTakeDamage;

        private MessageHandler<int> m_MessageHandler = new();
        IMessageDispatcher<int> IListenable.MessageDispatcher => m_MessageHandler;

        public void DispatchEvent(EEvent e, object extraData)
        {
            m_MessageHandler.Dispatch((int)e, extraData);
        }

        public void AddCauseDamageRequest(Entity attacker, Entity target, int damage, DamageType damageType, Vector3 damagePos)
        {
            var request = ObjectPool<DamageRequest>.Alloc();
            request.Attacker = attacker;
            request.Target = target;
            request.Damage = damage;
            request.DamageType = damageType;
            request.DamagePos = damagePos;
            CauseDamageRequests.Add(request);
        }

        public void AddTakeDamageRequest(DamageRequest request)
        {
            TakeDamageRequests.Add(request);
        }

        public override void OnAllocate()
        {
            CauseDamageRequests = SimplePool<List<DamageRequest>>.Alloc();
            TakeDamageRequests = SimplePool<List<DamageRequest>>.Alloc();
        }

        public override void OnCollect()
        {
            CauseDamageRequests.CollectAndClearElements(true);
            TakeDamageRequests.CollectAndClearElements(true);
        }
    }
}
