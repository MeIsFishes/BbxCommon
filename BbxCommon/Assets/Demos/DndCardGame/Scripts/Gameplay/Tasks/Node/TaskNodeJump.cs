using System;
using System.Collections.Generic;
using BbxCommon;
using BbxCommon.Ui;
using Unity.Entities;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace Dcg
{
    public class TaskNodeJump : TaskBase
    {
        public EntityID AttackerEntityId;
        public float JumpHeight;

        private float m_JumpTime;
        private float m_TotalJumpTime = 0.5f;
        public enum EField
        {
            AttackerEntityId,
            JumpHeight,
        }

        protected override Type GetFieldEnumType()
        {
            return typeof(EField);
        }

        protected override void RegisterFields()
        {
            RegisterField(EField.AttackerEntityId, AttackerEntityId);
            RegisterField(EField.JumpHeight, JumpHeight);
        }

        public override void ReadFieldInfo(int fieldEnum, TaskFieldInfo fieldInfo, TaskContextBase context)
        {
            switch (fieldEnum)
            {
                case (int)EField.AttackerEntityId:
                    AttackerEntityId = ReadValue<EntityID>(fieldInfo, context);
                    break;
                case (int)EField.JumpHeight:
                    JumpHeight = ReadFloat(fieldInfo, context);
                    break;
            }
        }

        public override void OnCollect()
        {
            AttackerEntityId = EntityID.INVALID;
            JumpHeight = 0;
        }

        protected override void OnEnter()
        {
            m_JumpTime = 0;
        }

        protected override ETaskRunState OnUpdate(float deltaTime)
        {
            m_JumpTime += deltaTime;

            float progress = m_JumpTime / m_TotalJumpTime;
            if (progress > 1)
            {
                return ETaskRunState.Succeeded;
            }

            float height = JumpHeight * Mathf.Sin(progress * Mathf.PI);
            var entity = AttackerEntityId.GetEntity().GetGameObject();
            entity.transform.position = new Vector3(entity.transform.position.x, height, entity.transform.position.z);
            return ETaskRunState.Running;
        }

        protected override void OnExit()
        {
            // No additional logic needed for OnExit in this case
        }
    }
}
