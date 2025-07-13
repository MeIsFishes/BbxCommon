
using System;
using System.Collections.Generic;
using BbxCommon.Internal;
using UnityEngine;

namespace BbxCommon
{
    public class TaskNodeSelector : TaskBase
    {
        public TaskConnectPoint Tasks = new();
        private int m_CurrentIndex;
        private bool m_FoundValid;

        public enum EField
        {
            Tasks,
        }

        protected override void OnEnter()
        {
            m_CurrentIndex = 0;
            m_FoundValid = false;
            if (Tasks.Tasks.Count > 0)
            {
                Tasks.Tasks[m_CurrentIndex].Run();
            }
        }

        protected override ETaskRunState OnUpdate(float deltaTime)
        {
            if (Tasks.Tasks.Count == 0)
            {
                return ETaskRunState.Failed;
            }

            while (m_CurrentIndex < Tasks.Tasks.Count)
            {
                var task = Tasks.Tasks[m_CurrentIndex];
                if (!m_FoundValid)
                {
                    task.Run();
                    m_FoundValid = true;
                }
                var state = task.Update(deltaTime);

                if (state == ETaskRunState.Running)
                {
                    return ETaskRunState.Running;
                }
                if (state == ETaskRunState.Succeeded)
                {
                    return ETaskRunState.Succeeded;
                }
                    
                m_CurrentIndex++;
                m_FoundValid = false;
            }
            return ETaskRunState.Failed;
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
                    Tasks = ReadValue<TaskConnectPoint>(fieldInfo, context);
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
