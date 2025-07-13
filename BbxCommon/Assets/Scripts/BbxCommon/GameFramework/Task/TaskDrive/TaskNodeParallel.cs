
using System;
using System.Collections.Generic;
using BbxCommon;
using BbxCommon.Internal;
using UnityEngine;
using static UnityEditor.VersionControl.Asset;

namespace BbxCommon
{
    public class TaskNodeParallel : TaskBase
    {
        public TaskConnectPoint Tasks = new();

        public enum EField
        {
            Tasks,
        }

        protected override void OnEnter()
        {
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
            bool allFinished = true;
            for (int i = 0; i < Tasks.Tasks.Count; i++)
            {
                var state = Tasks.Tasks[i].Update(deltaTime);
                
                if (state == ETaskRunState.Running)
                    allFinished = false;
            }
            return allFinished ? ETaskRunState.Succeeded : ETaskRunState.Running;
        }

        protected override void OnExit()
        {
            
        }

        public override void OnCollect()
        {
            base.OnCollect();
            Tasks = null;
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
