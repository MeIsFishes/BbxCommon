using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon.Internal
{
    public enum ETaskConnectPointType
    {
        Single,
        Multiple,
    }

    public class TaskConnectPoint
    {
        public ETaskConnectPointType ConnectPointType = ETaskConnectPointType.Multiple;
        public List<TaskBase> Tasks = new();
        internal List<int> TaskRefrenceIds = new();

        public void Reset()
        {
            Tasks.Clear();
            TaskRefrenceIds.Clear();
        }
    }
}
