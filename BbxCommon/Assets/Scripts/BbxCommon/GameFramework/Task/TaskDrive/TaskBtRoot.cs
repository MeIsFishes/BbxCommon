using System;
using BbxCommon;
using System.Collections.Generic;
using BbxCommon.Ui;
using BbxCommon.Internal;

namespace Dcg
{
    // ��Ϊ�����ڵ�
    public class TaskBtRoot : TaskBase
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
            Tasks.Tasks[0].Enter();
        }

        protected override ETaskRunState OnUpdate(float deltaTime)
        {
            if (Tasks.Tasks.Count == 0 || Tasks.Tasks[0] == null)
            {
                return ETaskRunState.Succeeded;
            }
                
            var state = Tasks.Tasks[0].Update(deltaTime);
            return state;
        }

        protected override void OnExit()
        {
            
        }

        protected override void OnTaskCollect()
        {
            Tasks = null;
        }

        protected override void RegisterFields() 
        {
            RegisterField(EField.Tasks, Tasks, (fieldInfo, context) => { Tasks = ReadConnectPoint(fieldInfo, context); });
        }
    }
}
