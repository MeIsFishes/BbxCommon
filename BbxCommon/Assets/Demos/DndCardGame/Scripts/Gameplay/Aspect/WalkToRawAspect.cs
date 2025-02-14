﻿using UnityEngine;
using BbxCommon;

namespace Dcg
{
    /// <summary>
    /// 对于能显示在场景中的单位，命令他们走到某个目标所需要的数据
    /// </summary>
    public class WalkToRawAspect : EcsRawAspect
    {
        public Vector3 Position { get { return m_Transform.position; }}
        public Quaternion Rotation { get { return m_Transform.rotation; } set { m_Transform.rotation = value; } }
        public CharacterController CharacterController;
        public Animator Animator;
        public bool Finished
        {
            get { return m_WalkToComp.Request.Finished; }
            set { m_WalkToComp.Request.Finished = value; }
        }
        public Vector3 Destination => m_WalkToComp.Request.Destination;
        public float WalkSpeed => DataApi.GetData<ModelAttributesData>().WalkSpeed;

        private Transform m_Transform;
        private WalkToRawComponent m_WalkToComp;

        protected override void CreateAspect()
        {
            m_Transform = GetGameObjectComponent<Transform>();
            CharacterController = GetGameObjectComponent<CharacterController>();
            Animator = GetGameObjectComponent<Animator>();
            m_WalkToComp = GetRawComponent<WalkToRawComponent>();
        }
    }
}
