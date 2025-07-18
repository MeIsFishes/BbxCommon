
using System;
using System.Collections.Generic;
using BbxCommon.Internal;
using UnityEngine;

namespace BbxCommon
{
    public class TaskNodeSelector : TaskBase
    {
        public TaskConnectPoint Tasks = new();
        private int m_CurrentIndex = -1;

        public enum EField
        {
            Tasks,
        }

        protected override void OnEnter()
        {
            m_CurrentIndex = 0;

            for (int i = 0; i < Tasks.Tasks.Count; i++)
            {
                if (Tasks.Tasks[i].CanEnter())
                {
                    m_CurrentIndex = i;
                    Tasks.Tasks[i].Enter();
                    return;
                }
            }
        }

        protected override ETaskRunState OnUpdate(float deltaTime)
        {
            if (Tasks.Tasks.Count == 0 || m_CurrentIndex == -1)
            {
                return ETaskRunState.Failed;
            }

            var task = Tasks.Tasks[m_CurrentIndex];
                
            var state = task.Update(deltaTime);

            if (state == ETaskRunState.Running)
            {
                return ETaskRunState.Running;
            }
            //extra logic?

            return ETaskRunState.Succeeded;
        }

        protected override void OnExit()
        {
            
        }

        public override void OnCollect()
        {
            base.OnCollect();
            Tasks = null;
            m_CurrentIndex = -1;
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
