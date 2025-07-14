
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BbxCommon;
using BbxCommon.Internal;
using UnityEngine;

namespace BbxCommon
{
    public class TaskNodeSequence : TaskBase
    {
        public TaskConnectPoint Tasks = new();
        private int m_CurrentIndex;

        public enum EField
        {
            Tasks,
        }

        protected override void OnEnter()
        {
            m_CurrentIndex = 0;
            if (Tasks.Tasks.Count > 0)
            {
                Tasks.Tasks[m_CurrentIndex].Enter();
            }
        }

        protected override ETaskRunState OnUpdate(float deltaTime)
        {
            if (Tasks.Tasks.Count == 0)
            {
                return ETaskRunState.Succeeded;
            }
                

            while (m_CurrentIndex < Tasks.Tasks.Count)
            {
                var state = Tasks.Tasks[m_CurrentIndex].Update(deltaTime);
                
                if (state == ETaskRunState.Running)
                {
                    return ETaskRunState.Running;
                }

                m_CurrentIndex++;
                if (m_CurrentIndex < Tasks.Tasks.Count)
                {
                    Tasks.Tasks[m_CurrentIndex].Enter();
                }
                    
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
            m_CurrentIndex = 0;
        }

        public override void ReadFieldInfo(int fieldEnum, TaskFieldInfo fieldInfo, TaskContextBase context)
        {
            switch (fieldEnum)
            {
                case (int)EField.Tasks:
                    Tasks = ReadConnectPoint(fieldInfo, context);
                    break;
                default:
                    break;
            }
        }

        protected override void RegisterFields()
        {
            RegisterField(EField.Tasks, Tasks);
        }

        protected override Type GetFieldEnumType()
        {
            return typeof(EField);
        }
    }
}
