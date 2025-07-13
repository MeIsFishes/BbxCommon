
using System;
using System.Collections.Generic;
using BbxCommon;
using BbxCommon.Internal;
using Unity.Transforms;
using UnityEngine;
using static UnityEditor.VersionControl.Asset;

namespace BbxCommon
{
    public class TaskNodeTimer : TaskBase
    {
        public TaskConnectPoint Tasks = new();
        public float Duration;
        private float m_Time;

        public enum EField
        {
            Tasks,
            Duration,
        }

        protected override void OnEnter()
        {
            m_Time = 0f;
            if (Tasks.Tasks.Count == 0)
            {
                return;
            }
            for (int i = 0; i < Tasks.Tasks.Count; i++)
            {
                Tasks.Tasks[0].Run();
            }
        }

        protected override ETaskRunState OnUpdate(float deltaTime)
        {
            m_Time += deltaTime;
            if (Tasks.Tasks[0] == null)
            {
                return ETaskRunState.Succeeded;
            }

            var state = Tasks.Tasks[0].Update(deltaTime);
            if (state != ETaskRunState.Running)
            {
                return ETaskRunState.Succeeded;
            }
            if (m_Time >= Duration)
            {
                return ETaskRunState.Succeeded;
            }
                
            return ETaskRunState.Running;
        }

        protected override void OnExit()
        {
           
        }

        public override void OnCollect()
        {
            base.OnCollect();
            Tasks = null;
            Duration = 0f;
        }

        public override void ReadFieldInfo(int fieldEnum, TaskFieldInfo fieldInfo, TaskContextBase context)
        {
            switch (fieldEnum)
            {
                case (int)EField.Tasks:
                    Tasks = ReadValue<TaskConnectPoint>(fieldInfo, context);
                    break;
                case (int)EField.Duration:
                    Duration = ReadValue<float>(fieldInfo, context);
                    break;
                default:
                    break;
            }
        }

        protected override void RegisterFields()
        {
            RegisterField(EField.Tasks, Tasks);
            RegisterField(EField.Duration, Duration);
        }

        protected override Type GetFieldEnumType()
        {
            return typeof(EField);
        }
    }
}
