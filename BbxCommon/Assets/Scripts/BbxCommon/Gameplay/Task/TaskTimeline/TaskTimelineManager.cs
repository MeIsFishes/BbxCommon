using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon.TaskInternal
{
    /// <summary>
    /// A monobehavior singleton sets and ticks all TaskTimelines.
    /// </summary>
    public class TaskTimelineManager : MonoSingleton<TaskTimelineManager>
    {
        private HashSet<TaskTimeline> m_TaskTimelineSet = new HashSet<TaskTimeline>();
        private List<TaskTimeline> m_StoppedSet = new List<TaskTimeline>();

        protected void Update()
        {
            foreach (var timeline in m_TaskTimelineSet)
            {
                if (timeline.OnUpdate(Time.deltaTime) == false)
                {
                    m_StoppedSet.Add(timeline);
                }
            }

            foreach (var timeline in m_StoppedSet)
            {
                m_TaskTimelineSet.Remove(timeline);
                timeline.CollectToPool();
            }
            m_StoppedSet.Clear();
        }

        /// <summary>
        /// Add the given timeline to the manager.
        /// </summary>
        public void AddTimeline(TaskTimeline pooledTaskTimeline)
        {
            if (m_TaskTimelineSet.Contains(pooledTaskTimeline) ||
                m_StoppedSet.Contains(pooledTaskTimeline))
            {
                return;
            }
            m_TaskTimelineSet.Add(pooledTaskTimeline);
        }
    }
}
