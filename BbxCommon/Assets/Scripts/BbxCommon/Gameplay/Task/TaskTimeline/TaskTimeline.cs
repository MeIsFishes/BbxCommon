using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BbxCommon.TaskInternal;

namespace BbxCommon
{
    // TaskTimeline is a timeline which runs tasks.
    // * Tasks will decide when to exit themselves.
    // * However, tasks that managed by TaskTimeline may be stopped when timeline's time up.
    // * Comparing with traditional timeline, task timeline is not so strictly fit the definition,
    //   but it's lightweight and detachable, you can either keep tasks into a timeline or run
    //   each task separately.

    /// <summary>
    /// TaskTimeline is a timeline which runs tasks.
    /// </summary>
    public class TaskTimeline : PooledObject
    {
        /// <summary>
        /// Timeline will not be stopped if duration is negative.
        /// </summary>
        public float Duration = -1;
        /// <summary>
        /// Set if timeline will stop task when itself is terminated.
        /// </summary>
        public bool StopTasks = true;
        /// <summary>
        /// A class which stores game-related data that may be needed by tasks.
        /// </summary>
        public TaskTimelineContextBase Context;
        /// <summary>
        /// Set if Context will be collected while the current TaskTimeline is collected.
        /// </summary>
        public bool AutoCollectContext = true;

        private List<TaskBase> m_TaskSet = new List<TaskBase>();
        private float m_PastTime = 0;
        private bool m_ReadyToRun = false;
        private bool m_ForcedToStop = false;

        private List<CreateTaskData> m_CreateTaskDatas = new List<CreateTaskData>();

        /// <summary>
        /// Return false to stop, or return true to continue.
        /// </summary>
        public bool OnUpdate(float deltaTime)
        {
            if (m_ReadyToRun == false)
                return true;

            m_PastTime += deltaTime;

            TryCreateTask();

            if (m_ForcedToStop)
                return false;

            if (Duration < 0)
                return true;

            if (m_PastTime > Duration)
            {
                Stop();
                return false;
            }
            else
                return true;
        }

        public void Start()
        {
            m_CreateTaskDatas.Sort((x, y) => x.CreatingTime - y.CreatingTime > 0 ? 1 : 0);
            TryCreateTask();
            m_ReadyToRun = true;
        }

        public void Stop()
        {
            m_ForcedToStop = true;

            if (StopTasks)
            {
                foreach (var task in m_TaskSet)
                {
                    task.Stop();
                }
            }
        }

        /// <summary>
        /// Create and return a pooled TaskTimeline.
        /// </summary>
        public static TaskTimeline Create()
        {
            var res = ObjectPool<TaskTimeline>.Alloc();
            TaskTimelineManager.Instance.AddTimeline(res);
            return res;
        }

        public void AddCreateTaskData(CreateTaskData createTaskData)
        {
            m_CreateTaskDatas.Add(createTaskData);
        }

        private int m_CurrentCreatingOrder = 0;

        /// <summary>
        /// Check if there are creating task requests reach their creating time.
        /// </summary>
        private void TryCreateTask()
        {
            while (m_CurrentCreatingOrder < m_CreateTaskDatas.Count)
            {
                // Ones with negative creating time are invalid.
                if (m_CreateTaskDatas[m_CurrentCreatingOrder].CreatingTime < 0)
                {
                    m_CurrentCreatingOrder++;
                    continue;
                }
                if (m_CreateTaskDatas[m_CurrentCreatingOrder].CreatingTime <= m_PastTime)
                {
                    var task = m_CreateTaskDatas[m_CurrentCreatingOrder].CreatingTaskFunc(Context);
                    m_TaskSet.Add(task);
                    task.SetTaskTimelineBelongs(this);
                    m_CurrentCreatingOrder++;
                }
                else
                {
                    break;
                }
            }
        }

        public override void OnCollect()
        {
            Duration = -1;
            StopTasks = true;
            if (AutoCollectContext)
            {
                Context.CollectToPool();
            }
            AutoCollectContext = true;
            Context = null;
            m_TaskSet.Clear();
            m_PastTime = 0;
            m_ReadyToRun = false;
            m_ForcedToStop = false;
        }
    }
}
