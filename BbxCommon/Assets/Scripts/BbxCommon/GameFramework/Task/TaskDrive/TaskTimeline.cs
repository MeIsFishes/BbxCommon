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
        internal List<TaskManager.RunningTaskInfo> RunningChildTaskInfos = new();

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
            var finishInfos = SimplePool<List<TaskSystem.TaskFinishInfo>>.Alloc();
            for (int i = 0; i < RunningChildTaskInfos.Count; i++)
            {
                var taskInfo = RunningChildTaskInfos[i];
                var taskState = ETaskRunState.Running;
                // check if task should stop
                var finishedIndexes = SimplePool<List<int>>.Alloc();
                for (int k = 0; k < m_RunningTaskIndexes.Count; k++)
                {
                    var taskTimelineInfo = m_TaskInfos[m_RunningTaskIndexes[k]];
                    if (taskTimelineInfo.EndTime < m_ElapsedTime)
                    {
                        taskTimelineInfo.Task.Stop();
                    }
                }
                for (int k = finishedIndexes.Count - 1; k >= 0; k--)
                {
                    m_RunningTaskIndexes.RemoveAt(finishedIndexes[k]);
                }
                finishedIndexes.CollectToPool();

                switch (taskInfo.State)
                {
                    case TaskManager.ERunningTaskState.NewEnter:
                        taskState = taskInfo.Task.Update(0);
                        RunningChildTaskInfos[i] =
                            new TaskManager.RunningTaskInfo(taskInfo.Task, TaskManager.ERunningTaskState.Keep);
                        break;
                    case TaskManager.ERunningTaskState.Keep:
                        taskState = taskInfo.Task.Update(TimeApi.DeltaTime);
                        break;
                }

                var finishInfo = new TaskSystem.TaskFinishInfo();
                switch (taskState)
                {
                    case ETaskRunState.Succeeded:
                        finishInfo.Index = i;
                        finishInfo.Succeeded = true;
                        finishInfos.Add(finishInfo);
                        break;
                    case ETaskRunState.Failed:
                        finishInfo.Index = i;
                        finishInfo.Succeeded = false;
                        finishInfos.Add(finishInfo);
                        break;
                }
            }

            // exit
            // Since in some extreme cases, running order may cause bugs, we promise that tasks always run in the order of adding.
            for (int i = 0; i < finishInfos.Count; i++)
            {
                var finishInfo = finishInfos[i];
                var taskInfo = RunningChildTaskInfos[finishInfo.Index];
                if (finishInfo.Succeeded)
                    taskInfo.Task.OnNodeSucceeded();
                else
                    taskInfo.Task.OnNodeFailed();
                taskInfo.Task.Exit();
            }

            for (int i = finishInfos.Count - 1; i >= 0; i--)
            {
                RunningChildTaskInfos.RemoveAt(finishInfos[i].Index);
            }

            // collect collections
            finishInfos.CollectToPool();
        }

        protected override void OnExit()
        {
            for (int i = 0; i < m_RunningTaskIndexes.Count; i++)
            {
                m_TaskInfos[m_RunningTaskIndexes[i]].Task.Stop();
            }
            // execute exit of child tasks
            UpdateChild();
        }

        private void StartTask(float cutOffTime)
        {
            while (m_CurCheckIndex < m_TaskInfos.Count && m_TaskInfos[m_CurCheckIndex].StartTime <= cutOffTime)
            {
                var task = m_TaskInfos[m_CurCheckIndex].Task;
                task.Enter();
                RunningChildTaskInfos.Add(new TaskManager.RunningTaskInfo(task, TaskManager.ERunningTaskState.NewEnter));
                
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
            RunningChildTaskInfos.Clear();
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
