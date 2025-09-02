using System;
using System.Collections.Generic;

namespace BbxCommon
{
    public abstract class TaskDurationBase : TaskBase
    {
        /// <summary>
        /// When reach the duration time, current task will be stoped. Negative value means never ends.
        /// </summary>
        public float Duration;
        /// <summary>
        /// <see cref="TaskDurationBase"/> executes <see cref="OnInterval"/> each <see cref="Interval"/> time.
        /// 0 means executing every frame, and negative value means never executes <see cref="OnInterval"/>.
        /// </summary>
        public float Interval;

        #region Field Info
        public enum EDurationField
        {
            Duration,
            Interval,
        }

        protected sealed override void RegisterFields()
        {
            RegisterField(EDurationField.Duration, Duration, (fieldInfo, context) => { Duration = ReadFloat(fieldInfo, context); });
            RegisterField(EDurationField.Interval, Interval, (fieldInfo, context) => { Interval = ReadFloat(fieldInfo, context); });
            RegisterTaskFields();
        }
        protected abstract void RegisterTaskFields();

        protected override void OnTaskCollect()
        {
            Duration = -1;
            Interval = -1;
        }
        #endregion

        #region Body
        protected enum EDurationState
        {
            Running,
            Failed,
        }

        private float m_ElapsedTime = 0;
        private float m_LastOnInterval = 0;

        protected sealed override void OnEnter()
        {
            m_ElapsedTime = 0;
            m_LastOnInterval = 0;
            OnTaskEnter();
        }

        protected sealed override ETaskRunState OnUpdate(float deltaTime)
        {
            m_ElapsedTime += deltaTime;
            var state = ETaskRunState.Succeeded;

            float onIntervalCutOff = m_ElapsedTime > Duration ? Duration : m_ElapsedTime;
            if (Interval == 0)
            {
                var durationState = OnInterval();
                switch (durationState)
                {
                    case EDurationState.Running:
                        state = ETaskRunState.Running;
                        break;
                    case EDurationState.Failed:
                        state = ETaskRunState.Failed;
                        break;
                }
            }
            else if (Interval > 0)
            {
                while (m_LastOnInterval + Interval <= onIntervalCutOff)
                {
                    var durationState = OnInterval();
                    switch (durationState)
                    {
                        case EDurationState.Running:
                            state = ETaskRunState.Running;
                            break;
                        case EDurationState.Failed:
                            state = ETaskRunState.Failed;
                            break;
                    }
                    m_LastOnInterval = m_LastOnInterval + Interval;
                }
            }

            if (Duration > 0 && m_ElapsedTime > Duration && state != ETaskRunState.Failed)
            {
                state = ETaskRunState.Succeeded;
            }

            return state;
        }

        protected sealed override void OnExit()
        {
            OnTaskExit();
        }

        protected virtual EDurationState OnInterval() { return EDurationState.Running; }
        protected virtual void OnTaskEnter() { }
        protected virtual void OnTaskExit() { }
        #endregion
    }
}
