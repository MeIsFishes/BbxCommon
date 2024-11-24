using System;
using System.Collections;
using System.Collections.Generic;
using BbxCommon.Internal;

namespace BbxCommon
{
    public enum ETaskRunState
    {
        Running,
        Succeeded,
        Failed,
    }

    public abstract class TaskBase : PooledObject
    {
        #region Lifecycle
        public bool IsRunning { get; private set; }

        private bool m_RequiredStop = false;
        private bool m_BlockEnter = false;

        // EnterCondition: Check once when task executes Enter().
        // Condition: Check every frame. If failed, the task will be stopped and return failed.
        // ExitCondition: Check every frame. If succeeded, the task will be stopped and return succeeded.
        private List<TaskConditionBase> m_EnterCondition = new();
        private List<TaskConditionBase> m_Conditions = new();
        private List<TaskConditionBase> m_ExitConditions = new();

        public void Run()
        {
            TaskManager.Instance.RunTask(this);
        }

        /// <summary>
        /// Force the task to stop and return <see cref="ETaskRunState.Succeeded"/>.
        /// </summary>
        public void Stop()
        {
            m_RequiredStop = true;
        }

        public void AddEnterCondition(TaskConditionBase condition)
        {
            m_EnterCondition.Add(condition);
        }

        public void AddCondition(TaskConditionBase condition)
        {
            m_Conditions.Add(condition);
        }

        public void AddExitCondition(TaskConditionBase condition)
        {
            m_ExitConditions.Add(condition);
        }

        internal void Enter()
        {
            IsRunning = true;
            m_RequiredStop = false;
            m_BlockEnter = false;
            for (int i = 0; i < m_EnterCondition.Count; i++)
            {
                var condition = m_EnterCondition[i];
                condition.Enter();
                var state = condition.Update(0);
                condition.Exit();
                if (state == ETaskRunState.Failed)
                {
                    m_BlockEnter = true;
                    return;
                }
            }
            for (int i = 0; i < m_Conditions.Count; i++)
            {
                m_Conditions[i].Enter();
            }
            OnEnter();
        }

        internal ETaskRunState Update(float deltaTime)
        {
            if (m_RequiredStop)
                return ETaskRunState.Succeeded;
            if (m_BlockEnter)
                return ETaskRunState.Failed;
            for (int i = 0; i < m_Conditions.Count; i++)
            {
                if (m_Conditions[i].Update(deltaTime) == ETaskRunState.Failed)
                {
                    return ETaskRunState.Failed;
                }
            }
            for (int i = 0; i < m_ExitConditions.Count; i++)
            {
                if (m_ExitConditions[i].Update(deltaTime) == ETaskRunState.Succeeded)
                {
                    return ETaskRunState.Succeeded;
                }
            }
            return OnUpdate(deltaTime);
        }

        internal void Exit()
        {
            IsRunning = false;
            if (m_BlockEnter)
                return;
            for (int i = 0; i < m_Conditions.Count; i++)
            {
                m_Conditions[i].Exit();
            }
            for (int i = 0; i < m_ExitConditions.Count; i++)
            {
                m_ExitConditions[i].Exit();
            }
            OnExit();
        }

        internal void OnNodeSucceeded() { OnSucceeded(); }
        internal void OnNodeFailed() { OnFailed(); }

        protected virtual void OnEnter() { }
        protected virtual ETaskRunState OnUpdate(float deltaTime) { return ETaskRunState.Succeeded; }
        protected virtual void OnExit() { }
        protected virtual void OnSucceeded() { }
        protected virtual void OnFailed() { }
        #endregion

        #region Read Field Info
        public virtual void GetFieldEnumTypes(List<Type> res)
        {
            res.Add(GetFieldEnumType());
        }
        protected abstract Type GetFieldEnumType();
        public abstract void ReadFieldInfo(int fieldEnum, TaskFieldInfo fieldInfo, TaskContextBase context);
        /// <summary>
        /// Read task refrences for child nodes. Most frequently used in low-level drive nodes, such as Sequence, Selector in
        /// behavior tree.
        /// </summary>
        public virtual void ReadRefrenceInfo(int fieldEnum, List<TaskBase> tasks, TaskContextBase context) { }

        protected T ReadValue<T>(TaskFieldInfo fieldInfo, TaskContextBase context)
        {
            var res = default(T);
            switch (fieldInfo.ValueSource)
            {
                case ETaskFieldValueSource.Value:
                    DebugApi.LogWarning("Use other functions which meets the type required instead!");
                    break;
                case ETaskFieldValueSource.Context:
                    res = (T)context.GetValue(fieldInfo.Value);
                    break;
            }
            return res;
        }

        protected bool ReadBool(TaskFieldInfo fieldInfo, TaskContextBase context)
        {
            bool res = default;
            switch (fieldInfo.ValueSource)
            {
                case ETaskFieldValueSource.Value:
                    res = bool.Parse(fieldInfo.Value);
                    break;
                case ETaskFieldValueSource.Context:
                    res = (bool)context.GetValue(fieldInfo.Value);
                    break;
            }
            return res;
        }

        protected short ReadShort(TaskFieldInfo fieldInfo, TaskContextBase context)
        {
            short res = default;
            switch (fieldInfo.ValueSource)
            {
                case ETaskFieldValueSource.Value:
                    res = short.Parse(fieldInfo.Value);
                    break;
                case ETaskFieldValueSource.Context:
                    res = (short)context.GetValue(fieldInfo.Value);
                    break;
            }
            return res;
        }

        protected ushort ReadUshort(TaskFieldInfo fieldInfo, TaskContextBase context)
        {
            ushort res = default;
            switch (fieldInfo.ValueSource)
            {
                case ETaskFieldValueSource.Value:
                    res = ushort.Parse(fieldInfo.Value);
                    break;
                case ETaskFieldValueSource.Context:
                    res = (ushort)context.GetValue(fieldInfo.Value);
                    break;
            }
            return res;
        }

        protected int ReadInt(TaskFieldInfo fieldInfo, TaskContextBase context)
        {
            int res = default;
            switch (fieldInfo.ValueSource)
            {
                case ETaskFieldValueSource.Value:
                    res = int.Parse(fieldInfo.Value);
                    break;
                case ETaskFieldValueSource.Context:
                    res = (int)context.GetValue(fieldInfo.Value);
                    break;
            }
            return res;
        }

        protected uint ReadUint(TaskFieldInfo fieldInfo, TaskContextBase context)
        {
            uint res = default;
            switch (fieldInfo.ValueSource)
            {
                case ETaskFieldValueSource.Value:
                    res = uint.Parse(fieldInfo.Value);
                    break;
                case ETaskFieldValueSource.Context:
                    res = (uint)context.GetValue(fieldInfo.Value);
                    break;
            }
            return res;
        }

        protected long ReadLong(TaskFieldInfo fieldInfo, TaskContextBase context)
        {
            long res = default;
            switch (fieldInfo.ValueSource)
            {
                case ETaskFieldValueSource.Value:
                    res = long.Parse(fieldInfo.Value);
                    break;
                case ETaskFieldValueSource.Context:
                    res = (long)context.GetValue(fieldInfo.Value);
                    break;
            }
            return res;
        }

        protected ulong ReadUlong(TaskFieldInfo fieldInfo, TaskContextBase context)
        {
            ulong res = default;
            switch (fieldInfo.ValueSource)
            {
                case ETaskFieldValueSource.Value:
                    res = ulong.Parse(fieldInfo.Value);
                    break;
                case ETaskFieldValueSource.Context:
                    res = (ulong)context.GetValue(fieldInfo.Value);
                    break;
            }
            return res;
        }

        protected float ReadFloat(TaskFieldInfo fieldInfo, TaskContextBase context)
        {
            float res = default;
            switch (fieldInfo.ValueSource)
            {
                case ETaskFieldValueSource.Value:
                    res = float.Parse(fieldInfo.Value);
                    break;
                case ETaskFieldValueSource.Context:
                    res = (float)context.GetValue(fieldInfo.Value);
                    break;
            }
            return res;
        }

        protected double ReadDouble(TaskFieldInfo fieldInfo, TaskContextBase context)
        {
            double res = default;
            switch (fieldInfo.ValueSource)
            {
                case ETaskFieldValueSource.Value:
                    res = double.Parse(fieldInfo.Value);
                    break;
                case ETaskFieldValueSource.Context:
                    res = (double)context.GetValue(fieldInfo.Value);
                    break;
            }
            return res;
        }

        protected string ReadString(TaskFieldInfo fieldInfo, TaskContextBase context)
        {
            string res = default;
            switch (fieldInfo.ValueSource)
            {
                case ETaskFieldValueSource.Value:
                    res = fieldInfo.Value;
                    break;
                case ETaskFieldValueSource.Context:
                    res = (string)context.GetValue(fieldInfo.Value);
                    break;
            }
            return res;
        }
        #endregion

        #region Register Field Info
        internal class RegisteredField
        {
            internal string Name;
            internal int EnumValue;
            internal TaskExportTypeInfo TypeInfo;
        }

        private List<RegisteredField> m_TempFieldList;

        internal TaskExportInfo GenerateExportInfo()
        {
            m_TempFieldList = new();
            RegisterFields();
            var res = new TaskExportInfo();
            res.TaskTypeName = this.GetType().Name;
            foreach (var field in m_TempFieldList)
            {
                var exportFieldInfo = new TaskExportFieldInfo();
                exportFieldInfo.FieldName = field.Name;
                exportFieldInfo.TypeInfo = field.TypeInfo;
                res.FieldInfos.Add(exportFieldInfo);
            }
            return res;
        }

        protected abstract void RegisterFields();

        protected void RegisterField<TEnum, TObj>(TEnum fieldEnum, TObj obj) where TEnum : Enum
        {
            var field = new RegisteredField();
            field.Name = fieldEnum.ToString();
            field.EnumValue = fieldEnum.GetHashCode();
            field.TypeInfo = TaskApi.GenerateTaskTypeInfo(typeof(TObj));
            m_TempFieldList.Add(field);
        }
        #endregion
    }

    #region Task Extension
    public static class TaskExtension
    {
        public static TaskValueInfo CreateTaskValueInfo<T>(this TaskGroupInfo groupInfo, int id)
        {
            var info = new TaskValueInfo();
            info.FullTypeName = typeof(T).FullName;
            info.AssemblyQualifiedName = typeof(T).AssemblyQualifiedName;
            groupInfo.TaskInfos[id] = info;
            return info;
        }

        public static TaskValueInfo CreateTaskTimelineValueInfo(this TaskGroupInfo groupInfo, int id, float duration)
        {
            var info = groupInfo.CreateTaskValueInfo<TaskTimeline>(id);
            info.AddFieldInfo(TaskTimeline.EField.Duration, duration);
            return info;
        }
    }
    #endregion
}
