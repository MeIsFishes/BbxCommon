using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon.TaskInternal
{
    public interface ITaskManager
    {
        void OnUpdate();
    }

    /// <summary>
    /// A monobehavior singleton sets and ticks all TaskManagers.
    /// </summary>
    public class TaskManagerSet : MonoSingleton<TaskManagerSet>
    {
        private List<ITaskManager> m_ReadyToEnterSet = new List<ITaskManager>();
        private List<ITaskManager> m_TaskManagerSet = new List<ITaskManager>();

        void Update()
        {
            foreach (var taskManager in m_ReadyToEnterSet)
            {
                m_TaskManagerSet.Add(taskManager);
            }
            m_ReadyToEnterSet.Clear();
            foreach (var taskManager in m_TaskManagerSet)
            {
                taskManager.OnUpdate();
            }
        }

        public void AddManager(ITaskManager taskManager)
        {
            m_ReadyToEnterSet.Add(taskManager);
        }
    }
}
