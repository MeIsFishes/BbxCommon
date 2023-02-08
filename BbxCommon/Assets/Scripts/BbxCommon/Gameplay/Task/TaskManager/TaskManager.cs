using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BbxCommon.TaskInternal;

namespace BbxCommon
{
    /// <summary>
    /// A class which sets and ticks tasks of specific type.
    /// </summary>
    public class TaskManager<TaskType> : Singleton<TaskManager<TaskType>>, ITaskManager
        where TaskType : TaskBase, new()
    {
        protected bool m_Initialized = false;
        protected HashSet<TaskType> m_NewTaskSet = new HashSet<TaskType>();
        protected HashSet<TaskType> m_TaskSet = new HashSet<TaskType>();
        protected HashSet<TaskType> m_FinishedSet = new HashSet<TaskType>();

        void ITaskManager.OnUpdate()
        {
            foreach (var newTask in m_NewTaskSet)
            {
                newTask.Enter();
                m_TaskSet.Add(newTask);
            }
            m_NewTaskSet.Clear();

            foreach (var task in m_TaskSet)
            {
                if (task.HasExited)
                {
                    m_FinishedSet.Add(task);
                    continue;
                }
                var runningState = task.Execute();
                if (runningState == ETaskState.Finished)
                {
                    m_FinishedSet.Add(task);
                }
            }

            foreach (var task in m_FinishedSet)
            {
                if (task.HasExited == false)
                {
                    task.Exit();
                }
                task.Collect();
                m_TaskSet.Remove(task);
            }
            m_FinishedSet.Clear();
        }

        /// <summary>
        /// Create a task and add it to set. You should initialize the task externally.
        /// </summary>
        public TaskType CreateTask()
        {
            if (m_Initialized == false)
            {
                TaskManagerSet.Instance.AddManager(this);
                m_Initialized = true;
            }
            var pooledTask = ObjectPool<TaskType>.Alloc();
            m_NewTaskSet.Add(pooledTask);
            return pooledTask;
        }

        /// <summary>
        /// Return a pooled task without adding it to the manager.
        /// </summary>
        public TaskType CreateARawTask()
        {
            var pooledTask = ObjectPool<TaskType>.Alloc();
            return pooledTask;
        }
    }
}
