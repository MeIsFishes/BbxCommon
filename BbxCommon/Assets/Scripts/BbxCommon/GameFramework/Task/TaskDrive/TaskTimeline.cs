using BbxCommon.Internal;
using System;
using System.Collections.Generic;

namespace BbxCommon
{
    public class TaskTimeline : TaskBase
    {
        #region Init
        private struct TaskInfo : IComparable<TaskInfo>
        {
            public float StartTime;
            public float EndTime;
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

        /// <summary>
        /// Order by <see cref="TaskInfo.StartTime"/> ascendingly. That means tasks' start time might be: 0, 0, 5, 10.
        /// </summary>
        internal void SortItems()
        {
            m_TaskInfos.Sort();
        }
        #endregion

        #region Body
        public float Duration;

        private float m_ElapsedTime;
        private int m_CurCheckIndex;    // for check which tasks should run
        private List<int> m_RunningTaskIndexes = new();

        protected override void OnEnter()
        {
            m_ElapsedTime = 0;
            m_CurCheckIndex = 0;
            m_RunningTaskIndexes.Clear();
            RunTask(0);
        }

        protected override ETaskRunState OnUpdate(float deltaTime)
        {
            // start task
            m_ElapsedTime += deltaTime;
            if (Duration >= 0)  // negative value means never stop
            {
                if (m_ElapsedTime > Duration)
                    m_ElapsedTime = Duration;
                RunTask(m_ElapsedTime);
                if (m_ElapsedTime >= Duration)
                    return ETaskRunState.Succeeded;
            }
            // stop tasks
            var finishedIndexes = SimplePool<List<int>>.Alloc();
            for (int i = 0; i < m_RunningTaskIndexes.Count; i++)
            {
                var taskInfo = m_TaskInfos[m_RunningTaskIndexes[i]];
                if (taskInfo.EndTime < m_ElapsedTime)
                {
                    finishedIndexes.Add(i);
                    taskInfo.Task.Stop();
                }
            }
            for (int i = finishedIndexes.Count - 1; i >= 0; i--)
            {
                m_RunningTaskIndexes.RemoveAt(finishedIndexes[i]);
            }
            finishedIndexes.CollectToPool();

            return ETaskRunState.Running;
        }

        protected override void OnExit()
        {
            for (int i = 0; i < m_RunningTaskIndexes.Count; i++)
            {
                m_TaskInfos[m_RunningTaskIndexes[i]].Task.Stop();
            }
        }

        private void RunTask(float cutOffTime)
        {
            while (m_CurCheckIndex < m_TaskInfos.Count && m_TaskInfos[m_CurCheckIndex].StartTime <= cutOffTime)
            {
                m_TaskInfos[m_CurCheckIndex].Task.Run();
                m_RunningTaskIndexes.Add(m_CurCheckIndex);
                m_CurCheckIndex++;
            }
        }
        #endregion

        #region Field Info
        public enum EField
        {
            Duration,
        }
        protected override Type GetFieldEnumType()
        {
            return typeof(EField);
        }
        protected override void RegisterField()
        {
            RegisterField(EField.Duration, Duration);
        }
        public override void ReadFieldInfo(int fieldEnum, TaskFieldInfo fieldInfo, TaskContextBase context)
        {
            switch (fieldEnum)
            {
                case (int)EField.Duration:
                    Duration = ReadFloat(fieldInfo, context);
                    break;
            }
        }
        #endregion
    }
}
