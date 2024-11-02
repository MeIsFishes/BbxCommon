using System;
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
        public void Run()
        {
            TaskManager.Instance.RunTask(this);
        }

        internal void Enter() { OnEnter(); }
        internal ETaskRunState Update(float deltaTime) { return OnUpdate(deltaTime); }
        internal void Exit() { OnExit(); }
        internal void OnNodeSucceeded() { OnSucceeded(); }
        internal void OnNodeFailed() { OnFailed(); }

        protected virtual void OnEnter() { }
        protected virtual ETaskRunState OnUpdate(float deltaTime) { return ETaskRunState.Succeeded; }
        protected virtual void OnExit() { }
        protected virtual void OnSucceeded() { }
        protected virtual void OnFailed() { }
        #endregion

        #region Initialization
        public abstract Type GetFieldEnumType();
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
    }
}
