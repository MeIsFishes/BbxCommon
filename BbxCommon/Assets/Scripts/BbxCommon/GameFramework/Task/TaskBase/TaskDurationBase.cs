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
            Duration = 10000,
            Interval = 10001,
        }

        public override void GetFieldEnumTypes(List<Type> res)
        {
            res.Add(typeof(EDurationField));
            res.Add(GetFieldEnumType());
        }

        protected sealed override void RegisterField()
        {
            RegisterField(EDurationField.Duration, Duration);
            RegisterField(EDurationField.Interval, Interval);
            RegisterTaskField();
        }
        protected abstract void RegisterTaskField();

        public sealed override void ReadFieldInfo(int fieldEnum, TaskFieldInfo fieldInfo, TaskContextBase context)
        {
            switch (fieldEnum)
            {
                case (int)EDurationField.Duration:
                    Duration = ReadFloat(fieldInfo, context);
                    break;
                case (int)EDurationField.Interval:
                    Interval = ReadFloat(fieldInfo, context);
                    break;
            }
            ReadTaskFieldInfo(fieldEnum, fieldInfo, context);
        }
        protected abstract void ReadTaskFieldInfo(int fieldEnum, TaskFieldInfo fieldInfo, TaskContextBase context);

        public override void OnAllocate()
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

        protected override void OnEnter()
        {
            m_ElapsedTime = 0;
            m_LastOnInterval = 0;
            OnTaskEnter();
        }

        protected override ETaskRunState OnUpdate(float deltaTime)
        {
            m_ElapsedTime += deltaTime;
            var state = ETaskRunState.Succeeded;
            if (Duration < 0)
            {
                var durationState = OnTaskUpdate(deltaTime);
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
            else
            {
                if (m_ElapsedTime > Duration)
                {
                    state = ETaskRunState.Succeeded;
                }
                else
                {
                    var durationState = OnTaskUpdate(deltaTime);
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
            }

            float onIntervalCutOff = m_ElapsedTime > Duration ? Duration : m_ElapsedTime;
            if (Interval == 0)
            {
                OnInterval();
            }
            else if (Interval > 0)
            {
                while (m_LastOnInterval + Interval <= onIntervalCutOff)
                {
                    OnInterval();
                    m_LastOnInterval = m_LastOnInterval + Interval;
                }
            }

            return state;
        }

        protected virtual void OnInterval() { }
        protected virtual void OnTaskEnter() { }
        protected virtual EDurationState OnTaskUpdate(float deltaTime) { return EDurationState.Running; }
        protected virtual void OnTaskExit() { }
        #endregion
    }
}
