using UnityEngine;

namespace BbxCommon
{
    /// <summary>
    /// A task which will be automatically ended depends on time. It will call a trigger function each interval.
    /// </summary>
    public abstract class TaskDurationBase : TaskBase
    {
        /// <summary>
        /// Set how long will this task run. A negative value will keep it run forever.
        /// </summary>
        public float TaskDuration { get; protected set; } = -1;
        /// <summary>
        /// Set how long will the trigger function be called each time. A negative value will keep it never be called, and 0 keeps it being called every frame.
        /// </summary>
        public float TriggerInterval { get; protected set; } = -1;
        /// <summary>
        /// Set if OnInterval() will be called when the task begins.
        /// </summary>
        public bool TriggerAtBeginning { get; protected set; }

        // TaskDuration and TriggerInterval are all protected set values. For there will be different requirements in different situations,
        // some tasks may decide duration themselves, some may not have interval triggers, so initializing those values in Init() function of
        // derrived task class is suggested.

        protected float m_PastTime { get; private set; }
        protected float m_IntervalTimer { get; private set; }

        public void InitTaskDuration(float taskDuration, float triggerInterval, bool triggerAtBeginning)
        {
            TaskDuration = taskDuration;
            TriggerInterval = triggerInterval;
            TriggerAtBeginning = triggerAtBeginning;
        }

        protected override ETaskState OnExecute()
        {
            if (TriggerAtBeginning && m_PastTime == 0)
                OnInterval();

            if (TriggerInterval == 0)
            {
                OnInterval();
            }
            else if (TriggerInterval > 0)
            {
                m_IntervalTimer += Time.deltaTime;
                if (m_IntervalTimer > TriggerInterval)
                {
                    var triggerTimes = m_IntervalTimer / TriggerInterval;
                    m_IntervalTimer -= triggerTimes * TriggerInterval;
                    for (int i = 0; i < triggerTimes; i++)
                    {
                        OnInterval();
                    }
                }
            }

            if (TaskDuration < 0)
                return ETaskState.Running;
            m_PastTime += Time.deltaTime;
            if (m_PastTime < TaskDuration)
                return ETaskState.Running;
            else
                return ETaskState.Finished;
        }

        protected virtual void OnInterval() { }

        public override void OnCollect()
        {
            base.OnCollect();
            TaskDuration = -1;
            TriggerInterval = -1;
            TriggerAtBeginning = false;
            m_PastTime = 0;
            m_IntervalTimer = 0;
        }
    }
}
