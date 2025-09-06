using BbxCommon.Internal;
using System;
using System.Collections.Generic;

namespace BbxCommon
{
    [TaskTag(TaskTagAttribute.ESetTag.Override, TaskExportCrossVariable.TaskTagTimeline)]
    public class TaskTimeline : TaskBase
    {
        #region Init
        private struct TaskInfo : IComparable<TaskInfo>
        {
            public float StartTime;
            public float EndTime;
            public float LastUpdateTime;
            public TaskBase Task;

            public int CompareTo(TaskInfo target)
            {
                if (StartTime > target.StartTime)
                    return 1;
                if (StartTime == target.StartTime)
                    return 0;
                return -1;
            }
        }

        private List<TaskInfo> m_TaskInfos = new();

        internal void ReadTimelineItem(TaskTimelineItemInfo item, TaskBase task)
        {
            var taskInfo = new TaskInfo();
            taskInfo.StartTime = item.StartTime;
            if (item.Duration >= 0)
                taskInfo.EndTime = item.StartTime + item.Duration;
            else
                taskInfo.EndTime = -1;
            taskInfo.Task = task;
            m_TaskInfos.Add(taskInfo);
        }
        #endregion

        #region Body
        /// <summary>
        /// If negative, the timeline will never end.
        /// </summary>
        public float Duration;

        private float m_ElapsedTime;
        private int m_CurCheckIndex;    // for check which tasks should run
        private List<int> m_RunningTaskIndexes = new();

        protected override void OnEnter()
        {
            StartTask(0);
        }

        protected override ETaskRunState OnUpdate(float deltaTime)
        {
            // start task
            m_ElapsedTime += deltaTime;
            if (Duration >= 0)  // negative value means never stop
            {
                if (m_ElapsedTime > Duration)
                    m_ElapsedTime = Duration;
                StartTask(m_ElapsedTime);
                // update tasks
                UpdateChild();
                if (m_ElapsedTime >= Duration)
                    return ETaskRunState.Succeeded;
            }
            return ETaskRunState.Running;
        }

        private void UpdateChild()
        {
            // update tasks
            var finishedIndexes = SimplePool<List<int>>.Alloc();
            for (int i = 0; i < m_RunningTaskIndexes.Count; i++)
            {
                var index = m_RunningTaskIndexes[i];
                var taskInfo = m_TaskInfos[index];
                if (taskInfo.EndTime < m_ElapsedTime)
                {
                    taskInfo.Task.Update(taskInfo.EndTime - taskInfo.LastUpdateTime);
                    finishedIndexes.Add(i);
                }
                else
                {
                    var taskState = taskInfo.Task.Update(m_ElapsedTime - taskInfo.LastUpdateTime);
                    taskInfo.LastUpdateTime = m_ElapsedTime;
                    // add finished
                    if (taskState == ETaskRunState.Succeeded || taskState == ETaskRunState.Failed)
                        finishedIndexes.Add(index);
                }
            }

            // exit
            // Since in some extreme cases, running order may causes bugs, we promise that tasks always run in the order of adding.
            for (int i = 0; i < finishedIndexes.Count; i++)
            {
                var taskInfo = m_TaskInfos[finishedIndexes[i]];
                taskInfo.Task.Exit();
            }

            // collect collections
            finishedIndexes.CollectToPool();
        }

        protected override void OnExit()
        {
            for (int i = 0; i < m_RunningTaskIndexes.Count; i++)
            {
                m_TaskInfos[m_RunningTaskIndexes[i]].Task.Exit();
            }
        }

        private void StartTask(float cutOffTime)
        {
            while (m_CurCheckIndex < m_TaskInfos.Count && m_TaskInfos[m_CurCheckIndex].StartTime <= cutOffTime)
            {
                var taskInfo = m_TaskInfos[m_CurCheckIndex];
                var task = taskInfo.Task;
                taskInfo.LastUpdateTime = 0;
                task.Enter();
                m_RunningTaskIndexes.Add(m_CurCheckIndex);
                m_CurCheckIndex++;
            }
        }

        protected override void OnTaskCollect()
        {
            Duration = default;
            m_ElapsedTime = default;
            m_CurCheckIndex = default;
            m_RunningTaskIndexes.Clear();
            for (int i = 0; i < m_TaskInfos.Count; i++)
            {
                m_TaskInfos[i].Task.CollectToPool();
            }
            m_TaskInfos.Clear();
        }
        #endregion

        #region Field Info
        public enum EField
        {
            Duration,
        }

        protected override void RegisterFields()
        {
            RegisterField(EField.Duration, Duration, (fieldInfo, context) => { Duration = ReadFloat(fieldInfo, context); });
        }
        #endregion
    }
}
