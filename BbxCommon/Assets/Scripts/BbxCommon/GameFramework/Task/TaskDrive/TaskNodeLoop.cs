
using System;
using System.Collections.Generic;
using BbxCommon;
using BbxCommon.Internal;
using Unity.Transforms;
using UnityEngine;
using static UnityEditor.VersionControl.Asset;

namespace BbxCommon
{
    public class TaskNodeLoop: TaskBase
    {
        public TaskConnectPoint Tasks = new();
        public int LoopCount = -1;
        private int m_CurrentCount;

        public enum EField
        {
            Tasks,
            LoopCount,
        }

        protected override void OnEnter()
        {
            m_CurrentCount = 0;
            if (Tasks.Tasks.Count == 0)
            {
                return;
            }
            for (int i = 0; i < Tasks.Tasks.Count; i++)
            {
                Tasks.Tasks[0].Enter();
            }
        }

        protected override ETaskRunState OnUpdate(float deltaTime)
        {
            while (LoopCount < 0 || m_CurrentCount < LoopCount)
            {
                var state = Tasks.Tasks[0].Update(deltaTime);
                if (state == ETaskRunState.Running)
                {
                    return ETaskRunState.Running;
                }
                    
                m_CurrentCount++;
                if (LoopCount > 0 && m_CurrentCount >= LoopCount)
                {
                    return ETaskRunState.Succeeded;
                }

                Tasks.Tasks[0].Enter();
            }
            return ETaskRunState.Succeeded;
        }

        protected override void OnExit()
        {
           
        }

        public override void OnCollect()
        {
            base.OnCollect();
            Tasks = null;
            LoopCount = -1;
        }

        public override void ReadFieldInfo(int fieldEnum, TaskFieldInfo fieldInfo, TaskContextBase context)
        {
            switch (fieldEnum)
            {
                case (int)EField.Tasks:
                    Tasks = ReadConnectPoint(fieldInfo, context);
                    break;
                case (int)EField.LoopCount:
                    LoopCount = ReadInt(fieldInfo, context);
                    break;
                default:
                    break;
            }
        }

        protected override void RegisterFields()
        {
            RegisterField(EField.Tasks, Tasks);
            RegisterField(EField.LoopCount, LoopCount);
        }

        protected override Type GetFieldEnumType()
        {
            return typeof(EField);
        }
    }
}
